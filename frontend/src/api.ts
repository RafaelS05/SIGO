import type {
  Canal, Departamento, Estado, Oficio, OficioFiltros,
  Prioridad, Rol, TipoOficio, Usuario,
} from './types'

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(url, options)
  if (!res.ok) {
    let detail = `${res.status} ${res.statusText}`
    try {
      const body = await res.json()
      if (body?.errors) detail = Object.values(body.errors).flat().join(' ')
      else if (body?.title) detail = body.title
    } catch {
      /* respuesta sin cuerpo JSON */
    }
    throw new Error(detail)
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
    cambiarEstado: (id: number, idEstadoDestino: number, comentario: string, idUsuario: number) =>
      request<Oficio>(`/api/Oficios/${id}/estado`, json('POST', { idEstadoDestino, comentario, idUsuario })),
    asignar: (id: number, idUsuario: number, asignadoPor: number, comentario: string) =>
      request<Oficio>(`/api/Oficios/${id}/asignar`, json('POST', { idUsuario, asignadoPor, comentario })),
    comentar: (id: number, comentario: string, idUsuario: number) =>
      request<Oficio>(`/api/Oficios/${id}/comentarios`, json('POST', { comentario, idUsuario })),
    subirAdjunto: (id: number, archivo: File, subidoPor: number) => {
      const form = new FormData()
      form.append('archivo', archivo)
      form.append('subidoPor', String(subidoPor))
      return request<Oficio>(`/api/Oficios/${id}/adjuntos`, { method: 'POST', body: form })
    },
    urlAdjunto: (id: number, idAdjunto: number) => `/api/Oficios/${id}/adjuntos/${idAdjunto}`,
  },
}
