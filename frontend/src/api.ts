import type {
  Canal, Departamento, Estado, Oficio, OficioFiltros,
  Prioridad, RegistroUsuario, Rol, Sesion, TipoOficio, Usuario,
} from './types'

// ---- Sesión (token JWT) ----
// El token vive en localStorage; AuthContext lo carga y lo limpia. Ante un 401 el cliente
// dispara `sigo:no-autorizado` para que la app cierre sesión y vuelva al login.
const TOKEN_KEY = 'sigo.token'

export const sesion = {
  getToken: () => localStorage.getItem(TOKEN_KEY),
  setToken: (t: string | null) => (t ? localStorage.setItem(TOKEN_KEY, t) : localStorage.removeItem(TOKEN_KEY)),
}

export class ApiError extends Error {
  status: number
  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

function conAuth(options: RequestInit = {}): RequestInit {
  const token = sesion.getToken()
  const headers = new Headers(options.headers)
  if (token) headers.set('Authorization', `Bearer ${token}`)
  return { ...options, headers }
}

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(url, conAuth(options))
  if (!res.ok) {
    let detail = `${res.status} ${res.statusText}`
    if (res.status === 403) detail = 'No tiene permisos para realizar esta acción.'
    try {
      const body = await res.json()
      if (body?.errors) detail = Object.values(body.errors).flat().join(' ')
      else if (body?.title) detail = body.title
    } catch {
      /* respuesta sin cuerpo JSON */
    }
    if (res.status === 401 && sesion.getToken()) {
      window.dispatchEvent(new CustomEvent('sigo:no-autorizado'))
    }
    throw new ApiError(res.status, detail)
  }
  if (res.status === 204) return undefined as T
  return res.json()
}

const json = (method: string, body: unknown): RequestInit => ({
  method,
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(body),
})

export const api = {
  auth: {
    login: (correo: string, password: string) => request<Sesion>('/api/Auth/login', json('POST', { correo, password })),
    me: () => request<Usuario>('/api/Auth/me'),
    registrar: (datos: RegistroUsuario) => request<Usuario>('/api/Auth/registro', json('POST', datos)),
    cambiarPassword: (passwordActual: string, passwordNueva: string, confirmarPassword: string) =>
      request<void>('/api/Auth/cambiar-password', json('POST', { passwordActual, passwordNueva, confirmarPassword })),
    restablecerPassword: (idUsuario: number, passwordNueva: string) =>
      request<void>(`/api/Auth/usuarios/${idUsuario}/restablecer-password`, json('POST', { passwordNueva })),
  },
  departamentos: {
    list: () => request<Departamento[]>('/api/Departamentos'),
    create: (d: Omit<Departamento, 'id'>) => request<Departamento>('/api/Departamentos', json('POST', d)),
    update: (id: number, d: Omit<Departamento, 'id'>) => request<Departamento>(`/api/Departamentos/${id}`, json('PUT', d)),
    remove: (id: number) => request<void>(`/api/Departamentos/${id}`, { method: 'DELETE' }),
  },
  roles: {
    list: () => request<Rol[]>('/api/Roles'),
  },
  usuarios: {
    list: () => request<Usuario[]>('/api/Usuarios'),
    create: (u: object) => request<Usuario>('/api/Usuarios', json('POST', u)),
    update: (id: number, u: object) => request<Usuario>(`/api/Usuarios/${id}`, json('PUT', u)),
    remove: (id: number) => request<void>(`/api/Usuarios/${id}`, { method: 'DELETE' }),
  },
  prioridades: {
    list: () => request<Prioridad[]>('/api/Prioridades'),
    create: (p: Omit<Prioridad, 'id'>) => request<Prioridad>('/api/Prioridades', json('POST', p)),
    update: (id: number, p: Omit<Prioridad, 'id'>) => request<Prioridad>(`/api/Prioridades/${id}`, json('PUT', p)),
    remove: (id: number) => request<void>(`/api/Prioridades/${id}`, { method: 'DELETE' }),
  },
  estados: {
    list: () => request<Estado[]>('/api/Estados'),
    create: (e: Omit<Estado, 'id'>) => request<Estado>('/api/Estados', json('POST', e)),
    update: (id: number, e: Omit<Estado, 'id'>) => request<Estado>(`/api/Estados/${id}`, json('PUT', e)),
    remove: (id: number) => request<void>(`/api/Estados/${id}`, { method: 'DELETE' }),
  },
  tiposOficios: {
    list: () => request<TipoOficio[]>('/api/TiposOficios'),
    create: (t: Omit<TipoOficio, 'id'>) => request<TipoOficio>('/api/TiposOficios', json('POST', t)),
    update: (id: number, t: Omit<TipoOficio, 'id'>) => request<TipoOficio>(`/api/TiposOficios/${id}`, json('PUT', t)),
    remove: (id: number) => request<void>(`/api/TiposOficios/${id}`, { method: 'DELETE' }),
  },
  canales: {
    list: () => request<Canal[]>('/api/Canales'),
    create: (c: Omit<Canal, 'id'>) => request<Canal>('/api/Canales', json('POST', c)),
    update: (id: number, c: Omit<Canal, 'id'>) => request<Canal>(`/api/Canales/${id}`, json('PUT', c)),
    remove: (id: number) => request<void>(`/api/Canales/${id}`, { method: 'DELETE' }),
  },
  oficios: {
    list: (filtros: OficioFiltros = {}) => {
      const params = new URLSearchParams()
      if (filtros.estadoId) params.set('estadoId', String(filtros.estadoId))
      if (filtros.prioridadId) params.set('prioridadId', String(filtros.prioridadId))
      if (filtros.departamentoId) params.set('departamentoId', String(filtros.departamentoId))
      if (filtros.responsableId) params.set('responsableId', String(filtros.responsableId))
      if (filtros.busqueda) params.set('busqueda', filtros.busqueda)
      if (filtros.desde) params.set('desde', filtros.desde)
      if (filtros.hasta) params.set('hasta', filtros.hasta)
      if (filtros.atrasados) params.set('atrasados', 'true')
      const qs = params.toString()
      return request<Oficio[]>(`/api/Oficios${qs ? `?${qs}` : ''}`)
    },
    get: (id: number) => request<Oficio>(`/api/Oficios/${id}`),
    create: (o: object) => request<Oficio>('/api/Oficios', json('POST', o)),
    update: (id: number, o: object) => request<Oficio>(`/api/Oficios/${id}`, json('PUT', o)),
    remove: (id: number) => request<void>(`/api/Oficios/${id}`, { method: 'DELETE' }),
    // El autor de cada acción lo toma el backend del token; no viaja en el cuerpo.
    cambiarEstado: (id: number, idEstadoDestino: number, comentario: string) =>
      request<Oficio>(`/api/Oficios/${id}/estado`, json('POST', { idEstadoDestino, comentario })),
    asignar: (id: number, idUsuario: number, comentario: string) =>
      request<Oficio>(`/api/Oficios/${id}/asignar`, json('POST', { idUsuario, comentario })),
    comentar: (id: number, comentario: string) =>
      request<Oficio>(`/api/Oficios/${id}/comentarios`, json('POST', { comentario })),
    subirAdjunto: (id: number, archivo: File) => {
      const form = new FormData()
      form.append('archivo', archivo)
      return request<Oficio>(`/api/Oficios/${id}/adjuntos`, { method: 'POST', body: form })
    },
    // La descarga necesita el header Authorization, así que se baja como blob y se abre.
    descargarAdjunto: async (id: number, idAdjunto: number, nombreArchivo: string) => {
      const res = await fetch(`/api/Oficios/${id}/adjuntos/${idAdjunto}`, conAuth())
      if (!res.ok) throw new ApiError(res.status, 'No se pudo descargar el adjunto.')
      const url = URL.createObjectURL(await res.blob())
      const a = document.createElement('a')
      a.href = url
      a.download = nombreArchivo
      document.body.appendChild(a)
      a.click()
      a.remove()
      setTimeout(() => URL.revokeObjectURL(url), 10_000)
    },
  },
}
