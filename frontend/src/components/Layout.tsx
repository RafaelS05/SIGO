import { useEffect, useRef, useState } from 'react'
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../auth/AuthContext'
import { PERMISOS, ROLES, ROL_ETIQUETA } from '../auth/permisos'
import {
  Escudo, IconoAccesibilidad, IconoBandeja, IconoCampana, IconoChevron, IconoConfiguracion,
  IconoDepartamento, IconoDocumento, IconoInicio, IconoMasCirculo, IconoReloj, IconoReportes,
  IconoSalir, IconoUsuario, IconoUsuarioMas, LogoSigo,
} from './Icons'

/** Cascarón de la aplicación (machote): cabecera institucional, barra lateral por rol y pie. */
export default function Layout() {
  const { usuario, puede, tieneRol, logout } = useAuth()
  const navigate = useNavigate()
  const [menuAbierto, setMenuAbierto] = useState(false)
  const [alertas, setAlertas] = useState(0)
  const menuRef = useRef<HTMLDivElement>(null)

  const esAuditor = tieneRol(ROLES.Auditor) && !tieneRol(ROLES.Administrador)

  // Campana: oficios atrasados dentro del alcance del usuario.
  useEffect(() => {
    api.oficios.list({ atrasados: true }).then((o) => setAlertas(o.length)).catch(() => setAlertas(0))
  }, [usuario?.id])

  useEffect(() => {
    if (!menuAbierto) return
    const cerrar = (e: MouseEvent) => {
      if (!menuRef.current?.contains(e.target as Node)) setMenuAbierto(false)
    }
    document.addEventListener('mousedown', cerrar)
    return () => document.removeEventListener('mousedown', cerrar)
  }, [menuAbierto])

  const salir = () => {
    logout()
    navigate('/login', { replace: true })
  }

  const rolPrincipal = usuario?.roles[0]?.nombre ?? ''

  return (
    <div className="app">
      <header className="cabecera">
        <div className="cabecera-marca">
          <Link to="/" className="marca-muni" aria-label="Municipalidad de Paraíso">
            <Escudo size={52} />
            <div className="marca-muni-texto">
              <span className="marca-muni-sup">Municipalidad de</span>
              <span className="marca-muni-nombre">PARAÍSO</span>
              <span className="marca-muni-lema">Juntos construimos un mejor cantón</span>
            </div>
          </Link>
          <span className="marca-separador" aria-hidden="true" />
          <Link to="/" className="marca-sigo" aria-label="SIGO, Sistema de Gestión de Oficios">
            <LogoSigo size={46} />
            <div>
              <span className="marca-sigo-nombre">SIGO</span>
              <span className="marca-sigo-sub">Sistema de Gestión de Oficios</span>
            </div>
          </Link>
        </div>

        <div className="cabecera-panorama" aria-hidden="true" />

        <div className="cabecera-acciones">
          <span className="cabecera-eslogan" aria-hidden="true">
            <em>Paraíso</em>
            <small>somos todos</small>
          </span>
          <Link to="/bandeja/atrasados" className="icono-boton" aria-label={`${alertas} oficios atrasados`} title="Oficios atrasados">
            <IconoCampana size={24} />
            {alertas > 0 && <span className="insignia-alerta">{alertas > 99 ? '99+' : alertas}</span>}
          </Link>
          <button type="button" className="icono-boton" aria-label="Accesibilidad" title="Accesibilidad" onClick={() => document.documentElement.classList.toggle('texto-grande')}>
            <IconoAccesibilidad size={24} />
          </button>

          <div className="usuario-menu" ref={menuRef}>
            <button type="button" className="usuario-boton" onClick={() => setMenuAbierto((v) => !v)} aria-expanded={menuAbierto} aria-haspopup="menu">
              <span className="avatar"><IconoUsuario size={22} /></span>
              <span className="usuario-texto">
                <strong>{usuario?.nombre ?? 'Usuario'}</strong>
                <small>{usuario?.departamentoNombre ?? ''}</small>
              </span>
              <IconoChevron size={18} className="usuario-chevron" />
            </button>
            {menuAbierto && (
              <div className="usuario-desplegable" role="menu">
                <div className="usuario-desplegable-cab">
                  <strong>{usuario?.nombre}</strong>
                  <small>{usuario?.correo}</small>
                  <span className="badge badge-rol">{ROL_ETIQUETA[rolPrincipal] ?? rolPrincipal}</span>
                </div>
                <Link to="/perfil" role="menuitem" onClick={() => setMenuAbierto(false)}>
                  <IconoUsuario size={18} /> Mi perfil y contraseña
                </Link>
                <button type="button" role="menuitem" onClick={salir}>
                  <IconoSalir size={18} /> Cerrar sesión
                </button>
              </div>
            )}
          </div>
        </div>
      </header>

      <div className="cuerpo">
        <aside className="lateral">
          <nav aria-label="Principal">
            <NavLink to="/" end><IconoInicio /> <span>Inicio</span></NavLink>
            {!esAuditor && <NavLink to="/bandeja/mi"><IconoBandeja /> <span>Mi bandeja</span></NavLink>}
            {!esAuditor && <NavLink to="/bandeja/departamento"><IconoDepartamento /> <span>Departamento</span></NavLink>}
            <NavLink to="/bandeja/atrasados"><IconoReloj /> <span>Atrasados</span></NavLink>
            {puede(PERMISOS.VerTodosLosOficios) && <NavLink to="/bandeja/todos"><IconoDocumento /> <span>Todos los oficios</span></NavLink>}
            {puede(PERMISOS.CrearOficios) && <NavLink to="/nuevo"><IconoMasCirculo /> <span>Nuevo oficio</span></NavLink>}

            {(puede(PERMISOS.VerReportes) || puede(PERMISOS.AdministrarCatalogos) || puede(PERMISOS.AdministrarUsuarios)) && (
              <span className="lateral-separador" aria-hidden="true" />
            )}
            {puede(PERMISOS.VerReportes) && <NavLink to="/reportes"><IconoReportes /> <span>Reportes</span></NavLink>}
            {puede(PERMISOS.AdministrarCatalogos) && <NavLink to="/catalogos"><IconoConfiguracion /> <span>Configuración</span></NavLink>}
            {puede(PERMISOS.AdministrarUsuarios) && <NavLink to="/admin/registro-usuario"><IconoUsuarioMas /> <span>Registrar usuario</span></NavLink>}
          </nav>

          <div className="lateral-pie">
            <svg viewBox="0 0 200 90" aria-hidden="true" className="lateral-silueta">
              <path d="M0 80 L0 60 L20 60 L20 45 L30 45 L30 60 L50 60 L50 35 L60 25 L70 35 L70 60 L85 60 L85 50 L95 50 L95 60 L110 60 L110 30 L120 30 L120 20 L125 10 L130 20 L130 30 L140 30 L140 60 L160 60 L160 48 L175 48 L175 60 L200 60 L200 80 Z" fill="none" stroke="currentColor" strokeWidth="1.2" />
            </svg>
            <p>Comprometidos con el desarrollo de Paraíso</p>
          </div>
        </aside>

        <main className="contenido">
          <Outlet />
        </main>
      </div>

      <footer className="pie">
        <span><strong>Municipalidad de Paraíso</strong> | Cédula Jurídica: 3-014-042079 | Paraíso, Cartago, Costa Rica</span>
        <span>SIGO v1.0 | Desarrollado para servir al cantón</span>
      </footer>
    </div>
  )
}
