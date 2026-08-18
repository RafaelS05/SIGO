import { NavLink, Navigate, Route, Routes } from 'react-router-dom'
import { useUser } from './UserContext'
import BandejaPage from './pages/BandejaPage'
import OficioDetallePage from './pages/OficioDetallePage'
import NuevoOficioPage from './pages/NuevoOficioPage'
import CatalogosPage from './pages/CatalogosPage'
import ReportesPage from './pages/ReportesPage'

export default function App() {
  const { usuarios, usuarioActual, setUsuarioActual } = useUser()
  const esAdmin = usuarioActual?.roles.some((r) => r.nombre === 'Administrador') ?? false
  const esAuditor = usuarioActual?.roles.some((r) => r.nombre === 'Auditor') ?? false

  return (
    <div className="layout">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-logo">S</span>
          <div>
            <strong>SIGO</strong>
            <small>Gestión de Oficios</small>
          </div>
        </div>
        <nav>
          <span className="nav-section">Bandejas</span>
          <NavLink to="/bandeja/mi">Mi bandeja</NavLink>
          <NavLink to="/bandeja/departamento">Departamento</NavLink>
          <NavLink to="/bandeja/atrasados">Atrasados</NavLink>
          {(esAdmin || esAuditor) && <NavLink to="/bandeja/todos">Todos</NavLink>}
          <span className="nav-section">Gestión</span>
          <NavLink to="/nuevo">＋ Nuevo oficio</NavLink>
          <NavLink to="/reportes">Reportes</NavLink>
          {esAdmin && <NavLink to="/catalogos">Catálogos</NavLink>}
        </nav>
      </aside>
      <div className="main">
        <header className="topbar">
          <span className="topbar-title">Sistema de Gestión de Oficios</span>
          <div className="user-picker">
            <label htmlFor="usuario">Usuario:</label>
            <select
              id="usuario"
              value={usuarioActual?.id ?? ''}
              onChange={(e) => {
                const u = usuarios.find((x) => x.id === Number(e.target.value))
                if (u) setUsuarioActual(u)
              }}
            >
              {usuarios.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.nombre} — {u.roles.map((r) => r.nombre).join(', ')}
                </option>
              ))}
            </select>
          </div>
        </header>
        <main className="content">
          <Routes>
            <Route path="/" element={<Navigate to="/bandeja/mi" replace />} />
            <Route path="/bandeja/:tipo" element={<BandejaPage />} />
            <Route path="/oficio/:id" element={<OficioDetallePage />} />
            <Route path="/nuevo" element={<NuevoOficioPage />} />
            <Route path="/reportes" element={<ReportesPage />} />
            <Route path="/catalogos" element={<CatalogosPage />} />
          </Routes>
        </main>
      </div>
    </div>
  )
}
