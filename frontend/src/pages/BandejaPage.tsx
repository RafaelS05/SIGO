import { useCallback, useEffect, useMemo, useState } from 'react'
import { Link, Navigate, useParams } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../auth/AuthContext'
import { PERMISOS } from '../auth/permisos'
import { EstadoBadge, PrioridadBadge, SlaBadge, fmtFechaCorta } from '../components/Badges'
import { IconoMas } from '../components/Icons'
import type { Estado, Oficio, OficioFiltros, Prioridad } from '../types'

const TITULOS: Record<string, string> = {
  mi: 'Mi bandeja',
  departamento: 'Bandeja de departamento',
  atrasados: 'Bandeja de atrasados',
  todos: 'Todos los oficios',
}

type Rapido = '' | 'hoy' | '7dias' | 'mes'

export default function BandejaPage() {
  const { tipo = 'mi' } = useParams()
  const { usuario: usuarioActual, puede } = useAuth()
  const [oficios, setOficios] = useState<Oficio[]>([])
  const [error, setError] = useState('')
  const [estados, setEstados] = useState<Estado[]>([])
  const [prioridades, setPrioridades] = useState<Prioridad[]>([])
  const [cargando, setCargando] = useState(true)

  const [busqueda, setBusqueda] = useState('')
  const [estadoId, setEstadoId] = useState('')
  const [prioridadId, setPrioridadId] = useState('')
  const [rapido, setRapido] = useState<Rapido>('')

  useEffect(() => {
    api.estados.list().then(setEstados)
    api.prioridades.list().then(setPrioridades)
  }, [])

  const cargar = useCallback(() => {
    if (!usuarioActual) return
    const filtros: OficioFiltros = {}
    if (tipo === 'mi') filtros.responsableId = usuarioActual.id
    if (tipo === 'departamento') filtros.departamentoId = usuarioActual.idDepartamento
    if (tipo === 'atrasados') filtros.atrasados = true
    if (estadoId) filtros.estadoId = Number(estadoId)
    if (prioridadId) filtros.prioridadId = Number(prioridadId)
    if (busqueda.trim()) filtros.busqueda = busqueda.trim()
    if (rapido === 'hoy') filtros.desde = new Date(new Date().setHours(0, 0, 0, 0)).toISOString()
    if (rapido === '7dias') filtros.desde = new Date(Date.now() - 7 * 864e5).toISOString()
    if (rapido === 'mes') {
      const hoy = new Date()
      filtros.desde = new Date(hoy.getFullYear(), hoy.getMonth(), 1).toISOString()
    }
    setCargando(true)
    setError('')
    api.oficios.list(filtros)
      .then(setOficios)
      .catch((e) => setError(e instanceof Error ? e.message : String(e)))
      .finally(() => setCargando(false))
  }, [tipo, usuarioActual, estadoId, prioridadId, busqueda, rapido])

  useEffect(cargar, [cargar])

  const verTodos = puede(PERMISOS.VerTodosLosOficios)

  const subtitulo = useMemo(() => {
    if (tipo === 'mi') return `Oficios asignados a ${usuarioActual?.nombre ?? '…'}`
    if (tipo === 'departamento') return `Destinados a ${usuarioActual?.departamentoNombre ?? '…'}`
    if (tipo === 'atrasados') return verTodos ? 'Oficios con SLA vencido y sin cierre' : `SLA vencido en ${usuarioActual?.departamentoNombre ?? 'su departamento'}`
    return 'Vista completa (Administrador / Auditor)'
  }, [tipo, usuarioActual, verTodos])

  // La bandeja "Todos" es exclusiva de Administrador/Auditor (el backend también lo exige).
  if (tipo === 'todos' && !verTodos) return <Navigate to="/acceso-denegado" replace />

  return (
    <div>
      <div className="page-head">
        <div>
          <h1>{TITULOS[tipo] ?? 'Bandeja'}</h1>
          <p className="subtitle">{subtitulo}</p>
        </div>
        {puede(PERMISOS.CrearOficios) && <Link className="btn btn-primary" to="/nuevo"><IconoMas size={16} /> Nuevo oficio</Link>}
      </div>

      {error && <div className="alert">{error}</div>}

      <div className="filters">
        <input
          type="search"
          placeholder="Buscar por número, asunto, remitente…"
          value={busqueda}
          onChange={(e) => setBusqueda(e.target.value)}
        />
        <select value={estadoId} onChange={(e) => setEstadoId(e.target.value)}>
          <option value="">Estado: todos</option>
          {estados.map((s) => <option key={s.id} value={s.id}>{s.nombre}</option>)}
        </select>
        <select value={prioridadId} onChange={(e) => setPrioridadId(e.target.value)}>
          <option value="">Prioridad: todas</option>
          {prioridades.map((p) => <option key={p.id} value={p.id}>{p.nombre}</option>)}
        </select>
        <div className="chip-row" role="group" aria-label="Filtros rápidos">
          {([['', 'Todo'], ['hoy', 'Hoy'], ['7dias', 'Últimos 7 días'], ['mes', 'Este mes']] as [Rapido, string][]).map(([v, txt]) => (
            <button key={v} className={rapido === v ? 'chip chip-active' : 'chip'} onClick={() => setRapido(v)}>
              {txt}
            </button>
          ))}
        </div>
      </div>

      {cargando ? (
        <p className="muted">Cargando…</p>
      ) : oficios.length === 0 ? (
        <div className="empty">No hay oficios que coincidan con los filtros.</div>
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Número</th>
                <th>Asunto</th>
                <th>Remitente</th>
                <th>Depto. destino</th>
                <th>Responsable</th>
                <th>Prioridad</th>
                <th>Estado</th>
                <th>SLA</th>
                <th>Ingreso</th>
              </tr>
            </thead>
            <tbody>
              {oficios.map((o) => (
                <tr key={o.id}>
                  <td><Link className="num-link" to={`/oficio/${o.id}`}>{o.numero}</Link></td>
                  <td className="td-asunto">{o.asunto}</td>
                  <td>{o.remitente}</td>
                  <td>{o.departamentoDestinoNombre}</td>
                  <td>{o.responsableActual || <span className="muted">Sin asignar</span>}</td>
                  <td><PrioridadBadge nombre={o.prioridadNombre} nivel={prioridades.find((p) => p.id === o.idPrioridad)?.nivel} /></td>
                  <td><EstadoBadge nombre={o.estadoNombre} /></td>
                  <td><SlaBadge oficio={o} /></td>
                  <td>{fmtFechaCorta(o.fechaIngreso)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
