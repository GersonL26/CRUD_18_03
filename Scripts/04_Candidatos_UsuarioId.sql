-- ============================================================
-- Script de migración: Agregar UsuarioId a Candidatos
-- Permite vincular candidatos con usuarios registrados
-- Fecha: 2026-03-19
-- ============================================================

USE CrudGenerico;

ALTER TABLE Candidatos
    ADD COLUMN UsuarioId CHAR(36) NULL AFTER EvaluacionId;

ALTER TABLE Candidatos
    ADD CONSTRAINT FK_Candidatos_Usuarios
        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
        ON DELETE RESTRICT;
