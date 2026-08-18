import type { Oficio } from '../types'

const ESTADO_CLASE: Record<string, string> = {
  'Recibido': 'badge badge-recibido',
  'Asignado': 'badge badge-asignado',
  'En proceso': 'badge badge-proceso',
  'Pendiente de respuesta': 'badge badge-pendiente',
  'Enviado a respuesta': 'badge badge-enviado',
  'Cerrado': 'badge badge-cerrado',
  'Rechazado/Devolución': 'badge badge-rechazado',
}

export function EstadoBadge({ nombre }: { nombre: string }) {
  return <span className={ESTADO_CLASE[nombre] ?? 'badge'}>{nombre}</span>
}

export function PrioridadBadge({ nombre, nivel }: { nombre: string; nivel?: number }) {
  const clase = nivel === 1 ? 'badge badge-p-urgente' : nivel === 2 ? 'badge badge-p-alta' : 'badge badge-p-normal'
  return <span className={clase}>{nombre}</span>
}

export function SlaBadge({ oficio }: { oficio: Oficio }) {
  if (oficio.estadoCerrado) return <span className="badge badge-cerrado">Cerrado</span>
  if (oficio.atrasado) return <span className="badge badge-atrasado">⚠ Vencido</span>
  const horas = (new Date(oficio.fechaVencimientoSla).getTime() - Date.now()) / 36e5
  if (horas <= 48) return <span className="badge badge-porvencer">Vence en {Math.max(1, Math.round(horas))} h</span>
  return <span className="badge badge-entiempo">En tiempo</span>
}

export function fmtFecha(iso: string | null): string {
  if (!iso) return '—'
  return new Date(iso).toLocaleString('es-CR', { dateStyle: 'short', timeStyle: 'short' })
}

export function fmtFechaCorta(iso: string | null): string {
  if (!iso) return '—'
  return new Date(iso).toLocaleDateString('es-CR', { dateStyle: 'medium' })
}
