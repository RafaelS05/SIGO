import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { api, sesion } from '../api'
import type { Usuario } from '../types'
import { tienePermiso, type Permiso } from './permisos'

// Sesión real: JWT emitido por api/Auth/login. Al recargar la app se valida el token
// contra api/Auth/me; si expiró o el usuario fue desactivado, se cierra sesión.
interface AuthContextValue {
  usuario: Usuario | null
  cargando: boolean
  login: (correo: string, password: string) => Promise<Usuario>
  logout: () => void
  refrescar: () => Promise<void>
  roles: string[]
  tieneRol: (...roles: string[]) => boolean
  puede: (permiso: Permiso) => boolean
}

const AuthContext = createContext<AuthContextValue>({
  usuario: null,
  cargando: true,
  login: async () => { throw new Error('AuthProvider ausente') },
  logout: () => {},
  refrescar: async () => {},
  roles: [],
  tieneRol: () => false,
  puede: () => false,
})

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<Usuario | null>(null)
  const [cargando, setCargando] = useState(true)

  const logout = useCallback(() => {
    sesion.setToken(null)
    setUsuario(null)
  }, [])

  const refrescar = useCallback(async () => {
    if (!sesion.getToken()) {
      setUsuario(null)
      return
    }
    try {
      setUsuario(await api.auth.me())
    } catch {
      logout()
    }
  }, [logout])

  useEffect(() => {
    refrescar().finally(() => setCargando(false))
  }, [refrescar])

  // Cualquier 401 recibido por el cliente API cierra la sesión.
  useEffect(() => {
    window.addEventListener('sigo:no-autorizado', logout)
    return () => window.removeEventListener('sigo:no-autorizado', logout)
  }, [logout])

  const login = useCallback(async (correo: string, password: string) => {
    const s = await api.auth.login(correo, password)
    sesion.setToken(s.token)
    setUsuario(s.usuario)
    return s.usuario
  }, [])

  const roles = useMemo(() => usuario?.roles.map((r) => r.nombre) ?? [], [usuario])

  const value = useMemo<AuthContextValue>(() => ({
    usuario,
    cargando,
    login,
    logout,
    refrescar,
    roles,
    tieneRol: (...rs) => rs.some((r) => roles.includes(r)),
    puede: (permiso) => tienePermiso(roles, permiso),
  }), [usuario, cargando, login, logout, refrescar, roles])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export const useAuth = () => useContext(AuthContext)
