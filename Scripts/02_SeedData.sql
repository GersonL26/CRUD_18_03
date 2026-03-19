USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'EvaluacionTecnicaDB')
BEGIN
    ALTER DATABASE EvaluacionTecnicaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EvaluacionTecnicaDB;
END
GO

CREATE DATABASE EvaluacionTecnicaDB;
GO

USE EvaluacionTecnicaDB;
GO

CREATE TABLE Usuarios (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    NombreCompleto NVARCHAR(200) NOT NULL,
    Email NVARCHAR(250) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Rol INT NOT NULL,
    CreadoEn DATETIME2 NOT NULL,
    ModificadoEn DATETIME2 NULL,
    EstaActivo BIT NOT NULL
);
GO
CREATE UNIQUE INDEX IX_Usuarios_Email ON Usuarios (Email);
GO

CREATE TABLE Evaluaciones (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Titulo NVARCHAR(200) NOT NULL,
    Descripcion NVARCHAR(1000) NULL,
    Tecnologia NVARCHAR(100) NOT NULL,
    Nivel INT NOT NULL,
    Estado INT NOT NULL,
    TiempoLimiteTotalMinutos INT NOT NULL,
    RequiereCamara BIT NOT NULL,
    RequiereMicrofono BIT NOT NULL,
    EvaluadorId UNIQUEIDENTIFIER NOT NULL,
    CreadoEn DATETIME2 NOT NULL,
    ModificadoEn DATETIME2 NULL,
    EstaActivo BIT NOT NULL,
    CONSTRAINT FK_Evaluaciones_Usuarios FOREIGN KEY (EvaluadorId) REFERENCES Usuarios(Id)
);
GO

CREATE TABLE Preguntas (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Texto NVARCHAR(2000) NOT NULL,
    Tipo INT NOT NULL,
    Rubrica NVARCHAR(1000) NULL,
    PuntajeMaximo INT NOT NULL,
    OrdenEnEvaluacion INT NOT NULL,
    TiempoLimiteSegundos INT NOT NULL,
    EvaluacionId UNIQUEIDENTIFIER NOT NULL,
    CreadoEn DATETIME2 NOT NULL,
    ModificadoEn DATETIME2 NULL,
    EstaActivo BIT NOT NULL,
    CONSTRAINT FK_Preguntas_Evaluaciones FOREIGN KEY (EvaluacionId) REFERENCES Evaluaciones(Id)
);
GO

CREATE TABLE Candidatos (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    Email NVARCHAR(250) NOT NULL,
    Token NVARCHAR(64) NOT NULL,
    FechaInicioRespuesta DATETIME2 NULL,
    FechaFinRespuesta DATETIME2 NULL,
    EvaluacionId UNIQUEIDENTIFIER NOT NULL,
    UsuarioId UNIQUEIDENTIFIER NULL,
    CreadoEn DATETIME2 NOT NULL,
    ModificadoEn DATETIME2 NULL,
    EstaActivo BIT NOT NULL,
    CONSTRAINT FK_Candidatos_Evaluaciones FOREIGN KEY (EvaluacionId) REFERENCES Evaluaciones(Id),
    CONSTRAINT FK_Candidatos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);
GO
CREATE UNIQUE INDEX IX_Candidatos_Token ON Candidatos (Token);
GO

CREATE TABLE Respuestas (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Contenido NVARCHAR(MAX) NOT NULL,
    [Timestamp] DATETIME2 NOT NULL,
    TiempoUsadoSegundos INT NOT NULL,
    CandidatoId UNIQUEIDENTIFIER NOT NULL,
    PreguntaId UNIQUEIDENTIFIER NOT NULL,
    ScoreIA DECIMAL(5,2) NULL,
    FeedbackIA NVARCHAR(2000) NULL,
    BrechasIdentificadas NVARCHAR(2000) NULL,
    CreadoEn DATETIME2 NOT NULL,
    ModificadoEn DATETIME2 NULL,
    EstaActivo BIT NOT NULL,
    CONSTRAINT FK_Respuestas_Candidatos FOREIGN KEY (CandidatoId) REFERENCES Candidatos(Id),
    CONSTRAINT FK_Respuestas_Preguntas FOREIGN KEY (PreguntaId) REFERENCES Preguntas(Id)
);
GO

CREATE TABLE ResultadosEvaluacion (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ScoreTotal DECIMAL(5,2) NOT NULL,
    Recomendacion NVARCHAR(200) NOT NULL,
    ResumenIA NVARCHAR(MAX) NOT NULL,
    BrechasDetectadas NVARCHAR(4000) NULL,
    FortalezasDetectadas NVARCHAR(4000) NULL,
    CandidatoId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    GeneradoEn DATETIME2 NOT NULL,
    CreadoEn DATETIME2 NOT NULL,
    ModificadoEn DATETIME2 NULL,
    EstaActivo BIT NOT NULL,
    CONSTRAINT FK_ResultadosEvaluacion_Candidatos FOREIGN KEY (CandidatoId) REFERENCES Candidatos(Id)
);
GO

CREATE TABLE SesionesEnVivo (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    EvaluacionId UNIQUEIDENTIFIER NOT NULL,
    CandidatoId UNIQUEIDENTIFIER NOT NULL,
    PreguntaActualIndex INT NOT NULL,
    InicioFaseActual DATETIME2 NULL,
    SesionActiva BIT NOT NULL,
    FueCompletada BIT NOT NULL,
    CreadoEn DATETIME2 NOT NULL,
    ModificadoEn DATETIME2 NULL,
    EstaActivo BIT NOT NULL,
    CONSTRAINT FK_SesionesEnVivo_Evaluaciones FOREIGN KEY (EvaluacionId) REFERENCES Evaluaciones(Id),
    CONSTRAINT FK_SesionesEnVivo_Candidatos FOREIGN KEY (CandidatoId) REFERENCES Candidatos(Id)
);
GO

INSERT INTO Usuarios (Id, NombreCompleto, Email, PasswordHash, Rol, CreadoEn, EstaActivo) VALUES ('A1000000-0000-0000-0000-000000000001', N'Carlos Administrador', N'admin@evaluacion.com', N'$2a$11$nDFziAQ0JLqFFcZoxtxD8OX2F6dopS/4vvNCAoGuX.HE4ckXXrDc.', 1, '2026-03-01 08:00:00', 1);

INSERT INTO Usuarios (Id, NombreCompleto, Email, PasswordHash, Rol, CreadoEn, EstaActivo) VALUES ('B1000000-0000-0000-0000-000000000001', N'Maria Garcia Lopez', N'maria.garcia@evaluacion.com', N'$2a$11$nDFziAQ0JLqFFcZoxtxD8OX2F6dopS/4vvNCAoGuX.HE4ckXXrDc.', 2, '2026-03-01 09:00:00', 1);

INSERT INTO Usuarios (Id, NombreCompleto, Email, PasswordHash, Rol, CreadoEn, EstaActivo) VALUES ('B1000000-0000-0000-0000-000000000002', N'Roberto Hernandez', N'roberto.hernandez@evaluacion.com', N'$2a$11$nDFziAQ0JLqFFcZoxtxD8OX2F6dopS/4vvNCAoGuX.HE4ckXXrDc.', 2, '2026-03-02 10:00:00', 1);

INSERT INTO Usuarios (Id, NombreCompleto, Email, PasswordHash, Rol, CreadoEn, EstaActivo) VALUES ('C1000000-0000-0000-0000-000000000001', N'Ana Martinez Rivera', N'ana.martinez@gmail.com', N'$2a$11$nDFziAQ0JLqFFcZoxtxD8OX2F6dopS/4vvNCAoGuX.HE4ckXXrDc.', 3, '2026-03-05 08:30:00', 1);

INSERT INTO Usuarios (Id, NombreCompleto, Email, PasswordHash, Rol, CreadoEn, EstaActivo) VALUES ('C1000000-0000-0000-0000-000000000002', N'Luis Fernando Perez', N'luis.perez@gmail.com', N'$2a$11$nDFziAQ0JLqFFcZoxtxD8OX2F6dopS/4vvNCAoGuX.HE4ckXXrDc.', 3, '2026-03-05 09:00:00', 1);

INSERT INTO Usuarios (Id, NombreCompleto, Email, PasswordHash, Rol, CreadoEn, EstaActivo) VALUES ('C1000000-0000-0000-0000-000000000003', N'Sofia Ramirez Castro', N'sofia.ramirez@gmail.com', N'$2a$11$nDFziAQ0JLqFFcZoxtxD8OX2F6dopS/4vvNCAoGuX.HE4ckXXrDc.', 3, '2026-03-06 10:00:00', 1);

INSERT INTO Usuarios (Id, NombreCompleto, Email, PasswordHash, Rol, CreadoEn, EstaActivo) VALUES ('C1000000-0000-0000-0000-000000000004', N'Diego Morales Ortiz', N'diego.morales@gmail.com', N'$2a$11$nDFziAQ0JLqFFcZoxtxD8OX2F6dopS/4vvNCAoGuX.HE4ckXXrDc.', 3, '2026-03-06 11:00:00', 1);

INSERT INTO Usuarios (Id, NombreCompleto, Email, PasswordHash, Rol, CreadoEn, EstaActivo) VALUES ('C1000000-0000-0000-0000-000000000005', N'Valentina Torres Nunez', N'valentina.torres@gmail.com', N'$2a$11$nDFziAQ0JLqFFcZoxtxD8OX2F6dopS/4vvNCAoGuX.HE4ckXXrDc.', 3, '2026-03-07 08:00:00', 1);

INSERT INTO Evaluaciones (Id, Titulo, Descripcion, Tecnologia, Nivel, Estado, TiempoLimiteTotalMinutos, RequiereCamara, RequiereMicrofono, EvaluadorId, CreadoEn, EstaActivo) VALUES ('E1000000-0000-0000-0000-000000000001', N'Evaluacion Angular Avanzado', N'Evaluacion tecnica para desarrolladores Angular con experiencia en arquitectura de componentes, signals, RxJS y optimizacion de rendimiento.', N'Angular', 3, 2, 90, 0, 0, 'B1000000-0000-0000-0000-000000000001', '2026-03-10 09:00:00', 1);

INSERT INTO Evaluaciones (Id, Titulo, Descripcion, Tecnologia, Nivel, Estado, TiempoLimiteTotalMinutos, RequiereCamara, RequiereMicrofono, EvaluadorId, CreadoEn, EstaActivo) VALUES ('E1000000-0000-0000-0000-000000000002', N'Evaluacion .NET Core Backend', N'Prueba tecnica enfocada en Entity Framework Core, API REST, patrones de diseno y manejo de errores en aplicaciones .NET.', N'.NET / C#', 2, 2, 60, 0, 0, 'B1000000-0000-0000-0000-000000000001', '2026-03-11 10:00:00', 1);

INSERT INTO Evaluaciones (Id, Titulo, Descripcion, Tecnologia, Nivel, Estado, TiempoLimiteTotalMinutos, RequiereCamara, RequiereMicrofono, EvaluadorId, CreadoEn, EstaActivo) VALUES ('E1000000-0000-0000-0000-000000000003', N'Evaluacion React Basico', N'Evaluacion para desarrolladores junior en React: componentes funcionales, hooks, estado y renderizado condicional.', N'React', 1, 1, 45, 0, 0, 'B1000000-0000-0000-0000-000000000001', '2026-03-12 08:00:00', 1);

INSERT INTO Evaluaciones (Id, Titulo, Descripcion, Tecnologia, Nivel, Estado, TiempoLimiteTotalMinutos, RequiereCamara, RequiereMicrofono, EvaluadorId, CreadoEn, EstaActivo) VALUES ('E1000000-0000-0000-0000-000000000004', N'Evaluacion SQL y Bases de Datos', N'Prueba avanzada de SQL: consultas complejas, optimizacion, indices, procedimientos almacenados y diseno de esquemas.', N'SQL Server', 3, 2, 75, 0, 0, 'B1000000-0000-0000-0000-000000000002', '2026-03-12 14:00:00', 1);

INSERT INTO Evaluaciones (Id, Titulo, Descripcion, Tecnologia, Nivel, Estado, TiempoLimiteTotalMinutos, RequiereCamara, RequiereMicrofono, EvaluadorId, CreadoEn, EstaActivo) VALUES ('E1000000-0000-0000-0000-000000000005', N'Evaluacion Python Data Engineering', N'Evaluacion de Python enfocada en procesamiento de datos, pandas, manejo de archivos y automatizacion.', N'Python', 2, 3, 60, 0, 0, 'B1000000-0000-0000-0000-000000000002', '2026-03-08 09:00:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000001', N'Cual es la diferencia entre Signals y Observables en Angular? En que casos usarias cada uno?', 1, N'Debe mencionar reactividad sincrona vs asincrona, simplicidad de signals, integracion con change detection, y casos donde RxJS sigue siendo necesario.', 20, 1, 600, 'E1000000-0000-0000-0000-000000000001', '2026-03-10 09:30:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000002', N'Implementa un componente Angular standalone que muestre una lista de productos con busqueda en tiempo real usando signals. Incluye el template y la logica del componente.', 2, N'Debe usar component standalone, signal() para estado, computed() para filtrado, efecto limpio. Bonus: debounce en la busqueda.', 30, 2, 900, 'E1000000-0000-0000-0000-000000000001', '2026-03-10 09:35:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000003', N'Explica la estrategia de lazy loading en Angular y como impacta el rendimiento de una aplicacion. Que metricas usarias para medir la mejora?', 1, N'Debe explicar loadChildren, rutas lazy, preloadingStrategy, impacto en bundle size, metricas como FCP, LCP, TTI.', 20, 3, 600, 'E1000000-0000-0000-0000-000000000001', '2026-03-10 09:40:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000004', N'Cual de las siguientes opciones describe correctamente el Change Detection con OnPush en Angular?', 3, N'Respuesta correcta: Solo se ejecuta cuando cambian las referencias de @Input o se dispara un evento en el componente.', 15, 4, 300, 'E1000000-0000-0000-0000-000000000001', '2026-03-10 09:45:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000005', N'Escribe un interceptor HTTP en Angular que maneje automaticamente tokens JWT, refresh tokens y errores 401, redirigiendo al login cuando el token expire.', 2, N'Debe implementar HttpInterceptorFn, clonar request con Authorization header, manejar error 401, intentar refresh, y redirigir si falla.', 25, 5, 900, 'E1000000-0000-0000-0000-000000000001', '2026-03-10 09:50:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000006', N'Que es el patron Repository y Unit of Work? Cuando es recomendable usarlos con Entity Framework Core y cuando no?', 1, N'Debe explicar ambos patrones, ventajas (testabilidad, abstraccion), desventajas (duplicacion sobre DbContext), y cuando EF ya actua como repository/UoW.', 20, 1, 600, 'E1000000-0000-0000-0000-000000000002', '2026-03-11 10:30:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000007', N'Implementa un endpoint API REST en .NET que reciba una lista de productos, los valide y los inserte en lote usando Entity Framework Core. Maneja errores de validacion y duplicados.', 2, N'Debe usar ActionResult, FluentValidation o DataAnnotations, AddRange, SaveChangesAsync, try/catch para constraint violations, respuesta 400 con detalles.', 30, 2, 900, 'E1000000-0000-0000-0000-000000000002', '2026-03-11 10:35:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000008', N'Explica la diferencia entre AddScoped, AddTransient y AddSingleton en la inyeccion de dependencias de .NET. Da un ejemplo practico de cuando usar cada uno.', 1, N'Debe explicar ciclo de vida de cada uno, Scoped=por request HTTP, Transient=nueva instancia, Singleton=toda la app.', 20, 3, 600, 'E1000000-0000-0000-0000-000000000002', '2026-03-11 10:40:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000009', N'Cual es la forma correcta de manejar excepciones globalmente en una API ASP.NET Core?', 3, N'Respuesta correcta: Middleware de excepciones personalizado o UseExceptionHandler con ProblemDetails.', 15, 4, 300, 'E1000000-0000-0000-0000-000000000002', '2026-03-11 10:45:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000010', N'Que son los hooks en React? Explica useState y useEffect con ejemplos practicos.', 1, N'Debe explicar que son funciones para anadir estado y efectos a componentes funcionales.', 25, 1, 600, 'E1000000-0000-0000-0000-000000000003', '2026-03-12 08:30:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000011', N'Crea un componente React que muestre un contador con botones de incrementar, decrementar y resetear. Usa hooks.', 2, N'Debe usar useState, handlers para cada accion, JSX limpio, componente funcional.', 25, 2, 600, 'E1000000-0000-0000-0000-000000000003', '2026-03-12 08:35:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000012', N'Para que sirve la prop key cuando renderizas listas en React?', 3, N'Ayuda a React a identificar que elementos cambiaron, se agregaron o eliminaron para optimizar el re-render del DOM virtual.', 15, 3, 300, 'E1000000-0000-0000-0000-000000000003', '2026-03-12 08:40:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000013', N'Explica la diferencia entre indices clustered y non-clustered. Como decides cuando crear un indice y sobre que columnas?', 1, N'Debe explicar que clustered ordena fisicamente los datos (1 por tabla), non-clustered es estructura separada.', 20, 1, 600, 'E1000000-0000-0000-0000-000000000004', '2026-03-12 14:30:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000014', N'Escribe una consulta SQL que devuelva el top 3 de productos mas vendidos por categoria, incluyendo el total vendido y el porcentaje respecto al total de su categoria.', 2, N'Debe usar window functions (ROW_NUMBER, SUM OVER), CTE o subquery, calculo de porcentaje con particion por categoria.', 30, 2, 900, 'E1000000-0000-0000-0000-000000000004', '2026-03-12 14:35:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000015', N'Que es un deadlock en bases de datos y como lo previenes? Describe un escenario real donde podria ocurrir.', 1, N'Debe explicar bloqueo mutuo circular, deteccion automatica del DBMS, prevencion con orden consistente de acceso a tablas.', 25, 3, 600, 'E1000000-0000-0000-0000-000000000004', '2026-03-12 14:40:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000016', N'Cual de las siguientes opciones sobre transacciones es INCORRECTA?', 3, N'Respuesta incorrecta a marcar: READ UNCOMMITTED previene todos los problemas de concurrencia.', 15, 4, 300, 'E1000000-0000-0000-0000-000000000004', '2026-03-12 14:45:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000017', N'Explica la diferencia entre una lista, una tupla y un diccionario en Python. Cuando usarias cada estructura?', 1, N'Lista: mutable, ordenada. Tupla: inmutable, ordenada, hasheable. Diccionario: pares clave-valor, busqueda O(1).', 20, 1, 600, 'E1000000-0000-0000-0000-000000000005', '2026-03-08 09:30:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000018', N'Escribe un script en Python usando pandas que lea un CSV de ventas, calcule el total por region, identifique la region con mas ventas y exporte el resultado a un nuevo CSV.', 2, N'Debe usar pd.read_csv, groupby, agg/sum, idxmax, to_csv. Codigo limpio y funcional.', 30, 2, 900, 'E1000000-0000-0000-0000-000000000005', '2026-03-08 09:35:00', 1);

INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES ('P1000000-0000-0000-0000-000000000019', N'Que es un decorador en Python? Implementa un decorador que mida el tiempo de ejecucion de una funcion.', 2, N'Debe implementar funcion wrapper con *args/**kwargs, usar time.perf_counter(), retornar resultado de funcion original.', 25, 3, 900, 'E1000000-0000-0000-0000-000000000005', '2026-03-08 09:40:00', 1);

INSERT INTO Candidatos (Id, Nombre, Email, Token, FechaInicioRespuesta, FechaFinRespuesta, EvaluacionId, UsuarioId, CreadoEn, EstaActivo) VALUES ('D1000000-0000-0000-0000-000000000001', N'Ana Martinez Rivera', N'ana.martinez@gmail.com', N'a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6', '2026-03-15 10:00:00', '2026-03-15 11:15:00', 'E1000000-0000-0000-0000-000000000001', 'C1000000-0000-0000-0000-000000000001', '2026-03-14 09:00:00', 1);

INSERT INTO Candidatos (Id, Nombre, Email, Token, FechaInicioRespuesta, FechaFinRespuesta, EvaluacionId, UsuarioId, CreadoEn, EstaActivo) VALUES ('D1000000-0000-0000-0000-000000000002', N'Luis Fernando Perez', N'luis.perez@gmail.com', N'b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7', NULL, NULL, 'E1000000-0000-0000-0000-000000000001', 'C1000000-0000-0000-0000-000000000002', '2026-03-14 09:30:00', 1);

INSERT INTO Candidatos (Id, Nombre, Email, Token, FechaInicioRespuesta, FechaFinRespuesta, EvaluacionId, UsuarioId, CreadoEn, EstaActivo) VALUES ('D1000000-0000-0000-0000-000000000003', N'Sofia Ramirez Castro', N'sofia.ramirez@gmail.com', N'c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8', NULL, NULL, 'E1000000-0000-0000-0000-000000000002', 'C1000000-0000-0000-0000-000000000003', '2026-03-14 10:00:00', 1);

INSERT INTO Candidatos (Id, Nombre, Email, Token, FechaInicioRespuesta, FechaFinRespuesta, EvaluacionId, UsuarioId, CreadoEn, EstaActivo) VALUES ('D1000000-0000-0000-0000-000000000004', N'Diego Morales Ortiz', N'diego.morales@gmail.com', N'd4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9', '2026-03-13 14:00:00', '2026-03-13 15:10:00', 'E1000000-0000-0000-0000-000000000004', 'C1000000-0000-0000-0000-000000000004', '2026-03-13 10:00:00', 1);

INSERT INTO Candidatos (Id, Nombre, Email, Token, FechaInicioRespuesta, FechaFinRespuesta, EvaluacionId, UsuarioId, CreadoEn, EstaActivo) VALUES ('D1000000-0000-0000-0000-000000000005', N'Ana Martinez Rivera', N'ana.martinez@gmail.com', N'e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0', '2026-03-09 09:00:00', '2026-03-09 09:50:00', 'E1000000-0000-0000-0000-000000000005', 'C1000000-0000-0000-0000-000000000001', '2026-03-08 15:00:00', 1);

INSERT INTO Candidatos (Id, Nombre, Email, Token, FechaInicioRespuesta, FechaFinRespuesta, EvaluacionId, UsuarioId, CreadoEn, EstaActivo) VALUES ('D1000000-0000-0000-0000-000000000006', N'Valentina Torres Nunez', N'valentina.torres@gmail.com', N'f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1', NULL, NULL, 'E1000000-0000-0000-0000-000000000002', 'C1000000-0000-0000-0000-000000000005', '2026-03-15 08:00:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000001', N'Los Signals en Angular son primitivas reactivas sincronas que representan un valor que puede cambiar. A diferencia de los Observables de RxJS que son asincronos y basados en push/stream, los Signals son sincronos y se integran directamente con el change detection de Angular. Usaria Signals para estado local del componente y computed values. Usaria Observables para streams de datos como WebSockets, HTTP requests, o cuando necesito operadores complejos como debounceTime, switchMap, combineLatest.', '2026-03-15 10:05:00', 420, 'D1000000-0000-0000-0000-000000000001', 'P1000000-0000-0000-0000-000000000001', 85.5, N'Excelente comprension de ambos conceptos. Buena diferenciacion entre casos de uso.', N'No menciono effect() ni la integracion con formularios reactivos.', '2026-03-15 10:05:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000002', N'Componente standalone con signal para busqueda, computed para filtrado, template con @for y track. Implementacion funcional pero sin debounce.', '2026-03-15 10:20:00', 780, 'D1000000-0000-0000-0000-000000000001', 'P1000000-0000-0000-0000-000000000002', 78.0, N'Buen uso de signals y computed. Componente funcional pero falto debounce.', N'No implemento debounce. La integracion ngModel/signal no es correcta sin model().', '2026-03-15 10:20:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000003', N'Lazy loading en Angular permite cargar modulos o componentes bajo demanda en vez de incluirlos en el bundle principal. Se configura usando loadComponent o loadChildren en las rutas. Esto reduce significativamente el bundle inicial, mejorando el First Contentful Paint y Time to Interactive. Para medir el impacto, usaria Lighthouse para FCP, LCP y TTI, webpack-bundle-analyzer para ver el tamano de chunks, y el Network tab de Chrome DevTools para ver los tiempos de carga.', '2026-03-15 10:32:00', 540, 'D1000000-0000-0000-0000-000000000001', 'P1000000-0000-0000-0000-000000000003', 90.0, N'Muy buena explicacion con metricas concretas. Demuestra conocimiento practico de optimizacion.', N'Podria mencionar preloadingStrategy y server-side rendering como complemento.', '2026-03-15 10:32:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000004', N'Solo se ejecuta cuando cambian las referencias de @Input o se dispara un evento en el componente.', '2026-03-15 10:36:00', 120, 'D1000000-0000-0000-0000-000000000001', 'P1000000-0000-0000-0000-000000000004', 100.0, N'Respuesta correcta.', NULL, '2026-03-15 10:36:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000005', N'Interceptor funcional con HttpInterceptorFn que clona el request para agregar el Authorization header con el token JWT. Maneja errores 401 haciendo logout y redirigiendo al login. No implemento refresh token.', '2026-03-15 10:50:00', 660, 'D1000000-0000-0000-0000-000000000001', 'P1000000-0000-0000-0000-000000000005', 72.0, N'Interceptor funcional con manejo de 401. No implemento refresh token.', N'No implemento logica de refresh token. El interceptor es basico sin retry despues de refrescar.', '2026-03-15 10:50:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000006', N'Un indice clustered define el orden fisico de los datos en la tabla. Solo puede haber uno por tabla y generalmente se crea sobre la primary key. Un indice non-clustered es una estructura separada con punteros a los datos, puede haber multiples por tabla. Para decidir si crear un indice evaluo: frecuencia de la columna en WHERE/JOIN, selectividad (alta cardinalidad es mejor), si se usa en ORDER BY, y el costo de mantenimiento en operaciones INSERT/UPDATE.', '2026-03-13 14:10:00', 480, 'D1000000-0000-0000-0000-000000000004', 'P1000000-0000-0000-0000-000000000013', 92.0, N'Explicacion tecnicamente precisa y completa. Excelente mencion de criterios de decision.', NULL, '2026-03-13 14:10:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000007', N'WITH VentasPorProductoCategoria AS (SELECT p.Categoria, p.Nombre, SUM(v.Cantidad * v.PrecioUnitario) AS TotalVendido FROM Ventas v INNER JOIN Productos p ON v.ProductoId = p.Id GROUP BY p.Categoria, p.Nombre), Ranking AS (SELECT *, ROW_NUMBER() OVER (PARTITION BY Categoria ORDER BY TotalVendido DESC) AS Rn FROM VentasPorProductoCategoria) SELECT Categoria, Nombre, TotalVendido FROM Ranking WHERE Rn <= 3 ORDER BY Categoria, Rn', '2026-03-13 14:25:00', 720, 'D1000000-0000-0000-0000-000000000004', 'P1000000-0000-0000-0000-000000000014', 95.0, N'Consulta excelente. Uso correcto de CTEs, window functions. Codigo bien formateado y eficiente.', NULL, '2026-03-13 14:25:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000008', N'Un deadlock ocurre cuando dos o mas transacciones se bloquean mutuamente esperando recursos que la otra tiene. Por ejemplo: Transaccion A bloquea tabla Pedidos y espera tabla Inventario, mientras Transaccion B bloquea Inventario y espera Pedidos. El DBMS detecta el deadlock y mata una de las transacciones (victim). Para prevenir: mantener transacciones cortas, acceder tablas siempre en el mismo orden, usar nivel de aislamiento READ COMMITTED, evitar interaccion del usuario durante transacciones.', '2026-03-13 14:35:00', 510, 'D1000000-0000-0000-0000-000000000004', 'P1000000-0000-0000-0000-000000000015', 88.0, N'Muy buena explicacion con ejemplo practico. Buenas recomendaciones de prevencion.', N'No menciono herramientas de diagnostico como SQL Profiler o Extended Events.', '2026-03-13 14:35:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000009', N'READ UNCOMMITTED previene todos los problemas de concurrencia.', '2026-03-13 14:38:00', 90, 'D1000000-0000-0000-0000-000000000004', 'P1000000-0000-0000-0000-000000000016', 100.0, N'Correctamente identifico la afirmacion incorrecta.', NULL, '2026-03-13 14:38:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000010', N'Lista: es mutable y ordenada, se usa para colecciones que cambian como un carrito de compras. Tupla: inmutable y ordenada, se usa para datos fijos como coordenadas (lat, lon) o retornos multiples de funciones. Diccionario: pares clave-valor con busqueda O(1), ideal para mapeos como configuraciones o caches. Desde Python 3.7 los diccionarios mantienen el orden de insercion.', '2026-03-09 09:08:00', 360, 'D1000000-0000-0000-0000-000000000005', 'P1000000-0000-0000-0000-000000000017', 88.0, N'Buena explicacion con ejemplos practicos. Correcto sobre el orden en diccionarios desde 3.7.', N'Podria mencionar sets y frozensets como complemento.', '2026-03-09 09:08:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000011', N'import pandas as pd
df = pd.read_csv("ventas.csv")
ventas_por_region = df.groupby("region")["monto"].sum().reset_index()
ventas_por_region.columns = ["region", "total_ventas"]
region_top = ventas_por_region.loc[ventas_por_region["total_ventas"].idxmax()]
ventas_por_region.to_csv("resultado_ventas.csv", index=False)', '2026-03-09 09:25:00', 720, 'D1000000-0000-0000-0000-000000000005', 'P1000000-0000-0000-0000-000000000018', 82.0, N'Script funcional y limpio. Buen uso de groupby y export.', N'Sin manejo de excepciones ni validacion del CSV.', '2026-03-09 09:25:00', 1);

INSERT INTO Respuestas (Id, Contenido, [Timestamp], TiempoUsadoSegundos, CandidatoId, PreguntaId, ScoreIA, FeedbackIA, BrechasIdentificadas, CreadoEn, EstaActivo) VALUES ('R1000000-0000-0000-0000-000000000012', N'Un decorador es una funcion que recibe otra funcion y extiende su comportamiento sin modificarla. Se aplica con @nombre_decorador.
import time
def medir_tiempo(func):
    def wrapper(*args, **kwargs):
        inicio = time.perf_counter()
        resultado = func(*args, **kwargs)
        fin = time.perf_counter()
        return resultado
    return wrapper', '2026-03-09 09:40:00', 600, 'D1000000-0000-0000-0000-000000000005', 'P1000000-0000-0000-0000-000000000019', 90.0, N'Implementacion correcta del decorador con medicion de tiempo.', N'Falta @functools.wraps para preservar metadata de la funcion original.', '2026-03-09 09:40:00', 1);

INSERT INTO ResultadosEvaluacion (Id, ScoreTotal, Recomendacion, ResumenIA, BrechasDetectadas, FortalezasDetectadas, CandidatoId, GeneradoEn, CreadoEn, EstaActivo) VALUES ('RE100000-0000-0000-0000-000000000001', 82.3, N'Recomendada para posicion Senior con mentoria', N'Ana demuestra solidos conocimientos en Angular moderno, con buen dominio de Signals, lazy loading y arquitectura de componentes. Su comprension teorica es excelente pero la implementacion practica tiene areas de mejora.', N'Implementacion de refresh tokens en interceptores HTTP; Uso avanzado de la API model() para integracion de signals con formularios; Estrategias de preloading y SSR', N'Excelente comprension teorica de Signals vs Observables; Conocimiento solido de lazy loading y metricas de rendimiento; Change Detection con OnPush; Capacidad de escribir codigo limpio y estructurado', 'D1000000-0000-0000-0000-000000000001', '2026-03-15 12:00:00', '2026-03-15 12:00:00', 1);

INSERT INTO ResultadosEvaluacion (Id, ScoreTotal, Recomendacion, ResumenIA, BrechasDetectadas, FortalezasDetectadas, CandidatoId, GeneradoEn, CreadoEn, EstaActivo) VALUES ('RE100000-0000-0000-0000-000000000002', 93.5, N'Altamente recomendado para posicion Senior DBA', N'Diego muestra un dominio excepcional de SQL y bases de datos. Sus consultas son eficientes y bien estructuradas, con uso avanzado de CTEs y window functions. Es un candidato fuerte para roles senior de base de datos.', N'Herramientas de diagnostico avanzadas (SQL Profiler, Extended Events); Query hints y LOCK_TIMEOUT', N'Dominio de indices clustered/non-clustered; Consultas SQL complejas con CTEs y window functions; Comprension profunda de deadlocks y estrategias de prevencion', 'D1000000-0000-0000-0000-000000000004', '2026-03-13 16:00:00', '2026-03-13 16:00:00', 1);

INSERT INTO ResultadosEvaluacion (Id, ScoreTotal, Recomendacion, ResumenIA, BrechasDetectadas, FortalezasDetectadas, CandidatoId, GeneradoEn, CreadoEn, EstaActivo) VALUES ('RE100000-0000-0000-0000-000000000003', 86.0, N'Recomendada para posicion Mid Data Engineer', N'Ana muestra buenos conocimientos de Python y procesamiento de datos con pandas. Su codigo es limpio y funcional. Entiende bien las estructuras de datos y patrones de Python como decoradores.', N'Manejo de excepciones en scripts de datos; Uso de functools.wraps en decoradores; Validacion de entrada de datos', N'Comprension solida de estructuras de datos Python; Buen manejo de pandas para analisis de datos; Implementacion correcta de decoradores; Codigo limpio y legible', 'D1000000-0000-0000-0000-000000000005', '2026-03-09 10:30:00', '2026-03-09 10:30:00', 1);
GO
