-- =====================
-- CREAR Y USAR BASE DE DATOS
-- =====================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MyCleanAppDB')
BEGIN
    CREATE DATABASE MyCleanAppDB;
END;
GO
USE MyCleanAppDB;
GO

-- =====================
-- CREAR TABLAS
-- =====================

-- Tabla Persona
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Persona' AND xtype='U')
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
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Usuario' AND xtype='U')
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
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NivelAcademico' AND xtype='U')
CREATE TABLE NivelAcademico (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(50),
    descripcion VARCHAR(200)
);

-- Tabla Docente
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Docente' AND xtype='U')
CREATE TABLE Docente (
    id INT PRIMARY KEY IDENTITY(1,1),
    usuarioId INT UNIQUE,
    nivelAcademicoId INT,
    fechaInicioNivel DATE,
    FOREIGN KEY (usuarioId) REFERENCES Usuario(id),
    FOREIGN KEY (nivelAcademicoId) REFERENCES NivelAcademico(id)
);

-- Tabla EvaluacionDocente
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EvaluacionDocente' AND xtype='U')
CREATE TABLE EvaluacionDocente (
    id INT PRIMARY KEY IDENTITY(1,1),
    periodo VARCHAR(20),
    puntaje FLOAT,
    docenteId INT UNIQUE,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Tabla CursoCapacitacion
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CursoCapacitacion' AND xtype='U')
CREATE TABLE CursoCapacitacion (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100),
    horas INT,
    fechaInicio DATE,
    fechaFin DATE,
    docenteId INT,
    externo BIT DEFAULT 0,
    certificado VARBINARY(MAX) NULL, -- Para guardar PDF
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Tabla ProyectoInvestigacion
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProyectoInvestigacion' AND xtype='U')
CREATE TABLE ProyectoInvestigacion (
    id INT PRIMARY KEY IDENTITY(1,1),
    titulo VARCHAR(200),
    fechaInicio DATE,
    fechaFin DATE,
    rolEnProyecto VARCHAR(100),
    docenteId INT,
    externo BIT DEFAULT 0,
    documento VARBINARY(MAX) NULL, -- Para guardar PDF u otro archivo
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Asegura que todos los registros existentes tengan un valor válido en 'externo'
-- UPDATE ProyectoInvestigacion SET externo = 0 WHERE externo IS NULL;

-- Hace que la columna no acepte valores nulos en el futuro
-- ALTER TABLE ProyectoInvestigacion ALTER COLUMN externo BIT NOT NULL;

-- Tabla PublicacionAcademica
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PublicacionAcademica' AND xtype='U')
CREATE TABLE PublicacionAcademica (
    id INT PRIMARY KEY IDENTITY(1,1),
    titulo VARCHAR(200),
    revista VARCHAR(100),
    volumen VARCHAR(50),
    anio INT,
    tipo VARCHAR(50),
    docenteId INT,
    externo BIT DEFAULT 0,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Asegura que todos los registros existentes tengan un valor válido en 'externo'
-- UPDATE PublicacionAcademica SET externo = 0 WHERE externo IS NULL;

-- Hace que la columna no acepte valores nulos en el futuro
-- ALTER TABLE PublicacionAcademica ALTER COLUMN externo BIT NOT NULL;

-- Tabla RequisitoPromocion
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RequisitoPromocion' AND xtype='U')
CREATE TABLE RequisitoPromocion (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(200),
    porcentajeAsignado INT
);

-- Tabla CumplimientoRequisito
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CumplimientoRequisito' AND xtype='U')
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
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ReporteAvance' AND xtype='U')
CREATE TABLE ReporteAvance (
    id INT PRIMARY KEY IDENTITY(1,1),
    fechaGeneracion DATE,
    docenteId INT,
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SolicitudAvanceRango' AND xtype='U')
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
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TipoRequisito' AND xtype='U')
CREATE TABLE TipoRequisito (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) -- Ejemplo: 'Años en el rango', 'Papers', 'Puntaje Evaluación', 'Horas Capacitación', 'Investigaciones'
);

-- Tabla RequisitoNivelAcademico (requisitos por nivel)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RequisitoNivelAcademico' AND xtype='U')
CREATE TABLE RequisitoNivelAcademico (
    id INT PRIMARY KEY IDENTITY(1,1),
    nivelAcademicoId INT,
    tipoRequisitoId INT,
    valorRequerido FLOAT, -- Puede ser años, cantidad, porcentaje, etc.
    FOREIGN KEY (nivelAcademicoId) REFERENCES NivelAcademico(id),
    FOREIGN KEY (tipoRequisitoId) REFERENCES TipoRequisito(id)
);

-- Eliminar columnas duplicadas si existen
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PublicacionAcademica' AND COLUMN_NAME = 'certificado')
ALTER TABLE PublicacionAcademica DROP COLUMN certificado;

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CursoCapacitacion' AND COLUMN_NAME = 'documento')
ALTER TABLE CursoCapacitacion DROP COLUMN documento;

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ProyectoInvestigacion' AND COLUMN_NAME = 'archivo')
ALTER TABLE ProyectoInvestigacion DROP COLUMN archivo;

-- Agregar columnas solo si no existen
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PublicacionAcademica' AND COLUMN_NAME = 'archivo')
ALTER TABLE PublicacionAcademica ADD archivo VARBINARY(MAX) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CursoCapacitacion' AND COLUMN_NAME = 'certificado')
ALTER TABLE CursoCapacitacion ADD certificado VARBINARY(MAX) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ProyectoInvestigacion' AND COLUMN_NAME = 'documento')
ALTER TABLE ProyectoInvestigacion ADD documento VARBINARY(MAX) NULL;


-- =====================
-- INSERTAR DATOS DE PRUEBA
-- =====================

-- Personas (insertar solo si no existen)
IF NOT EXISTS (SELECT 1 FROM Persona WHERE cedula = '0102030405')
INSERT INTO Persona (nombres, apellidos, cedula, telefono, direccion, fechaNacimiento) VALUES
('María', 'López Ramírez', '0102030405', '0987654321', 'Av. Simón Bolívar y Loja, Ambato', '1985-05-15');

IF NOT EXISTS (SELECT 1 FROM Persona WHERE cedula = '0607080910')
INSERT INTO Persona (nombres, apellidos, cedula, telefono, direccion, fechaNacimiento) VALUES
('Pedro', 'Ruiz Morales', '0607080910', '0991234567', 'Av. Cevallos y Espejo, Ambato', '1980-11-22');

-- Usuarios (insertar solo si no existen)
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE correo = 'maria@example.com')
INSERT INTO Usuario (correo, passwordHash, rol, personaId, activo) VALUES
('maria@example.com', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'ADMINISTRADOR', 1, 1);

IF NOT EXISTS (SELECT 1 FROM Usuario WHERE correo = 'pedro@example.com')
INSERT INTO Usuario (correo, passwordHash, rol, personaId, activo) VALUES
('pedro@example.com', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'DOCENTE', 2, 1);

-- Niveles Académicos (insertar solo si no existen)
IF NOT EXISTS (SELECT 1 FROM NivelAcademico WHERE nombre = 'DT2')
INSERT INTO NivelAcademico (nombre, descripcion) VALUES
('DT2', 'Docente con experiencia inicial');

IF NOT EXISTS (SELECT 1 FROM NivelAcademico WHERE nombre = 'DT3')
INSERT INTO NivelAcademico (nombre, descripcion) VALUES
('DT3', 'Docente con experiencia media');

IF NOT EXISTS (SELECT 1 FROM NivelAcademico WHERE nombre = 'DT4')
INSERT INTO NivelAcademico (nombre, descripcion) VALUES
('DT4', 'Docente con experiencia avanzada');

IF NOT EXISTS (SELECT 1 FROM NivelAcademico WHERE nombre = 'DT5')
INSERT INTO NivelAcademico (nombre, descripcion) VALUES
('DT5', 'Docente con experiencia superior');

IF NOT EXISTS (SELECT 1 FROM NivelAcademico WHERE nombre = 'DT1')
INSERT INTO NivelAcademico (nombre, descripcion) VALUES
('DT1', 'Docente sin experiencia');

-- Docente (insertar solo si no existe)
IF NOT EXISTS (SELECT 1 FROM Docente WHERE usuarioId = 2)
INSERT INTO Docente (usuarioId, nivelAcademicoId, fechaInicioNivel) VALUES
(2, 1, '2020-05-01');

-- Evaluación Docente (insertar solo si no existe)
IF NOT EXISTS (SELECT 1 FROM EvaluacionDocente WHERE docenteId = 1 AND periodo = '2024A')
INSERT INTO EvaluacionDocente (periodo, puntaje, docenteId) VALUES
('2024A', 87, 1);

-- Curso Capacitación (insertar solo si no existe)
IF NOT EXISTS (SELECT 1 FROM CursoCapacitacion WHERE nombre = 'Innovación educativa' AND docenteId = 1)
INSERT INTO CursoCapacitacion (nombre, horas, fechaInicio, fechaFin, docenteId, externo) VALUES
('Innovación educativa', 40, '2024-01-10', '2024-01-20', 1, 0);

-- Proyecto de Investigación (insertar solo si no existe)
IF NOT EXISTS (SELECT 1 FROM ProyectoInvestigacion WHERE titulo = 'IA en educación' AND docenteId = 1)
INSERT INTO ProyectoInvestigacion (titulo, fechaInicio, fechaFin, rolEnProyecto, docenteId, externo) VALUES
('IA en educación', '2024-03-01', '2024-06-30', 'Investigador Principal', 1, 0);

-- Publicación Académica (insertar solo si no existe)
IF NOT EXISTS (SELECT 1 FROM PublicacionAcademica WHERE titulo = 'Nuevas metodologías' AND docenteId = 1)
INSERT INTO PublicacionAcademica (titulo, revista, volumen, anio, tipo, docenteId, externo) VALUES
('Nuevas metodologías', 'Revista EDUCA', 'Vol. 12', 2024, 'Artículo', 1, 0);

-- Requisito de Promoción (insertar solo si no existe)
IF NOT EXISTS (SELECT 1 FROM RequisitoPromocion WHERE nombre = 'Participación en proyecto de investigación')
INSERT INTO RequisitoPromocion (nombre, porcentajeAsignado) VALUES
('Participación en proyecto de investigación', 30);

-- Cumplimiento del Requisito (insertar solo si no existe)
IF NOT EXISTS (SELECT 1 FROM CumplimientoRequisito WHERE docenteId = 1 AND requisitoId = 1)
INSERT INTO CumplimientoRequisito (docenteId, requisitoId, cumplido, fechaCumplimiento) VALUES
(1, 1, 1, '2024-06-01');

-- Reporte de Avance (insertar solo si no existe)
IF NOT EXISTS (SELECT 1 FROM ReporteAvance WHERE docenteId = 1 AND fechaGeneracion = '2024-06-01')
INSERT INTO ReporteAvance (fechaGeneracion, docenteId) VALUES
('2024-06-01', 1);

-- Tipos de requisito (insertar solo si no existen)
IF NOT EXISTS (SELECT 1 FROM TipoRequisito WHERE nombre = 'Años en el rango')
INSERT INTO TipoRequisito (nombre) VALUES ('Años en el rango');

IF NOT EXISTS (SELECT 1 FROM TipoRequisito WHERE nombre = 'Papers')
INSERT INTO TipoRequisito (nombre) VALUES ('Papers');

IF NOT EXISTS (SELECT 1 FROM TipoRequisito WHERE nombre = 'Puntaje Evaluación')
INSERT INTO TipoRequisito (nombre) VALUES ('Puntaje Evaluación');

IF NOT EXISTS (SELECT 1 FROM TipoRequisito WHERE nombre = 'Horas Capacitación')
INSERT INTO TipoRequisito (nombre) VALUES ('Horas Capacitación');

IF NOT EXISTS (SELECT 1 FROM TipoRequisito WHERE nombre = 'Investigaciones')
INSERT INTO TipoRequisito (nombre) VALUES ('Investigaciones');

-- Requisitos para DT2 (insertar solo si no existen)
IF NOT EXISTS (SELECT 1 FROM RequisitoNivelAcademico WHERE nivelAcademicoId = 1 AND tipoRequisitoId = 1)
BEGIN
    INSERT INTO RequisitoNivelAcademico (nivelAcademicoId, tipoRequisitoId, valorRequerido) VALUES
    (1, 1, 4),    -- 4 años en DT1
    (1, 2, 1),    -- 1 paper
    (1, 3, 75),   -- 75% puntaje
    (1, 4, 96);   -- 96 horas capacitación
END

-- Requisitos para DT3 (insertar solo si no existen)
IF NOT EXISTS (SELECT 1 FROM RequisitoNivelAcademico WHERE nivelAcademicoId = 2 AND tipoRequisitoId = 1)
BEGIN
    INSERT INTO RequisitoNivelAcademico (nivelAcademicoId, tipoRequisitoId, valorRequerido) VALUES
    (2, 1, 4),    -- 4 años en DT2
    (2, 2, 2),    -- 2 papers
    (2, 3, 75),   -- 75% puntaje
    (2, 4, 96),   -- 96 horas capacitación
    (2, 5, 12);    -- 12 meses de investigación
END

-- Requisitos para DT4 (insertar solo si no existen)
IF NOT EXISTS (SELECT 1 FROM RequisitoNivelAcademico WHERE nivelAcademicoId = 3 AND tipoRequisitoId = 1)
BEGIN
    INSERT INTO RequisitoNivelAcademico (nivelAcademicoId, tipoRequisitoId, valorRequerido) VALUES
    (3, 1, 4),    -- 4 años en DT3
    (3, 2, 3),    -- 3 papers
    (3, 3, 75),   -- 75% puntaje
    (3, 4, 128),   -- 128 horas capacitación
    (3, 5, 24);    -- 24 meses de investigación
END

-- Requisitos para DT5 (insertar solo si no existen)
IF NOT EXISTS (SELECT 1 FROM RequisitoNivelAcademico WHERE nivelAcademicoId = 4 AND tipoRequisitoId = 1)
BEGIN
    INSERT INTO RequisitoNivelAcademico (nivelAcademicoId, tipoRequisitoId, valorRequerido) VALUES
    (4, 1, 4),    -- 4 años en DT4
    (4, 2, 5),    -- 5 papers
    (4, 3, 75),   -- 75% puntaje
    (4, 4, 160),   -- 160 horas capacitación
    (4, 5, 24);    -- 24 meses de investigación
END

-- Actualizar registros existentes para asegurar consistencia
UPDATE PublicacionAcademica SET externo = 0 WHERE externo IS NULL;

-- Tabla para la Comisión Académica de Escalafón y Promoción
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ComisionAcademica' AND xtype='U')
CREATE TABLE ComisionAcademica (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100),
    cargo VARCHAR(100), -- Presidente, Miembro, Secretario, Asesor
    usuarioId INT,
    activo BIT DEFAULT 1,
    fechaDesignacion DATE,
    fechaFinPeriodo DATE,
    FOREIGN KEY (usuarioId) REFERENCES Usuario(id)
);

-- Tabla para Lista de Verificación (Anexo 1)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ListaVerificacion' AND xtype='U')
CREATE TABLE ListaVerificacion (
    id INT PRIMARY KEY IDENTITY(1,1),
    nivelAcademicoId INT,
    nombreDocumento VARCHAR(200),
    descripcion VARCHAR(500),
    obligatorio BIT DEFAULT 1,
    orden INT,
    activo BIT DEFAULT 1,
    FOREIGN KEY (nivelAcademicoId) REFERENCES NivelAcademico(id)
);

-- Tabla para el seguimiento de verificación de documentos por solicitud
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='VerificacionDocumentos' AND xtype='U')
CREATE TABLE VerificacionDocumentos (
    id INT PRIMARY KEY IDENTITY(1,1),
    solicitudId INT,
    listaVerificacionId INT,
    verificado BIT DEFAULT 0,
    observaciones VARCHAR(300),
    fechaVerificacion DATE,
    verificadoPor INT, -- usuarioId quien verificó
    FOREIGN KEY (solicitudId) REFERENCES SolicitudAvanceRango(id),
    FOREIGN KEY (listaVerificacionId) REFERENCES ListaVerificacion(id),
    FOREIGN KEY (verificadoPor) REFERENCES Usuario(id)
);

-- Tabla para Apelaciones
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ApelacionPromocion' AND xtype='U')
CREATE TABLE ApelacionPromocion (
    id INT PRIMARY KEY IDENTITY(1,1),
    solicitudId INT,
    fechaApelacion DATE,
    motivoApelacion VARCHAR(1000),
    documentosRespaldo VARCHAR(500), -- Lista de documentos adjuntos
    estado VARCHAR(20) DEFAULT 'PENDIENTE', -- PENDIENTE, APROBADA, RECHAZADA
    fechaRespuesta DATE,
    respuestaComision VARCHAR(1000),
    resuelto BIT DEFAULT 0,
    FOREIGN KEY (solicitudId) REFERENCES SolicitudAvanceRango(id)
);

-- Tabla para seguimiento de plazos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SeguimientoPlazos' AND xtype='U')
CREATE TABLE SeguimientoPlazos (
    id INT PRIMARY KEY IDENTITY(1,1),
    solicitudId INT,
    tipoEvento VARCHAR(100), -- 'NOTIFICACION_RESULTADO', 'PLAZO_RESPUESTA', 'APELACION', 'RESPUESTA_APELACION'
    fechaEvento DATE,
    fechaLimite DATE,
    cumplido BIT DEFAULT 0,
    observaciones VARCHAR(300),
    FOREIGN KEY (solicitudId) REFERENCES SolicitudAvanceRango(id)
);

-- Tabla para Informes Finales
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InformeFinalPromocion' AND xtype='U')
CREATE TABLE InformeFinalPromocion (
    id INT PRIMARY KEY IDENTITY(1,1),
    solicitudId INT,
    fechaGeneracion DATE,
    contenido TEXT,
    estado VARCHAR(20) DEFAULT 'GENERADO', -- GENERADO, ENVIADO_CONSEJO, APROBADO
    fechaEnvioConsejo DATE,
    fechaAprobacionConsejo DATE,
    generadoPor INT, -- usuarioId quien generó
    FOREIGN KEY (solicitudId) REFERENCES SolicitudAvanceRango(id),
    FOREIGN KEY (generadoPor) REFERENCES Usuario(id)
);

-- Tabla para Planificación Institucional
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PlanificacionInstitucional' AND xtype='U')
CREATE TABLE PlanificacionInstitucional (
    id INT PRIMARY KEY IDENTITY(1,1),
    anio INT,
    fechaInicioConvocatoria DATE,
    fechaFinConvocatoria DATE,
    fechaInicioEvaluacion DATE,
    fechaFinEvaluacion DATE,
    fechaNotificacionResultados DATE,
    presupuestoDisponible DECIMAL(15,2),
    activo BIT DEFAULT 1,
    aprobadoConsejo BIT DEFAULT 0,
    fechaAprobacion DATE
);

-- Agregar campos adicionales a SolicitudAvanceRango para seguimiento completo
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SolicitudAvanceRango' AND COLUMN_NAME = 'fechaPresentacion')
ALTER TABLE SolicitudAvanceRango 
ADD fechaPresentacion DATE,
    fechaRecepcionTalentoHumano DATE,
    fechaEnvioComision DATE,
    documentosVerificados BIT DEFAULT 0,
    verificadoPor INT,
    planificacionId INT;

-- Agregar foreign keys solo si no existen
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SolicitudAvanceRango_VerificadoPor')
ALTER TABLE SolicitudAvanceRango 
ADD CONSTRAINT FK_SolicitudAvanceRango_VerificadoPor FOREIGN KEY (verificadoPor) REFERENCES Usuario(id);

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SolicitudAvanceRango_Planificacion')
ALTER TABLE SolicitudAvanceRango 
ADD CONSTRAINT FK_SolicitudAvanceRango_Planificacion FOREIGN KEY (planificacionId) REFERENCES PlanificacionInstitucional(id);

-- Insertar datos para la metodología oficial

-- Crear rol específico para la comisión académica (solo si no existe)
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE correo = 'vicerrector.academico@uta.edu.ec')
INSERT INTO Usuario (correo, passwordHash, rol, personaId, activo) VALUES
('vicerrector.academico@uta.edu.ec', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'COMISION_PRESIDENTE', 1, 1);

-- Comisión Académica de Escalafón y Promoción (solo si no existe)
IF NOT EXISTS (SELECT 1 FROM ComisionAcademica WHERE cargo = 'PRESIDENTE' AND usuarioId = 3)
INSERT INTO ComisionAcademica (nombre, cargo, usuarioId, activo, fechaDesignacion, fechaFinPeriodo) VALUES
('Vicerrector Académico', 'PRESIDENTE', 3, 1, '2024-01-01', '2026-01-01');

IF NOT EXISTS (SELECT 1 FROM ComisionAcademica WHERE cargo = 'SECRETARIO' AND usuarioId = 1)
INSERT INTO ComisionAcademica (nombre, cargo, usuarioId, activo, fechaDesignacion, fechaFinPeriodo) VALUES
('Director de Talento Humano', 'SECRETARIO', 1, 1, '2024-01-01', '2026-01-01');

-- Lista de Verificación para DT2 (ejemplo)
INSERT INTO ListaVerificacion (nivelAcademicoId, nombreDocumento, descripcion, obligatorio, orden, activo) VALUES
(1, 'Hoja de Vida Actualizada', 'Curriculum vitae actualizado y firmado', 1, 1, 1),
(1, 'Cédula de Identidad', 'Copia certificada de la cédula de identidad', 1, 2, 1),
(1, 'Título de Tercer Nivel', 'Copia certificada del título de grado', 1, 3, 1),
(1, 'Certificado de Tiempo de Servicio', 'Certificado que acredite 4 años en DT1', 1, 4, 1),
(1, 'Publicaciones Académicas', 'Certificado de al menos 1 publicación', 1, 5, 1),
(1, 'Evaluación Docente', 'Certificado de evaluación ≥75%', 1, 6, 1),
(1, 'Certificados de Capacitación', 'Certificados que sumen 96 horas', 1, 7, 1);

-- Lista de Verificación para DT3
INSERT INTO ListaVerificacion (nivelAcademicoId, nombreDocumento, descripcion, obligatorio, orden, activo) VALUES
(2, 'Hoja de Vida Actualizada', 'Curriculum vitae actualizado y firmado', 1, 1, 1),
(2, 'Cédula de Identidad', 'Copia certificada de la cédula de identidad', 1, 2, 1),
(2, 'Título de Tercer Nivel', 'Copia certificada del título de grado', 1, 3, 1),
(2, 'Certificado de Tiempo de Servicio', 'Certificado que acredite 4 años en DT2', 1, 4, 1),
(2, 'Publicaciones Académicas', 'Certificado de al menos 2 publicaciones', 1, 5, 1),
(2, 'Evaluación Docente', 'Certificado de evaluación ≥75%', 1, 6, 1),
(2, 'Certificados de Capacitación', 'Certificados que sumen 96 horas', 1, 7, 1),
(2, 'Proyectos de Investigación', 'Certificado de 12 meses de investigación', 1, 8, 1);

-- Planificación Institucional 2024 (solo si no existe)
IF NOT EXISTS (SELECT 1 FROM PlanificacionInstitucional WHERE anio = 2024)
INSERT INTO PlanificacionInstitucional (anio, fechaInicioConvocatoria, fechaFinConvocatoria, fechaInicioEvaluacion, fechaFinEvaluacion, fechaNotificacionResultados, presupuestoDisponible, activo, aprobadoConsejo, fechaAprobacion) VALUES
(2024, '2024-07-01', '2024-07-31', '2024-08-01', '2024-08-31', '2024-09-15', 50000.00, 1, 1, '2024-06-15');

-- Actualizar la solicitud existente con los nuevos campos
UPDATE SolicitudAvanceRango 
SET fechaPresentacion = '2024-07-15',
    fechaRecepcionTalentoHumano = '2024-07-15',
    fechaEnvioComision = '2024-07-16',
    documentosVerificados = 0,
    planificacionId = 1
WHERE id = 1;

-- Agregar algunas verificaciones de documentos de ejemplo
-- (Estas se insertarían cuando una solicitud se presenta)

-- Actualizar para agregar más roles (solo si no existen)
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE correo = 'comision.miembro1@uta.edu.ec')
INSERT INTO Usuario (correo, passwordHash, rol, personaId, activo) VALUES
('comision.miembro1@uta.edu.ec', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'COMISION_MIEMBRO', 1, 1);

IF NOT EXISTS (SELECT 1 FROM Usuario WHERE correo = 'consejo.universitario@uta.edu.ec')
INSERT INTO Usuario (correo, passwordHash, rol, personaId, activo) VALUES
('consejo.universitario@uta.edu.ec', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'CONSEJO_UNIVERSITARIO', 1, 1);

-- Actualizar registros existentes para asegurar consistencia
UPDATE PublicacionAcademica SET externo = 0 WHERE externo IS NULL;

-- =====================
-- INSERTAR MÁS DATOS DE EJEMPLO PARA PRUEBAS COMPLETAS
-- =====================

-- Agregar más personas para tener más docentes (solo si no existen)
IF NOT EXISTS (SELECT 1 FROM Persona WHERE cedula = '1234567890')
INSERT INTO Persona (nombres, apellidos, cedula, telefono, direccion, fechaNacimiento) VALUES
('Ana', 'García Pérez', '1234567890', '0987654322', 'Av. Los Andes 123, Ambato', '1982-03-10');

IF NOT EXISTS (SELECT 1 FROM Persona WHERE cedula = '9876543210')
INSERT INTO Persona (nombres, apellidos, cedula, telefono, direccion, fechaNacimiento) VALUES
('Carlos', 'Mendoza Silva', '9876543210', '0998877665', 'Calle Bolívar 456, Ambato', '1978-09-25');

IF NOT EXISTS (SELECT 1 FROM Persona WHERE cedula = '5555666677')
INSERT INTO Persona (nombres, apellidos, cedula, telefono, direccion, fechaNacimiento) VALUES
('Laura', 'Vásquez Torres', '5555666677', '0991122334', 'Av. Atahualpa 789, Ambato', '1985-12-08');

-- Agregar más usuarios (solo si no existen)
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE correo = 'ana.garcia@uta.edu.ec')
INSERT INTO Usuario (correo, passwordHash, rol, personaId, activo) VALUES
('ana.garcia@uta.edu.ec', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'DOCENTE', 3, 1);

IF NOT EXISTS (SELECT 1 FROM Usuario WHERE correo = 'carlos.mendoza@uta.edu.ec')
INSERT INTO Usuario (correo, passwordHash, rol, personaId, activo) VALUES
('carlos.mendoza@uta.edu.ec', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'DOCENTE', 4, 1);

IF NOT EXISTS (SELECT 1 FROM Usuario WHERE correo = 'laura.vasquez@uta.edu.ec')
INSERT INTO Usuario (correo, passwordHash, rol, personaId, activo) VALUES
('laura.vasquez@uta.edu.ec', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'DOCENTE', 5, 1);

-- Agregar más docentes con diferentes niveles (solo si no existen)
IF NOT EXISTS (SELECT 1 FROM Docente WHERE usuarioId = 6)
INSERT INTO Docente (usuarioId, nivelAcademicoId, fechaInicioNivel) VALUES
(6, 2, '2020-01-01'); -- Ana en DT3

IF NOT EXISTS (SELECT 1 FROM Docente WHERE usuarioId = 7)
INSERT INTO Docente (usuarioId, nivelAcademicoId, fechaInicioNivel) VALUES
(7, 3, '2019-06-01'); -- Carlos en DT4

IF NOT EXISTS (SELECT 1 FROM Docente WHERE usuarioId = 8)
INSERT INTO Docente (usuarioId, nivelAcademicoId, fechaInicioNivel) VALUES
(8, 1, '2022-03-01'); -- Laura en DT2

-- Agregar más evaluaciones docentes
INSERT INTO EvaluacionDocente (periodo, puntaje, docenteId) VALUES
('2024A', 92, 2), -- Ana
('2024A', 88, 3), -- Carlos
('2024A', 76, 4); -- Laura

-- Agregar más cursos de capacitación
INSERT INTO CursoCapacitacion (nombre, horas, fechaInicio, fechaFin, docenteId, externo) VALUES
('Metodologías Activas', 32, '2024-02-01', '2024-02-10', 2, 0),
('Evaluación por Competencias', 40, '2024-03-15', '2024-03-25', 2, 0),
('Tecnología Educativa', 24, '2024-04-01', '2024-04-08', 2, 0),
('Investigación Científica', 48, '2024-01-10', '2024-01-25', 3, 0),
('Gestión Académica', 40, '2024-02-15', '2024-02-25', 3, 0),
('Liderazgo Educativo', 32, '2024-03-01', '2024-03-10', 3, 0),
('Pedagogía Universitaria', 40, '2024-05-01', '2024-05-15', 4, 0),
('Didáctica General', 32, '2024-06-01', '2024-06-10', 4, 0);

-- Agregar más proyectos de investigación
INSERT INTO ProyectoInvestigacion (titulo, fechaInicio, fechaFin, rolEnProyecto, docenteId, externo) VALUES
('Machine Learning en Educación', '2023-01-01', '2024-12-31', 'Investigador Principal', 2, 0),
('Metodologías Innovadoras', '2023-06-01', '2024-05-31', 'Co-investigador', 2, 0),
('Gestión del Conocimiento', '2022-01-01', '2024-06-30', 'Director', 3, 0),
('Calidad Educativa', '2023-03-01', '2025-02-28', 'Investigador Principal', 3, 0),
('Tecnología y Aprendizaje', '2024-01-01', '2024-12-31', 'Co-investigador', 4, 0);

-- Agregar más publicaciones académicas
INSERT INTO PublicacionAcademica (titulo, revista, volumen, anio, tipo, docenteId, externo) VALUES
('IA aplicada a la educación superior', 'Revista Tecnología Educativa', 'Vol. 15', 2024, 'Artículo', 2, 0),
('Metodologías activas en ingeniería', 'Journal of Engineering Education', 'Vol. 8', 2023, 'Artículo', 2, 0),
('Gestión universitaria moderna', 'Revista Gestión Académica', 'Vol. 22', 2024, 'Artículo', 3, 0),
('Calidad en educación superior', 'Revista Evaluación Educativa', 'Vol. 12', 2023, 'Artículo', 3, 0),
('Innovación tecnológica educativa', 'Tech in Education', 'Vol. 5', 2024, 'Artículo', 3, 0),
('Aprendizaje digital', 'Digital Learning Journal', 'Vol. 3', 2024, 'Artículo', 4, 0);

-- Agregar más requisitos de promoción
INSERT INTO RequisitoPromocion (nombre, porcentajeAsignado) VALUES
('Tiempo de servicio en el nivel', 25),
('Publicaciones científicas', 20),
('Evaluación del desempeño docente', 20),
('Capacitación profesional', 15),
('Participación en investigación', 20);

-- Agregar más cumplimientos de requisitos
INSERT INTO CumplimientoRequisito (docenteId, requisitoId, cumplido, fechaCumplimiento) VALUES
-- Para docente 1 (Pedro)
(1, 2, 1, '2024-01-15'),
(1, 3, 1, '2024-02-20'),
(1, 4, 1, '2024-03-10'),
(1, 5, 1, '2024-04-05'),
-- Para docente 2 (Ana)
(2, 1, 1, '2024-01-01'),
(2, 2, 1, '2024-02-15'),
(2, 3, 1, '2024-03-20'),
(2, 4, 1, '2024-04-10'),
(2, 5, 1, '2024-05-05'),
-- Para docente 3 (Carlos)
(3, 1, 1, '2024-01-01'),
(3, 2, 1, '2024-02-10'),
(3, 3, 1, '2024-03-15'),
(3, 4, 1, '2024-04-20'),
(3, 5, 1, '2024-05-25'),
-- Para docente 4 (Laura)
(4, 1, 0, NULL), -- No cumple tiempo
(4, 2, 1, '2024-06-01'),
(4, 3, 1, '2024-03-20'),
(4, 4, 1, '2024-05-15'),
(4, 5, 0, NULL); -- No cumple investigación

-- Agregar más solicitudes de avance
INSERT INTO SolicitudAvanceRango (docenteId, fechaSolicitud, estado, fechaRespuesta, observaciones, nuevoNivelAcademicoId, fechaPresentacion, fechaRecepcionTalentoHumano, fechaEnvioComision, documentosVerificados, planificacionId) VALUES
(2, '2024-07-20', 'APROBADA', '2024-08-15', 'Cumple con todos los requisitos', 3, '2024-07-20', '2024-07-20', '2024-07-21', 1, 1),
(3, '2024-07-25', 'PENDIENTE', NULL, NULL, 4, '2024-07-25', '2024-07-25', '2024-07-26', 1, 1),
(4, '2024-08-01', 'RECHAZADA', '2024-08-20', 'No cumple con el tiempo mínimo en el nivel', 2, '2024-08-01', '2024-08-01', '2024-08-02', 1, 1);

-- Agregar más miembros a la comisión
INSERT INTO ComisionAcademica (nombre, cargo, usuarioId, activo, fechaDesignacion, fechaFinPeriodo) VALUES
('Dr. Juan Pérez', 'MIEMBRO', 9, 1, '2024-01-01', '2026-01-01'),
('Dra. María González', 'MIEMBRO', 10, 1, '2024-01-01', '2026-01-01');

-- Agregar verificaciones de documentos para las solicitudes
INSERT INTO VerificacionDocumentos (solicitudId, listaVerificacionId, verificado, observaciones, fechaVerificacion, verificadoPor) VALUES
-- Para solicitud 1 (Pedro)
(1, 1, 1, 'Documento completo y actualizado', '2024-07-16', 1),
(1, 2, 1, 'Cédula vigente', '2024-07-16', 1),
(1, 3, 1, 'Título registrado en SENESCYT', '2024-07-16', 1),
(1, 4, 0, 'Falta certificado actualizado', '2024-07-16', 1),
(1, 5, 1, 'Publicación verificada', '2024-07-16', 1),
(1, 6, 1, 'Evaluación satisfactoria', '2024-07-16', 1),
(1, 7, 1, 'Certificados válidos', '2024-07-16', 1),
-- Para solicitud 2 (Ana)
(2, 8, 1, 'Documento completo', '2024-07-21', 1),
(2, 9, 1, 'Cédula vigente', '2024-07-21', 1),
(2, 10, 1, 'Título válido', '2024-07-21', 1),
(2, 11, 1, 'Tiempo de servicio verificado', '2024-07-21', 1),
(2, 12, 1, 'Publicaciones verificadas', '2024-07-21', 1),
(2, 13, 1, 'Evaluación excelente', '2024-07-21', 1),
(2, 14, 1, 'Capacitaciones completas', '2024-07-21', 1),
(2, 15, 1, 'Investigación documentada', '2024-07-21', 1);

-- Agregar algunas apelaciones de ejemplo
INSERT INTO ApelacionPromocion (solicitudId, fechaApelacion, motivoApelacion, documentosRespaldo, estado, fechaRespuesta, respuestaComision, resuelto) VALUES
(4, '2024-08-25', 'Considero que mi experiencia previa debe ser tomada en cuenta para el cálculo del tiempo en el nivel actual', 'CV actualizado, certificados de experiencia', 'PENDIENTE', NULL, NULL, 0);

-- Agregar seguimiento de plazos
INSERT INTO SeguimientoPlazos (solicitudId, tipoEvento, fechaEvento, fechaLimite, cumplido, observaciones) VALUES
(1, 'NOTIFICACION_RESULTADO', '2024-08-15', '2024-08-15', 1, 'Notificación enviada en tiempo'),
(2, 'PLAZO_RESPUESTA', '2024-07-26', '2024-08-26', 0, 'En proceso de evaluación'),
(3, 'NOTIFICACION_RESULTADO', '2024-08-20', '2024-08-20', 1, 'Notificación de rechazo enviada'),
(3, 'PLAZO_RESPUESTA', '2024-08-25', '2024-09-25', 0, 'Apelación presentada');

-- Agregar informes finales
INSERT INTO InformeFinalPromocion (solicitudId, fechaGeneracion, contenido, estado, fechaEnvioConsejo, fechaAprobacionConsejo, generadoPor) VALUES
(2, '2024-08-30', 'Informe favorable para promoción de Ana García de DT3 a DT4. Cumple con todos los requisitos establecidos.', 'GENERADO', NULL, NULL, 1);

-- Actualizar la planificación para que esté activa
UPDATE PlanificacionInstitucional SET activo = 1 WHERE anio = 2024;

-- =====================
-- SCRIPT COMPLETADO EXITOSAMENTE
-- =====================
PRINT '========================================';
PRINT 'Script init.sql ejecutado exitosamente.';
PRINT 'Base de datos MyCleanAppDB configurada completamente.';
PRINT 'Todas las tablas han sido creadas con verificaciones IF NOT EXISTS.';
PRINT 'Todos los datos de ejemplo han sido insertados de forma idempotente.';
PRINT 'El script puede ejecutarse múltiples veces sin errores.';
PRINT '========================================';
