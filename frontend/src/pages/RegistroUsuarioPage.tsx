import { useEffect, useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api'
import { ROL_DESCRIPCION, ROL_ETIQUETA } from '../auth/permisos'
import { IconoCheck, IconoOjo, IconoOjoCerrado, IconoUsuarioMas } from '../components/Icons'
import type { Departamento, Rol, Usuario } from '../types'

const vacio = () => ({
  nombre: '',
  correo: '',
  idDepartamento: '' as number | '',
  cargo: '',
  password: '',
  confirmarPassword: '',
  rolIds: [] as number[],
  activo: true,
})

/** URL de registro restringida al Administrador: datos, credenciales y roles del nuevo usuario. */
export default function RegistroUsuarioPage() {
  const [departamentos, setDepartamentos] = useState<Departamento[]>([])
  const [roles, setRoles] = useState<Rol[]>([])
  const [form, setForm] = useState(vacio())
  const [verPassword, setVerPassword] = useState(false)
  const [error, setError] = useState('')
  const [creado, setCreado] = useState<Usuario | null>(null)
  const [enviando, setEnviando] = useState(false)

  useEffect(() => {
    api.departamentos.list().then((d) => setDepartamentos(d.filter((x) => x.activo)))
    api.roles.list().then(setRoles)
  }, [])

  const set = <K extends keyof ReturnType<typeof vacio>>(campo: K, valor: ReturnType<typeof vacio>[K]) =>
    setForm((f) => ({ ...f, [campo]: valor }))

  const toggleRol = (id: number) =>
    set('rolIds', form.rolIds.includes(id) ? form.rolIds.filter((r) => r !== id) : [...form.rolIds, id])

  const fortaleza = (() => {
    const p = form.password
    let n = 0
    if (p.length >= 8) n++
    if (/[A-Z]/.test(p) && /[a-z]/.test(p)) n++
    if (/\d/.test(p)) n++
    if (/[^A-Za-z0-9]/.test(p)) n++
    return n
  })()

  const enviar = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    if (form.idDepartamento === '') return setError('Seleccione el departamento.')
    if (form.rolIds.length === 0) return setError('Asigne al menos un rol.')
    if (form.password.length < 8) return setError('La contraseña debe tener al menos 8 caracteres.')
    if (form.password !== form.confirmarPassword) return setError('Las contraseñas no coinciden.')

    setEnviando(true)
    try {
      const u = await api.auth.registrar({ ...form, idDepartamento: Number(form.idDepartamento) })
      setCreado(u)
      setForm(vacio())
    } catch (err) {
      setError(err instanceof Error ? err.message : String(err))
    } finally {
      setEnviando(false)
    }
  }

  return (
    <div className="form-page">
      <div className="page-head">
        <div>
          <p className="breadcrumb"><Link to="/catalogos">Configuración</Link> / Registrar usuario</p>
          <h1>Registrar usuario</h1>
          <p className="subtitle">Acceso exclusivo del Administrador. Cree la cuenta, asigne credenciales y defina el rol RBAC.</p>
        </div>
      </div>

      {creado && (
        <div className="aviso-exito" role="status">
          <IconoCheck size={20} />
          <div>
            <strong>Usuario creado: {creado.nombre}</strong>
            <div className="muted">{creado.correo} · {creado.departamentoNombre} · {creado.roles.map((r) => ROL_ETIQUETA[r.nombre] ?? r.nombre).join(', ')}</div>
          </div>
          <Link className="btn btn-sm" to="/catalogos?tab=usuarios">Ver usuarios</Link>
        </div>
      )}

      {error && <div className="alert" role="alert">{error}</div>}

      <form className="card form" onSubmit={enviar} noValidate>
        <h2 className="form-seccion"><IconoUsuarioMas size={18} /> Datos del usuario</h2>
        <div className="form-row">
          <label>Nombre completo<input value={form.nombre} onChange={(e) => set('nombre', e.target.value)} required maxLength={200} /></label>
          <label>Correo institucional<input type="email" value={form.correo} onChange={(e) => set('correo', e.target.value)} required maxLength={200} placeholder="nombre@muniparaiso.go.cr" /></label>
        </div>
        <div className="form-row">
          <label>
            Departamento
            <select value={form.idDepartamento} onChange={(e) => set('idDepartamento', e.target.value ? Number(e.target.value) : '')} required>
              <option value="">Seleccione…</option>
              {departamentos.map((d) => <option key={d.id} value={d.id}>{d.nombre} ({d.sigla})</option>)}
            </select>
          </label>
          <label>Cargo<input value={form.cargo} onChange={(e) => set('cargo', e.target.value)} required maxLength={150} /></label>
        </div>

        <h2 className="form-seccion">Credenciales</h2>
        <div className="form-row">
          <label>
            Contraseña
            <span className="campo-input">
              <input
                type={verPassword ? 'text' : 'password'}
                autoComplete="new-password"
                value={form.password}
                onChange={(e) => set('password', e.target.value)}
                required
                minLength={8}
                maxLength={128}
              />
              <button type="button" className="campo-ojo" onClick={() => setVerPassword((v) => !v)} aria-label={verPassword ? 'Ocultar' : 'Mostrar'}>
                {verPassword ? <IconoOjoCerrado size={18} /> : <IconoOjo size={18} />}
              </button>
            </span>
            <span className="fortaleza" aria-hidden="true">
              {[1, 2, 3, 4].map((n) => <i key={n} className={fortaleza >= n ? `fortaleza-${fortaleza}` : ''} />)}
            </span>
          </label>
          <label>
            Confirmar contraseña
            <input
              type={verPassword ? 'text' : 'password'}
              autoComplete="new-password"
              value={form.confirmarPassword}
              onChange={(e) => set('confirmarPassword', e.target.value)}
              required
            />
          </label>
        </div>
        <p className="hint">Mínimo 8 caracteres. Se recomienda combinar mayúsculas, números y símbolos. El usuario podrá cambiarla desde su perfil.</p>

        <h2 className="form-seccion">Rol y acceso (RBAC)</h2>
        <div className="roles-grid">
          {roles.map((r) => (
            <label key={r.id} className={form.rolIds.includes(r.id) ? 'rol-card rol-card-activa' : 'rol-card'}>
              <input type="checkbox" checked={form.rolIds.includes(r.id)} onChange={() => toggleRol(r.id)} />
              <span>
                <strong>{ROL_ETIQUETA[r.nombre] ?? r.nombre}</strong>
                <small>{ROL_DESCRIPCION[r.nombre] ?? ''}</small>
              </span>
            </label>
          ))}
        </div>
        <label className="check-label">
          <input type="checkbox" checked={form.activo} onChange={(e) => set('activo', e.target.checked)} />
          Cuenta activa (puede iniciar sesión)
        </label>

        <div className="form-actions">
          <button type="button" className="btn" onClick={() => { setForm(vacio()); setError('') }}>Limpiar</button>
          <button type="submit" className="btn btn-primary" disabled={enviando}>{enviando ? 'Creando…' : 'Crear usuario'}</button>
        </div>
      </form>
    </div>
  )
}
