-- ============================================================
-- Script de migración: Sistema de Evaluación Técnica
-- Proyecto: CRUD Genérico + Evaluación Técnica con IA
-- Fecha: 2026-03-19
-- ============================================================

USE CrudGenerico;

-- ═══════════════════════════════════════════════════════════
-- Tablas del sistema de evaluación técnica
-- ═══════════════════════════════════════════════════════════

-- ─── Tabla: Evaluaciones ────────────────────────────────────
CREATE TABLE IF NOT EXISTS Evaluaciones (
    Id                          CHAR(36)        NOT NULL PRIMARY KEY,
    Titulo                      VARCHAR(200)    NOT NULL,
    Descripcion                 VARCHAR(1000)   NULL,
    Tecnologia                  VARCHAR(100)    NOT NULL,
    Nivel                       INT             NOT NULL COMMENT '1=Junior, 2=Mid, 3=Senior, 4=Lead',
    Estado                      INT             NOT NULL DEFAULT 1 COMMENT '1=Borrador, 2=Activa, 3=Cerrada',
    TiempoLimiteTotalMinutos    INT             NOT NULL,
    RequiereCamara              TINYINT(1)      NOT NULL DEFAULT 0,
    RequiereMicrofono           TINYINT(1)      NOT NULL DEFAULT 0,
    EvaluadorId                 CHAR(36)        NOT NULL,
    CreadoEn                    DATETIME(6)     NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    ModificadoEn                DATETIME(6)     NULL,
    EstaActivo                  TINYINT(1)      NOT NULL DEFAULT 1
) ENGINE=InnoDB;

-- ─── Tabla: Preguntas ───────────────────────────────────────
CREATE TABLE IF NOT EXISTS Preguntas (
    Id                      CHAR(36)        NOT NULL PRIMARY KEY,
    Texto                   VARCHAR(2000)   NOT NULL,
    Tipo                    INT             NOT NULL COMMENT '1=TextoLibre, 2=Codigo, 3=OpcionMultiple',
    Rubrica                 VARCHAR(1000)   NULL,
    PuntajeMaximo           INT             NOT NULL,
    OrdenEnEvaluacion       INT             NOT NULL,
    TiempoLimiteSegundos    INT             NOT NULL,
    EvaluacionId            CHAR(36)        NOT NULL,
    CreadoEn                DATETIME(6)     NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    ModificadoEn            DATETIME(6)     NULL,
    EstaActivo              TINYINT(1)      NOT NULL DEFAULT 1,
    CONSTRAINT FK_Preguntas_Evaluaciones
        FOREIGN KEY (EvaluacionId) REFERENCES Evaluaciones(Id)
        ON DELETE RESTRICT
) ENGINE=InnoDB;

-- ─── Tabla: Candidatos ──────────────────────────────────────
CREATE TABLE IF NOT EXISTS Candidatos (
    Id                      CHAR(36)        NOT NULL PRIMARY KEY,
    Nombre                  VARCHAR(200)    NOT NULL,
    Email                   VARCHAR(250)    NOT NULL,
    Token                   VARCHAR(64)     NOT NULL,
    FechaInicioRespuesta    DATETIME(6)     NULL,
    FechaFinRespuesta       DATETIME(6)     NULL,
    EvaluacionId            CHAR(36)        NOT NULL,
    CreadoEn                DATETIME(6)     NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    ModificadoEn            DATETIME(6)     NULL,
    EstaActivo              TINYINT(1)      NOT NULL DEFAULT 1,
    CONSTRAINT FK_Candidatos_Evaluaciones
        FOREIGN KEY (EvaluacionId) REFERENCES Evaluaciones(Id)
        ON DELETE RESTRICT,
    CONSTRAINT UQ_Candidatos_Token UNIQUE (Token)
) ENGINE=InnoDB;

-- ─── Tabla: Respuestas ──────────────────────────────────────
CREATE TABLE IF NOT EXISTS Respuestas (
    Id                      CHAR(36)        NOT NULL PRIMARY KEY,
    Contenido               TEXT            NOT NULL,
    Timestamp               DATETIME(6)     NOT NULL,
    TiempoUsadoSegundos     INT             NULL,
    CandidatoId             CHAR(36)        NOT NULL,
    PreguntaId              CHAR(36)        NOT NULL,
    ScoreIA                 DECIMAL(5,2)    NULL,
    FeedbackIA              VARCHAR(2000)   NULL,
    BrechasIdentificadas    VARCHAR(2000)   NULL,
    CreadoEn                DATETIME(6)     NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    ModificadoEn            DATETIME(6)     NULL,
    EstaActivo              TINYINT(1)      NOT NULL DEFAULT 1,
    CONSTRAINT FK_Respuestas_Candidatos
        FOREIGN KEY (CandidatoId) REFERENCES Candidatos(Id)
        ON DELETE RESTRICT,
    CONSTRAINT FK_Respuestas_Preguntas
        FOREIGN KEY (PreguntaId) REFERENCES Preguntas(Id)
        ON DELETE RESTRICT
) ENGINE=InnoDB;

-- ─── Tabla: ResultadosEvaluacion ────────────────────────────
CREATE TABLE IF NOT EXISTS ResultadosEvaluacion (
    Id                      CHAR(36)        NOT NULL PRIMARY KEY,
    ScoreTotal              DECIMAL(5,2)    NOT NULL,
    Recomendacion           VARCHAR(200)    NOT NULL,
    ResumenIA               TEXT            NOT NULL,
    BrechasDetectadas       VARCHAR(4000)   NULL,
    FortalezasDetectadas    VARCHAR(4000)   NULL,
    CandidatoId             CHAR(36)        NOT NULL,
    GeneradoEn              DATETIME(6)     NOT NULL,
    CreadoEn                DATETIME(6)     NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    ModificadoEn            DATETIME(6)     NULL,
    EstaActivo              TINYINT(1)      NOT NULL DEFAULT 1,
    CONSTRAINT FK_Resultados_Candidatos
        FOREIGN KEY (CandidatoId) REFERENCES Candidatos(Id)
        ON DELETE RESTRICT,
    CONSTRAINT UQ_Resultados_Candidato UNIQUE (CandidatoId)
) ENGINE=InnoDB;

-- ─── Tabla: SesionesEnVivo ──────────────────────────────────
CREATE TABLE IF NOT EXISTS SesionesEnVivo (
    Id                      CHAR(36)        NOT NULL PRIMARY KEY,
    EvaluacionId            CHAR(36)        NOT NULL,
    CandidatoId             CHAR(36)        NOT NULL,
    PreguntaActualIndex     INT             NOT NULL DEFAULT 0,
    InicioFaseActual        DATETIME(6)     NULL,
    SesionActiva            TINYINT(1)      NOT NULL DEFAULT 0,
    FueCompletada           TINYINT(1)      NOT NULL DEFAULT 0,
    CreadoEn                DATETIME(6)     NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    ModificadoEn            DATETIME(6)     NULL,
    EstaActivo              TINYINT(1)      NOT NULL DEFAULT 1,
    CONSTRAINT FK_Sesiones_Evaluaciones
        FOREIGN KEY (EvaluacionId) REFERENCES Evaluaciones(Id)
        ON DELETE RESTRICT,
    CONSTRAINT FK_Sesiones_Candidatos
        FOREIGN KEY (CandidatoId) REFERENCES Candidatos(Id)
        ON DELETE RESTRICT
) ENGINE=InnoDB;
