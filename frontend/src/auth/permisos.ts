// Réplica de backend/Auth/Permisos.cs. Sirve para ocultar navegación y acciones;
// la autoridad real es el backend (cada endpoint valida su policy).

export const ROLES = {
  Administrador: 'Administrador',
  MesaEntrada: 'MesaEntrada',
  Jefatura: 'Jefatura',
  Analista: 'Analista',
  Auditor: 'Auditor',
} as const

export type Rol = (typeof ROLES)[keyof typeof ROLES]

export const PERMISOS = {
  AdministrarUsuarios: 'AdministrarUsuarios',
  AdministrarCatalogos: 'AdministrarCatalogos',
  VerTodosLosOficios: 'VerTodosLosOficios',
  CrearOficios: 'CrearOficios',
  EditarOficios: 'EditarOficios',
  EliminarOficios: 'EliminarOficios',
  AsignarOficios: 'AsignarOficios',
  TramitarOficios: 'TramitarOficios',
  VerReportes: 'VerReportes',
} as const

export type Permiso = (typeof PERMISOS)[keyof typeof PERMISOS]

export const MATRIZ: Record<Permiso, Rol[]> = {
  AdministrarUsuarios: [ROLES.Administrador],
  AdministrarCatalogos: [ROLES.Administrador],
  VerTodosLosOficios: [ROLES.Administrador, ROLES.Auditor],
  CrearOficios: [ROLES.Administrador, ROLES.MesaEntrada, ROLES.Jefatura],
  EditarOficios: [ROLES.Administrador, ROLES.MesaEntrada, ROLES.Jefatura],
  EliminarOficios: [ROLES.Administrador],
  AsignarOficios: [ROLES.Administrador, ROLES.Jefatura],
  TramitarOficios: [ROLES.Administrador, ROLES.MesaEntrada, ROLES.Jefatura, ROLES.Analista],
  VerReportes: [ROLES.Administrador, ROLES.Jefatura, ROLES.Auditor],
}

export const ROL_ETIQUETA: Record<string, string> = {
  Administrador: 'Administrador',
  MesaEntrada: 'Mesa de entrada',
  Jefatura: 'Jefatura',
  Analista: 'Analista',
  Auditor: 'Auditor',
}

export const ROL_DESCRIPCION: Record<string, string> = {
  Administrador: 'Acceso total: usuarios, catálogos, todos los oficios y reportes.',
  MesaEntrada: 'Registra oficios entrantes/salientes y les da trámite en su departamento.',
  Jefatura: 'Asigna responsables, tramita oficios de su departamento y consulta reportes.',
  Analista: 'Atiende los oficios que se le asignan: cambia estado, comenta y adjunta.',
  Auditor: 'Solo lectura sobre todos los oficios y reportes.',
}

export function tienePermiso(roles: string[], permiso: Permiso): boolean {
  return MATRIZ[permiso].some((r) => roles.includes(r))
}
