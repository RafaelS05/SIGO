import { Navigate, Route, Routes } from 'react-router-dom'
import { PERMISOS } from './auth/permisos'
import Layout from './components/Layout'
import { RequierePermiso, RequiereSesion, SoloAnonimo } from './components/RutaProtegida'
import AccesoDenegadoPage from './pages/AccesoDenegadoPage'
import BandejaPage from './pages/BandejaPage'
import CatalogosPage from './pages/CatalogosPage'
import InicioPage from './pages/InicioPage'
import LoginPage from './pages/LoginPage'
import NuevoOficioPage from './pages/NuevoOficioPage'
import OficioDetallePage from './pages/OficioDetallePage'
import PerfilPage from './pages/PerfilPage'
import RegistroUsuarioPage from './pages/RegistroUsuarioPage'
import ReportesPage from './pages/ReportesPage'

/**
 * Enrutado RBAC:
 *  - /login: público (redirige al inicio si ya hay sesión).
 *  - Todo lo demás exige sesión; las secciones sensibles exigen además un permiso de la matriz.
 *  - /admin/registro-usuario: URL de registro, solo Administrador.
 */
export default function App() {
  return (
    <Routes>
      <Route element={<SoloAnonimo />}>
        <Route path="/login" element={<LoginPage />} />
      </Route>

      <Route element={<RequiereSesion />}>
        <Route element={<Layout />}>
          <Route index element={<InicioPage />} />
          <Route path="/bandeja/:tipo" element={<BandejaPage />} />
          <Route path="/oficio/:id" element={<OficioDetallePage />} />
          <Route path="/perfil" element={<PerfilPage />} />
          <Route path="/acceso-denegado" element={<AccesoDenegadoPage />} />

          <Route element={<RequierePermiso permiso={PERMISOS.CrearOficios} />}>
            <Route path="/nuevo" element={<NuevoOficioPage />} />
          </Route>
          <Route element={<RequierePermiso permiso={PERMISOS.VerReportes} />}>
            <Route path="/reportes" element={<ReportesPage />} />
          </Route>
          <Route element={<RequierePermiso permiso={PERMISOS.AdministrarCatalogos} />}>
            <Route path="/catalogos" element={<CatalogosPage />} />
          </Route>
          <Route element={<RequierePermiso permiso={PERMISOS.AdministrarUsuarios} />}>
            <Route path="/admin/registro-usuario" element={<RegistroUsuarioPage />} />
          </Route>

          <Route path="*" element={<Navigate to="/" replace />} />
        </Route>
      </Route>
    </Routes>
  )
}
