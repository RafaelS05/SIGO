import type { SVGProps } from 'react'

// Íconos de línea (estilo del machote), 24x24, heredan `currentColor`.
type P = SVGProps<SVGSVGElement> & { size?: number }

const base = ({ size = 20, ...rest }: P): SVGProps<SVGSVGElement> => ({
  width: size,
  height: size,
  viewBox: '0 0 24 24',
  fill: 'none',
  stroke: 'currentColor',
  strokeWidth: 2,
  strokeLinecap: 'round',
  strokeLinejoin: 'round',
  'aria-hidden': true,
  ...rest,
})

export const IconoInicio = (p: P) => (
  <svg {...base(p)}><path d="M3 11 12 3l9 8" /><path d="M5 10v10h5v-6h4v6h5V10" /></svg>
)
export const IconoBandeja = (p: P) => (
  <svg {...base(p)}><path d="M3 7h18v12H3z" /><path d="m3 7 9 6 9-6" /></svg>
)
export const IconoDepartamento = (p: P) => (
  <svg {...base(p)}><path d="M3 21V5a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v16" /><path d="M15 9h4a2 2 0 0 1 2 2v10" /><path d="M7 7h2M7 11h2M7 15h2M18 13h1M18 17h1M3 21h18" /></svg>
)
export const IconoReloj = (p: P) => (
  <svg {...base(p)}><circle cx="12" cy="12" r="9" /><path d="M12 7v5l3 2" /></svg>
)
export const IconoDocumento = (p: P) => (
  <svg {...base(p)}><path d="M6 2h8l5 5v15H6z" /><path d="M14 2v5h5M9 13h6M9 17h6" /></svg>
)
export const IconoMas = (p: P) => (
  <svg {...base(p)}><path d="M12 5v14M5 12h14" /></svg>
)
export const IconoMasCirculo = (p: P) => (
  <svg {...base(p)}><circle cx="12" cy="12" r="9" /><path d="M12 8v8M8 12h8" /></svg>
)
export const IconoReportes = (p: P) => (
  <svg {...base(p)}><path d="M4 20V10M10 20V4M16 20v-7M22 20H2" /></svg>
)
export const IconoConfiguracion = (p: P) => (
  <svg {...base(p)}><circle cx="12" cy="12" r="3" /><path d="M19.4 15a1.7 1.7 0 0 0 .3 1.8l.1.1a2 2 0 1 1-2.8 2.8l-.1-.1a1.7 1.7 0 0 0-1.8-.3 1.7 1.7 0 0 0-1 1.5V21a2 2 0 1 1-4 0v-.1a1.7 1.7 0 0 0-1.1-1.5 1.7 1.7 0 0 0-1.8.3l-.1.1a2 2 0 1 1-2.8-2.8l.1-.1a1.7 1.7 0 0 0 .3-1.8 1.7 1.7 0 0 0-1.5-1H3a2 2 0 1 1 0-4h.1a1.7 1.7 0 0 0 1.5-1.1 1.7 1.7 0 0 0-.3-1.8l-.1-.1a2 2 0 1 1 2.8-2.8l.1.1a1.7 1.7 0 0 0 1.8.3H9a1.7 1.7 0 0 0 1-1.5V3a2 2 0 1 1 4 0v.1a1.7 1.7 0 0 0 1 1.5 1.7 1.7 0 0 0 1.8-.3l.1-.1a2 2 0 1 1 2.8 2.8l-.1.1a1.7 1.7 0 0 0-.3 1.8V9a1.7 1.7 0 0 0 1.5 1H21a2 2 0 1 1 0 4h-.1a1.7 1.7 0 0 0-1.5 1z" /></svg>
)
export const IconoUsuarios = (p: P) => (
  <svg {...base(p)}><circle cx="9" cy="8" r="4" /><path d="M2 21a7 7 0 0 1 14 0" /><path d="M16 4a4 4 0 0 1 0 8M22 21a7 7 0 0 0-5-6.7" /></svg>
)
export const IconoUsuarioMas = (p: P) => (
  <svg {...base(p)}><circle cx="10" cy="8" r="4" /><path d="M3 21a7 7 0 0 1 14 0" /><path d="M19 8v6M16 11h6" /></svg>
)
export const IconoCampana = (p: P) => (
  <svg {...base(p)}><path d="M6 8a6 6 0 0 1 12 0v5l2 3H4l2-3z" /><path d="M10 20a2 2 0 0 0 4 0" /></svg>
)
export const IconoAccesibilidad = (p: P) => (
  <svg {...base(p)}><circle cx="12" cy="12" r="10" /><circle cx="12" cy="7" r="1.3" fill="currentColor" /><path d="M7 10h10M12 10v4l-2.5 5M12 14l2.5 5" /></svg>
)
export const IconoUsuario = (p: P) => (
  <svg {...base(p)}><circle cx="12" cy="8" r="4" /><path d="M4 21a8 8 0 0 1 16 0" /></svg>
)
export const IconoChevron = (p: P) => (
  <svg {...base(p)}><path d="m6 9 6 6 6-6" /></svg>
)
export const IconoBuscar = (p: P) => (
  <svg {...base(p)}><circle cx="11" cy="11" r="7" /><path d="m20 20-3.5-3.5" /></svg>
)
export const IconoFiltro = (p: P) => (
  <svg {...base(p)}><path d="M3 5h18l-7 8v6l-4 2v-8z" /></svg>
)
export const IconoAlerta = (p: P) => (
  <svg {...base(p)}><path d="M12 3 2 21h20z" /><path d="M12 10v5M12 18h.01" /></svg>
)
export const IconoEngranaje = (p: P) => (
  <svg {...base(p)}><circle cx="12" cy="12" r="3.5" /><path d="M12 2v3M12 19v3M2 12h3M19 12h3M4.9 4.9l2.1 2.1M17 17l2.1 2.1M4.9 19.1 7 17M17 7l2.1-2.1" /></svg>
)
export const IconoCheck = (p: P) => (
  <svg {...base(p)}><path d="m5 12 5 5L20 7" /></svg>
)
export const IconoClip = (p: P) => (
  <svg {...base(p)}><path d="m21 11-8.5 8.5a5 5 0 0 1-7-7L14 4a3.3 3.3 0 0 1 4.7 4.7L10.5 17a1.7 1.7 0 0 1-2.4-2.4L16 6.7" /></svg>
)
export const IconoIntercambio = (p: P) => (
  <svg {...base(p)}><path d="M4 7h13l-3-3M20 17H7l3 3" /></svg>
)
export const IconoCandado = (p: P) => (
  <svg {...base(p)}><rect x="4" y="11" width="16" height="10" rx="2" /><path d="M8 11V7a4 4 0 0 1 8 0v4" /></svg>
)
export const IconoCorreo = (p: P) => (
  <svg {...base(p)}><rect x="3" y="5" width="18" height="14" rx="2" /><path d="m3 7 9 6 9-6" /></svg>
)
export const IconoSalir = (p: P) => (
  <svg {...base(p)}><path d="M10 17l5-5-5-5M15 12H3" /><path d="M13 3h6v18h-6" /></svg>
)
export const IconoOjo = (p: P) => (
  <svg {...base(p)}><path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12z" /><circle cx="12" cy="12" r="3" /></svg>
)
export const IconoOjoCerrado = (p: P) => (
  <svg {...base(p)}><path d="M3 3l18 18M10.6 10.6A3 3 0 0 0 13.4 13.4" /><path d="M9.9 5.2A10.4 10.4 0 0 1 12 5c6.5 0 10 7 10 7a17 17 0 0 1-3.2 4.1M6.6 6.6C3.9 8.4 2 12 2 12s3.5 7 10 7c1.6 0 3-.4 4.3-1" /></svg>
)
export const IconoFlechaArriba = (p: P) => (
  <svg {...base(p)}><path d="M12 19V5M5 12l7-7 7 7" /></svg>
)
export const IconoPuntos = (p: P) => (
  <svg {...base(p)}><circle cx="12" cy="5" r="1.5" fill="currentColor" /><circle cx="12" cy="12" r="1.5" fill="currentColor" /><circle cx="12" cy="19" r="1.5" fill="currentColor" /></svg>
)

/** Escudo institucional simplificado (sustituir por el logo oficial en public/escudo.png). */
export const Escudo = ({ size = 44 }: { size?: number }) => (
  <svg width={size} height={size} viewBox="0 0 48 48" aria-hidden="true">
    <path d="M24 3 6 9v14c0 11 8 19 18 22 10-3 18-11 18-22V9z" fill="#1d5fbf" />
    <path d="M24 7 10 11.5V23c0 9 6.3 15.4 14 18 7.7-2.6 14-9 14-18V11.5z" fill="#fff" />
    <path d="M24 12 14 15.5V23c0 6.6 4.5 11.3 10 13.4 5.5-2.1 10-6.8 10-13.4v-7.5z" fill="#e8b10c" />
    <path d="M24 16.5 18 18.6V23c0 4 2.7 7 6 8.4 3.3-1.4 6-4.4 6-8.4v-4.4z" fill="#1d5fbf" />
    <path d="M21 26h6v6h-6z" fill="#fff" />
  </svg>
)

/** Logo SIGO (documento con engranaje) del machote. */
export const LogoSigo = ({ size = 44 }: { size?: number }) => (
  <svg width={size} height={size} viewBox="0 0 48 48" aria-hidden="true" fill="none" stroke="#1d5fbf" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round">
    <path d="M12 5h16l8 8v18" /><path d="M28 5v8h8" /><path d="M12 5v38h14" /><path d="M18 20h12M18 27h8" />
    <circle cx="35" cy="38" r="5" /><path d="M35 30v2M35 44v2M27 38h2M41 38h2M29.3 32.3l1.4 1.4M39.3 42.3l1.4 1.4M29.3 43.7l1.4-1.4M39.3 33.7l1.4-1.4" strokeWidth="2.5" />
  </svg>
)
