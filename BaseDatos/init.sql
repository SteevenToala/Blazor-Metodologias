-- CREAR TABLA Rol
CREATE TABLE Rol (
    rol VARCHAR(20) PRIMARY KEY
);

-- CREAR TABLA Usuario
CREATE TABLE Usuario (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100),
    correo VARCHAR(100) UNIQUE,
    contraseña VARCHAR(100),
    rol VARCHAR(20),
    FOREIGN KEY (rol) REFERENCES Rol(rol)
);

-- CREAR TABLA Administrador
CREATE TABLE Administrador (
    idAdmin INT PRIMARY KEY IDENTITY(1,1),
    usuarioId INT UNIQUE,
    FOREIGN KEY (usuarioId) REFERENCES Usuario(id)
);

-- CREAR TABLA EvaluacionDocente
CREATE TABLE EvaluacionDocente (
    idEvaluacion INT PRIMARY KEY IDENTITY(1,1),
    periodo VARCHAR(20),
    puntaje FLOAT,
    docenteId INT UNIQUE,
    FOREIGN KEY (docenteId) REFERENCES Usuario(id)
);

-- CREAR TABLA CursoCapacitacion
CREATE TABLE CursoCapacitacion (
    idCurso INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100),
    horas INT,
    fecha DATE,
    fechaFin DATE,
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Usuario(id)
);

-- CREAR TABLA ProyectoInvestigacion
CREATE TABLE ProyectoInvestigacion (
    idProyecto INT PRIMARY KEY IDENTITY(1,1),
    titulo VARCHAR(200),
    fechaInicio DATE,
    fechaFin DATE,
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Usuario(id)
);

-- CREAR TABLA PublicacionAcademica
CREATE TABLE PublicacionAcademica (
    idPublicacion INT PRIMARY KEY IDENTITY(1,1),
    titulo VARCHAR(200),
    revista VARCHAR(100),
    año INT,
    tipo VARCHAR(50),
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Usuario(id)
);

-- CREAR TABLA RequisitoPromocion
CREATE TABLE RequisitoPromocion (
    idRequisito INT PRIMARY KEY IDENTITY(1,1),
    descripcion VARCHAR(200),
    cumplido BIT,
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Usuario(id)
);

-- CREAR TABLA ReporteAvance
CREATE TABLE ReporteAvance (
    idReporte INT PRIMARY KEY IDENTITY(1,1),
    fechaGeneracion DATE,
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Usuario(id)
);

-- INSERTAR DATOS DE PRUEBA
-- Roles
INSERT INTO Rol VALUES ('DOCENTE'), ('ADMINISTRADOR');

-- Usuarios
INSERT INTO Usuario (nombre, correo, contraseña, rol) VALUES
('María López', 'maria@example.com', 'maria123', 'DOCENTE'),
('Pedro Ruiz', 'pedro@example.com', 'pedro123', 'ADMINISTRADOR');

-- Administrador
INSERT INTO Administrador (usuarioId) VALUES (2);

-- Evaluación Docente
INSERT INTO EvaluacionDocente (periodo, puntaje, docenteId) VALUES
('2024A', 8.7, 1);

-- Curso Capacitación
INSERT INTO CursoCapacitacion (nombre, horas, fecha, fechaFin, docenteId) VALUES
('Innovación educativa', 40, '2024-01-10', '2024-01-20', 1);

-- Proyecto de Investigación
INSERT INTO ProyectoInvestigacion (titulo, fechaInicio, fechaFin, docenteId) VALUES
('IA en educación', '2024-03-01', '2024-06-30', 1);

-- Publicación Académica
INSERT INTO PublicacionAcademica (titulo, revista, año, tipo, docenteId) VALUES
('Nuevas metodologías', 'Revista EDUCA', 2024, 'Artículo', 1);

-- Requisito de Promoción
INSERT INTO RequisitoPromocion (descripcion, cumplido, docenteId) VALUES
('Participación en proyecto de investigación', 1, 1);

-- Reporte de Avance
INSERT INTO ReporteAvance (fechaGeneracion, docenteId) VALUES
('2024-06-01', 1);
