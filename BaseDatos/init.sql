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
    externo BIT DEFAULT 0,
    certificado VARBINARY(MAX) NULL, -- Para guardar PDF
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
    externo BIT DEFAULT 0,
    documento VARBINARY(MAX) NULL, -- Para guardar PDF u otro archivo
    FOREIGN KEY (docenteId) REFERENCES Docente(id)
);

-- Asegura que todos los registros existentes tengan un valor válido en 'externo'
UPDATE ProyectoInvestigacion SET externo = 0 WHERE externo IS NULL;

-- Hace que la columna no acepte valores nulos en el futuro
ALTER TABLE ProyectoInvestigacion ALTER COLUMN externo BIT NOT NULL;

-- Tabla PublicacionAcademica
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
UPDATE PublicacionAcademica SET externo = 0 WHERE externo IS NULL;

-- Hace que la columna no acepte valores nulos en el futuro
ALTER TABLE PublicacionAcademica ALTER COLUMN externo BIT NOT NULL;

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
INSERT INTO CursoCapacitacion ( nombre, horas, fechaInicio, fechaFin, docenteId, externo) VALUES
( 'Innovación educativa', 40, '2024-01-10', '2024-01-20', 1, 0);

-- Proyecto de Investigación
INSERT INTO ProyectoInvestigacion ( titulo, fechaInicio, fechaFin, rolEnProyecto, docenteId, externo) VALUES
('IA en educación', '2024-03-01', '2024-06-30', 'Investigador Principal', 1, 0);

-- Publicación Académica
INSERT INTO PublicacionAcademica ( titulo, revista, volumen, anio, tipo, docenteId, Externo) VALUES
('Nuevas metodologías', 'Revista EDUCA', 'Vol. 12', 2024, 'Artículo', 1, 0);

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
(1, 4, 96);   -- 96 horas capacitación

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
(4, 2, 5),    -- 5 papers
(4, 3, 75),   -- 75% puntaje
(4, 4, 160),   -- 160 horas capacitación
(4, 5, 24);    -- 24 meses de investigación

UPDATE PublicacionAcademica SET Externo = 0 WHERE Externo IS NULL;

-- Tabla para la Comisión Académica de Escalafón y Promoción
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
ALTER TABLE SolicitudAvanceRango 
ADD fechaPresentacion DATE,
    fechaRecepcionTalentoHumano DATE,
    fechaEnvioComision DATE,
    documentosVerificados BIT DEFAULT 0,
    verificadoPor INT,
    planificacionId INT;

-- Agregar foreign keys
ALTER TABLE SolicitudAvanceRango 
ADD CONSTRAINT FK_SolicitudAvanceRango_VerificadoPor FOREIGN KEY (verificadoPor) REFERENCES Usuario(id);

ALTER TABLE SolicitudAvanceRango 
ADD CONSTRAINT FK_SolicitudAvanceRango_Planificacion FOREIGN KEY (planificacionId) REFERENCES PlanificacionInstitucional(id);

-- Insertar datos para la metodología oficial

-- Crear rol específico para la comisión académica
INSERT INTO Usuario (correo, passwordHash, rol, personaId, activo) VALUES
('vicerrector.academico@uta.edu.ec', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'COMISION_PRESIDENTE', 1, 1);

-- Comisión Académica de Escalafón y Promoción
INSERT INTO ComisionAcademica (nombre, cargo, usuarioId, activo, fechaDesignacion, fechaFinPeriodo) VALUES
('Vicerrector Académico', 'PRESIDENTE', 3, 1, '2024-01-01', '2026-01-01'),
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

-- Planificación Institucional 2024
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

-- Actualizar para agregar más roles
INSERT INTO Usuario (correo, passwordHash, rol, personaId, activo) VALUES
('comision.miembro1@uta.edu.ec', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'COMISION_MIEMBRO', 1, 1),
('consejo.universitario@uta.edu.ec', '$2a$11$uwsP6IVBrxm2Ju2wUcSSJ.ufVr5.3TMaOhegAOTxg62PU3meNY/cS', 'CONSEJO_UNIVERSITARIO', 1, 1);

UPDATE PublicacionAcademica SET Externo = 0 WHERE Externo IS NULL;
