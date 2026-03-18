-- ============================================================
-- Script de creación de base de datos y tablas
-- Proyecto: CRUD Genérico Basado en Metadatos
-- Servidor: mysql-k3s-dev.grupofarsiman.io
-- Fecha: 2026-03-18
-- ============================================================

CREATE DATABASE IF NOT EXISTS CrudGenerico
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE CrudGenerico;

-- ─── Tabla: Sucursales ───────────────────────────────────────
CREATE TABLE IF NOT EXISTS Sucursales (
    Id          CHAR(36)        NOT NULL PRIMARY KEY,
    Nombre      VARCHAR(150)    NOT NULL,
    Ubicacion   VARCHAR(250)    NOT NULL,
    Activo      TINYINT(1)      NOT NULL DEFAULT 1
) ENGINE=InnoDB;

-- ─── Tabla: Categorias ──────────────────────────────────────
CREATE TABLE IF NOT EXISTS Categorias (
    Id          CHAR(36)        NOT NULL PRIMARY KEY,
    Descripcion VARCHAR(250)    NOT NULL,
    Codigo      VARCHAR(50)     NOT NULL
) ENGINE=InnoDB;

-- ─── Tabla: Supervisores ────────────────────────────────────
CREATE TABLE IF NOT EXISTS Supervisores (
    Id          CHAR(36)        NOT NULL PRIMARY KEY,
    Nombre      VARCHAR(150)    NOT NULL,
    Correo      VARCHAR(250)    NOT NULL,
    SucursalId  CHAR(36)        NOT NULL,
    CONSTRAINT FK_Supervisores_Sucursales
        FOREIGN KEY (SucursalId) REFERENCES Sucursales(Id)
        ON DELETE RESTRICT
) ENGINE=InnoDB;

-- ─── Tabla: Productos ───────────────────────────────────────
CREATE TABLE IF NOT EXISTS Productos (
    Id          CHAR(36)        NOT NULL PRIMARY KEY,
    Nombre      VARCHAR(200)    NOT NULL,
    CategoriaId CHAR(36)        NOT NULL,
    Precio      DECIMAL(18,2)   NOT NULL,
    Activo      TINYINT(1)      NOT NULL DEFAULT 1,
    CONSTRAINT FK_Productos_Categorias
        FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
        ON DELETE RESTRICT
) ENGINE=InnoDB;

-- ─── Tabla: Proveedores ─────────────────────────────────────
CREATE TABLE IF NOT EXISTS Proveedores (
    Id          CHAR(36)        NOT NULL PRIMARY KEY,
    Nombre      VARCHAR(150)    NOT NULL,
    Telefono    VARCHAR(20)     NULL,
    Correo      VARCHAR(250)    NOT NULL
) ENGINE=InnoDB;

-- ─── Tabla: Ubicaciones ─────────────────────────────────────
CREATE TABLE IF NOT EXISTS Ubicaciones (
    Id          CHAR(36)        NOT NULL PRIMARY KEY,
    Direccion   VARCHAR(250)    NOT NULL,
    Municipio   VARCHAR(150)    NOT NULL,
    Departamento VARCHAR(150)   NOT NULL,
    Pais        VARCHAR(150)    NOT NULL
) ENGINE=InnoDB;
