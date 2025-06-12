const express = require('express')
const fs = require('fs')
const cors = require('cors')  // <-- importar cors

const app = express()
const PORT = 3000

app.use(cors())  // <-- habilitar CORS para todas las rutas y orígenes
app.use(express.json())

// Leer datos desde db.json
function readData() {
  const data = fs.readFileSync('db.json', 'utf-8')
  return JSON.parse(data)
}

// Endpoint personalizado para total docentes
app.get('/totalDocentes', (req, res) => {
  const db = readData()
  const usuarios = db.Usuario || []
  const docentes = usuarios.filter(u => u.rol === 'DOCENTE')
  res.json({ totalDocentes: docentes.length })
})

// Otros endpoints para obtener datos completos, por ejemplo:
app.get('/Usuario', (req, res) => {
  const db = readData()
  res.json(db.Usuario || [])
})


// Buscar datos completos por cédula
app.get('/persona/:cedula', (req, res) => {
  const db = readData()
  const { cedula } = req.params
  const persona = db.Persona.find(p => p.cedula === cedula)
  if (!persona) return res.status(404).json({ error: 'Persona no encontrada' })

  const usuario = db.Usuario.find(u => u.personaId === persona.id)
  const docente = db.Docente.find(d => d.usuarioId === usuario?.id)
  const nivel = db.NivelAcademico.find(n => n.id === docente?.nivelAcademicoId)

  if (!docente) return res.status(404).json({ error: 'No es un docente' })

  const publicaciones = db.PublicacionAcademica.filter(p => p.docenteId === docente.id)
  const evaluaciones = db.EvaluacionDocente.filter(e => e.docenteId === docente.id)
  const cursos = db.CursoCapacitacion.filter(c => c.docenteId === docente.id)
  const proyectos = db.ProyectoInvestigacion.filter(p => p.docenteId === docente.id)
  const cumplimientos = db.CumplimientoRequisito.filter(c => c.docenteId === docente.id)
  const fechaInicioNivel = new Date(docente.fechaInicioNivel)
  const fechaActual = new Date()
  // Años en nivel actual
  const añoInicio = new Date(evaluaciones?.[0]?.periodo?.substring(0, 4) || new Date().getFullYear())
  const añoActual = new Date().getFullYear()
  let añosEnNivel = fechaActual.getFullYear() - fechaInicioNivel.getFullYear()
  const mesDiff = fechaActual.getMonth() - fechaInicioNivel.getMonth()
  const diaDiff = fechaActual.getDate() - fechaInicioNivel.getDate()
  if (mesDiff < 0 || (mesDiff === 0 && diaDiff < 0)) {
    añosEnNivel--
  }

  // Evaluación promedio
  const promedioEvaluacion = evaluaciones.length > 0
    ? (evaluaciones.reduce((acc, ev) => acc + ev.puntaje, 0) / evaluaciones.length)
    : 0

  // Horas de capacitación
  const totalHoras = cursos.reduce((acc, cur) => acc + cur.horas, 0)

  // Meses de investigación
  const mesesInvestigacion = proyectos.reduce((acc, p) => {
    const inicio = new Date(p.fechaInicio)
    const fin = new Date(p.fechaFin)
    const meses = (fin.getFullYear() - inicio.getFullYear()) * 12 + (fin.getMonth() - inicio.getMonth())
    return acc + meses
  }, 0)

  // Datos para promoción
  const requisitosPorRango = {
    DT1: { anios: 0, publicaciones: 1, evaluacion: 75, horas: 96, investigacion: 0 },
    DT2: { anios: 4, publicaciones: 2, evaluacion: 75, horas: 96, investigacion: 0 },
    DT3: { anios: 4, publicaciones: 3, evaluacion: 75, horas: 128, investigacion: 12 },
    DT4: { anios: 4, publicaciones: 5, evaluacion: 75, horas: 160, investigacion: 24 },
    DT5: { anios: 4, publicaciones: 6, evaluacion: 75, horas: 160, investigacion: 24 },
  }

  const nivelActual = nivel?.nombre || 'DT1'
  const rango = requisitosPorRango[nivelActual]

  const cumple = {
    años: añosEnNivel >= rango.anios,
    publicaciones: publicaciones.length >= rango.publicaciones,
    evaluacion: promedioEvaluacion >= rango.evaluacion,
    capacitacion: totalHoras >= rango.horas,
    investigacion: mesesInvestigacion >= rango.investigacion,
  }

  const requisitosFaltantes = Object.entries(cumple)
    .filter(([k, v]) => !v)
    .map(([k]) => k)

  res.json({
    usuario,
    docente,
    nivel: nivel?.nombre,
    añosEnNivel,
    promedioEvaluacion,
    totalHorasCapacitacion: totalHoras,
    publicaciones: publicaciones.length,
    mesesInvestigacion,
    cumpleRequisitos: cumple,
    requisitosFaltantes,
  })
})


// Levantar servidor
app.listen(PORT, () => {
  console.log(`API corriendo en http://localhost:${PORT}`)
})
