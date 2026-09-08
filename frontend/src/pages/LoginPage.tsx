import { useState, type FormEvent } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { Escudo, IconoCandado, IconoCorreo, IconoOjo, IconoOjoCerrado, LogoSigo } from '../components/Icons'

/** Página pública de inicio de sesión. */
export default function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const destino = (location.state as { desde?: string } | null)?.desde ?? '/'

  const [correo, setCorreo] = useState('')
  const [password, setPassword] = useState('')
  const [verPassword, setVerPassword] = useState(false)
  const [error, setError] = useState('')
  const [enviando, setEnviando] = useState(false)

  const enviar = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setEnviando(true)
    try {
      await login(correo.trim(), password)
      navigate(destino, { replace: true })
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No fue posible iniciar sesión.')
    } finally {
      setEnviando(false)
    }
  }

  return (
    <div className="login">
      <section className="login-panel" aria-hidden="true">
        <div className="login-panel-marca">
          <Escudo size={64} />
          <div>
            <span className="marca-muni-sup">Municipalidad de</span>
            <span className="marca-muni-nombre">PARAÍSO</span>
          </div>
        </div>
        <div className="login-panel-texto">
          <h2>Sistema de Gestión de Oficios</h2>
          <p>Control, seguimiento y trazabilidad de las comunicaciones oficiales de la Municipalidad de Paraíso.</p>
          <ul>
            <li>Bandejas por usuario y departamento</li>
            <li>Alertas de vencimiento (SLA)</li>
            <li>Bitácora y adjuntos con trazabilidad</li>
          </ul>
        </div>
        <svg viewBox="0 0 400 120" className="login-panel-silueta">
          <path d="M0 110 L0 80 L40 80 L40 60 L60 60 L60 80 L100 80 L100 45 L120 30 L140 45 L140 80 L170 80 L170 65 L190 65 L190 80 L220 80 L220 40 L240 40 L240 25 L250 8 L260 25 L260 40 L280 40 L280 80 L320 80 L320 62 L350 62 L350 80 L400 80 L400 110 Z" fill="none" stroke="currentColor" strokeWidth="1.5" />
        </svg>
        <p className="login-panel-lema">Comprometidos con el desarrollo de Paraíso</p>
      </section>

      <section className="login-form-zona">
        <form className="login-card" onSubmit={enviar} noValidate>
          <div className="login-card-marca">
            <LogoSigo size={48} />
            <div>
              <span className="marca-sigo-nombre">SIGO</span>
              <span className="marca-sigo-sub">Sistema de Gestión de Oficios</span>
            </div>
          </div>

          <h1>Iniciar sesión</h1>
          <p className="subtitle">Ingrese con su correo institucional y contraseña.</p>

          {error && <div className="alert" role="alert">{error}</div>}

          <label className="campo">
            <span>Correo electrónico</span>
            <span className="campo-input">
              <IconoCorreo size={18} />
              <input
                type="email"
                autoComplete="username"
                placeholder="usuario@muniparaiso.go.cr"
                value={correo}
                onChange={(e) => setCorreo(e.target.value)}
                required
                autoFocus
              />
            </span>
          </label>

          <label className="campo">
            <span>Contraseña</span>
            <span className="campo-input">
              <IconoCandado size={18} />
              <input
                type={verPassword ? 'text' : 'password'}
                autoComplete="current-password"
                placeholder="••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
              <button type="button" className="campo-ojo" onClick={() => setVerPassword((v) => !v)} aria-label={verPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}>
                {verPassword ? <IconoOjoCerrado size={18} /> : <IconoOjo size={18} />}
              </button>
            </span>
          </label>

          <button type="submit" className="btn btn-primary btn-bloque" disabled={enviando || !correo || !password}>
            {enviando ? 'Ingresando…' : 'Ingresar'}
          </button>

          <p className="hint">
            ¿Sin cuenta? Las cuentas las crea el Administrador del sistema (Área de Tecnologías).
          </p>
        </form>
        <p className="login-pie">Municipalidad de Paraíso · Paraíso, Cartago, Costa Rica</p>
      </section>
    </div>
  )
}
