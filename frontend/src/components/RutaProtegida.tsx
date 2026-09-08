import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import type { Permiso } from '../auth/permisos'

/** Exige sesión iniciada. Guarda la ruta destino para volver tras el login. */
export function RequiereSesion() {
  const { usuario, cargando } = useAuth()
  const location = useLocation()

  if (cargando) return <div className="pantalla-carga">Verificando sesión…</div>
  if (!usuario) return <Navigate to="/login" replace state={{ desde: location.pathname }} />
  return <Outlet />
}

/** Exige un permiso de la matriz RBAC; si falta, muestra 403. */
export function RequierePermiso({ permiso }: { permiso: Permiso }) {
  const { puede } = useAuth()
  return puede(permiso) ? <Outlet /> : <Navigate to="/acceso-denegado" replace />
}

/** Rutas públicas que no tienen sentido con sesión activa (login). */
export function SoloAnonimo() {
  const { usuario, cargando } = useAuth()
  if (cargando) return <div className="pantalla-carga">Verificando sesión…</div>
  return usuario ? <Navigate to="/" replace /> : <Outlet />
}
