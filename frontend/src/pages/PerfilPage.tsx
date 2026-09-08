import { useState, type FormEvent } from 'react'
import { api } from '../api'
import { useAuth } from '../auth/AuthContext'
import { MATRIZ, PERMISOS, ROL_ETIQUETA, type Permiso } from '../auth/permisos'
import { fmtFecha } from '../components/Badges'
import { IconoCheck } from '../components/Icons'

const PERMISO_ETIQUETA: Record<Permiso, string> = {
  AdministrarUsuarios: 'Administrar usuarios',
  AdministrarCatalogos: 'Administrar catálogos',
  VerTodosLosOficios: 'Ver todos los oficios',
  CrearOficios: 'Registrar oficios',
  EditarOficios: 'Editar oficios',
  EliminarOficios: 'Eliminar oficios',
  AsignarOficios: 'Asignar responsables',
  TramitarOficios: 'Tramitar (estado, comentarios, adjuntos)',
  VerReportes: 'Ver reportes',
}

export default function PerfilPage() {
  const { usuario, roles, puede } = useAuth()
  const [actual, setActual] = useState('')
  const [nueva, setNueva] = useState('')
  const [confirmar, setConfirmar] = useState('')
  const [error, setError] = useState('')
  const [ok, setOk] = useState(false)

  const cambiar = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setOk(false)
    if (nueva.length < 8) return setError('La nueva contraseña debe tener al menos 8 caracteres.')
    if (nueva !== confirmar) return setError('Las contraseñas no coinciden.')
    try {
      await api.auth.cambiarPassword(actual, nueva, confirmar)
      setOk(true)
      setActual(''); setNueva(''); setConfirmar('')
    } catch (err) {
      setError(err instanceof Error ? err.message : String(err))
    }
  }

  if (!usuario) return null

  return (
    <div>
      <h1>Mi perfil</h1>
      <p className="subtitle">Datos de la cuenta, permisos vigentes y cambio de contraseña.</p>

      <div className="detail-grid">
        <section className="card">
          <h2>Cuenta</h2>
          <dl className="ficha">
            <dt>Nombre</dt><dd>{usuario.nombre}</dd>
            <dt>Correo</dt><dd>{usuario.correo}</dd>
            <dt>Departamento</dt><dd>{usuario.departamentoNombre}</dd>
            <dt>Cargo</dt><dd>{usuario.cargo}</dd>
            <dt>Roles</dt><dd>{roles.map((r) => <span key={r} className="badge badge-rol">{ROL_ETIQUETA[r] ?? r}</span>)}</dd>
            <dt>Último acceso</dt><dd>{fmtFecha(usuario.ultimoAcceso)}</dd>
          </dl>
          <h3 style={{ marginTop: '1rem' }}>Permisos (RBAC)</h3>
          <ul className="lista-permisos">
            {(Object.keys(MATRIZ) as Permiso[]).map((p) => (
              <li key={p} className={puede(p) ? 'permiso-si' : 'permiso-no'}>
                {puede(p) ? <IconoCheck size={16} /> : <span className="permiso-punto" />}
                {PERMISO_ETIQUETA[p] ?? PERMISOS[p]}
              </li>
            ))}
          </ul>
        </section>

        <section className="card">
          <h2>Cambiar contraseña</h2>
          {error && <div className="alert">{error}</div>}
          {ok && <div className="aviso-exito"><IconoCheck size={18} /> Contraseña actualizada.</div>}
          <form className="form" onSubmit={cambiar}>
            <label>Contraseña actual<input type="password" autoComplete="current-password" value={actual} onChange={(e) => setActual(e.target.value)} required /></label>
            <label>Nueva contraseña<input type="password" autoComplete="new-password" value={nueva} onChange={(e) => setNueva(e.target.value)} required minLength={8} /></label>
            <label>Confirmar nueva contraseña<input type="password" autoComplete="new-password" value={confirmar} onChange={(e) => setConfirmar(e.target.value)} required /></label>
            <div className="form-actions">
              <button className="btn btn-primary" type="submit" disabled={!actual || !nueva || !confirmar}>Actualizar</button>
            </div>
          </form>
        </section>
      </div>
    </div>
  )
}
