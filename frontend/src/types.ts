export interface Departamento {
  id: number
  nombre: string
  sigla: string
  activo: boolean
}

export interface Rol {
  id: number
  nombre: string
}

export interface UsuarioRol {
  idRol: number
  nombre: string
}

export interface Usuario {
  id: number
  nombre: string
  correo: string
  idDepartamento: number
  departamentoNombre: string
  cargo: string
  activo: boolean
  origenAd: boolean
  roles: UsuarioRol[]
}

export interface Prioridad {
  id: number
  nombre: string
  nivel: number
  slaHoras: number
}

export interface Estado {
  id: number
  nombre: string
  cerradoFlag: boolean
}

export interface TipoOficio {
  id: number
  nombre: string
  requiereRespuestaFlag: boolean
}

export interface Canal {
  id: number
  nombre: string
}

export interface OficioResponsable {
  id: number
  idUsuario: number
  usuarioNombre: string
  activo: boolean
  asignadoEn: string
  asignadoPorNombre: string
}

export interface OficioBitacora {
  id: number
  estadoOrigenNombre: string
  estadoDestinoNombre: string
  comentario: string
  usuarioNombre: string
  fechaEvento: string
}

export interface OficioAdjunto {
  id: number
  nombreArchivo: string
  pesoBytes: number
  version: number
  subidoPorNombre: string
  subidoEn: string
}

export interface Oficio {
  id: number
  numero: string
  anio: number
  direccion: string
  idTipoOficio: number
  tipoOficioNombre: string
  idDepartamentoOrigen: number | null
  departamentoOrigenNombre: string
  idDepartamentoDestino: number
  departamentoDestinoNombre: string
  remitente: string
  destinatario: string
  asunto: string
  descripcion: string
  idPrioridad: number
  prioridadNombre: string
  idEstadoActual: number
  estadoNombre: string
  estadoCerrado: boolean
  idCanal: number
  canalNombre: string
  fechaIngreso: string
  fechaVencimientoSla: string
  fechaCierre: string | null
  atrasado: boolean
  creadoPor: number
  creadoPorNombre: string
  creadoEn: string
  responsableActual: string
  idResponsableActual: number | null
  responsables: OficioResponsable[]
  bitacora: OficioBitacora[]
  adjuntos: OficioAdjunto[]
}

export interface OficioFiltros {
  estadoId?: number
  prioridadId?: number
  departamentoId?: number
  responsableId?: number
  busqueda?: string
  desde?: string
  hasta?: string
  atrasados?: boolean
}
