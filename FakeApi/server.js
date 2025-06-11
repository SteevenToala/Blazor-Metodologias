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

// Levantar servidor
app.listen(PORT, () => {
  console.log(`API corriendo en http://localhost:${PORT}`)
})
