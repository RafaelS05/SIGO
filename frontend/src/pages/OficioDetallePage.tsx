import { useCallback, useEffect, useRef, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../auth/AuthContext'
import { PERMISOS } from '../auth/permisos'
import { EstadoBadge, PrioridadBadge, SlaBadge, fmtFecha } from '../components/Badges'
import type { Estado, Oficio, Usuario } from '../types'

type Tab = 'bitacora' | 'adjuntos' | 'asignaciones'

export default function OficioDetallePage() {
  const { id } = useParams()
  const { puede } = useAuth()
  const [oficio, setOficio] = useState<Oficio | null>(null)
  const [estados, setEstados] = useState<Estado[]>([])
  const [usuarios, setUsuarios] = useState<Usuario[]>([])
  const [tab, setTab] = useState<Tab>('bitacora')
  const [error, setError] = useState('')

  const [nuevoEstado, setNuevoEstado] = useState('')
  const [motivoEstado, setMotivoEstado] = useState('')
  const [nuevoResponsable, setNuevoResponsable] = useState('')
  const [comentario, setComentario] = useState('')
  const fileRef = useRef<HTMLInputElement>(null)

  // Acciones visibles según la matriz RBAC (el backend vuelve a validar cada una).
  const puedeTramitar = puede(PERMISOS.TramitarOficios)
  const puedeAsignar = puede(PERMISOS.AsignarOficios)

  const cargar = useCallback(() => {
    api.oficios.get(Number(id)).then(setOficio).catch((e) => setError(e.message))
  }, [id])

  useEffect(() => {
    cargar()
    api.estados.list().then(setEstados)
    if (puedeAsignar) api.usuarios.list().then(setUsuarios).catch(() => setUsuarios([]))
  }, [cargar, puedeAsignar])

  const ejecutar = async (fn: () => Promise<Oficio>) => {
    setError('')
    try {
      setOficio(await fn())
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e))
    }
  }

  if (!oficio) return <p className="muted">{error || 'Cargando…'}</p>

  return (
    <div>
      <div className="page-head">
        <div>
          <p className="breadcrumb"><Link to="/">Inicio</Link> / <Link to="/bandeja/mi">Bandejas</Link> / {oficio.numero}</p>
          <h1>{oficio.asunto}</h1>
          <div className="badge-row">
            <span className="badge badge-dir">{oficio.direccion}</span>
            <EstadoBadge nombre={oficio.estadoNombre} />
            <PrioridadBadge nombre={oficio.prioridadNombre} />
            <SlaBadge oficio={oficio} />
          </div>
        </div>
      </div>

      {error && <div className="alert">{error}</div>}

      <div className="detail-grid">
        <section className="card">
          <h2>Ficha</h2>
          <dl className="ficha">
            <dt>Número</dt><dd>{oficio.numero}</dd>
            <dt>Tipo</dt><dd>{oficio.tipoOficioNombre}</dd>
            <dt>Canal</dt><dd>{oficio.canalNombre}</dd>
            <dt>Remitente</dt><dd>{oficio.remitente}</dd>
            <dt>Destinatario</dt><dd>{oficio.destinatario}</dd>
            <dt>Depto. origen</dt><dd>{oficio.departamentoOrigenNombre || '—'}</dd>
            <dt>Depto. destino</dt><dd>{oficio.departamentoDestinoNombre}</dd>
            <dt>Responsable</dt><dd>{oficio.responsableActual || 'Sin asignar'}</dd>
            <dt>Fecha ingreso</dt><dd>{fmtFecha(oficio.fechaIngreso)}</dd>
            <dt>Vencimiento SLA</dt><dd>{fmtFecha(oficio.fechaVencimientoSla)}</dd>
            <dt>Fecha cierre</dt><dd>{fmtFecha(oficio.fechaCierre)}</dd>
            <dt>Registrado por</dt><dd>{oficio.creadoPorNombre}</dd>
          </dl>
          {oficio.descripcion && <p className="descripcion">{oficio.descripcion}</p>}
        </section>

        <section className="card">
          <h2>Acciones rápidas</h2>

          {!puedeTramitar && !puedeAsignar && (
            <p className="muted">Su rol es de solo lectura sobre este oficio.</p>
          )}

          {puedeTramitar && (
            <div className="action">
              <h3>Cambiar estado</h3>
              <select value={nuevoEstado} onChange={(e) => setNuevoEstado(e.target.value)}>
                <option value="">Seleccione estado…</option>
                {estados.filter((s) => s.id !== oficio.idEstadoActual).map((s) => (
                  <option key={s.id} value={s.id}>{s.nombre}</option>
                ))}
              </select>
              <input placeholder="Motivo (queda en bitácora)" value={motivoEstado} onChange={(e) => setMotivoEstado(e.target.value)} />
              <button
                className="btn btn-primary"
                disabled={!nuevoEstado}
                onClick={() => ejecutar(() => api.oficios.cambiarEstado(oficio.id, Number(nuevoEstado), motivoEstado)).then(() => { setNuevoEstado(''); setMotivoEstado('') })}
              >
                Aplicar
              </button>
            </div>
          )}

          {puedeAsignar && (
            <div className="action">
              <h3>Reasignar responsable</h3>
              <select value={nuevoResponsable} onChange={(e) => setNuevoResponsable(e.target.value)}>
                <option value="">Seleccione usuario…</option>
                {usuarios.filter((u) => u.activo).map((u) => (
                  <option key={u.id} value={u.id}>{u.nombre} ({u.departamentoNombre})</option>
                ))}
              </select>
              <button
                className="btn btn-primary"
                disabled={!nuevoResponsable}
                onClick={() => ejecutar(() => api.oficios.asignar(oficio.id, Number(nuevoResponsable), '')).then(() => setNuevoResponsable(''))}
              >
                Asignar
              </button>
            </div>
          )}

          {puedeTramitar && (
            <>
              <div className="action">
                <h3>Comentar</h3>
                <textarea rows={2} placeholder="Comentario…" value={comentario} onChange={(e) => setComentario(e.target.value)} />
                <button
                  className="btn"
                  disabled={!comentario.trim()}
                  onClick={() => ejecutar(() => api.oficios.comentar(oficio.id, comentario.trim())).then(() => setComentario(''))}
                >
                  Agregar comentario
                </button>
              </div>

              <div className="action">
                <h3>Adjuntar archivo</h3>
                <input ref={fileRef} type="file" accept=".pdf,.jpg,.jpeg,.png,.doc,.docx" />
                <button
                  className="btn"
                  onClick={() => {
                    const archivo = fileRef.current?.files?.[0]
                    if (!archivo) return
                    ejecutar(() => api.oficios.subirAdjunto(oficio.id, archivo)).then(() => {
                      if (fileRef.current) fileRef.current.value = ''
                    })
                  }}
                >
                  Subir adjunto
                </button>
              </div>
            </>
          )}
        </section>
      </div>

      <div className="tabs" role="tablist">
        <button role="tab" aria-selected={tab === 'bitacora'} className={tab === 'bitacora' ? 'tab tab-active' : 'tab'} onClick={() => setTab('bitacora')}>
          Bitácora ({oficio.bitacora.length})
        </button>
        <button role="tab" aria-selected={tab === 'adjuntos'} className={tab === 'adjuntos' ? 'tab tab-active' : 'tab'} onClick={() => setTab('adjuntos')}>
          Adjuntos ({oficio.adjuntos.length})
        </button>
        <button role="tab" aria-selected={tab === 'asignaciones'} className={tab === 'asignaciones' ? 'tab tab-active' : 'tab'} onClick={() => setTab('asignaciones')}>
          Asignaciones ({oficio.responsables.length})
        </button>
      </div>

      {tab === 'bitacora' && (
        <div className="timeline">
          {oficio.bitacora.map((b) => (
            <div className="timeline-item" key={b.id}>
              <div className="timeline-dot" />
              <div>
                <div className="timeline-head">
                  <strong>{b.usuarioNombre}</strong>
                  <span className="muted">{fmtFecha(b.fechaEvento)}</span>
                </div>
                {b.estadoDestinoNombre && (
                  <div className="timeline-estado">
                    {b.estadoOrigenNombre ? <>{b.estadoOrigenNombre} → <strong>{b.estadoDestinoNombre}</strong></> : <strong>{b.estadoDestinoNombre}</strong>}
                  </div>
                )}
                {b.comentario && <p>{b.comentario}</p>}
              </div>
            </div>
          ))}
        </div>
      )}

      {tab === 'adjuntos' && (
        oficio.adjuntos.length === 0 ? <div className="empty">Sin adjuntos.</div> : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr><th>Archivo</th><th>Versión</th><th>Tamaño</th><th>Subido por</th><th>Fecha</th><th /></tr>
              </thead>
              <tbody>
                {oficio.adjuntos.map((a) => (
                  <tr key={a.id}>
                    <td>{a.nombreArchivo}</td>
                    <td>v{a.version}</td>
                    <td>{(a.pesoBytes / 1024).toFixed(1)} KB</td>
                    <td>{a.subidoPorNombre}</td>
                    <td>{fmtFecha(a.subidoEn)}</td>
                    <td>
                      <button
                        className="btn btn-sm"
                        onClick={() => api.oficios.descargarAdjunto(oficio.id, a.id, a.nombreArchivo).catch((e) => setError(e.message))}
                      >
                        Descargar
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )
      )}

      {tab === 'asignaciones' && (
        oficio.responsables.length === 0 ? <div className="empty">Sin asignaciones.</div> : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr><th>Responsable</th><th>Estado</th><th>Asignado por</th><th>Fecha</th></tr>
              </thead>
              <tbody>
                {oficio.responsables.map((r) => (
                  <tr key={r.id}>
                    <td>{r.usuarioNombre}</td>
                    <td>{r.activo ? <span className="badge badge-entiempo">Activo</span> : <span className="badge">Histórico</span>}</td>
                    <td>{r.asignadoPorNombre}</td>
                    <td>{fmtFecha(r.asignadoEn)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )
      )}
    </div>
  )
}
