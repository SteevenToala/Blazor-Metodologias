-- =====================
-- CREAR TABLAS
-- =====================

-- Tabla Persona
CREATE TABLE Persona (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombres VARCHAR(100),
    apellidos VARCHAR(100),
    cedula VARCHAR(20) UNIQUE,
    telefono VARCHAR(20),
    direccion VARCHAR(200),
    fechaNacimiento DATE
);

-- Tabla Usuario
CREATE TABLE Usuario (
    id INT PRIMARY KEY IDENTITY(1,1),
    correo VARCHAR(100) UNIQUE,
    passwordHash VARCHAR(255),
    rol VARCHAR(100),
    personaId INT,
    activo BIT,
    FOREIGN KEY (personaId) REFERENCES Persona(id)
);


-- Tabla NivelAcademico
CREATE TABLE NivelAcademico (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(50),
    descripcion VARCHAR(200)
);

-- Tabla Docente
CREATE TABLE Docente (
    id INT PRIMARY KEY IDENTITY(1,1),
    usuarioId INT UNIQUE,
    nivelAcademicoId INT,
    fechaInicioNivel DATE,
    FOREIGN KEY (usuarioId) REFERENCES Usuario(id),
    FOREIGN KEY (nivelAcademicoId) REFERENCES NivelAcademico(id)
);

-- Tabla EvaluacionDocente
CREATE TABLE EvaluacionDocente (
    id INT PRIMARY KEY IDENTITY(1,1),
    periodo VARCHAR(20),
    puntaje FLOAT,
    docenteId INT UNIQUE,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Tabla CursoCapacitacion
CREATE TABLE CursoCapacitacion (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100),
    horas INT,
    fechaInicio DATE,
    fechaFin DATE,
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Tabla ProyectoInvestigacion
CREATE TABLE ProyectoInvestigacion (
    id INT PRIMARY KEY IDENTITY(1,1),
    titulo VARCHAR(200),
    fechaInicio DATE,
    fechaFin DATE,
    rolEnProyecto VARCHAR(100),
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Tabla PublicacionAcademica
CREATE TABLE PublicacionAcademica (
    id INT PRIMARY KEY IDENTITY(1,1),
    titulo VARCHAR(200),
    revista VARCHAR(100),
    volumen VARCHAR(50),
    anio INT,
    tipo VARCHAR(50),
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Tabla RequisitoPromocion
CREATE TABLE RequisitoPromocion (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(200),
    porcentajeAsignado INT
);

-- Tabla CumplimientoRequisito
CREATE TABLE CumplimientoRequisito (
    id INT PRIMARY KEY IDENTITY(1,1),
    docenteId INT,
    requisitoId INT,
    cumplido BIT,
    fechaCumplimiento DATE,
    FOREIGN KEY (docenteId) REFERENCES Docente(id),
    FOREIGN KEY (requisitoId) REFERENCES RequisitoPromocion(id)
);

-- Tabla ReporteAvance
CREATE TABLE ReporteAvance (
    id INT PRIMARY KEY IDENTITY(1,1),
    fechaGeneracion DATE,
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

CREATE TABLE SolicitudAvanceRango (
    id INT PRIMARY KEY IDENTITY(1,1),
    docenteId INT,
    fechaSolicitud DATE,
    estado VARCHAR(20), -- PENDIENTE, APROBADA, RECHAZADA
    fechaRespuesta DATE,
    observaciones VARCHAR(300),
    nuevoNivelAcademicoId INT,
    FOREIGN KEY (docenteId) REFERENCES Docente(id),
    FOREIGN KEY (nuevoNivelAcademicoId) REFERENCES NivelAcademico(id)
);

-- Tabla TipoRequisito (define el tipo de requisito)
CREATE TABLE TipoRequisito (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) -- Ejemplo: 'Años en el rango', 'Papers', 'Puntaje Evaluación', 'Horas Capacitación', 'Investigaciones'
);

-- Tabla RequisitoNivelAcademico (requisitos por nivel)
CREATE TABLE RequisitoNivelAcademico (
    id INT PRIMARY KEY IDENTITY(1,1),
    nivelAcademicoId INT,
    tipoRequisitoId INT,
    valorRequerido FLOAT, -- Puede ser años, cantidad, porcentaje, etc.
    FOREIGN KEY (nivelAcademicoId) REFERENCES NivelAcademico(id),
    FOREIGN KEY (tipoRequisitoId) REFERENCES TipoRequisito(id)
);

ALTER TABLE PublicacionAcademica
ADD archivo VARBINARY(MAX) NULL; -- o puedes usar NVARCHAR(MAX) para una URL

ALTER TABLE CursoCapacitacion
ADD certificado VARBINARY(MAX) NULL; -- o NVARCHAR(MAX) para una URL

ALTER TABLE ProyectoInvestigacion
ADD documento VARBINARY(MAX) NULL; -- o NVARCHAR(MAX) para una URL


-- =====================
-- INSERTAR DATOS DE PRUEBA
-- =====================


-- Personas
INSERT INTO Persona ( nombres, apellidos, cedula, telefono, direccion, fechaNacimiento) VALUES
( 'María', 'López Ramírez', '0102030405', '0987654321', 'Av. Simón Bolívar y Loja, Ambato', '1985-05-15'),
( 'Pedro', 'Ruiz Morales', '0607080910', '0991234567', 'Av. Cevallos y Espejo, Ambato', '1980-11-22');

-- Usuarios
INSERT INTO Usuario ( correo, passwordHash, rol, personaId, activo) VALUES
( 'maria@example.com', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', "ADMINISTRADOR", 1, 1),
( 'pedro@example.com', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', "DOCENTE", 2, 1);

-- Niveles Académicos
INSERT INTO NivelAcademico ( nombre, descripcion) VALUES
( 'DT2', 'Docente con experiencia inicial'),
( 'DT3', 'Docente con experiencia media'),
( 'DT4', 'Docente con experiencia avanzada'),
( 'DT5', 'Docente con experiencia superior'),
( 'DT1', 'Docente sin experiencia');


-- Docente
INSERT INTO Docente ( usuarioId, nivelAcademicoId, fechaInicioNivel) VALUES
( 2, 1, '2020-05-01');

-- Evaluación Docente
INSERT INTO EvaluacionDocente ( periodo, puntaje, docenteId) VALUES
('2024A', 87, 1);

-- Curso Capacitación
INSERT INTO CursoCapacitacion ( nombre, horas, fechaInicio, fechaFin, docenteId) VALUES
( 'Innovación educativa', 40, '2024-01-10', '2024-01-20', 1);

-- Proyecto de Investigación
INSERT INTO ProyectoInvestigacion ( titulo, fechaInicio, fechaFin, rolEnProyecto, docenteId) VALUES
('IA en educación', '2024-03-01', '2024-06-30', 'Investigador Principal', 1);

-- Publicación Académica
INSERT INTO PublicacionAcademica ( titulo, revista, volumen, anio, tipo, docenteId) VALUES
('Nuevas metodologías', 'Revista EDUCA', 'Vol. 12', 2024, 'Artículo', 1);

-- Requisito de Promoción
INSERT INTO RequisitoPromocion ( nombre, porcentajeAsignado) VALUES
('Participación en proyecto de investigación', 30);

-- Cumplimiento del Requisito
INSERT INTO CumplimientoRequisito ( docenteId, requisitoId, cumplido, fechaCumplimiento) VALUES
( 1, 1, 1, '2024-06-01');

-- Reporte de Avance
INSERT INTO ReporteAvance ( fechaGeneracion, docenteId) VALUES
('2024-06-01', 1);

-- Tipos de requisito
INSERT INTO TipoRequisito (nombre) VALUES
('Años en el rango'),
('Papers'),
('Puntaje Evaluación'),
('Horas Capacitación'),
('Investigaciones');

-- Requisitos para DT2
INSERT INTO RequisitoNivelAcademico (nivelAcademicoId, tipoRequisitoId, valorRequerido) VALUES
(1, 1, 4),    -- 4 años en DT1
(1, 2, 1),    -- 1 paper
(1, 3, 75),   -- 75% puntaje
(1, 4, 96),   -- 96 horas capacitación

-- Requisitos para DT3
INSERT INTO RequisitoNivelAcademico (nivelAcademicoId, tipoRequisitoId, valorRequerido) VALUES
(2, 1, 4),    -- 4 años en DT2
(2, 2, 2),    -- 2 papers
(2, 3, 75),   -- 75% puntaje
(2, 4, 96),   -- 96 horas capacitación
(2, 5, 12);    -- 12 meses de investigación

-- Y así para DT4 
INSERT INTO RequisitoNivelAcademico (nivelAcademicoId, tipoRequisitoId, valorRequerido) VALUES
(3, 1, 4),    -- 4 años en DT3
(3, 2, 3),    -- 3 papers
(3, 3, 75),   -- 75% puntaje
(3, 4, 128),   -- 128 horas capacitación
(3, 5, 24);    -- 24 meses de investigación

-- Y así para DT5 
INSERT INTO RequisitoNivelAcademico (nivelAcademicoId, tipoRequisitoId, valorRequerido) VALUES
(4, 1, 4),    -- 4 años en DT4
(4, 2, 5),    -- 2 papers
(4, 3, 75),   -- 75% puntaje
(4, 4, 160),   -- 160 horas capacitación
(4, 5, 24);    -- 24 meses de investigación

