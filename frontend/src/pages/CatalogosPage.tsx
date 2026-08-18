import { useCallback, useEffect, useState } from 'react'
import { api } from '../api'
import { useUser } from '../UserContext'
import type { Departamento, Rol, Usuario } from '../types'

type Campo = {
  clave: string
  etiqueta: string
  tipo: 'texto' | 'numero' | 'bool'
}

type CatalogoDef = {
  titulo: string
  campos: Campo[]
  list: () => Promise<Record<string, unknown>[]>
  create: (d: Record<string, unknown>) => Promise<unknown>
  update: (id: number, d: Record<string, unknown>) => Promise<unknown>
  remove: (id: number) => Promise<void>
}

const asAny = (o: object) => o as Record<string, unknown>

const CATALOGOS: Record<string, CatalogoDef> = {
  departamentos: {
    titulo: 'Departamentos',
    campos: [
      { clave: 'nombre', etiqueta: 'Nombre', tipo: 'texto' },
      { clave: 'sigla', etiqueta: 'Sigla', tipo: 'texto' },
      { clave: 'activo', etiqueta: 'Activo', tipo: 'bool' },
    ],
    list: () => api.departamentos.list().then((r) => r.map(asAny)),
    create: (d) => api.departamentos.create(d as unknown as Omit<Departamento, 'id'>),
    update: (id, d) => api.departamentos.update(id, d as unknown as Omit<Departamento, 'id'>),
    remove: (id) => api.departamentos.remove(id),
  },
  prioridades: {
    titulo: 'Prioridades y SLA',
    campos: [
      { clave: 'nombre', etiqueta: 'Nombre', tipo: 'texto' },
      { clave: 'nivel', etiqueta: 'Nivel', tipo: 'numero' },
      { clave: 'slaHoras', etiqueta: 'SLA (horas)', tipo: 'numero' },
    ],
    list: () => api.prioridades.list().then((r) => r.map(asAny)),
    create: (d) => api.prioridades.create(d as never),
    update: (id, d) => api.prioridades.update(id, d as never),
    remove: (id) => api.prioridades.remove(id),
  },
  estados: {
    titulo: 'Estados',
    campos: [
      { clave: 'nombre', etiqueta: 'Nombre', tipo: 'texto' },
      { clave: 'cerradoFlag', etiqueta: 'Cierra el oficio', tipo: 'bool' },
    ],
    list: () => api.estados.list().then((r) => r.map(asAny)),
    create: (d) => api.estados.create(d as never),
    update: (id, d) => api.estados.update(id, d as never),
    remove: (id) => api.estados.remove(id),
  },
  tipos: {
    titulo: 'Tipos de oficio',
    campos: [
      { clave: 'nombre', etiqueta: 'Nombre', tipo: 'texto' },
      { clave: 'requiereRespuestaFlag', etiqueta: 'Requiere respuesta', tipo: 'bool' },
    ],
    list: () => api.tiposOficios.list().then((r) => r.map(asAny)),
    create: (d) => api.tiposOficios.create(d as never),
    update: (id, d) => api.tiposOficios.update(id, d as never),
    remove: (id) => api.tiposOficios.remove(id),
  },
  canales: {
    titulo: 'Canales',
    campos: [{ clave: 'nombre', etiqueta: 'Nombre', tipo: 'texto' }],
    list: () => api.canales.list().then((r) => r.map(asAny)),
    create: (d) => api.canales.create(d as never),
    update: (id, d) => api.canales.update(id, d as never),
    remove: (id) => api.canales.remove(id),
  },
}

function CatalogoCrud({ def }: { def: CatalogoDef }) {
  const [filas, setFilas] = useState<Record<string, unknown>[]>([])
  const [editando, setEditando] = useState<Record<string, unknown> | null>(null)
  const [error, setError] = useState('')

  const cargar = useCallback(() => { def.list().then(setFilas) }, [def])
  useEffect(cargar, [cargar])

  const vacio = () =>
    Object.fromEntries(def.campos.map((c) => [c.clave, c.tipo === 'bool' ? true : c.tipo === 'numero' ? 1 : '']))

  const guardar = async () => {
    if (!editando) return
    setError('')
    try {
      const { id, ...datos } = editando
      if (id) await def.update(Number(id), datos)
      else await def.create(datos)
      setEditando(null)
      cargar()
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e))
    }
  }

  const eliminar = async (id: number) => {
    if (!confirm('¿Eliminar este registro? Solo es posible si ningún oficio lo referencia.')) return
    setError('')
    try {
      await def.remove(id)
      cargar()
    } catch {
      setError('No se puede eliminar: hay registros que lo utilizan.')
    }
  }

  return (
    <div>
      {error && <div className="alert">{error}</div>}
      <div className="cat-toolbar">
        <button className="btn btn-primary" onClick={() => setEditando(vacio())}>＋ Agregar</button>
      </div>
      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              {def.campos.map((c) => <th key={c.clave}>{c.etiqueta}</th>)}
              <th />
            </tr>
          </thead>
          <tbody>
            {filas.map((f) => (
              <tr key={String(f.id)}>
                {def.campos.map((c) => (
                  <td key={c.clave}>
                    {c.tipo === 'bool' ? ((f[c.clave] as boolean) ? 'Sí' : 'No') : String(f[c.clave] ?? '')}
                  </td>
                ))}
                <td className="td-actions">
                  <button className="btn btn-sm" onClick={() => setEditando({ ...f })}>Editar</button>
                  <button className="btn btn-sm btn-danger" onClick={() => eliminar(Number(f.id))}>Eliminar</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {editando && (
        <div className="modal-backdrop" onClick={() => setEditando(null)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <h3>{editando.id ? 'Editar' : 'Agregar'} — {def.titulo}</h3>
            {def.campos.map((c) => (
              <label key={c.clave}>
                {c.etiqueta}
                {c.tipo === 'bool' ? (
                  <select
                    value={(editando[c.clave] as boolean) ? '1' : '0'}
                    onChange={(e) => setEditando({ ...editando, [c.clave]: e.target.value === '1' })}
                  >
                    <option value="1">Sí</option>
                    <option value="0">No</option>
                  </select>
                ) : (
                  <input
                    type={c.tipo === 'numero' ? 'number' : 'text'}
                    value={String(editando[c.clave] ?? '')}
                    onChange={(e) =>
                      setEditando({ ...editando, [c.clave]: c.tipo === 'numero' ? Number(e.target.value) : e.target.value })}
                  />
                )}
              </label>
            ))}
            <div className="form-actions">
              <button className="btn" onClick={() => setEditando(null)}>Cancelar</button>
              <button className="btn btn-primary" onClick={guardar}>Guardar</button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

function UsuariosCrud() {
  const { recargarUsuarios } = useUser()
  const [usuarios, setUsuarios] = useState<Usuario[]>([])
  const [roles, setRoles] = useState<Rol[]>([])
  const [departamentos, setDepartamentos] = useState<Departamento[]>([])
  const [editando, setEditando] = useState<{
    id?: number; nombre: string; correo: string; idDepartamento: number | ''; cargo: string;
    activo: boolean; origenAd: boolean; rolIds: number[]
  } | null>(null)
  const [error, setError] = useState('')

  const cargar = useCallback(() => {
    api.usuarios.list().then(setUsuarios)
    api.roles.list().then(setRoles)
    api.departamentos.list().then(setDepartamentos)
  }, [])
  useEffect(cargar, [cargar])

  const guardar = async () => {
    if (!editando || editando.idDepartamento === '' || editando.rolIds.length === 0) {
      setError('Complete departamento y al menos un rol.')
      return
    }
    setError('')
    try {
      const { id, ...datos } = editando
      if (id) await api.usuarios.update(id, datos)
      else await api.usuarios.create(datos)
      setEditando(null)
      cargar()
      recargarUsuarios()
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e))
    }
  }

  return (
    <div>
      {error && <div className="alert">{error}</div>}
      <div className="cat-toolbar">
        <button
          className="btn btn-primary"
          onClick={() => setEditando({ nombre: '', correo: '', idDepartamento: '', cargo: '', activo: true, origenAd: false, rolIds: [] })}
        >
          ＋ Agregar
        </button>
      </div>
      <div className="table-wrap">
        <table>
          <thead>
            <tr><th>Nombre</th><th>Correo</th><th>Departamento</th><th>Cargo</th><th>Roles</th><th>Activo</th><th /></tr>
          </thead>
          <tbody>
            {usuarios.map((u) => (
              <tr key={u.id}>
                <td>{u.nombre}</td>
                <td>{u.correo}</td>
                <td>{u.departamentoNombre}</td>
                <td>{u.cargo}</td>
                <td>{u.roles.map((r) => r.nombre).join(', ')}</td>
                <td>{u.activo ? 'Sí' : 'No'}</td>
                <td className="td-actions">
                  <button
                    className="btn btn-sm"
                    onClick={() => setEditando({
                      id: u.id, nombre: u.nombre, correo: u.correo, idDepartamento: u.idDepartamento,
                      cargo: u.cargo, activo: u.activo, origenAd: u.origenAd, rolIds: u.roles.map((r) => r.idRol),
                    })}
                  >
                    Editar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {editando && (
        <div className="modal-backdrop" onClick={() => setEditando(null)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <h3>{editando.id ? 'Editar' : 'Agregar'} usuario</h3>
            <label>Nombre<input value={editando.nombre} onChange={(e) => setEditando({ ...editando, nombre: e.target.value })} /></label>
            <label>Correo<input type="email" value={editando.correo} onChange={(e) => setEditando({ ...editando, correo: e.target.value })} /></label>
            <label>
              Departamento
              <select
                value={editando.idDepartamento}
                onChange={(e) => setEditando({ ...editando, idDepartamento: Number(e.target.value) })}
              >
                <option value="">Seleccione…</option>
                {departamentos.map((d) => <option key={d.id} value={d.id}>{d.nombre}</option>)}
              </select>
            </label>
            <label>Cargo<input value={editando.cargo} onChange={(e) => setEditando({ ...editando, cargo: e.target.value })} /></label>
            <fieldset className="roles-fieldset">
              <legend>Roles</legend>
              {roles.map((r) => (
                <label key={r.id} className="check-label">
                  <input
                    type="checkbox"
                    checked={editando.rolIds.includes(r.id)}
                    onChange={(e) =>
                      setEditando({
                        ...editando,
                        rolIds: e.target.checked
                          ? [...editando.rolIds, r.id]
                          : editando.rolIds.filter((x) => x !== r.id),
                      })}
                  />
                  {r.nombre}
                </label>
              ))}
            </fieldset>
            <label className="check-label">
              <input type="checkbox" checked={editando.activo} onChange={(e) => setEditando({ ...editando, activo: e.target.checked })} />
              Activo
            </label>
            <div className="form-actions">
              <button className="btn" onClick={() => setEditando(null)}>Cancelar</button>
              <button className="btn btn-primary" onClick={guardar}>Guardar</button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

export default function CatalogosPage() {
  const [tab, setTab] = useState('departamentos')
  const tabs = [...Object.entries(CATALOGOS).map(([k, v]) => [k, v.titulo] as const), ['usuarios', 'Usuarios'] as const]

  return (
    <div>
      <h1>Catálogos y configuración</h1>
      <p className="subtitle">Departamentos, tipos de oficio, prioridades/SLA, estados, canales y usuarios.</p>
      <div className="tabs">
        {tabs.map(([k, titulo]) => (
          <button key={k} className={tab === k ? 'tab tab-active' : 'tab'} onClick={() => setTab(k)}>{titulo}</button>
        ))}
      </div>
      {tab === 'usuarios' ? <UsuariosCrud /> : <CatalogoCrud key={tab} def={CATALOGOS[tab]} />}
    </div>
  )
}
