-- =====================
-- CREAR TABLAS
-- =====================

-- Tabla Rol
CREATE TABLE Rol (
    id INT PRIMARY KEY,
    rol VARCHAR(20) UNIQUE
);

-- Tabla Persona
CREATE TABLE Persona (
    id INT PRIMARY KEY,
    nombres VARCHAR(100),
    apellidos VARCHAR(100),
    cedula VARCHAR(20) UNIQUE,
    telefono VARCHAR(20),
    direccion VARCHAR(200),
    fechaNacimiento DATE
);

-- Tabla Usuario
CREATE TABLE Usuario (
    id INT PRIMARY KEY,
    correo VARCHAR(100) UNIQUE,
    passwordHash VARCHAR(255),
    rolId INT,
    personaId INT,
    activo BIT,
    FOREIGN KEY (rolId) REFERENCES Rol(id),
    FOREIGN KEY (personaId) REFERENCES Persona(id)
);

-- Tabla Administrador
CREATE TABLE Administrador (
    id INT PRIMARY KEY,
    usuarioId INT UNIQUE,
    FOREIGN KEY (usuarioId) REFERENCES Usuario(id)
);

-- Tabla NivelAcademico
CREATE TABLE NivelAcademico (
    id INT PRIMARY KEY,
    nombre VARCHAR(50),
    descripcion VARCHAR(200)
);

-- Tabla Docente
CREATE TABLE Docente (
    id INT PRIMARY KEY,
    usuarioId INT UNIQUE,
    nivelAcademicoId INT,
    fechaInicioNivel DATE,
    FOREIGN KEY (usuarioId) REFERENCES Usuario(id),
    FOREIGN KEY (nivelAcademicoId) REFERENCES NivelAcademico(id)
);

-- Tabla EvaluacionDocente
CREATE TABLE EvaluacionDocente (
    id INT PRIMARY KEY,
    periodo VARCHAR(20),
    puntaje FLOAT,
    docenteId INT UNIQUE,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Tabla CursoCapacitacion
CREATE TABLE CursoCapacitacion (
    id INT PRIMARY KEY,
    nombre VARCHAR(100),
    horas INT,
    fechaInicio DATE,
    fechaFin DATE,
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Tabla ProyectoInvestigacion
CREATE TABLE ProyectoInvestigacion (
    id INT PRIMARY KEY,
    titulo VARCHAR(200),
    fechaInicio DATE,
    fechaFin DATE,
    rolEnProyecto VARCHAR(100),
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Tabla PublicacionAcademica
CREATE TABLE PublicacionAcademica (
    id INT PRIMARY KEY,
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
    id INT PRIMARY KEY,
    nombre VARCHAR(200),
    porcentajeAsignado INT
);

-- Tabla CumplimientoRequisito
CREATE TABLE CumplimientoRequisito (
    id INT PRIMARY KEY,
    docenteId INT,
    requisitoId INT,
    cumplido BIT,
    fechaCumplimiento DATE,
    FOREIGN KEY (docenteId) REFERENCES Docente(id),
    FOREIGN KEY (requisitoId) REFERENCES RequisitoPromocion(id)
);

-- Tabla ReporteAvance
CREATE TABLE ReporteAvance (
    id INT PRIMARY KEY,
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
    administradorId INT,
    observaciones VARCHAR(300),
    nuevoNivelAcademicoId INT,
    FOREIGN KEY (docenteId) REFERENCES Docente(id),
    FOREIGN KEY (administradorId) REFERENCES Administrador(id),
    FOREIGN KEY (nuevoNivelAcademicoId) REFERENCES NivelAcademico(id)
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

-- Roles
INSERT INTO Rol (id, rol) VALUES (1, 'DOCENTE'), (2, 'ADMINISTRADOR');

-- Personas
INSERT INTO Persona (id, nombres, apellidos, cedula, telefono, direccion, fechaNacimiento) VALUES
(1, 'María', 'López Ramírez', '0102030405', '0987654321', 'Av. Simón Bolívar y Loja, Ambato', '1985-05-15'),
(2, 'Pedro', 'Ruiz Morales', '0607080910', '0991234567', 'Av. Cevallos y Espejo, Ambato', '1980-11-22');

-- Usuarios
INSERT INTO Usuario (id, correo, passwordHash, rolId, personaId, activo) VALUES
(1, 'maria@example.com', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 1, 1, 1),
(2, 'pedro@example.com', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 2, 2, 1);

-- Administrador
INSERT INTO Administrador (id, usuarioId) VALUES
(1, 2);

-- Nivel Académico
INSERT INTO NivelAcademico (id, nombre, descripcion) VALUES
(1, 'DT3', 'Docente con experiencia media');

-- Docente
INSERT INTO Docente (id, usuarioId, nivelAcademicoId, fechaInicioNivel) VALUES
(1, 1, 1, '2020-05-01');

-- Evaluación Docente
INSERT INTO EvaluacionDocente (id, periodo, puntaje, docenteId) VALUES
(1, '2024A', 8.7, 1);

-- Curso Capacitación
INSERT INTO CursoCapacitacion (id, nombre, horas, fechaInicio, fechaFin, docenteId) VALUES
(1, 'Innovación educativa', 40, '2024-01-10', '2024-01-20', 1);

-- Proyecto de Investigación
INSERT INTO ProyectoInvestigacion (id, titulo, fechaInicio, fechaFin, rolEnProyecto, docenteId) VALUES
(1, 'IA en educación', '2024-03-01', '2024-06-30', 'Investigador Principal', 1);

-- Publicación Académica
INSERT INTO PublicacionAcademica (id, titulo, revista, volumen, anio, tipo, docenteId) VALUES
(1, 'Nuevas metodologías', 'Revista EDUCA', 'Vol. 12', 2024, 'Artículo', 1);

-- Requisito de Promoción
INSERT INTO RequisitoPromocion (id, nombre, porcentajeAsignado) VALUES
(1, 'Participación en proyecto de investigación', 30);

-- Cumplimiento del Requisito
INSERT INTO CumplimientoRequisito (id, docenteId, requisitoId, cumplido, fechaCumplimiento) VALUES
(1, 1, 1, 1, '2024-06-01');

-- Reporte de Avance
INSERT INTO ReporteAvance (id, fechaGeneracion, docenteId) VALUES
(1, '2024-06-01', 1);
