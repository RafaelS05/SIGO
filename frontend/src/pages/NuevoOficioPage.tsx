import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { api } from '../api'
import { useUser } from '../UserContext'
import type { Canal, Departamento, Prioridad, TipoOficio } from '../types'

export default function NuevoOficioPage() {
  const navigate = useNavigate()
  const { usuarioActual } = useUser()
  const [departamentos, setDepartamentos] = useState<Departamento[]>([])
  const [tipos, setTipos] = useState<TipoOficio[]>([])
  const [prioridades, setPrioridades] = useState<Prioridad[]>([])
  const [canales, setCanales] = useState<Canal[]>([])
  const [error, setError] = useState('')
  const [guardando, setGuardando] = useState(false)

  const [form, setForm] = useState({
    direccion: 'Entrante',
    idTipoOficio: '',
    idDepartamentoOrigen: '',
    idDepartamentoDestino: '',
    remitente: '',
    destinatario: '',
    asunto: '',
    descripcion: '',
    idPrioridad: '',
    idCanal: '',
    fechaIngreso: new Date().toISOString().slice(0, 16),
  })

  useEffect(() => {
    api.departamentos.list().then(setDepartamentos)
    api.tiposOficios.list().then(setTipos)
    api.prioridades.list().then(setPrioridades)
    api.canales.list().then(setCanales)
  }, [])

  const set = (campo: string) => (e: { target: { value: string } }) =>
    setForm((f) => ({ ...f, [campo]: e.target.value }))

  const prioridadSel = prioridades.find((p) => p.id === Number(form.idPrioridad))

  const enviar = async (e: FormEvent) => {
    e.preventDefault()
    if (!usuarioActual) return
    setError('')
    setGuardando(true)
    try {
      const creado = await api.oficios.create({
        direccion: form.direccion,
        idTipoOficio: Number(form.idTipoOficio),
        idDepartamentoOrigen: form.idDepartamentoOrigen ? Number(form.idDepartamentoOrigen) : null,
        idDepartamentoDestino: Number(form.idDepartamentoDestino),
        remitente: form.remitente,
        destinatario: form.destinatario,
        asunto: form.asunto,
        descripcion: form.descripcion,
        idPrioridad: Number(form.idPrioridad),
        idCanal: Number(form.idCanal),
        fechaIngreso: form.fechaIngreso,
        creadoPor: usuarioActual.id,
      })
      navigate(`/oficio/${creado.id}`)
    } catch (err) {
      setError(err instanceof Error ? err.message : String(err))
    } finally {
      setGuardando(false)
    }
  }

  return (
    <div className="form-page">
      <h1>Registrar oficio</h1>
      <p className="subtitle">La numeración (prefijo + área + año + consecutivo) y el vencimiento SLA se calculan automáticamente.</p>

      {error && <div className="alert">{error}</div>}

      <form className="card form" onSubmit={enviar}>
        <div className="form-row">
          <label>
            Dirección *
            <select value={form.direccion} onChange={set('direccion')} required>
              <option>Entrante</option>
              <option>Saliente</option>
            </select>
          </label>
          <label>
            Tipo de oficio *
            <select value={form.idTipoOficio} onChange={set('idTipoOficio')} required>
              <option value="">Seleccione…</option>
              {tipos.map((t) => <option key={t.id} value={t.id}>{t.nombre}{t.requiereRespuestaFlag ? ' (requiere respuesta)' : ''}</option>)}
            </select>
          </label>
          <label>
            Canal *
            <select value={form.idCanal} onChange={set('idCanal')} required>
              <option value="">Seleccione…</option>
              {canales.map((c) => <option key={c.id} value={c.id}>{c.nombre}</option>)}
            </select>
          </label>
        </div>

        <div className="form-row">
          <label>
            Departamento origen
            <select value={form.idDepartamentoOrigen} onChange={set('idDepartamentoOrigen')}>
              <option value="">Externo / N.A.</option>
              {departamentos.filter((d) => d.activo).map((d) => <option key={d.id} value={d.id}>{d.nombre}</option>)}
            </select>
          </label>
          <label>
            Departamento destino *
            <select value={form.idDepartamentoDestino} onChange={set('idDepartamentoDestino')} required>
              <option value="">Seleccione…</option>
              {departamentos.filter((d) => d.activo).map((d) => <option key={d.id} value={d.id}>{d.nombre} ({d.sigla})</option>)}
            </select>
          </label>
          <label>
            Prioridad *
            <select value={form.idPrioridad} onChange={set('idPrioridad')} required>
              <option value="">Seleccione…</option>
              {prioridades.map((p) => <option key={p.id} value={p.id}>{p.nombre} — SLA {p.slaHoras} h</option>)}
            </select>
          </label>
        </div>

        <div className="form-row">
          <label>
            Remitente *
            <input value={form.remitente} onChange={set('remitente')} maxLength={200} required />
          </label>
          <label>
            Destinatario *
            <input value={form.destinatario} onChange={set('destinatario')} maxLength={200} required />
          </label>
          <label>
            Fecha de ingreso *
            <input type="datetime-local" value={form.fechaIngreso} onChange={set('fechaIngreso')} required />
          </label>
        </div>

        <label>
          Asunto *
          <input value={form.asunto} onChange={set('asunto')} maxLength={300} required />
        </label>

        <label>
          Descripción
          <textarea rows={4} value={form.descripcion} onChange={set('descripcion')} maxLength={4000} />
        </label>

        {prioridadSel && (
          <p className="hint">
            Con prioridad <strong>{prioridadSel.nombre}</strong> el oficio vence {prioridadSel.slaHoras} horas después del ingreso.
          </p>
        )}

        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={guardando}>
            {guardando ? 'Registrando…' : 'Registrar oficio'}
          </button>
        </div>
      </form>
    </div>
  )
}
