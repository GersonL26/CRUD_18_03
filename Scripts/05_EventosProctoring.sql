-- =====================================================
-- 05_EventosProctoring.sql
-- Tabla para registro de eventos de proctoring
-- =====================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EventosProctoring')
BEGIN
    CREATE TABLE EventosProctoring (
        Id                  UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT NEWID(),
        CandidatoId         UNIQUEIDENTIFIER    NOT NULL,
        EvaluacionId        UNIQUEIDENTIFIER    NOT NULL,
        Tipo                NVARCHAR(50)        NOT NULL,  -- CopyPaste, TabSwitch, TranscripcionAudio
        Detalle             NVARCHAR(4000)      NULL,
        Timestamp           DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CreadoEn            DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        ModificadoEn        DATETIME2           NULL,
        EstaActivo          BIT                 NOT NULL DEFAULT 1,

        CONSTRAINT FK_EventosProctoring_Candidatos
            FOREIGN KEY (CandidatoId) REFERENCES Candidatos(Id),
        CONSTRAINT FK_EventosProctoring_Evaluaciones
            FOREIGN KEY (EvaluacionId) REFERENCES Evaluaciones(Id)
    );

    CREATE INDEX IX_EventosProctoring_Candidato_Evaluacion
        ON EventosProctoring (CandidatoId, EvaluacionId);

    PRINT 'Tabla EventosProctoring creada correctamente.';
END
ELSE
BEGIN
    PRINT 'La tabla EventosProctoring ya existe.';
END
GO
