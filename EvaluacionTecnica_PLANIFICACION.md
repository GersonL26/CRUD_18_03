# 📋 PLANIFICACIÓN COMPLETA — Sistema de Evaluación Técnica Asistida por IA

> **Bitácora maestra del proyecto. Contiene todo lo necesario para desarrollar el sistema de principio a fin, incluyendo arquitectura, fases, código base, pruebas y buenas prácticas.**  
> **Stack:** ASP.NET Core 8 (C#) + Angular 17 + Entity Framework Core + OpenAI API  
> **Arquitectura:** Clean Architecture (Hexagonal simplificada)

---

## 📌 ÍNDICE

1. [Resumen del sistema](#1-resumen-del-sistema)
2. [Stack tecnológico](#2-stack-tecnológico)
3. [Principios SOLID y Clean Code explicados simplemente](#3-principios-solid-y-clean-code)
4. [Estructura de carpetas del proyecto](#4-estructura-de-carpetas)
5. [Modelo de datos y entidades](#5-modelo-de-datos-y-entidades)
6. [CRUD genérico reutilizable (EF Core)](#6-crud-genérico)
7. [Endpoints de la API](#7-endpoints-de-la-api)
8. [FASE 0 — Configuración inicial del proyecto](#fase-0--configuración-inicial)
9. [FASE 1 — Autenticación y roles de usuario](#fase-1--autenticación-y-roles)
10. [FASE 2 — CRUD de evaluaciones](#fase-2--crud-de-evaluaciones)
11. [FASE 3 — Respuestas del candidato con timestamp](#fase-3--respuestas-del-candidato)
12. [FASE 4 — Endpoint de análisis con IA](#fase-4--análisis-con-ia)
13. [FASE 5 — Pantalla de resultados y recomendación](#fase-5--pantalla-de-resultados)
14. [FASE 6 — Exportar reporte en PDF](#fase-6--exportar-pdf)
15. [FASE 7 — Sesión en vivo con temporizador](#fase-7--sesión-en-vivo)
16. [FASE 8 — Comparación de candidatos y ranking](#fase-8--comparación-y-ranking)
17. [Estrategia de pruebas (Testing)](#estrategia-de-pruebas)
18. [Frontend Angular — estructura y componentes](#frontend-angular)
19. [Bitácora de progreso](#bitácora-de-progreso)
20. [Comandos útiles de referencia](#comandos-útiles)

---

## 1. Resumen del sistema

### ¿Qué hace este sistema?

Este sistema permite a una empresa **evaluar candidatos técnicos** con ayuda de Inteligencia Artificial. Funciona así:

1. Un **evaluador** (usuario con rol de administrador o reclutador) crea una prueba técnica con preguntas.
2. El sistema genera un **enlace único** para el candidato.
3. El **candidato** entra al enlace, responde preguntas dentro de un tiempo límite.
4. Al terminar, la **IA analiza las respuestas**, genera un score (puntaje) e identifica brechas de conocimiento.
5. El evaluador ve un **reporte final** con score, recomendación (contratar / segunda entrevista / rechazar) y puede exportarlo en PDF.
6. Si hay múltiples candidatos, puede **compararlos** y ver un ranking.

### Roles del sistema

| Rol | Qué puede hacer |
|-----|----------------|
| **Admin** | Todo: crear evaluadores, ver todos los resultados, configurar el sistema |
| **Evaluador** | Crear/editar pruebas, invitar candidatos, ver resultados de sus pruebas |
| **Candidato** | Entrar con token único, responder prueba, ver su propio resultado |

---

## 2. Stack tecnológico

| Componente | Tecnología | Versión | Para qué sirve |
|------------|-----------|---------|----------------|
| Backend | ASP.NET Core | 8.0 | API REST que maneja la lógica del negocio |
| ORM | Entity Framework Core | 8.0 | Conecta C# con la base de datos sin escribir SQL |
| Base de datos | SQL Server / PostgreSQL | - | Guarda todos los datos del sistema |
| Autenticación | ASP.NET Identity + JWT | - | Manejo de usuarios, login, tokens de sesión |
| IA | OpenAI SDK para .NET | 2.x | Analiza respuestas y genera scores |
| PDF | iTextSharp / QuestPDF | - | Genera reportes en PDF |
| Tiempo real | SignalR | - | Temporizador en vivo para sesiones |
| Frontend | Angular | 17+ | Interfaz visual del sistema |
| HTTP Client | Angular HttpClient | - | Llama a los endpoints del backend |
| CSS | Angular Material / TailwindCSS | - | Componentes visuales y estilos |
| Testing Backend | xUnit + Moq | - | Pruebas automáticas del backend |
| Testing Frontend | Jest + Angular Testing Library | - | Pruebas automáticas del frontend |

### Herramientas necesarias instaladas

```
✅ .NET SDK 8.0        → https://dotnet.microsoft.com/download
✅ Node.js 20+         → https://nodejs.org
✅ Angular CLI         → npm install -g @angular/cli
✅ SQL Server / Docker → para la base de datos
✅ Visual Studio 2022 o VS Code con extensión C#
```

---

## 3. Principios SOLID y Clean Code

> **Filosofía central:** El código es para personas, no para máquinas. Una persona que no sabe programar debería poder leer el código y entender *qué hace* aunque no entienda *cómo lo hace*.

---

### SOLID — Explicado con ejemplos del proyecto

#### S — Single Responsibility Principle (Principio de Responsabilidad Única)

**En español simple:** Cada clase hace UNA sola cosa. Si puedes describir lo que hace una clase con "Y", entonces tiene demasiadas responsabilidades.

**Mal ejemplo:**
```csharp
// ❌ Esta clase hace DEMASIADAS cosas
public class EvaluacionService
{
    public void CrearEvaluacion() { }       // crea evaluaciones
    public void EnviarEmail() { }           // envía emails  ← ¡no es su trabajo!
    public void GenerarPDF() { }            // genera PDFs   ← ¡tampoco!
    public void AnalizarConIA() { }         // llama a OpenAI ← ¡menos!
}
```

**Buen ejemplo:**
```csharp
// ✅ Cada clase tiene UNA responsabilidad
public class EvaluacionService     { public void Crear() { } }
public class NotificadorEmail      { public void Enviar() { } }
public class GeneradorPDF          { public void Generar() { } }
public class AnalizadorIA          { public void Analizar() { } }
```

---

#### O — Open/Closed Principle (Abierto para extensión, cerrado para modificación)

**En español simple:** Puedes *agregar* funcionalidad sin *modificar* código existente.

**Ejemplo en el proyecto:** Si mañana quieres agregar Gemini AI además de OpenAI, no debes tocar el código existente, solo agregar una nueva clase.

```csharp
// ✅ Define el contrato abstracto
public interface IAnalizadorIA
{
    Task<ResultadoAnalisis> AnalizarAsync(List<Respuesta> respuestas);
}

// Implementación con OpenAI
public class AnalizadorOpenAI : IAnalizadorIA { ... }

// Mañana agregas Gemini SIN tocar lo anterior
public class AnalizadorGemini : IAnalizadorIA { ... }
```

---

#### L — Liskov Substitution Principle (Sustitución de Liskov)

**En español simple:** Si usas una interfaz, cualquier implementación concreta debe funcionar igual.

El código que usa `IAnalizadorIA` debe funcionar igual sin importar si usas `AnalizadorOpenAI` o `AnalizadorGemini`.

---

#### I — Interface Segregation (Segregación de Interfaces)

**En español simple:** No fuerces a una clase a implementar métodos que no necesita. Usa interfaces pequeñas y específicas.

```csharp
// ❌ Interfaz gigante que no todas las clases necesitan completamente
public interface IRepositorio
{
    Task Guardar();
    Task Eliminar();
    Task Exportar();   // ← no todos los repositorios exportan
    Task EnviarEmail(); // ← esto no es un repositorio!
}

// ✅ Interfaces pequeñas y específicas
public interface IRepositorioLectura<T> { Task<T> ObtenerPorIdAsync(Guid id); }
public interface IRepositorioEscritura<T> { Task GuardarAsync(T entidad); }
public interface IExportable { Task<byte[]> ExportarAsync(); }
```

---

#### D — Dependency Inversion (Inversión de Dependencias)

**En español simple:** Las clases no crean sus dependencias, se las dan por fuera (inyección de dependencias).

```csharp
// ❌ La clase crea su propia dependencia (no se puede testear fácilmente)
public class CrearEvaluacionHandler
{
    private readonly EvaluacionRepository _repo = new EvaluacionRepository(); // ❌
}

// ✅ La dependencia se inyecta desde afuera
public class CrearEvaluacionHandler
{
    private readonly IEvaluacionRepository _repo;

    public CrearEvaluacionHandler(IEvaluacionRepository repo) // ✅ inyección
    {
        _repo = repo;
    }
}
```

---

### Clean Code — Reglas del proyecto

#### Nombres que hablan por sí solos

```csharp
// ❌ Nombres crípticos
var d = DateTime.UtcNow;
var x = repo.Get(id);
bool f = x != null;

// ✅ Nombres que explican qué son
var fechaDeRespuesta = DateTime.UtcNow;
var evaluacion = _evaluacionRepository.ObtenerPorIdAsync(evaluacionId);
bool evaluacionExiste = evaluacion != null;
```

---

#### Funciones pequeñas (máximo 20 líneas)

```csharp
// ❌ Función gigante que hace todo
public async Task ProcesarEvaluacion(Guid candidatoId)
{
    // 50 líneas de código mezclado...
}

// ✅ Funciones pequeñas con nombres descriptivos
public async Task ProcesarEvaluacion(Guid candidatoId)
{
    var respuestas = await ObtenerRespuestasDelCandidatoAsync(candidatoId);
    var analisis = await AnalizarRespuestasConIAAsync(respuestas);
    await GuardarResultadoAsync(candidatoId, analisis);
    await NotificarEvaluadorAsync(candidatoId);
}
```

---

#### Evitar complejidad ciclomática (máximo 3-4 niveles de if anidados)

La complejidad ciclomática mide cuántos caminos posibles tiene una función. Más de 10 es peligroso.

```csharp
// ❌ Alta complejidad ciclomática (muchos if anidados)
public string DeterminarRecomendacion(double score)
{
    if (score > 0)
    {
        if (score > 40)
        {
            if (score > 70)
            {
                if (score > 90)
                    return "Contratar inmediatamente";
                else
                    return "Contratar";
            }
            else
                return "Segunda entrevista";
        }
        else
            return "Rechazar";
    }
    return "Sin datos";
}

// ✅ Baja complejidad, legible como una tabla
public string DeterminarRecomendacion(double score) => score switch
{
    > 90 => "Contratar inmediatamente",
    > 70 => "Contratar",
    > 40 => "Segunda entrevista",
    > 0  => "Rechazar",
    _    => "Sin datos"
};
```

---

#### No repetir código (DRY — Don't Repeat Yourself)

Si copias y pegas código, es señal de que necesitas extraerlo a una función o clase compartida.

```csharp
// ❌ Validación repetida en múltiples lugares
public void CrearEvaluacion(string titulo) {
    if (string.IsNullOrWhiteSpace(titulo)) throw new ArgumentException("Título requerido");
}
public void ActualizarEvaluacion(string titulo) {
    if (string.IsNullOrWhiteSpace(titulo)) throw new ArgumentException("Título requerido"); // ← repetido
}

// ✅ Centralizado
private static void ValidarTitulo(string titulo)
{
    if (string.IsNullOrWhiteSpace(titulo))
        throw new ArgumentException("El título es obligatorio y no puede estar vacío");
}
```

---

## 4. Estructura de carpetas

```
EvaluacionTecnica/                          ← Carpeta raíz del proyecto
│
├── EvaluacionTecnica.sln                  ← Archivo de solución Visual Studio
│
├── src/
│   ├── EvaluacionTecnica.Domain/          ← CAPA 1: Corazón del negocio
│   │   ├── Entities/                      ← Las "cosas" del sistema
│   │   │   ├── BaseEntity.cs             ← Clase base con Id y timestamps
│   │   │   ├── Evaluacion.cs             ← Una prueba técnica
│   │   │   ├── Pregunta.cs               ← Una pregunta dentro de la prueba
│   │   │   ├── Candidato.cs              ← Un candidato a evaluar
│   │   │   ├── Respuesta.cs              ← Respuesta de un candidato
│   │   │   ├── ResultadoEvaluacion.cs    ← Score + recomendación IA
│   │   │   ├── SesionEnVivo.cs           ← Sesión con temporizador
│   │   │   └── Usuario.cs                ← Usuario del sistema (Admin/Evaluador)
│   │   ├── Enums/
│   │   │   ├── NivelTecnico.cs           ← Junior/Mid/Senior
│   │   │   ├── TipoPregunta.cs           ← Código/TextoLibre/OpcionMultiple
│   │   │   ├── EstadoEvaluacion.cs       ← Borrador/Activa/Cerrada
│   │   │   └── RolUsuario.cs             ← Admin/Evaluador/Candidato
│   │   ├── Ports/                        ← Contratos (interfaces)
│   │   │   ├── IRepositorioGenerico.cs   ← CRUD genérico reutilizable
│   │   │   ├── IEvaluacionRepository.cs
│   │   │   ├── ICandidatoRepository.cs
│   │   │   ├── IRespuestaRepository.cs
│   │   │   ├── IResultadoRepository.cs
│   │   │   ├── IAnalizadorIA.cs
│   │   │   └── IGeneradorPDF.cs
│   │   └── ValueObjects/
│   │       ├── Email.cs
│   │       └── Score.cs
│   │
│   ├── EvaluacionTecnica.Application/     ← CAPA 2: Casos de uso
│   │   ├── UseCases/
│   │   │   ├── Evaluaciones/
│   │   │   │   ├── Crear/
│   │   │   │   │   ├── CrearEvaluacionCommand.cs
│   │   │   │   │   └── CrearEvaluacionHandler.cs
│   │   │   │   ├── Actualizar/
│   │   │   │   ├── Eliminar/
│   │   │   │   ├── Listar/
│   │   │   │   └── ObtenerPorId/
│   │   │   ├── Candidatos/
│   │   │   │   ├── InvitarCandidato/
│   │   │   │   └── ObtenerPorToken/
│   │   │   ├── Respuestas/
│   │   │   │   └── GuardarRespuestas/
│   │   │   ├── Analisis/
│   │   │   │   └── EjecutarAnalisisIA/
│   │   │   ├── Resultados/
│   │   │   │   ├── ObtenerResultado/
│   │   │   │   └── CompararCandidatos/
│   │   │   ├── Reportes/
│   │   │   │   └── ExportarPDF/
│   │   │   └── Sesiones/
│   │   │       └── IniciarSesionEnVivo/
│   │   ├── DTOs/                          ← Objetos de datos para la API
│   │   │   ├── EvaluacionDto.cs
│   │   │   ├── PreguntaDto.cs
│   │   │   ├── CandidatoDto.cs
│   │   │   ├── RespuestaDto.cs
│   │   │   └── ResultadoDto.cs
│   │   └── Validators/                    ← Validaciones con FluentValidation
│   │       ├── CrearEvaluacionValidator.cs
│   │       └── GuardarRespuestasValidator.cs
│   │
│   ├── EvaluacionTecnica.Infrastructure/  ← CAPA 3: Implementaciones
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs            ← Contexto de EF Core
│   │   │   ├── Configurations/            ← Configuración de tablas EF
│   │   │   │   ├── EvaluacionConfiguration.cs
│   │   │   │   └── CandidatoConfiguration.cs
│   │   │   └── Repositories/
│   │   │       ├── RepositorioGenerico.cs ← CRUD genérico reutilizable
│   │   │       ├── EvaluacionRepository.cs
│   │   │       ├── CandidatoRepository.cs
│   │   │       └── RespuestaRepository.cs
│   │   ├── AI/
│   │   │   └── AnalizadorOpenAI.cs
│   │   ├── PDF/
│   │   │   └── GeneradorPDFQuestPDF.cs
│   │   ├── Hubs/
│   │   │   └── SesionHub.cs              ← SignalR para temporizador
│   │   └── DependencyInjection.cs        ← Registro de servicios
│   │
│   └── EvaluacionTecnica.API/             ← CAPA 4: Punto de entrada
│       ├── Controllers/
│       │   ├── EvaluacionesController.cs
│       │   ├── CandidatosController.cs
│       │   ├── RespuestasController.cs
│       │   ├── AnalisisController.cs
│       │   ├── ResultadosController.cs
│       │   ├── ReportesController.cs
│       │   └── AuthController.cs
│       ├── Middleware/
│       │   ├── ExceptionMiddleware.cs    ← Manejo global de errores
│       │   └── RequestLoggingMiddleware.cs
│       └── Program.cs                   ← Configuración de la app
│
├── tests/
│   ├── EvaluacionTecnica.Domain.Tests/
│   │   └── ValueObjects/
│   │       ├── EmailTests.cs
│   │       └── ScoreTests.cs
│   ├── EvaluacionTecnica.Application.Tests/
│   │   └── UseCases/
│   │       ├── CrearEvaluacionHandlerTests.cs
│   │       ├── GuardarRespuestasHandlerTests.cs
│   │       └── EjecutarAnalisisIAHandlerTests.cs
│   └── EvaluacionTecnica.API.Tests/
│       └── Controllers/
│           └── EvaluacionesControllerTests.cs
│
└── frontend/
    └── evaluacion-tecnica/               ← Proyecto Angular
        └── src/
            └── app/
                ├── evaluador/            ← Módulo del evaluador
                ├── candidato/            ← Módulo del candidato
                ├── admin/                ← Módulo de administración
                ├── shared/               ← Componentes compartidos
                └── core/                 ← Servicios globales, guards, interceptors
```

---

## 5. Modelo de datos y entidades

### BaseEntity — Base de todas las entidades

```csharp
// Domain/Entities/BaseEntity.cs
// RESPONSABILIDAD ÚNICA: Solo define campos comunes a todas las entidades
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreadoEn { get; protected set; } = DateTime.UtcNow;
    public DateTime? ModificadoEn { get; protected set; }
    public bool EstaActivo { get; protected set; } = true;

    public void MarcarComoModificado()
    {
        ModificadoEn = DateTime.UtcNow;
    }

    public void Desactivar()
    {
        EstaActivo = false;
        ModificadoEn = DateTime.UtcNow;
    }
}
```

---

### Evaluacion (Prueba Técnica)

```csharp
// Domain/Entities/Evaluacion.cs
public class Evaluacion : BaseEntity
{
    public string Titulo { get; private set; }
    public string? Descripcion { get; private set; }
    public string Tecnologia { get; private set; }        // "C#", "Angular", "Python"
    public NivelTecnico Nivel { get; private set; }       // Junior / Mid / Senior
    public EstadoEvaluacion Estado { get; private set; }  // Borrador / Activa / Cerrada
    public int TiempoLimiteTotalMinutos { get; private set; }
    public Guid EvaluadorId { get; private set; }

    // Relaciones con otras entidades
    public List<Pregunta> Preguntas { get; private set; } = new();
    public List<Candidato> Candidatos { get; private set; } = new();

    // Constructor con validaciones (RESPONSABILIDAD ÚNICA: validar al crear)
    public Evaluacion(string titulo, string tecnologia, NivelTecnico nivel,
                      int tiempoLimiteMinutos, Guid evaluadorId)
    {
        ValidarTitulo(titulo);
        ValidarTecnologia(tecnologia);
        ValidarTiempoLimite(tiempoLimiteMinutos);

        Titulo = titulo;
        Tecnologia = tecnologia;
        Nivel = nivel;
        TiempoLimiteTotalMinutos = tiempoLimiteMinutos;
        EvaluadorId = evaluadorId;
        Estado = EstadoEvaluacion.Borrador;
    }

    // Comportamiento del negocio
    public void AgregarPregunta(Pregunta pregunta)
    {
        const int maximoDePreguntas = 30;

        if (Preguntas.Count >= maximoDePreguntas)
            throw new InvalidOperationException($"No se pueden agregar más de {maximoDePreguntas} preguntas");

        if (Estado != EstadoEvaluacion.Borrador)
            throw new InvalidOperationException("Solo se pueden agregar preguntas en estado Borrador");

        Preguntas.Add(pregunta);
        MarcarComoModificado();
    }

    public void Activar()
    {
        if (!Preguntas.Any())
            throw new InvalidOperationException("La evaluación debe tener al menos una pregunta para activarse");

        Estado = EstadoEvaluacion.Activa;
        MarcarComoModificado();
    }

    public void Cerrar()
    {
        Estado = EstadoEvaluacion.Cerrada;
        MarcarComoModificado();
    }

    // Validaciones privadas (RESPONSABILIDAD ÚNICA: solo validan su campo)
    private static void ValidarTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título de la evaluación es obligatorio");

        if (titulo.Length > 200)
            throw new ArgumentException("El título no puede tener más de 200 caracteres");
    }

    private static void ValidarTecnologia(string tecnologia)
    {
        if (string.IsNullOrWhiteSpace(tecnologia))
            throw new ArgumentException("La tecnología es obligatoria");
    }

    private static void ValidarTiempoLimite(int minutos)
    {
        if (minutos < 10 || minutos > 300)
            throw new ArgumentException("El tiempo límite debe estar entre 10 y 300 minutos");
    }
}
```

---

### Pregunta

```csharp
// Domain/Entities/Pregunta.cs
public class Pregunta : BaseEntity
{
    public string Texto { get; private set; }
    public TipoPregunta Tipo { get; private set; }
    public string? Rubrica { get; private set; }    // Criterios de evaluación para la IA
    public int PuntajeMaximo { get; private set; }
    public int OrdenEnEvaluacion { get; private set; }
    public int TiempoLimiteSegundos { get; private set; } // Para sesión en vivo
    public Guid EvaluacionId { get; private set; }
    public List<string> OpcionesRespuesta { get; private set; } = new(); // Para opción múltiple

    public Pregunta(string texto, TipoPregunta tipo, int puntajeMaximo,
                    int orden, int tiempoLimiteSegundos, Guid evaluacionId,
                    string? rubrica = null)
    {
        ValidarTexto(texto);
        ValidarPuntaje(puntajeMaximo);
        ValidarTiempo(tiempoLimiteSegundos);

        Texto = texto;
        Tipo = tipo;
        PuntajeMaximo = puntajeMaximo;
        OrdenEnEvaluacion = orden;
        TiempoLimiteSegundos = tiempoLimiteSegundos;
        EvaluacionId = evaluacionId;
        Rubrica = rubrica;
    }

    private static void ValidarTexto(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("El texto de la pregunta es obligatorio");
    }

    private static void ValidarPuntaje(int puntaje)
    {
        if (puntaje < 1 || puntaje > 100)
            throw new ArgumentException("El puntaje máximo debe estar entre 1 y 100");
    }

    private static void ValidarTiempo(int segundos)
    {
        if (segundos < 30 || segundos > 3600)
            throw new ArgumentException("El tiempo límite debe estar entre 30 segundos y 1 hora");
    }
}
```

---

### Candidato

```csharp
// Domain/Entities/Candidato.cs
public class Candidato : BaseEntity
{
    public string Nombre { get; private set; }
    public string Email { get; private set; }
    public string Token { get; private set; }          // UUID único para acceso sin login
    public DateTime? FechaInicioRespuesta { get; private set; }
    public DateTime? FechaFinRespuesta { get; private set; }
    public Guid EvaluacionId { get; private set; }
    public List<Respuesta> Respuestas { get; private set; } = new();
    public ResultadoEvaluacion? Resultado { get; private set; }

    public Candidato(string nombre, string email, Guid evaluacionId)
    {
        ValidarNombre(nombre);
        ValidarEmail(email);

        Nombre = nombre;
        Email = email;
        EvaluacionId = evaluacionId;
        Token = Guid.NewGuid().ToString("N"); // Token único para acceso sin password
    }

    public void IniciarRespuesta()
    {
        if (FechaInicioRespuesta.HasValue)
            throw new InvalidOperationException("La sesión ya fue iniciada anteriormente");

        FechaInicioRespuesta = DateTime.UtcNow;
    }

    public void FinalizarRespuesta()
    {
        if (!FechaInicioRespuesta.HasValue)
            throw new InvalidOperationException("No se puede finalizar una sesión que no fue iniciada");

        FechaFinRespuesta = DateTime.UtcNow;
    }

    public TimeSpan? ObtenerTiempoInvertido()
    {
        if (!FechaInicioRespuesta.HasValue || !FechaFinRespuesta.HasValue)
            return null;

        return FechaFinRespuesta.Value - FechaInicioRespuesta.Value;
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del candidato es obligatorio");
    }

    private static void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("El email del candidato no es válido");
    }
}
```

---

### Respuesta

```csharp
// Domain/Entities/Respuesta.cs
public class Respuesta : BaseEntity
{
    public string Contenido { get; private set; }      // Lo que escribió el candidato
    public DateTime Timestamp { get; private set; }    // Cuándo respondió exactamente
    public int? TiempoUsadoSegundos { get; private set; } // Cuánto tardó en responder
    public Guid CandidatoId { get; private set; }
    public Guid PreguntaId { get; private set; }

    // Datos del análisis IA por pregunta
    public double? ScoreIA { get; private set; }
    public string? FeedbackIA { get; private set; }
    public string? BrecharIdentificadas { get; private set; }

    public Respuesta(string contenido, Guid candidatoId, Guid preguntaId,
                     int? tiempoUsadoSegundos = null)
    {
        if (string.IsNullOrWhiteSpace(contenido))
            throw new ArgumentException("La respuesta no puede estar vacía");

        Contenido = contenido;
        Timestamp = DateTime.UtcNow;   // SE GUARDA AUTOMÁTICAMENTE
        CandidatoId = candidatoId;
        PreguntaId = preguntaId;
        TiempoUsadoSegundos = tiempoUsadoSegundos;
    }

    public void AgregarAnalisisIA(double score, string feedback, string brechas)
    {
        if (score < 0 || score > 100)
            throw new ArgumentException("El score debe estar entre 0 y 100");

        ScoreIA = score;
        FeedbackIA = feedback;
        BrecharIdentificadas = brechas;
        MarcarComoModificado();
    }
}
```

---

### ResultadoEvaluacion

```csharp
// Domain/Entities/ResultadoEvaluacion.cs
public class ResultadoEvaluacion : BaseEntity
{
    public double ScoreTotal { get; private set; }          // 0-100
    public string Recomendacion { get; private set; }       // "Contratar" / "Segunda entrevista" / "Rechazar"
    public string ResumenIA { get; private set; }           // Párrafo generado por IA
    public string BrecharDetectadas { get; private set; }   // JSON con brechas
    public string FortalezasDetectadas { get; private set; }
    public Guid CandidatoId { get; private set; }
    public DateTime GeneradoEn { get; private set; }

    public ResultadoEvaluacion(double score, string resumenIA, string brechas,
                                string fortalezas, Guid candidatoId)
    {
        ScoreTotal = score;
        ResumenIA = resumenIA;
        BrecharDetectadas = brechas;
        FortalezasDetectadas = fortalezas;
        CandidatoId = candidatoId;
        GeneradoEn = DateTime.UtcNow;
        Recomendacion = DeterminarRecomendacion(score);
    }

    // EXPRESIÓN SWITCH: legible, sin if anidados (evita complejidad ciclomática)
    private static string DeterminarRecomendacion(double score) => score switch
    {
        >= 85 => "Contratar inmediatamente",
        >= 70 => "Contratar",
        >= 50 => "Segunda entrevista técnica",
        >= 30 => "Segunda entrevista general",
        _     => "No recomendado para el puesto"
    };
}
```

---

### Enums

```csharp
// Domain/Enums/NivelTecnico.cs
public enum NivelTecnico
{
    Junior = 1,
    Mid = 2,
    Senior = 3,
    Lead = 4
}

// Domain/Enums/TipoPregunta.cs
public enum TipoPregunta
{
    TextoLibre = 1,       // Respuesta abierta escrita
    Codigo = 2,           // Bloque de código
    OpcionMultiple = 4    // Opciones predefinidas
}

// Domain/Enums/EstadoEvaluacion.cs
public enum EstadoEvaluacion
{
    Borrador = 1,    // El evaluador la está armando
    Activa = 2,      // Candidatos pueden responderla
    Cerrada = 3      // Ya no acepta respuestas
}
```

---

## 6. CRUD Genérico

> Aprovecha el CRUD genérico existente. Aquí está el patrón completo para integrarlo.

### Interfaz genérica (Domain/Ports)

```csharp
// Domain/Ports/IRepositorioGenerico.cs
// T es cualquier entidad que herede de BaseEntity
public interface IRepositorioGenerico<T> where T : BaseEntity
{
    Task<T?> ObtenerPorIdAsync(Guid id);
    Task<List<T>> ListarTodosAsync();
    Task<List<T>> ListarActivosAsync();
    Task AgregarAsync(T entidad);
    Task ActualizarAsync(T entidad);
    Task EliminarAsync(Guid id);      // Soft delete: pone EstaActivo = false
    Task<bool> ExisteAsync(Guid id);
    Task<int> ContarAsync();
}
```

### Implementación genérica (Infrastructure/Persistence)

```csharp
// Infrastructure/Persistence/Repositories/RepositorioGenerico.cs
public class RepositorioGenerico<T> : IRepositorioGenerico<T> where T : BaseEntity
{
    protected readonly AppDbContext _contexto;
    protected readonly DbSet<T> _dbSet;

    public RepositorioGenerico(AppDbContext contexto)
    {
        _contexto = contexto;
        _dbSet = contexto.Set<T>();
    }

    public async Task<T?> ObtenerPorIdAsync(Guid id)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id && e.EstaActivo);

    public async Task<List<T>> ListarTodosAsync()
        => await _dbSet.ToListAsync();

    public async Task<List<T>> ListarActivosAsync()
        => await _dbSet.Where(e => e.EstaActivo).ToListAsync();

    public async Task AgregarAsync(T entidad)
    {
        await _dbSet.AddAsync(entidad);
        await _contexto.SaveChangesAsync();
    }

    public async Task ActualizarAsync(T entidad)
    {
        entidad.MarcarComoModificado();
        _dbSet.Update(entidad);
        await _contexto.SaveChangesAsync();
    }

    public async Task EliminarAsync(Guid id)
    {
        var entidad = await ObtenerPorIdAsync(id);
        if (entidad is null) return;

        entidad.Desactivar(); // Soft delete: no borra, solo desactiva
        await _contexto.SaveChangesAsync();
    }

    public async Task<bool> ExisteAsync(Guid id)
        => await _dbSet.AnyAsync(e => e.Id == id && e.EstaActivo);

    public async Task<int> ContarAsync()
        => await _dbSet.CountAsync(e => e.EstaActivo);
}
```

### Repositorio específico — hereda del genérico y agrega consultas propias

```csharp
// Infrastructure/Persistence/Repositories/EvaluacionRepository.cs
public class EvaluacionRepository : RepositorioGenerico<Evaluacion>, IEvaluacionRepository
{
    public EvaluacionRepository(AppDbContext contexto) : base(contexto) { }

    // Consulta específica que el CRUD genérico no tiene
    public async Task<List<Evaluacion>> ObtenerPorEvaluadorAsync(Guid evaluadorId)
        => await _dbSet
            .Where(e => e.EvaluadorId == evaluadorId && e.EstaActivo)
            .Include(e => e.Preguntas)
            .OrderByDescending(e => e.CreadoEn)
            .ToListAsync();

    public async Task<Evaluacion?> ObtenerConPreguntasAsync(Guid id)
        => await _dbSet
            .Include(e => e.Preguntas.Where(p => p.EstaActivo))
            .FirstOrDefaultAsync(e => e.Id == id && e.EstaActivo);
}
```

---

## 7. Endpoints de la API

### Mapa completo de la API

```
BASE URL: https://localhost:7001/api/v1

AUTH
POST   /auth/login                    → Login evaluador/admin
POST   /auth/refresh                  → Renovar token JWT
POST   /auth/logout                   → Cerrar sesión

EVALUACIONES
GET    /evaluaciones                  → Listar evaluaciones del evaluador
GET    /evaluaciones/{id}             → Obtener evaluación con preguntas
POST   /evaluaciones                  → Crear nueva evaluación
PUT    /evaluaciones/{id}             → Actualizar evaluación
DELETE /evaluaciones/{id}             → Eliminar (soft delete)
POST   /evaluaciones/{id}/activar     → Cambiar estado a Activa
POST   /evaluaciones/{id}/cerrar      → Cambiar estado a Cerrada

PREGUNTAS
POST   /evaluaciones/{id}/preguntas   → Agregar pregunta a evaluación
PUT    /preguntas/{id}                → Actualizar pregunta
DELETE /preguntas/{id}                → Eliminar pregunta

CANDIDATOS
GET    /evaluaciones/{id}/candidatos  → Listar candidatos de una evaluación
POST   /evaluaciones/{id}/candidatos  → Invitar candidato (genera token)
GET    /candidatos/token/{token}      → Obtener datos con token (sin auth)

RESPUESTAS
POST   /respuestas                    → Guardar respuestas (candidato con token)
GET    /candidatos/{id}/respuestas    → Ver respuestas de un candidato

ANÁLISIS IA
POST   /analisis/{candidatoId}        → Ejecutar análisis IA y generar resultado
GET    /analisis/{candidatoId}        → Obtener resultado ya generado

RESULTADOS
GET    /resultados/{candidatoId}      → Resultado de un candidato
GET    /evaluaciones/{id}/resultados  → Todos los resultados de una evaluación
GET    /evaluaciones/{id}/ranking     → Ranking de candidatos ordenado por score

REPORTES
GET    /reportes/{candidatoId}/pdf    → Descargar PDF del resultado

SESIONES EN VIVO
POST   /sesiones                      → Iniciar sesión en vivo
GET    /sesiones/{id}                 → Estado de la sesión
POST   /sesiones/{id}/siguiente       → Pasar a siguiente pregunta
POST   /sesiones/{id}/finalizar       → Terminar sesión
```

---

## FASE 0 — Configuración inicial

**Objetivo:** Tener el proyecto corriendo con la base de datos conectada.

**Duración estimada:** 3-4 horas

### Paso 1: Crear la solución

```bash
# Crear carpeta del proyecto
mkdir EvaluacionTecnica
cd EvaluacionTecnica

# Crear la solución
dotnet new sln -n EvaluacionTecnica

# Crear los proyectos
dotnet new classlib -n EvaluacionTecnica.Domain -o src/EvaluacionTecnica.Domain
dotnet new classlib -n EvaluacionTecnica.Application -o src/EvaluacionTecnica.Application
dotnet new classlib -n EvaluacionTecnica.Infrastructure -o src/EvaluacionTecnica.Infrastructure
dotnet new webapi -n EvaluacionTecnica.API -o src/EvaluacionTecnica.API

# Crear proyectos de tests
dotnet new xunit -n EvaluacionTecnica.Domain.Tests -o tests/EvaluacionTecnica.Domain.Tests
dotnet new xunit -n EvaluacionTecnica.Application.Tests -o tests/EvaluacionTecnica.Application.Tests

# Agregar proyectos a la solución
dotnet sln add src/EvaluacionTecnica.Domain/EvaluacionTecnica.Domain.csproj
dotnet sln add src/EvaluacionTecnica.Application/EvaluacionTecnica.Application.csproj
dotnet sln add src/EvaluacionTecnica.Infrastructure/EvaluacionTecnica.Infrastructure.csproj
dotnet sln add src/EvaluacionTecnica.API/EvaluacionTecnica.API.csproj
dotnet sln add tests/EvaluacionTecnica.Domain.Tests/EvaluacionTecnica.Domain.Tests.csproj
dotnet sln add tests/EvaluacionTecnica.Application.Tests/EvaluacionTecnica.Application.Tests.csproj
```

### Paso 2: Configurar referencias entre proyectos

```bash
# Application necesita Domain
dotnet add src/EvaluacionTecnica.Application/ reference src/EvaluacionTecnica.Domain/

# Infrastructure necesita Domain y Application
dotnet add src/EvaluacionTecnica.Infrastructure/ reference src/EvaluacionTecnica.Domain/
dotnet add src/EvaluacionTecnica.Infrastructure/ reference src/EvaluacionTecnica.Application/

# API necesita todos
dotnet add src/EvaluacionTecnica.API/ reference src/EvaluacionTecnica.Application/
dotnet add src/EvaluacionTecnica.API/ reference src/EvaluacionTecnica.Infrastructure/

# Tests referencian los proyectos que prueban
dotnet add tests/EvaluacionTecnica.Domain.Tests/ reference src/EvaluacionTecnica.Domain/
dotnet add tests/EvaluacionTecnica.Application.Tests/ reference src/EvaluacionTecnica.Application/
```

### Paso 3: Instalar paquetes NuGet

```bash
# Infrastructure — Entity Framework Core
dotnet add src/EvaluacionTecnica.Infrastructure/ package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/EvaluacionTecnica.Infrastructure/ package Microsoft.EntityFrameworkCore.Tools

# Infrastructure — OpenAI
dotnet add src/EvaluacionTecnica.Infrastructure/ package OpenAI --version 2.1.0

# Infrastructure — PDF
dotnet add src/EvaluacionTecnica.Infrastructure/ package QuestPDF

# API — JWT Auth
dotnet add src/EvaluacionTecnica.API/ package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/EvaluacionTecnica.API/ package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# API — SignalR (ya incluido en ASP.NET Core)
# API — Swagger
dotnet add src/EvaluacionTecnica.API/ package Swashbuckle.AspNetCore

# Application — Validación
dotnet add src/EvaluacionTecnica.Application/ package FluentValidation.AspNetCore

# Tests
dotnet add tests/EvaluacionTecnica.Application.Tests/ package Moq
dotnet add tests/EvaluacionTecnica.Application.Tests/ package FluentAssertions
```

### Paso 4: AppDbContext

```csharp
// Infrastructure/Persistence/AppDbContext.cs
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>();
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<Candidato> Candidatos => Set<Candidato>();
    public DbSet<Respuesta> Respuestas => Set<Respuesta>();
    public DbSet<ResultadoEvaluacion> Resultados => Set<ResultadoEvaluacion>();
    public DbSet<SesionEnVivo> Sesiones => Set<SesionEnVivo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas las configuraciones de la carpeta Configurations/
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

### Paso 5: Primera migración

```bash
dotnet ef migrations add InitialCreate --project src/EvaluacionTecnica.Infrastructure --startup-project src/EvaluacionTecnica.API
dotnet ef database update --project src/EvaluacionTecnica.Infrastructure --startup-project src/EvaluacionTecnica.API
```

### Paso 6: Program.cs base

```csharp
// API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar todos los servicios de infraestructura
builder.Services.AddInfrastructure(builder.Configuration);

// CORS para Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// SignalR para sesiones en vivo
builder.Services.AddSignalR();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<SesionHub>("/hubs/sesion");

app.Run();
```

### Checklist Fase 0

```
[ ] Solución creada con 4 proyectos + 2 de tests
[ ] Referencias entre proyectos configuradas
[ ] Paquetes NuGet instalados
[ ] Entidades del Domain creadas
[ ] AppDbContext configurado
[ ] Conexión a BD en appsettings.json
[ ] Primera migración ejecutada
[ ] API corre en https://localhost:7001
[ ] Swagger accesible en /swagger
```

---

## FASE 1 — Autenticación y roles

**Objetivo:** Login con JWT, roles Admin/Evaluador/Candidato (por token único).

**Duración estimada:** 4-6 horas

### Entidad de Usuario con ASP.NET Identity

```csharp
// Domain/Entities/Usuario.cs
// IdentityUser ya provee: Id, Email, PasswordHash, etc.
public class Usuario : IdentityUser<Guid>
{
    public string NombreCompleto { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
}
```

### Caso de uso: Login

```csharp
// Application/UseCases/Auth/Login/LoginCommand.cs
public record LoginCommand(string Email, string Password);

// Application/UseCases/Auth/Login/LoginHandler.cs
public class LoginHandler
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IJwtGenerador _jwtGenerador;

    public LoginHandler(UserManager<Usuario> userManager, IJwtGenerador jwtGenerador)
    {
        _userManager = userManager;
        _jwtGenerador = jwtGenerador;
    }

    public async Task<string> HandleAsync(LoginCommand command)
    {
        var usuario = await _userManager.FindByEmailAsync(command.Email)
            ?? throw new UnauthorizedAccessException("Credenciales inválidas");

        var passwordEsValida = await _userManager.CheckPasswordAsync(usuario, command.Password);
        if (!passwordEsValida)
            throw new UnauthorizedAccessException("Credenciales inválidas");

        return _jwtGenerador.Generar(usuario);
    }
}
```

### Proteger endpoints con roles

```csharp
// API/Controllers/EvaluacionesController.cs
[ApiController]
[Route("api/v1/[controller]")]
[Authorize] // Requiere JWT válido
public class EvaluacionesController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Evaluador")]
    public async Task<IActionResult> Listar() { ... }

    [HttpPost]
    [Authorize(Roles = "Evaluador")]
    public async Task<IActionResult> Crear([FromBody] CrearEvaluacionCommand command) { ... }
}

// Endpoint público para candidatos (solo necesitan token)
[HttpGet("token/{token}")]
[AllowAnonymous]
public async Task<IActionResult> ObtenerPorToken(string token) { ... }
```

### Checklist Fase 1

```
[ ] ASP.NET Identity configurado con AppDbContext
[ ] Endpoint POST /auth/login funcionando y devolviendo JWT
[ ] Endpoint POST /auth/refresh implementado
[ ] Roles Admin/Evaluador configurados
[ ] Sistema de token único para candidatos (GUID)
[ ] Middleware de autorización aplicado en controladores
[ ] Test: LoginHandler con credenciales correctas devuelve token
[ ] Test: LoginHandler con credenciales incorrectas lanza excepción
```

---

## FASE 2 — CRUD de evaluaciones

**Objetivo:** Crear, leer, actualizar, eliminar evaluaciones con sus preguntas.

**Duración estimada:** 5-6 horas

### Caso de uso: Crear Evaluación

```csharp
// Application/UseCases/Evaluaciones/Crear/CrearEvaluacionCommand.cs
public record CrearEvaluacionCommand(
    string Titulo,
    string? Descripcion,
    string Tecnologia,
    NivelTecnico Nivel,
    int TiempoLimiteTotalMinutos,
    List<AgregarPreguntaDto> Preguntas
);

public record AgregarPreguntaDto(
    string Texto,
    TipoPregunta Tipo,
    int PuntajeMaximo,
    int TiempoLimiteSegundos,
    string? Rubrica,
    List<string>? Opciones
);

// Application/UseCases/Evaluaciones/Crear/CrearEvaluacionHandler.cs
public class CrearEvaluacionHandler
{
    private readonly IEvaluacionRepository _evaluacionRepository;

    public CrearEvaluacionHandler(IEvaluacionRepository evaluacionRepository)
    {
        _evaluacionRepository = evaluacionRepository;
    }

    public async Task<Guid> HandleAsync(CrearEvaluacionCommand command, Guid evaluadorId)
    {
        // 1. Crear la entidad (las validaciones están en el constructor)
        var evaluacion = new Evaluacion(
            command.Titulo,
            command.Tecnologia,
            command.Nivel,
            command.TiempoLimiteTotalMinutos,
            evaluadorId
        );

        // 2. Agregar preguntas
        for (int i = 0; i < command.Preguntas.Count; i++)
        {
            var preguntaDto = command.Preguntas[i];
            var pregunta = new Pregunta(
                preguntaDto.Texto,
                preguntaDto.Tipo,
                preguntaDto.PuntajeMaximo,
                orden: i + 1,
                preguntaDto.TiempoLimiteSegundos,
                evaluacion.Id,
                preguntaDto.Rubrica
            );
            evaluacion.AgregarPregunta(pregunta);
        }

        // 3. Guardar en la base de datos
        await _evaluacionRepository.AgregarAsync(evaluacion);

        return evaluacion.Id;
    }
}
```

### Controller

```csharp
// API/Controllers/EvaluacionesController.cs
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,Evaluador")]
public class EvaluacionesController : ControllerBase
{
    private readonly CrearEvaluacionHandler _crearHandler;
    private readonly IEvaluacionRepository _repository;

    public EvaluacionesController(
        CrearEvaluacionHandler crearHandler,
        IEvaluacionRepository repository)
    {
        _crearHandler = crearHandler;
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEvaluacionCommand command)
    {
        var evaluadorId = ObtenerEvaluadorIdDelToken();
        var id = await _crearHandler.HandleAsync(command, evaluadorId);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var evaluadorId = ObtenerEvaluadorIdDelToken();
        var evaluaciones = await _repository.ObtenerPorEvaluadorAsync(evaluadorId);
        return Ok(evaluaciones);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var evaluacion = await _repository.ObtenerConPreguntasAsync(id);
        if (evaluacion is null) return NotFound();
        return Ok(evaluacion);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _repository.EliminarAsync(id); // Soft delete
        return NoContent();
    }

    // Obtiene el ID del evaluador desde el JWT
    private Guid ObtenerEvaluadorIdDelToken()
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException());
}
```

### Checklist Fase 2

```
[ ] CRUD completo de Evaluacion funcionando
[ ] CRUD de Preguntas dentro de evaluaciones
[ ] Solo el evaluador dueño puede modificar su evaluación
[ ] Soft delete implementado (no borra de la BD)
[ ] Validaciones de negocio probadas (máximo preguntas, tiempo límite)
[ ] Test: Crear evaluación con datos válidos
[ ] Test: Crear evaluación sin título lanza excepción
[ ] Test: Agregar más de 30 preguntas lanza excepción
[ ] Test: No se pueden agregar preguntas a evaluación Activa
```

---

## FASE 3 — Respuestas del candidato

**Objetivo:** El candidato entra con token, responde, se guarda con timestamp exacto.

**Duración estimada:** 4 horas

### Caso de uso: Guardar Respuestas

```csharp
// Application/UseCases/Respuestas/GuardarRespuestas/GuardarRespuestasCommand.cs
public record GuardarRespuestasCommand(
    string TokenCandidato,
    List<RespuestaItemDto> Respuestas
);

public record RespuestaItemDto(
    Guid PreguntaId,
    string Contenido,
    int TiempoUsadoSegundos
);

// Application/UseCases/Respuestas/GuardarRespuestas/GuardarRespuestasHandler.cs
public class GuardarRespuestasHandler
{
    private readonly ICandidatoRepository _candidatoRepository;
    private readonly IRespuestaRepository _respuestaRepository;

    public GuardarRespuestasHandler(
        ICandidatoRepository candidatoRepository,
        IRespuestaRepository respuestaRepository)
    {
        _candidatoRepository = candidatoRepository;
        _respuestaRepository = respuestaRepository;
    }

    public async Task HandleAsync(GuardarRespuestasCommand command)
    {
        // 1. Buscar candidato por token
        var candidato = await _candidatoRepository.ObtenerPorTokenAsync(command.TokenCandidato)
            ?? throw new NotFoundException("Token de candidato inválido");

        // 2. Verificar que no haya respondido antes
        var yaRespondio = await _respuestaRepository.ExistenRespuestasParaCandidatoAsync(candidato.Id);
        if (yaRespondio)
            throw new InvalidOperationException("El candidato ya envió sus respuestas");

        // 3. Crear y guardar cada respuesta
        var respuestas = command.Respuestas.Select(r => new Respuesta(
            r.Contenido,
            candidato.Id,
            r.PreguntaId,
            r.TiempoUsadoSegundos
        )).ToList();

        // 4. Finalizar la sesión del candidato
        candidato.FinalizarRespuesta();
        await _candidatoRepository.ActualizarAsync(candidato);
        await _respuestaRepository.GuardarVariasAsync(respuestas);
    }
}
```

### Checklist Fase 3

```
[ ] Endpoint público GET /candidatos/token/{token} funcionando
[ ] Candidato puede obtener la evaluación asignada con su token
[ ] Endpoint POST /respuestas guarda respuestas con timestamp UTC
[ ] Se registra FechaInicioRespuesta al entrar y FechaFinRespuesta al enviar
[ ] No permite enviar respuestas dos veces para el mismo candidato
[ ] Test: GuardarRespuestasHandler guarda correctamente el timestamp
[ ] Test: Token inválido lanza NotFoundException
[ ] Test: Candidato que ya respondió lanza InvalidOperationException
```

---

## FASE 4 — Análisis con IA

**Objetivo:** Enviar respuestas a OpenAI y recibir score + brechas + fortalezas.

**Duración estimada:** 5-6 horas

### Interfaz del analizador (Domain/Ports)

```csharp
// Domain/Ports/IAnalizadorIA.cs
public interface IAnalizadorIA
{
    Task<ResultadoAnalisis> AnalizarRespuestasAsync(
        List<PreguntaConRespuesta> preguntasYRespuestas,
        string tecnologia,
        NivelTecnico nivel);
}

public record PreguntaConRespuesta(
    string TextoPregunta,
    string? RubricaDeEvaluacion,
    int PuntajeMaximo,
    string RespuestaDelCandidato
);

public record ResultadoAnalisis(
    double ScoreTotal,
    string ResumenGeneral,
    string Brechas,
    string Fortalezas,
    List<ScorePorPregunta> DetallesPorPregunta
);

public record ScorePorPregunta(
    Guid PreguntaId,
    double Score,
    string Feedback,
    string BrecharEspecificas
);
```

### Implementación con OpenAI

```csharp
// Infrastructure/AI/AnalizadorOpenAI.cs
public class AnalizadorOpenAI : IAnalizadorIA
{
    private readonly OpenAIClient _client;

    public AnalizadorOpenAI(IConfiguration config)
    {
        _client = new OpenAIClient(config["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI API Key no configurada"));
    }

    public async Task<ResultadoAnalisis> AnalizarRespuestasAsync(
        List<PreguntaConRespuesta> items,
        string tecnologia,
        NivelTecnico nivel)
    {
        var prompt = ConstruirPromptDeAnalisis(items, tecnologia, nivel);
        var chatClient = _client.GetChatClient("gpt-4o-mini");

        var respuesta = await chatClient.CompleteChatAsync(
            new UserChatMessage(prompt)
        );

        var json = respuesta.Value.Content[0].Text;
        return DeserializarRespuestaIA(json);
    }

    private static string ConstruirPromptDeAnalisis(
        List<PreguntaConRespuesta> items,
        string tecnologia,
        NivelTecnico nivel)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Eres un evaluador técnico experto en {tecnologia} evaluando un candidato de nivel {nivel}.");
        sb.AppendLine("Analiza las siguientes respuestas y devuelve ÚNICAMENTE un JSON con este formato exacto:");
        sb.AppendLine("""
        {
          "scoreTotal": 75.5,
          "resumenGeneral": "Párrafo de resumen",
          "brechas": "Lista de brechas separadas por coma",
          "fortalezas": "Lista de fortalezas separadas por coma",
          "detalles": [
            {
              "preguntaIndex": 0,
              "score": 80,
              "feedback": "Comentario sobre la respuesta",
              "brechas": "Brechas específicas de esta pregunta"
            }
          ]
        }
        """);
        sb.AppendLine("\n--- PREGUNTAS Y RESPUESTAS ---");

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            sb.AppendLine($"\nPREGUNTA {i + 1}: {item.TextoPregunta}");
            sb.AppendLine($"Puntaje máximo: {item.PuntajeMaximo}");
            if (!string.IsNullOrEmpty(item.RubricaDeEvaluacion))
                sb.AppendLine($"Criterios: {item.RubricaDeEvaluacion}");
            sb.AppendLine($"RESPUESTA: {item.RespuestaDelCandidato}");
        }

        return sb.ToString();
    }

    private static ResultadoAnalisis DeserializarRespuestaIA(string json)
    {
        // Deserializar el JSON de respuesta de OpenAI
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var dto = JsonSerializer.Deserialize<AnalisisIADto>(json, options)
            ?? throw new InvalidOperationException("La IA devolvió una respuesta inválida");

        return new ResultadoAnalisis(
            dto.ScoreTotal,
            dto.ResumenGeneral,
            dto.Brechas,
            dto.Fortalezas,
            dto.Detalles.Select((d, i) => new ScorePorPregunta(
                Guid.Empty, // Se mapea después con el ID real de la pregunta
                d.Score,
                d.Feedback,
                d.Brechas
            )).ToList()
        );
    }
}
```

### Caso de uso: Ejecutar análisis

```csharp
// Application/UseCases/Analisis/EjecutarAnalisisIA/EjecutarAnalisisHandler.cs
public class EjecutarAnalisisHandler
{
    private readonly ICandidatoRepository _candidatoRepository;
    private readonly IRespuestaRepository _respuestaRepository;
    private readonly IEvaluacionRepository _evaluacionRepository;
    private readonly IAnalizadorIA _analizadorIA;
    private readonly IResultadoRepository _resultadoRepository;

    public EjecutarAnalisisHandler(
        ICandidatoRepository candidatoRepository,
        IRespuestaRepository respuestaRepository,
        IEvaluacionRepository evaluacionRepository,
        IAnalizadorIA analizadorIA,
        IResultadoRepository resultadoRepository)
    {
        _candidatoRepository = candidatoRepository;
        _respuestaRepository = respuestaRepository;
        _evaluacionRepository = evaluacionRepository;
        _analizadorIA = analizadorIA;
        _resultadoRepository = resultadoRepository;
    }

    public async Task<ResultadoEvaluacion> HandleAsync(Guid candidatoId)
    {
        var candidato = await ObtenerCandidatoOLanzarExcepcion(candidatoId);
        var evaluacion = await ObtenerEvaluacionConPreguntas(candidato.EvaluacionId);
        var respuestas = await _respuestaRepository.ObtenerPorCandidatoAsync(candidatoId);
        var itemsParaAnalisis = PrepararItemsParaAnalisis(evaluacion.Preguntas, respuestas);

        var analisis = await _analizadorIA.AnalizarRespuestasAsync(
            itemsParaAnalisis,
            evaluacion.Tecnologia,
            evaluacion.Nivel
        );

        var resultado = new ResultadoEvaluacion(
            analisis.ScoreTotal,
            analisis.ResumenGeneral,
            analisis.Brechas,
            analisis.Fortalezas,
            candidatoId
        );

        await _resultadoRepository.AgregarAsync(resultado);
        return resultado;
    }

    private async Task<Candidato> ObtenerCandidatoOLanzarExcepcion(Guid candidatoId)
        => await _candidatoRepository.ObtenerPorIdAsync(candidatoId)
            ?? throw new NotFoundException($"Candidato {candidatoId} no encontrado");

    private async Task<Evaluacion> ObtenerEvaluacionConPreguntas(Guid evaluacionId)
        => await _evaluacionRepository.ObtenerConPreguntasAsync(evaluacionId)
            ?? throw new NotFoundException($"Evaluación {evaluacionId} no encontrada");

    private static List<PreguntaConRespuesta> PrepararItemsParaAnalisis(
        List<Pregunta> preguntas,
        List<Respuesta> respuestas)
    {
        return preguntas
            .OrderBy(p => p.OrdenEnEvaluacion)
            .Select(pregunta =>
            {
                var respuesta = respuestas.FirstOrDefault(r => r.PreguntaId == pregunta.Id);
                return new PreguntaConRespuesta(
                    pregunta.Texto,
                    pregunta.Rubrica,
                    pregunta.PuntajeMaximo,
                    respuesta?.Contenido ?? "Sin respuesta"
                );
            })
            .ToList();
    }
}
```

### Checklist Fase 4

```
[ ] OpenAI API Key configurada en appsettings (no en código)
[ ] Endpoint POST /analisis/{candidatoId} funciona
[ ] Prompt estructurado que fuerza respuesta en JSON
[ ] Manejo de errores: si OpenAI falla, no pierde los datos
[ ] Resultado guardado en BD correctamente
[ ] Test con Mock de IAnalizadorIA (no llamar a OpenAI real en tests)
[ ] Test: EjecutarAnalisisHandler llama al analizador con los datos correctos
[ ] Test: Resultado se guarda correctamente en el repositorio
```

---

## FASE 5 — Pantalla de resultados

**Objetivo:** Vista clara con score, recomendación, fortalezas, brechas y detalle por pregunta.

**Duración estimada:** 4-5 horas (mayormente frontend)

### Endpoint de resultados

```csharp
// API/Controllers/ResultadosController.cs
[HttpGet("{candidatoId:guid}")]
public async Task<IActionResult> ObtenerResultado(Guid candidatoId)
{
    var resultado = await _resultadoRepository.ObtenerConDetallesAsync(candidatoId);
    if (resultado is null) return NotFound("El análisis aún no ha sido generado");

    var dto = new ResultadoCompletoDto
    {
        CandidatoNombre = resultado.Candidato.Nombre,
        Tecnologia = resultado.Candidato.Evaluacion.Tecnologia,
        Nivel = resultado.Candidato.Evaluacion.Nivel.ToString(),
        ScoreTotal = resultado.ScoreTotal,
        Recomendacion = resultado.Recomendacion,
        ResumenIA = resultado.ResumenIA,
        Brechas = resultado.BrecharDetectadas.Split(',').ToList(),
        Fortalezas = resultado.FortalezasDetectadas.Split(',').ToList(),
        TiempoInvertido = resultado.Candidato.ObtenerTiempoInvertido()?.ToString(@"hh\:mm\:ss"),
        GeneradoEn = resultado.GeneradoEn
    };

    return Ok(dto);
}
```

### Componente Angular — Pantalla de resultados

```typescript
// frontend/src/app/evaluador/ver-resultado/ver-resultado.component.ts
@Component({
  selector: 'app-ver-resultado',
  standalone: true,
  imports: [CommonModule, MatProgressBarModule, MatChipsModule],
  template: `
    <div class="resultado-container" *ngIf="resultado">

      <!-- Encabezado -->
      <div class="candidato-header">
        <h1>{{ resultado.candidatoNombre }}</h1>
        <span class="tecnologia-badge">{{ resultado.tecnologia }} — {{ resultado.nivel }}</span>
      </div>

      <!-- Score visual -->
      <div class="score-card" [class]="obtenerClaseScore(resultado.scoreTotal)">
        <span class="score-numero">{{ resultado.scoreTotal | number:'1.0-1' }}</span>
        <span class="score-label">/ 100</span>
        <h2 class="recomendacion">{{ resultado.recomendacion }}</h2>
      </div>

      <!-- Barra de progreso -->
      <mat-progress-bar
        [value]="resultado.scoreTotal"
        [color]="obtenerColorBarra(resultado.scoreTotal)">
      </mat-progress-bar>

      <!-- Resumen IA -->
      <div class="resumen-ia">
        <h3>Análisis de la IA</h3>
        <p>{{ resultado.resumenIA }}</p>
      </div>

      <!-- Fortalezas y Brechas -->
      <div class="fortalezas-brechas">
        <div class="fortalezas">
          <h3>✅ Fortalezas</h3>
          <mat-chip *ngFor="let f of resultado.fortalezas">{{ f }}</mat-chip>
        </div>
        <div class="brechas">
          <h3>⚠️ Brechas detectadas</h3>
          <mat-chip *ngFor="let b of resultado.brechas" color="warn">{{ b }}</mat-chip>
        </div>
      </div>

      <!-- Botón exportar PDF -->
      <button mat-raised-button color="primary" (click)="exportarPDF()">
        Descargar Reporte PDF
      </button>
    </div>
  `
})
export class VerResultadoComponent implements OnInit {
  resultado: ResultadoCompletoDto | null = null;

  constructor(
    private route: ActivatedRoute,
    private resultadoService: ResultadoService
  ) {}

  ngOnInit(): void {
    const candidatoId = this.route.snapshot.paramMap.get('id')!;
    this.resultadoService.obtener(candidatoId).subscribe(r => this.resultado = r);
  }

  obtenerClaseScore(score: number): string {
    if (score >= 70) return 'score-alto';
    if (score >= 50) return 'score-medio';
    return 'score-bajo';
  }

  obtenerColorBarra(score: number): string {
    if (score >= 70) return 'primary';
    if (score >= 50) return 'accent';
    return 'warn';
  }

  exportarPDF(): void {
    const candidatoId = this.route.snapshot.paramMap.get('id')!;
    this.resultadoService.exportarPDF(candidatoId).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `reporte-${this.resultado?.candidatoNombre}.pdf`;
      link.click();
    });
  }
}
```

### Checklist Fase 5

```
[ ] Endpoint GET /resultados/{candidatoId} devuelve toda la info
[ ] Componente Angular muestra score con color dinámico
[ ] Fortalezas y brechas como chips visuales
[ ] Recomendación destacada visualmente
[ ] Tiempo invertido por el candidato mostrado
[ ] Ruta protegida: solo el evaluador dueño puede ver el resultado
```

---

## FASE 6 — Exportar PDF

**Objetivo:** Generar un PDF profesional con el reporte del candidato.

**Duración estimada:** 3-4 horas

### Implementación con QuestPDF

```csharp
// Infrastructure/PDF/GeneradorPDFQuestPDF.cs
public class GeneradorPDFQuestPDF : IGeneradorPDF
{
    public byte[] GenerarReporteCandidato(ResultadoCompletoDto resultado)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(contenedor =>
        {
            contenedor.Page(pagina =>
            {
                pagina.Margin(40);

                pagina.Header().Element(ConstruirEncabezado(resultado));
                pagina.Content().Element(ConstruirContenido(resultado));
                pagina.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generado el ");
                    x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                });
            });
        }).GeneratePdf();
    }

    private static Action<IContainer> ConstruirEncabezado(ResultadoCompletoDto r)
        => container => container
            .PaddingBottom(20)
            .Column(col =>
            {
                col.Item().Text($"Reporte de Evaluación Técnica").FontSize(20).Bold();
                col.Item().Text($"Candidato: {r.CandidatoNombre}").FontSize(14);
                col.Item().Text($"Tecnología: {r.Tecnologia} — Nivel: {r.Nivel}");
            });

    private static Action<IContainer> ConstruirContenido(ResultadoCompletoDto r)
        => container => container
            .Column(col =>
            {
                // Score
                col.Item().Background("#f0f0f0").Padding(10).Row(row =>
                {
                    row.RelativeItem().Text($"Score: {r.ScoreTotal:F1}/100").FontSize(18).Bold();
                    row.RelativeItem().Text(r.Recomendacion).FontSize(16);
                });

                // Resumen IA
                col.Item().PaddingTop(15).Text("Análisis de la IA:").Bold();
                col.Item().Text(r.ResumenIA);

                // Fortalezas
                col.Item().PaddingTop(15).Text("Fortalezas:").Bold();
                foreach (var f in r.Fortalezas)
                    col.Item().Text($"• {f}");

                // Brechas
                col.Item().PaddingTop(15).Text("Brechas detectadas:").Bold();
                foreach (var b in r.Brechas)
                    col.Item().Text($"• {b}");
            });
}
```

### Checklist Fase 6

```
[ ] Paquete QuestPDF instalado e inicializado
[ ] Endpoint GET /reportes/{candidatoId}/pdf devuelve archivo PDF
[ ] PDF incluye: nombre candidato, score, recomendación, fortalezas, brechas, resumen IA
[ ] PDF tiene formato profesional con logo/colores
[ ] El frontend descarga el PDF al hacer clic en el botón
```

---

## FASE 7 — Sesión en vivo con temporizador

**Objetivo:** El candidato ve una cuenta regresiva por pregunta; el evaluador puede monitorear en tiempo real.

**Duración estimada:** 6-8 horas (incluye SignalR)

### Entidad SesionEnVivo

```csharp
// Domain/Entities/SesionEnVivo.cs
public class SesionEnVivo : BaseEntity
{
    public Guid EvaluacionId { get; private set; }
    public Guid CandidatoId { get; private set; }
    public int PreguntaActualIndex { get; private set; } = 0;
    public DateTime? InicioFaseActual { get; private set; }
    public bool EstaActiva { get; private set; } = false;
    public bool FueCompletada { get; private set; } = false;

    public SesionEnVivo(Guid evaluacionId, Guid candidatoId)
    {
        EvaluacionId = evaluacionId;
        CandidatoId = candidatoId;
    }

    public void Iniciar()
    {
        EstaActiva = true;
        InicioFaseActual = DateTime.UtcNow;
    }

    public void AvanzarPregunta(int totalPreguntas)
    {
        if (PreguntaActualIndex >= totalPreguntas - 1)
        {
            FueCompletada = true;
            EstaActiva = false;
            return;
        }

        PreguntaActualIndex++;
        InicioFaseActual = DateTime.UtcNow;
        MarcarComoModificado();
    }

    public int ObtenerSegundosTranscurridos()
        => InicioFaseActual.HasValue
            ? (int)(DateTime.UtcNow - InicioFaseActual.Value).TotalSeconds
            : 0;
}
```

### Hub de SignalR

```csharp
// Infrastructure/Hubs/SesionHub.cs
[Authorize]
public class SesionHub : Hub
{
    // El evaluador se une a un grupo para monitorear la sesión
    public async Task UnirseASesion(string sesionId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"sesion-{sesionId}");

    // El candidato se une con su token
    public async Task UnirseComoCandidato(string sesionId, string tokenCandidato)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"sesion-{sesionId}");

    // Notificar a todos en la sesión que cambió la pregunta
    public async Task NotificarCambioDePregunta(string sesionId, int preguntaIndex, int tiempoSegundos)
        => await Clients.Group($"sesion-{sesionId}")
            .SendAsync("PreguntaCambiada", preguntaIndex, tiempoSegundos);

    // Notificar que el tiempo se agotó
    public async Task NotificarTiempoAgotado(string sesionId)
        => await Clients.Group($"sesion-{sesionId}")
            .SendAsync("TiempoAgotado");
}
```

### Servicio Angular para SignalR

```typescript
// frontend/src/app/core/services/sesion.service.ts
@Injectable({ providedIn: 'root' })
export class SesionService {
  private hubConnection: signalR.HubConnection;

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/sesion', { accessTokenFactory: () => this.getToken() })
      .withAutomaticReconnect()
      .build();
  }

  async iniciarConexion(sesionId: string): Promise<void> {
    await this.hubConnection.start();
    await this.hubConnection.invoke('UnirseASesion', sesionId);
  }

  onPreguntaCambiada(callback: (index: number, tiempo: number) => void): void {
    this.hubConnection.on('PreguntaCambiada', callback);
  }

  onTiempoAgotado(callback: () => void): void {
    this.hubConnection.on('TiempoAgotado', callback);
  }

  private getToken(): string {
    return localStorage.getItem('jwt_token') ?? '';
  }
}
```

### Checklist Fase 7

```
[ ] SesionEnVivo entidad y repositorio creados
[ ] SesionHub configurado y mapeado en Program.cs
[ ] Candidato ve temporizador regresivo por pregunta
[ ] Al terminar el tiempo, se pasa automáticamente a la siguiente pregunta
[ ] Evaluador puede ver el progreso en tiempo real
[ ] Servicio Angular usa SignalR correctamente
[ ] La respuesta se guarda aunque el tiempo se agote
```

---

## FASE 8 — Comparación y ranking

**Objetivo:** El evaluador puede ver y comparar múltiples candidatos ordenados por score.

**Duración estimada:** 3-4 horas

### Endpoint de ranking

```csharp
// API/Controllers/ResultadosController.cs
[HttpGet("evaluaciones/{evaluacionId:guid}/ranking")]
public async Task<IActionResult> ObtenerRanking(Guid evaluacionId)
{
    var candidatos = await _candidatoRepository.ObtenerConResultadosPorEvaluacionAsync(evaluacionId);

    var ranking = candidatos
        .Where(c => c.Resultado != null)
        .OrderByDescending(c => c.Resultado!.ScoreTotal)
        .Select((c, index) => new RankingItemDto
        {
            Posicion = index + 1,
            CandidatoId = c.Id,
            Nombre = c.Nombre,
            Email = c.Email,
            Score = c.Resultado!.ScoreTotal,
            Recomendacion = c.Resultado.Recomendacion,
            TiempoInvertido = c.ObtenerTiempoInvertido()?.ToString(@"hh\:mm\:ss") ?? "N/D"
        })
        .ToList();

    return Ok(ranking);
}
```

### Componente Angular de comparación

```typescript
// frontend/src/app/evaluador/comparar-candidatos/comparar-candidatos.component.ts
@Component({
  selector: 'app-comparar-candidatos',
  standalone: true,
  template: `
    <h2>Ranking de Candidatos</h2>
    <table mat-table [dataSource]="ranking" class="ranking-table">

      <ng-container matColumnDef="posicion">
        <th mat-header-cell *matHeaderCellDef>Pos.</th>
        <td mat-cell *matCellDef="let item">
          <span [class]="'medalla-' + item.posicion">{{ item.posicion }}</span>
        </td>
      </ng-container>

      <ng-container matColumnDef="nombre">
        <th mat-header-cell *matHeaderCellDef>Candidato</th>
        <td mat-cell *matCellDef="let item">{{ item.nombre }}</td>
      </ng-container>

      <ng-container matColumnDef="score">
        <th mat-header-cell *matHeaderCellDef>Score</th>
        <td mat-cell *matCellDef="let item">
          <span [class]="obtenerClaseScore(item.score)">{{ item.score | number:'1.0-1' }}</span>
        </td>
      </ng-container>

      <ng-container matColumnDef="recomendacion">
        <th mat-header-cell *matHeaderCellDef>Recomendación</th>
        <td mat-cell *matCellDef="let item">{{ item.recomendacion }}</td>
      </ng-container>

      <ng-container matColumnDef="acciones">
        <th mat-header-cell *matHeaderCellDef>Acciones</th>
        <td mat-cell *matCellDef="let item">
          <button mat-button [routerLink]="['/resultados', item.candidatoId]">Ver detalle</button>
        </td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="columnas"></tr>
      <tr mat-row *matRowDef="let row; columns: columnas;"></tr>
    </table>
  `
})
export class CompararCandidatosComponent implements OnInit {
  ranking: RankingItemDto[] = [];
  columnas = ['posicion', 'nombre', 'score', 'recomendacion', 'acciones'];

  ngOnInit(): void {
    const evaluacionId = this.route.snapshot.paramMap.get('id')!;
    this.resultadoService.obtenerRanking(evaluacionId).subscribe(r => this.ranking = r);
  }
}
```

### Checklist Fase 8

```
[ ] Endpoint GET /evaluaciones/{id}/ranking ordena por score descendente
[ ] Solo muestra candidatos que ya tienen resultado IA generado
[ ] Tabla Angular con posiciones, colores por score
[ ] Enlace desde cada fila al detalle del candidato
[ ] Exportar ranking completo en PDF
```

---

## Estrategia de pruebas

### Pirámide de testing

```
        /\
       /  \
      / E2E \    ← 10%: Pruebas de extremo a extremo (navegador real)
     /--------\
    /Integration\  ← 20%: Pruebas de integración (BD en memoria)
   /------------\
  /  Unit Tests  \  ← 70%: Pruebas unitarias (la mayor parte)
 /----------------\
```

---

### Tests unitarios — Dominio

```csharp
// tests/EvaluacionTecnica.Domain.Tests/Entities/EvaluacionTests.cs
public class EvaluacionTests
{
    // Nombrar los tests: Escenario_CuandoCondicion_DebeResultado
    [Fact]
    public void CrearEvaluacion_CuandoTituloEsValido_DebeCrearseCorrectamente()
    {
        // ARRANGE — Preparar los datos
        var titulo = "Evaluación C# Senior";
        var tecnologia = "C#";
        var nivel = NivelTecnico.Senior;
        var tiempoMinutos = 60;
        var evaluadorId = Guid.NewGuid();

        // ACT — Ejecutar la acción
        var evaluacion = new Evaluacion(titulo, tecnologia, nivel, tiempoMinutos, evaluadorId);

        // ASSERT — Verificar el resultado
        evaluacion.Titulo.Should().Be(titulo);
        evaluacion.Estado.Should().Be(EstadoEvaluacion.Borrador);
        evaluacion.Preguntas.Should().BeEmpty();
    }

    [Fact]
    public void CrearEvaluacion_CuandoTituloEsVacio_DebeLanzarExcepcion()
    {
        // ARRANGE
        var tituloVacio = "";

        // ACT & ASSERT
        Action crearConTituloVacio = () =>
            new Evaluacion(tituloVacio, "C#", NivelTecnico.Senior, 60, Guid.NewGuid());

        crearConTituloVacio.Should().Throw<ArgumentException>()
            .WithMessage("El título de la evaluación es obligatorio");
    }

    [Fact]
    public void AgregarPregunta_CuandoHayMas30Preguntas_DebeLanzarExcepcion()
    {
        // ARRANGE
        var evaluacion = new Evaluacion("Test", "C#", NivelTecnico.Junior, 60, Guid.NewGuid());

        // Agregar exactamente 30 preguntas
        for (int i = 0; i < 30; i++)
            evaluacion.AgregarPregunta(CrearPreguntaValida(evaluacion.Id));

        // ACT & ASSERT
        Action agregarUnaPregunutaMas = () =>
            evaluacion.AgregarPregunta(CrearPreguntaValida(evaluacion.Id));

        agregarUnaPregunutaMas.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Activar_CuandoNoTienePreguntas_DebeLanzarExcepcion()
    {
        // ARRANGE
        var evaluacion = new Evaluacion("Test", "C#", NivelTecnico.Junior, 60, Guid.NewGuid());

        // ACT & ASSERT
        evaluacion.Invoking(e => e.Activar())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*al menos una pregunta*");
    }

    private static Pregunta CrearPreguntaValida(Guid evaluacionId)
        => new("¿Qué es SOLID?", TipoPregunta.TextoLibre, 10, 1, 300, evaluacionId);
}
```

---

### Tests unitarios — Casos de uso (con Mocks)

```csharp
// tests/EvaluacionTecnica.Application.Tests/UseCases/CrearEvaluacionHandlerTests.cs
public class CrearEvaluacionHandlerTests
{
    // Mock = objeto falso que simula el comportamiento real sin BD real
    private readonly Mock<IEvaluacionRepository> _repositorioMock;
    private readonly CrearEvaluacionHandler _handler;

    public CrearEvaluacionHandlerTests()
    {
        _repositorioMock = new Mock<IEvaluacionRepository>();
        _handler = new CrearEvaluacionHandler(_repositorioMock.Object);
    }

    [Fact]
    public async Task HandleAsync_CuandoDatosValidos_DebeGuardarEnRepositorio()
    {
        // ARRANGE
        var command = new CrearEvaluacionCommand(
            Titulo: "Eval C#",
            Descripcion: null,
            Tecnologia: "C#",
            Nivel: NivelTecnico.Mid,
            TiempoLimiteTotalMinutos: 60,
            Preguntas: new List<AgregarPreguntaDto>
            {
                new("¿Qué es async/await?", TipoPregunta.TextoLibre, 10, 300, "Criterios...", null)
            }
        );

        var evaluadorId = Guid.NewGuid();

        // Configurar el mock: cuando se llame AgregarAsync, no hacer nada (simulado)
        _repositorioMock
            .Setup(r => r.AgregarAsync(It.IsAny<Evaluacion>()))
            .Returns(Task.CompletedTask);

        // ACT
        var id = await _handler.HandleAsync(command, evaluadorId);

        // ASSERT
        id.Should().NotBeEmpty("Se debe retornar un ID válido");

        // Verificar que el repositorio fue llamado exactamente una vez
        _repositorioMock.Verify(
            r => r.AgregarAsync(It.Is<Evaluacion>(e => e.Titulo == "Eval C#")),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleAsync_CuandoAnalisisIAFalla_DebePropagarseLaExcepcion()
    {
        // Aquí testeas que si el repositorio lanza excepción, el handler la propaga
        _repositorioMock
            .Setup(r => r.AgregarAsync(It.IsAny<Evaluacion>()))
            .ThrowsAsync(new Exception("Error de BD"));

        var command = /* datos válidos */;

        await _handler.Invoking(h => h.HandleAsync(command, Guid.NewGuid()))
            .Should().ThrowAsync<Exception>();
    }
}
```

---

### Tests de integración (BD en memoria)

```csharp
// Para tests de integración, usar SQLite en memoria con EF Core
public class EvaluacionRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _contexto;
    private readonly EvaluacionRepository _repository;

    public EvaluacionRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // BD única por test
            .Options;

        _contexto = new AppDbContext(options);
        _repository = new EvaluacionRepository(_contexto);
    }

    [Fact]
    public async Task AgregarAsync_CuandoEvaluacionValida_DebePersistrirseEnBD()
    {
        // ARRANGE
        var evaluacion = new Evaluacion("Test BD", "Angular", NivelTecnico.Junior, 45, Guid.NewGuid());

        // ACT
        await _repository.AgregarAsync(evaluacion);
        var recuperada = await _repository.ObtenerPorIdAsync(evaluacion.Id);

        // ASSERT
        recuperada.Should().NotBeNull();
        recuperada!.Titulo.Should().Be("Test BD");
    }

    public void Dispose() => _contexto.Dispose();
}
```

---

### Cobertura de tests — objetivo mínimo

| Capa | Cobertura objetivo | Tipo de test |
|------|--------------------|-------------|
| Domain (Entities) | 95% | Unitarios |
| Application (Handlers) | 90% | Unitarios con Mocks |
| Infrastructure (Repositories) | 80% | Integración con BD en memoria |
| API (Controllers) | 70% | Integración con WebApplicationFactory |

### Comando para ver cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

---

## Frontend Angular

### Estructura de módulos

```
src/app/
├── core/                          ← Singleton services, guards, interceptors
│   ├── guards/
│   │   ├── auth.guard.ts          ← Redirige si no hay JWT
│   │   └── evaluador.guard.ts     ← Solo usuarios con rol Evaluador
│   ├── interceptors/
│   │   ├── jwt.interceptor.ts     ← Agrega el token a TODAS las peticiones
│   │   └── error.interceptor.ts   ← Manejo global de errores HTTP
│   ├── services/
│   │   ├── auth.service.ts
│   │   ├── evaluacion.service.ts
│   │   ├── candidato.service.ts
│   │   ├── resultado.service.ts
│   │   └── sesion.service.ts      ← SignalR
│   └── models/                    ← Interfaces TypeScript
│       ├── evaluacion.model.ts
│       ├── candidato.model.ts
│       └── resultado.model.ts
│
├── evaluador/                     ← Módulo del evaluador (lazy loaded)
│   ├── dashboard/
│   ├── crear-evaluacion/
│   ├── editar-evaluacion/
│   ├── mis-evaluaciones/
│   ├── candidatos/
│   ├── ver-resultado/
│   └── comparar-candidatos/
│
├── candidato/                     ← Módulo del candidato (lazy loaded)
│   ├── instrucciones/
│   ├── responder/
│   └── mi-resultado/
│
└── shared/                        ← Componentes reutilizables
    ├── components/
    │   ├── score-badge/
    │   ├── countdown-timer/
    │   └── loading-spinner/
    └── pipes/
        └── tiempo-invertido.pipe.ts
```

### Interceptor JWT (se agrega automáticamente a todas las peticiones)

```typescript
// core/interceptors/jwt.interceptor.ts
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('jwt_token');

  if (token) {
    const requestConToken = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
    return next(requestConToken);
  }

  return next(req);
};
```

### Rutas principales

```typescript
// app.routes.ts
export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },

  // Rutas del evaluador (protegidas)
  {
    path: 'evaluador',
    canActivate: [authGuard, evaluadorGuard],
    loadChildren: () => import('./evaluador/evaluador.routes')
  },

  // Rutas del candidato (solo necesitan token, no JWT)
  {
    path: 'candidato/:token',
    loadChildren: () => import('./candidato/candidato.routes')
  },

  { path: '**', redirectTo: '/login' }
];
```

---

## Bitácora de progreso

> Marca con ✅ cada ítem completado durante el desarrollo.

### FASE 0 — Setup
```
[ ] Solución .NET creada con 4 proyectos
[ ] Proyectos de tests creados
[ ] Referencias entre proyectos configuradas
[ ] NuGet packages instalados
[ ] AppDbContext configurado
[ ] Primera migración ejecutada
[ ] API corre correctamente
[ ] Swagger accesible
[ ] Proyecto Angular creado
[ ] Angular Material instalado
[ ] Variables de entorno configuradas
```

### FASE 1 — Autenticación
```
[ ] Usuario entidad con ASP.NET Identity
[ ] Endpoint POST /auth/login funciona
[ ] JWT generado y válido
[ ] Roles Admin/Evaluador configurados
[ ] Token único para candidatos implementado
[ ] Guard de Angular protege rutas
[ ] Interceptor JWT funciona
[ ] Test: Login correcto genera token
[ ] Test: Login incorrecto lanza 401
```

### FASE 2 — CRUD Evaluaciones
```
[ ] CrearEvaluacion funciona end-to-end
[ ] ListarEvaluaciones del evaluador funciona
[ ] ObtenerPorId con preguntas funciona
[ ] EliminarEvaluacion (soft delete) funciona
[ ] AgregarPregunta funciona
[ ] Activar/Cerrar evaluación funciona
[ ] Pantalla Angular de lista de evaluaciones
[ ] Pantalla Angular de crear evaluación (formulario)
[ ] Tests unitarios del Domain
[ ] Tests de handlers con Mocks
```

### FASE 3 — Respuestas
```
[ ] Candidato accede con token
[ ] Pantalla Angular de respuesta de prueba
[ ] Respuestas guardadas con timestamp
[ ] FechaInicio y FechaFin del candidato registradas
[ ] No permite doble envío
[ ] Test: GuardarRespuestas guarda timestamp correcto
```

### FASE 4 — IA
```
[ ] OpenAI API Key configurada
[ ] Prompt diseñado y probado manualmente
[ ] AnalizadorOpenAI implementado
[ ] EjecutarAnalisisHandler funciona
[ ] Resultado guardado en BD
[ ] Test con Mock de IAnalizadorIA
```

### FASE 5 — Resultados
```
[ ] Endpoint GET /resultados/{id} funciona
[ ] Pantalla Angular de resultados con score visual
[ ] Fortalezas y brechas mostradas
[ ] Recomendación destacada
```

### FASE 6 — PDF
```
[ ] QuestPDF instalado y configurado
[ ] PDF generado con los datos correctos
[ ] Endpoint devuelve el archivo
[ ] Angular descarga el PDF correctamente
```

### FASE 7 — Sesión en vivo
```
[ ] SesionEnVivo entidad creada
[ ] SignalR Hub configurado
[ ] Temporizador en frontend funciona
[ ] Cambio automático de pregunta al agotar tiempo
[ ] Evaluador ve progreso en tiempo real
```

### FASE 8 — Ranking
```
[ ] Endpoint de ranking implementado
[ ] Tabla Angular de ranking con posiciones
[ ] Colores por score
[ ] Enlace al detalle de cada candidato
```

---

## Comandos útiles

### .NET

```bash
# Compilar toda la solución
dotnet build

# Ejecutar la API
dotnet run --project src/EvaluacionTecnica.API

# Ejecutar tests
dotnet test

# Ejecutar tests con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Nueva migración
dotnet ef migrations add NombreMigracion --project src/EvaluacionTecnica.Infrastructure --startup-project src/EvaluacionTecnica.API

# Aplicar migraciones
dotnet ef database update --project src/EvaluacionTecnica.Infrastructure --startup-project src/EvaluacionTecnica.API

# Revertir última migración
dotnet ef migrations remove --project src/EvaluacionTecnica.Infrastructure --startup-project src/EvaluacionTecnica.API
```

### Angular

```bash
# Instalar Angular CLI globalmente
npm install -g @angular/cli

# Crear proyecto Angular
ng new evaluacion-tecnica --standalone --routing --style=css

# Instalar Angular Material
ng add @angular/material

# Instalar SignalR
npm install @microsoft/signalr

# Servidor de desarrollo
ng serve

# Build para producción
ng build --configuration production

# Ejecutar tests
ng test

# Ejecutar tests con cobertura
ng test --code-coverage
```

### Docker (opcional para BD)

```bash
# SQL Server en Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=TuPassword123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# PostgreSQL en Docker
docker run -e "POSTGRES_PASSWORD=TuPassword123!" -p 5432:5432 -d postgres:16
```

### appsettings.json — Variables importantes

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EvaluacionTecnicaDB;User Id=sa;Password=TuPassword123!;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "TuClaveSecretaMuyLargaMinimoDeCharacteres32",
    "Issuer": "EvaluacionTecnicaAPI",
    "Audience": "EvaluacionTecnicaFrontend",
    "ExpirationHours": 8
  },
  "OpenAI": {
    "ApiKey": "sk-tu-api-key-aqui"
  }
}
```

> **IMPORTANTE:** Nunca subir `appsettings.json` con datos reales al repositorio. Usar `appsettings.Development.json` que está en `.gitignore`.

---

*Documento generado el 19 de Marzo de 2026 — Sistema de Evaluación Técnica Asistida por IA*
