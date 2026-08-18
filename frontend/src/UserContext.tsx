import { createContext, useContext, useEffect, useState, type ReactNode } from 'react'
import { api } from './api'
import type { Usuario } from './types'

// Simula la sesión corporativa (Azure AD en producción): el usuario activo se
// elige en la barra superior y se conserva en localStorage.
interface UserContextValue {
  usuarios: Usuario[]
  usuarioActual: Usuario | null
  setUsuarioActual: (u: Usuario) => void
  recargarUsuarios: () => void
}

const UserContext = createContext<UserContextValue>({
  usuarios: [],
  usuarioActual: null,
  setUsuarioActual: () => {},
  recargarUsuarios: () => {},
})

export function UserProvider({ children }: { children: ReactNode }) {
  const [usuarios, setUsuarios] = useState<Usuario[]>([])
  const [usuarioActual, setUsuario] = useState<Usuario | null>(null)

  const cargar = () => {
    api.usuarios.list().then((lista) => {
      setUsuarios(lista)
      const guardado = Number(localStorage.getItem('sigo.usuarioId'))
      const actual = lista.find((u) => u.id === guardado) ?? lista[0] ?? null
      setUsuario(actual)
    })
  }

  useEffect(cargar, [])

  const setUsuarioActual = (u: Usuario) => {
    localStorage.setItem('sigo.usuarioId', String(u.id))
    setUsuario(u)
  }

  return (
    <UserContext.Provider value={{ usuarios, usuarioActual, setUsuarioActual, recargarUsuarios: cargar }}>
      {children}
    </UserContext.Provider>
  )
}

export const useUser = () => useContext(UserContext)
