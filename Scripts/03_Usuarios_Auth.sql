-- ============================================================
-- Script: Tabla de Usuarios para autenticación JWT
-- Proyecto: Sistema de Evaluación Técnica con IA
-- Fecha: 2026-03-19
-- ============================================================

USE CrudGenerico;

-- ─── Tabla: Usuarios ────────────────────────────────────────
CREATE TABLE IF NOT EXISTS Usuarios (
    Id              CHAR(36)        NOT NULL PRIMARY KEY,
    NombreCompleto  VARCHAR(200)    NOT NULL,
    Email           VARCHAR(250)    NOT NULL,
    PasswordHash    VARCHAR(500)    NOT NULL,
    Rol             INT             NOT NULL DEFAULT 2 COMMENT '1=Admin, 2=Evaluador, 3=Candidato',
    CreadoEn        DATETIME(6)     NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    ModificadoEn    DATETIME(6)     NULL,
    EstaActivo      TINYINT(1)      NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Usuarios_Email UNIQUE (Email)
) ENGINE=InnoDB;

-- ─── Seed: Usuario Admin por defecto ────────────────────────
-- Password: Admin123! (hash BCrypt)
INSERT INTO Usuarios (Id, NombreCompleto, Email, PasswordHash, Rol, CreadoEn, EstaActivo)
VALUES (
    UUID(),
    'Administrador',
    'admin@evaluacion.com',
    '$2a$11$KhW5eMXDnKgMYq3xNjKzXeQZ0VfWxJ5EJz5y5MDJY5C8LHqKMWbC',
    1,
    CURRENT_TIMESTAMP(6),
    1
)
ON DUPLICATE KEY UPDATE Id = Id;
