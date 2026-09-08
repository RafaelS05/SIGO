import { Link } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { ROL_ETIQUETA } from '../auth/permisos'
import { IconoCandado } from '../components/Icons'

export default function AccesoDenegadoPage() {
  const { roles } = useAuth()
  return (
    <div className="empty empty-grande">
      <IconoCandado size={40} />
      <h1>Acceso denegado</h1>
      <p>
        Su rol ({roles.map((r) => ROL_ETIQUETA[r] ?? r).join(', ') || 'sin rol'}) no tiene permiso para esta sección.
        Si cree que es un error, contacte al Administrador del sistema.
      </p>
      <Link className="btn btn-primary" to="/">Volver al inicio</Link>
    </div>
  )
}
