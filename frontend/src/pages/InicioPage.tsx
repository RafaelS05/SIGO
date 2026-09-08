import { useEffect, useMemo, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../auth/AuthContext'
import { PERMISOS, ROLES, ROL_ETIQUETA } from '../auth/permisos'
import { EstadoBadge, fmtFechaCorta } from '../components/Badges'
import {
  IconoAlerta, IconoBuscar, IconoCheck, IconoClip, IconoDocumento, IconoEngranaje, IconoFiltro,
  IconoIntercambio, IconoMas, IconoReloj, IconoReportes, IconoUsuario, IconoUsuarios,
} from '../components/Icons'
import type { Oficio, Prioridad } from '../types'

type Rapido = 'hoy' | '7dias' | 'mes' | 'atrasados'

const relativo = (iso: string) => {
  const min = Math.max(0, Math.round((Date.now() - new Date(iso).getTime()) / 60000))
  if (min < 60) return `Hace ${min} min`
  const h = Math.round(min / 60)
  if (h < 24) return `Hace ${h} hora${h === 1 ? '' : 's'}`
  const d = Math.round(h / 24)
  return `Hace ${d} día${d === 1 ? '' : 's'}`
}

const PRIORIDAD_PUNTO: Record<number, string> = { 1: 'punto-urgente', 2: 'punto-alta', 3: 'punto-media', 4: 'punto-baja' }

/** Panel de inicio (machote): indicadores, últimos oficios, alertas de SLA y actividad reciente, acotado por rol. */
export default function InicioPage() {
  const { usuario, puede, tieneRol } = useAuth()
  const navigate = useNavigate()
  const [oficios, setOficios] = useState<Oficio[]>([])
  const [prioridades, setPrioridades] = useState<Prioridad[]>([])
  const [cargando, setCargando] = useState(true)
  const [rapido, setRapido] = useState<Rapido>('mes')
  const [busqueda, setBusqueda] = useState('')

  const esAnalista = tieneRol(ROLES.Analista) && !tieneRol(ROLES.Administrador, ROLES.Jefatura)
  const esAuditor = tieneRol(ROLES.Auditor) && !tieneRol(ROLES.Administrador)

  useEffect(() => {
    if (!usuario) return
    setCargando(true)
    // El analista ve solo lo que tiene asignado; el resto, su alcance (departamento o global).
    const filtros = esAnalista ? { responsableId: usuario.id } : {}
    Promise.all([api.oficios.list(filtros), api.prioridades.list()])
      .then(([o, p]) => { setOficios(o); setPrioridades(p) })
      .finally(() => setCargando(false))
  }, [usuario, esAnalista])

  const ahora = Date.now()
  const inicioMes = useMemo(() => { const h = new Date(); return new Date(h.getFullYear(), h.getMonth(), 1).getTime() }, [])

  const kpi = useMemo(() => {
    const abiertos = oficios.filter((o) => !o.estadoCerrado)
    const recibidosMes = oficios.filter((o) => new Date(o.fechaIngreso).getTime() >= inicioMes)
    const enProceso = abiertos.filter((o) => /proceso|asignado|pendiente|enviado/i.test(o.estadoNombre))
    const proximos = abiertos.filter((o) => {
      const t = new Date(o.fechaVencimientoSla).getTime()
      return t > ahora && t - ahora <= 3 * 864e5
    })
    const atrasados = oficios.filter((o) => o.atrasado)
    const en24h = abiertos.filter((o) => {
      const t = new Date(o.fechaVencimientoSla).getTime()
      return t > ahora && t - ahora <= 864e5
    })
    const sinAsignar = abiertos.filter((o) => !o.idResponsableActual)
    const cerrados = oficios.filter((o) => o.fechaCierre)
    const promedioDias = cerrados.length
      ? cerrados.reduce((s, o) => s + (new Date(o.fechaCierre!).getTime() - new Date(o.fechaIngreso).getTime()) / 864e5, 0) / cerrados.length
      : 0
    return { recibidosMes, enProceso, proximos, atrasados, en24h, sinAsignar, promedioDias }
  }, [oficios, ahora, inicioMes])

  const ultimos = useMemo(() => {
    let lista = [...oficios]
    if (rapido === 'hoy') {
      const h = new Date().setHours(0, 0, 0, 0)
      lista = lista.filter((o) => new Date(o.fechaIngreso).getTime() >= h)
    } else if (rapido === '7dias') lista = lista.filter((o) => ahora - new Date(o.fechaIngreso).getTime() <= 7 * 864e5)
    else if (rapido === 'mes') lista = lista.filter((o) => new Date(o.fechaIngreso).getTime() >= inicioMes)
    else if (rapido === 'atrasados') lista = lista.filter((o) => o.atrasado)
    const q = busqueda.trim().toLowerCase()
    if (q) lista = lista.filter((o) => [o.numero, o.asunto, o.remitente].some((v) => v.toLowerCase().includes(q)))
    return lista.sort((a, b) => new Date(b.fechaIngreso).getTime() - new Date(a.fechaIngreso).getTime()).slice(0, 8)
  }, [oficios, rapido, busqueda, ahora, inicioMes])

  const actividad = useMemo(() => {
    const eventos = oficios.flatMap((o) => o.bitacora.map((b) => ({ ...b, numero: o.numero, idOficio: o.id })))
    return eventos.sort((a, b) => new Date(b.fechaEvento).getTime() - new Date(a.fechaEvento).getTime()).slice(0, 5)
  }, [oficios])

  const iconoEvento = (comentario: string, estado: string) => {
    if (/adjunto/i.test(comentario)) return <IconoClip size={16} />
    if (/asignado a/i.test(comentario)) return <IconoUsuario size={16} />
    if (/cerrado/i.test(estado)) return <IconoCheck size={16} />
    if (/registrado/i.test(comentario)) return <IconoDocumento size={16} />
    return <IconoIntercambio size={16} />
  }

  const rolPrincipal = usuario?.roles[0]?.nombre ?? ''
  const subtitulo = esAnalista
    ? 'Oficios asignados a usted y su avance frente al SLA.'
    : esAuditor
      ? 'Vista de auditoría (solo lectura) sobre todos los oficios de la Municipalidad.'
      : puede(PERMISOS.VerTodosLosOficios)
        ? 'Control, seguimiento y trazabilidad de las comunicaciones oficiales de la Municipalidad de Paraíso.'
        : `Control y seguimiento de los oficios de ${usuario?.departamentoNombre ?? 'su departamento'}.`

  return (
    <div className="inicio">
      <div className="inicio-principal">
        <div className="page-head">
          <div>
            <h1 className="titulo-grande">Gestión de Oficios</h1>
            <p className="subtitle">{subtitulo}</p>
            <span className="badge badge-rol">{ROL_ETIQUETA[rolPrincipal] ?? rolPrincipal}</span>
          </div>
          {puede(PERMISOS.CrearOficios) && (
            <Link className="btn btn-primary btn-grande" to="/nuevo"><IconoMas size={18} /> Nuevo oficio</Link>
          )}
        </div>

        <div className="kpis">
          <KpiTile icono={<IconoDocumento />} tono="azul" titulo="Oficios recibidos" valor={kpi.recibidosMes.length} nota="Este mes" to="/bandeja/departamento" />
          <KpiTile icono={<IconoEngranaje />} tono="azul" titulo="En proceso" valor={kpi.enProceso.length} nota="Abiertos con trámite" to="/bandeja/departamento" />
          <KpiTile icono={<IconoReloj />} tono="ambar" titulo="Próximos a vencer" valor={kpi.proximos.length} nota="En los próximos 3 días" to="/bandeja/departamento" />
          <KpiTile icono={<IconoAlerta />} tono="rojo" titulo="Atrasados" valor={kpi.atrasados.length} nota="Con SLA vencido" to="/bandeja/atrasados" />
        </div>

        <div className="barra-filtros">
          <div className="chip-row" role="group" aria-label="Rango">
            {([['hoy', 'Hoy'], ['7dias', 'Últimos 7 días'], ['mes', 'Este mes'], ['atrasados', 'Atrasados']] as [Rapido, string][]).map(([v, txt]) => (
              <button key={v} type="button" className={rapido === v ? 'chip chip-active' : 'chip'} onClick={() => setRapido(v)}>{txt}</button>
            ))}
          </div>
          <label className="buscador">
            <IconoBuscar size={18} />
            <input type="search" placeholder="Buscar por número, asunto o remitente…" value={busqueda} onChange={(e) => setBusqueda(e.target.value)} />
          </label>
          <button type="button" className="btn" onClick={() => navigate(esAuditor ? '/bandeja/todos' : '/bandeja/departamento')}><IconoFiltro size={16} /> Filtrar</button>
        </div>

        <section className="card card-tabla">
          <div className="card-cab">
            <h2>Últimos oficios</h2>
            <Link to={puede(PERMISOS.VerTodosLosOficios) ? '/bandeja/todos' : esAnalista ? '/bandeja/mi' : '/bandeja/departamento'}>Ver todos</Link>
          </div>
          {cargando ? (
            <p className="muted">Cargando…</p>
          ) : ultimos.length === 0 ? (
            <div className="empty">No hay oficios en este rango.</div>
          ) : (
            <div className="table-wrap table-plana">
              <table>
                <thead>
                  <tr>
                    <th>Número</th><th>Asunto</th><th>Departamento</th><th>Responsable</th>
                    <th>Estado</th><th>Prioridad</th><th>Fecha límite</th><th>Acciones</th>
                  </tr>
                </thead>
                <tbody>
                  {ultimos.map((o) => {
                    const nivel = prioridades.find((p) => p.id === o.idPrioridad)?.nivel ?? 3
                    return (
                      <tr key={o.id} className={o.atrasado ? 'fila-atrasada' : ''}>
                        <td><Link className="num-link" to={`/oficio/${o.id}`}>{o.numero}</Link></td>
                        <td className="td-asunto">{o.asunto}</td>
                        <td>{o.departamentoDestinoNombre}</td>
                        <td>{o.responsableActual || <span className="muted">—</span>}</td>
                        <td><EstadoBadge nombre={o.atrasado ? 'Atrasado' : o.estadoNombre} /></td>
                        <td><span className={`punto ${PRIORIDAD_PUNTO[nivel] ?? 'punto-media'}`} /> {o.prioridadNombre}</td>
                        <td className={o.atrasado ? 'texto-critico' : ''}>{fmtFechaCorta(o.fechaVencimientoSla)}</td>
                        <td><Link className="btn btn-sm btn-suave" to={`/oficio/${o.id}`}>Ver detalle</Link></td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>
          )}
        </section>
      </div>

      <aside className="inicio-lateral">
        <section className="card card-alertas">
          <div className="card-cab">
            <h2 className="titulo-alerta"><IconoAlerta size={20} /> Alertas de SLA</h2>
            <Link to="/bandeja/atrasados">Ver todas</Link>
          </div>
          <ul className="lista-alertas">
            <li><span className="alerta-icono ambar"><IconoReloj size={16} /></span><span><strong>{kpi.en24h.length}</strong> oficios vencen en las próximas <strong>24 horas</strong></span></li>
            <li><span className="alerta-icono rojo"><IconoReloj size={16} /></span><span><strong>{kpi.atrasados.length}</strong> oficios con SLA vencido</span></li>
            <li><span className="alerta-icono gris"><IconoUsuarios size={16} /></span><span><strong>{kpi.sinAsignar.length}</strong> oficios sin asignar</span></li>
            <li><span className="alerta-icono azul"><IconoReportes size={16} /></span><span>Tiempo promedio de respuesta: <strong>{kpi.promedioDias.toFixed(1)} días</strong></span></li>
          </ul>
        </section>

        <section className="card">
          <div className="card-cab">
            <h2>Actividad reciente</h2>
            <Link to={esAnalista ? '/bandeja/mi' : '/bandeja/departamento'}>Ver todas</Link>
          </div>
          {actividad.length === 0 ? (
            <p className="muted">Sin actividad registrada.</p>
          ) : (
            <ul className="lista-actividad">
              {actividad.map((ev) => (
                <li key={ev.id}>
                  <span className="actividad-icono">{iconoEvento(ev.comentario, ev.estadoDestinoNombre)}</span>
                  <span>
                    <Link to={`/oficio/${ev.idOficio}`} className="num-link">{ev.numero}</Link>{' '}
                    {ev.estadoDestinoNombre && ev.estadoOrigenNombre
                      ? <>cambió a <strong>{ev.estadoDestinoNombre}</strong></>
                      : ev.comentario}
                    <small>{relativo(ev.fechaEvento)} · {ev.usuarioNombre}</small>
                  </span>
                </li>
              ))}
            </ul>
          )}
        </section>
      </aside>
    </div>
  )
}

function KpiTile({ icono, tono, titulo, valor, nota, to }: {
  icono: React.ReactNode; tono: 'azul' | 'ambar' | 'rojo'; titulo: string; valor: number; nota: string; to: string
}) {
  return (
    <Link to={to} className="kpi">
      <span className={`kpi-icono kpi-${tono}`}>{icono}</span>
      <span className="kpi-texto">
        <span className="kpi-titulo">{titulo}</span>
        <span className="kpi-valor">{valor}</span>
        <span className="kpi-nota">{nota}</span>
      </span>
    </Link>
  )
}
