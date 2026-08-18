import { useEffect, useMemo, useState } from 'react'
import { api } from '../api'
import type { Oficio } from '../types'

// Serie única (azul #2a78d6, paleta validada) con etiquetas directas en cada barra;
// el incumplimiento SLA usa el color de estado "critical" acompañado de ícono y texto.

function BarraH({ etiqueta, valor, max, sufijo = '', critico = false }: {
  etiqueta: string; valor: number; max: number; sufijo?: string; critico?: boolean
}) {
  const ancho = max > 0 ? Math.max(2, (valor / max) * 100) : 0
  return (
    <div className="bar-row" title={`${etiqueta}: ${valor}${sufijo}`}>
      <span className="bar-label">{etiqueta}</span>
      <div className="bar-track">
        <div className={critico ? 'bar-fill bar-critico' : 'bar-fill'} style={{ width: `${ancho}%` }} />
      </div>
      <span className="bar-value">{valor}{sufijo}</span>
    </div>
  )
}

const MESES = ['ene', 'feb', 'mar', 'abr', 'may', 'jun', 'jul', 'ago', 'set', 'oct', 'nov', 'dic']

export default function ReportesPage() {
  const [oficios, setOficios] = useState<Oficio[]>([])
  const [cargando, setCargando] = useState(true)

  useEffect(() => {
    api.oficios.list().then(setOficios).finally(() => setCargando(false))
  }, [])

  const datos = useMemo(() => {
    const abiertos = oficios.filter((o) => !o.estadoCerrado)
    const atrasados = oficios.filter((o) => o.atrasado)
    const cerrados = oficios.filter((o) => o.estadoCerrado)

    // Lead time (días) de los cerrados
    const leads = cerrados
      .filter((o) => o.fechaCierre)
      .map((o) => (new Date(o.fechaCierre!).getTime() - new Date(o.fechaIngreso).getTime()) / 864e5)
    const leadProm = leads.length ? leads.reduce((a, b) => a + b, 0) / leads.length : 0

    // Volumen por mes (últimos 6 meses)
    const ahora = new Date()
    const meses: { etiqueta: string; valor: number }[] = []
    for (let i = 5; i >= 0; i--) {
      const d = new Date(ahora.getFullYear(), ahora.getMonth() - i, 1)
      const valor = oficios.filter((o) => {
        const f = new Date(o.fechaIngreso)
        return f.getFullYear() === d.getFullYear() && f.getMonth() === d.getMonth()
      }).length
      meses.push({ etiqueta: `${MESES[d.getMonth()]} ${String(d.getFullYear()).slice(2)}`, valor })
    }

    const porGrupo = (fn: (o: Oficio) => string) => {
      const mapa = new Map<string, number>()
      for (const o of oficios) {
        const k = fn(o) || '—'
        mapa.set(k, (mapa.get(k) ?? 0) + 1)
      }
      return [...mapa.entries()].map(([etiqueta, valor]) => ({ etiqueta, valor })).sort((a, b) => b.valor - a.valor)
    }

    const porDepto = porGrupo((o) => o.departamentoDestinoNombre)

    const carga = (() => {
      const mapa = new Map<string, number>()
      for (const o of abiertos) {
        if (o.responsableActual) mapa.set(o.responsableActual, (mapa.get(o.responsableActual) ?? 0) + 1)
      }
      return [...mapa.entries()].map(([etiqueta, valor]) => ({ etiqueta, valor })).sort((a, b) => b.valor - a.valor)
    })()

    // Incumplimiento SLA por prioridad: vencidos abiertos + cerrados después del vencimiento
    const slaPorPrioridad = (() => {
      const mapa = new Map<string, { total: number; incumplidos: number }>()
      for (const o of oficios) {
        const e = mapa.get(o.prioridadNombre) ?? { total: 0, incumplidos: 0 }
        e.total++
        const incumplido = o.fechaCierre
          ? new Date(o.fechaCierre) > new Date(o.fechaVencimientoSla)
          : o.atrasado
        if (incumplido) e.incumplidos++
        mapa.set(o.prioridadNombre, e)
      }
      return [...mapa.entries()].map(([etiqueta, v]) => ({
        etiqueta,
        valor: v.total ? Math.round((v.incumplidos / v.total) * 100) : 0,
        detalle: `${v.incumplidos}/${v.total}`,
      }))
    })()

    return { abiertos, atrasados, cerrados, leadProm, meses, porDepto, carga, slaPorPrioridad }
  }, [oficios])

  if (cargando) return <p className="muted">Cargando…</p>

  const maxMes = Math.max(...datos.meses.map((m) => m.valor), 1)

  return (
    <div>
      <h1>Reportes y analítica</h1>
      <p className="subtitle">Indicadores operativos calculados sobre {oficios.length} oficios registrados.</p>

      <div className="tiles">
        <div className="tile">
          <span className="tile-label">Abiertos</span>
          <span className="tile-value">{datos.abiertos.length}</span>
        </div>
        <div className="tile">
          <span className="tile-label">⚠ Atrasados (SLA vencido)</span>
          <span className="tile-value tile-critico">{datos.atrasados.length}</span>
        </div>
        <div className="tile">
          <span className="tile-label">Cerrados</span>
          <span className="tile-value">{datos.cerrados.length}</span>
        </div>
        <div className="tile">
          <span className="tile-label">Lead time promedio</span>
          <span className="tile-value">{datos.leadProm.toFixed(1)} <small>días</small></span>
        </div>
      </div>

      <div className="charts-grid">
        <section className="card">
          <h2>Volumen por mes</h2>
          <div className="cols" role="img" aria-label="Oficios ingresados por mes, últimos seis meses">
            {datos.meses.map((m) => (
              <div className="col" key={m.etiqueta} title={`${m.etiqueta}: ${m.valor} oficios`}>
                <span className="col-value">{m.valor}</span>
                <div className="col-bar" style={{ height: `${(m.valor / maxMes) * 140}px` }} />
                <span className="col-label">{m.etiqueta}</span>
              </div>
            ))}
          </div>
        </section>

        <section className="card">
          <h2>Volumen por departamento destino</h2>
          {datos.porDepto.map((d) => (
            <BarraH key={d.etiqueta} etiqueta={d.etiqueta} valor={d.valor} max={datos.porDepto[0]?.valor ?? 1} />
          ))}
        </section>

        <section className="card">
          <h2>Incumplimiento SLA por prioridad</h2>
          {datos.slaPorPrioridad.map((p) => (
            <div className="bar-row" key={p.etiqueta} title={`${p.etiqueta}: ${p.detalle} incumplidos`}>
              <span className="bar-label">{p.etiqueta}</span>
              <div className="bar-track">
                <div className="bar-fill bar-critico" style={{ width: `${Math.max(p.valor, 2)}%` }} />
              </div>
              <span className="bar-value">{p.valor}% ({p.detalle})</span>
            </div>
          ))}
          <p className="hint">Porcentaje de oficios cerrados después del vencimiento o abiertos ya vencidos.</p>
        </section>

        <section className="card">
          <h2>Carga de trabajo por responsable</h2>
          {datos.carga.length === 0 ? (
            <p className="muted">No hay oficios abiertos asignados.</p>
          ) : (
            datos.carga.map((c) => (
              <BarraH key={c.etiqueta} etiqueta={c.etiqueta} valor={c.valor} max={datos.carga[0].valor} />
            ))
          )}
          <p className="hint">Oficios abiertos con asignación activa.</p>
        </section>
      </div>
    </div>
  )
}
