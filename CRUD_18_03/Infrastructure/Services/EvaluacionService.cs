using CRUD_18_03.Application.DTOs.Evaluacion;
using CRUD_18_03.Application.DTOs.Pregunta;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Domain.Entities;
using CRUD_18_03.Domain.Enums;
using CRUD_18_03.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Infrastructure.Services;

public class EvaluacionService : IEvaluacionService
{
    private readonly ApplicationDbContext _dbContext;

    public EvaluacionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EvaluacionConPreguntasDto> CrearAsync(CrearEvaluacionConPreguntasDto dto, Guid evaluadorId)
    {
        if (dto.Preguntas?.Count > 30)
            throw new InvalidOperationException("Una evaluación no puede tener más de 30 preguntas.");

        var evaluacion = new Evaluacion
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            Tecnologia = dto.Tecnologia,
            Nivel = dto.Nivel,
            TiempoLimiteTotalMinutos = dto.TiempoLimiteTotalMinutos,
            RequiereCamara = dto.RequiereCamara,
            RequiereMicrofono = dto.RequiereMicrofono,
            EvaluadorId = evaluadorId,
            Estado = EstadoEvaluacion.Borrador
        };

        if (dto.Preguntas is { Count: > 0 })
        {
            for (int i = 0; i < dto.Preguntas.Count; i++)
            {
                var p = dto.Preguntas[i];
                evaluacion.Preguntas.Add(new Pregunta
                {
                    Texto = p.Texto,
                    Tipo = p.Tipo,
                    Rubrica = p.Rubrica,
                    PuntajeMaximo = p.PuntajeMaximo,
                    OrdenEnEvaluacion = i + 1,
                    TiempoLimiteSegundos = p.TiempoLimiteSegundos,
                    EvaluacionId = evaluacion.Id
                });
            }
        }

        _dbContext.Evaluaciones.Add(evaluacion);
        await _dbContext.SaveChangesAsync();

        return MapToConPreguntasDto(evaluacion);
    }

    public async Task<IEnumerable<EvaluacionDto>> ListarPorEvaluadorAsync(Guid evaluadorId)
    {
        var evaluaciones = await _dbContext.Evaluaciones
            .Where(e => e.EvaluadorId == evaluadorId && e.EstaActivo)
            .AsNoTracking()
            .OrderByDescending(e => e.CreadoEn)
            .ToListAsync();

        return evaluaciones.Select(MapToDto);
    }

    public async Task<EvaluacionConPreguntasDto?> ObtenerConPreguntasAsync(Guid id, Guid evaluadorId)
    {
        var evaluacion = await _dbContext.Evaluaciones
            .Include(e => e.Preguntas.Where(p => p.EstaActivo))
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.EstaActivo);

        if (evaluacion is null) return null;

        ValidarPropietario(evaluacion, evaluadorId);

        return MapToConPreguntasDto(evaluacion);
    }

    public async Task ActualizarAsync(Guid id, ActualizarEvaluacionDto dto, Guid evaluadorId)
    {
        var evaluacion = await ObtenerYValidar(id, evaluadorId);

        if (evaluacion.Estado != EstadoEvaluacion.Borrador)
            throw new InvalidOperationException("Solo se pueden modificar evaluaciones en estado Borrador.");

        if (dto.Titulo is not null) evaluacion.Titulo = dto.Titulo;
        if (dto.Descripcion is not null) evaluacion.Descripcion = dto.Descripcion;
        if (dto.Tecnologia is not null) evaluacion.Tecnologia = dto.Tecnologia;
        if (dto.Nivel.HasValue) evaluacion.Nivel = dto.Nivel.Value;
        if (dto.TiempoLimiteTotalMinutos.HasValue) evaluacion.TiempoLimiteTotalMinutos = dto.TiempoLimiteTotalMinutos.Value;
        if (dto.RequiereCamara.HasValue) evaluacion.RequiereCamara = dto.RequiereCamara.Value;
        if (dto.RequiereMicrofono.HasValue) evaluacion.RequiereMicrofono = dto.RequiereMicrofono.Value;

        evaluacion.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task EliminarAsync(Guid id, Guid evaluadorId)
    {
        var evaluacion = await ObtenerYValidar(id, evaluadorId);

        evaluacion.EstaActivo = false;
        evaluacion.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task ActivarAsync(Guid id, Guid evaluadorId)
    {
        var evaluacion = await _dbContext.Evaluaciones
            .Include(e => e.Preguntas.Where(p => p.EstaActivo))
            .FirstOrDefaultAsync(e => e.Id == id && e.EstaActivo)
            ?? throw new KeyNotFoundException($"Evaluación con ID '{id}' no encontrada.");

        ValidarPropietario(evaluacion, evaluadorId);

        if (evaluacion.Estado != EstadoEvaluacion.Borrador)
            throw new InvalidOperationException("Solo se pueden activar evaluaciones en estado Borrador.");

        if (evaluacion.Preguntas.Count == 0)
            throw new InvalidOperationException("No se puede activar una evaluación sin preguntas.");

        evaluacion.Estado = EstadoEvaluacion.Activa;
        evaluacion.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task CerrarAsync(Guid id, Guid evaluadorId)
    {
        var evaluacion = await ObtenerYValidar(id, evaluadorId);

        if (evaluacion.Estado != EstadoEvaluacion.Activa)
            throw new InvalidOperationException("Solo se pueden cerrar evaluaciones en estado Activa.");

        evaluacion.Estado = EstadoEvaluacion.Cerrada;
        evaluacion.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task ReactivarAsync(Guid id, Guid evaluadorId)
    {
        var evaluacion = await _dbContext.Evaluaciones
            .Include(e => e.Preguntas.Where(p => p.EstaActivo))
            .FirstOrDefaultAsync(e => e.Id == id && e.EstaActivo)
            ?? throw new KeyNotFoundException($"Evaluación con ID '{id}' no encontrada.");

        ValidarPropietario(evaluacion, evaluadorId);

        if (evaluacion.Estado != EstadoEvaluacion.Cerrada)
            throw new InvalidOperationException("Solo se pueden reactivar evaluaciones en estado Cerrada.");

        if (evaluacion.Preguntas.Count == 0)
            throw new InvalidOperationException("No se puede reactivar una evaluación sin preguntas.");

        evaluacion.Estado = EstadoEvaluacion.Activa;
        evaluacion.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PreguntaDto> AgregarPreguntaAsync(Guid evaluacionId, AgregarPreguntaDto dto, Guid evaluadorId)
    {
        var evaluacion = await _dbContext.Evaluaciones
            .Include(e => e.Preguntas.Where(p => p.EstaActivo))
            .FirstOrDefaultAsync(e => e.Id == evaluacionId && e.EstaActivo)
            ?? throw new KeyNotFoundException($"Evaluación con ID '{evaluacionId}' no encontrada.");

        ValidarPropietario(evaluacion, evaluadorId);

        if (evaluacion.Estado != EstadoEvaluacion.Borrador)
            throw new InvalidOperationException("Solo se pueden agregar preguntas a evaluaciones en estado Borrador.");

        if (evaluacion.Preguntas.Count >= 30)
            throw new InvalidOperationException("Una evaluación no puede tener más de 30 preguntas.");

        var orden = evaluacion.Preguntas.Count > 0
            ? evaluacion.Preguntas.Max(p => p.OrdenEnEvaluacion) + 1
            : 1;

        var pregunta = new Pregunta
        {
            Texto = dto.Texto,
            Tipo = dto.Tipo,
            Rubrica = dto.Rubrica,
            PuntajeMaximo = dto.PuntajeMaximo,
            OrdenEnEvaluacion = orden,
            TiempoLimiteSegundos = dto.TiempoLimiteSegundos,
            EvaluacionId = evaluacionId
        };

        _dbContext.Preguntas.Add(pregunta);
        await _dbContext.SaveChangesAsync();

        return MapToPreguntaDto(pregunta);
    }

    public async Task ActualizarPreguntaAsync(Guid evaluacionId, Guid preguntaId, ActualizarPreguntaDto dto, Guid evaluadorId)
    {
        var evaluacion = await ObtenerYValidar(evaluacionId, evaluadorId);

        if (evaluacion.Estado != EstadoEvaluacion.Borrador)
            throw new InvalidOperationException("Solo se pueden modificar preguntas de evaluaciones en estado Borrador.");

        var pregunta = await _dbContext.Preguntas
            .FirstOrDefaultAsync(p => p.Id == preguntaId && p.EvaluacionId == evaluacionId && p.EstaActivo)
            ?? throw new KeyNotFoundException($"Pregunta con ID '{preguntaId}' no encontrada en esta evaluación.");

        if (dto.Texto is not null) pregunta.Texto = dto.Texto;
        if (dto.Tipo.HasValue) pregunta.Tipo = dto.Tipo.Value;
        if (dto.Rubrica is not null) pregunta.Rubrica = dto.Rubrica;
        if (dto.PuntajeMaximo.HasValue) pregunta.PuntajeMaximo = dto.PuntajeMaximo.Value;
        if (dto.OrdenEnEvaluacion.HasValue) pregunta.OrdenEnEvaluacion = dto.OrdenEnEvaluacion.Value;
        if (dto.TiempoLimiteSegundos.HasValue) pregunta.TiempoLimiteSegundos = dto.TiempoLimiteSegundos.Value;

        pregunta.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task EliminarPreguntaAsync(Guid evaluacionId, Guid preguntaId, Guid evaluadorId)
    {
        var evaluacion = await ObtenerYValidar(evaluacionId, evaluadorId);

        if (evaluacion.Estado != EstadoEvaluacion.Borrador)
            throw new InvalidOperationException("Solo se pueden eliminar preguntas de evaluaciones en estado Borrador.");

        var pregunta = await _dbContext.Preguntas
            .FirstOrDefaultAsync(p => p.Id == preguntaId && p.EvaluacionId == evaluacionId && p.EstaActivo)
            ?? throw new KeyNotFoundException($"Pregunta con ID '{preguntaId}' no encontrada en esta evaluación.");

        pregunta.EstaActivo = false;
        pregunta.ModificadoEn = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    // --- Helpers privados ---

    private async Task<Evaluacion> ObtenerYValidar(Guid id, Guid evaluadorId)
    {
        var evaluacion = await _dbContext.Evaluaciones.FindAsync(id)
            ?? throw new KeyNotFoundException($"Evaluación con ID '{id}' no encontrada.");

        if (!evaluacion.EstaActivo)
            throw new KeyNotFoundException($"Evaluación con ID '{id}' no encontrada.");

        ValidarPropietario(evaluacion, evaluadorId);

        return evaluacion;
    }

    private static void ValidarPropietario(Evaluacion evaluacion, Guid evaluadorId)
    {
        if (evaluacion.EvaluadorId != evaluadorId)
            throw new UnauthorizedAccessException("No tiene permiso para acceder a esta evaluación.");
    }

    private static EvaluacionDto MapToDto(Evaluacion e) => new()
    {
        Id = e.Id,
        Titulo = e.Titulo,
        Descripcion = e.Descripcion,
        Tecnologia = e.Tecnologia,
        Nivel = e.Nivel,
        Estado = e.Estado,
        TiempoLimiteTotalMinutos = e.TiempoLimiteTotalMinutos,
        RequiereCamara = e.RequiereCamara,
        RequiereMicrofono = e.RequiereMicrofono,
        EvaluadorId = e.EvaluadorId,
        CreadoEn = e.CreadoEn,
        EstaActivo = e.EstaActivo
    };

    private static EvaluacionConPreguntasDto MapToConPreguntasDto(Evaluacion e) => new()
    {
        Id = e.Id,
        Titulo = e.Titulo,
        Descripcion = e.Descripcion,
        Tecnologia = e.Tecnologia,
        Nivel = e.Nivel,
        Estado = e.Estado,
        TiempoLimiteTotalMinutos = e.TiempoLimiteTotalMinutos,
        RequiereCamara = e.RequiereCamara,
        RequiereMicrofono = e.RequiereMicrofono,
        EvaluadorId = e.EvaluadorId,
        CreadoEn = e.CreadoEn,
        EstaActivo = e.EstaActivo,
        Preguntas = e.Preguntas
            .Where(p => p.EstaActivo)
            .OrderBy(p => p.OrdenEnEvaluacion)
            .Select(MapToPreguntaDto)
            .ToList()
    };

    private static PreguntaDto MapToPreguntaDto(Pregunta p) => new()
    {
        Id = p.Id,
        Texto = p.Texto,
        Tipo = p.Tipo,
        Rubrica = p.Rubrica,
        PuntajeMaximo = p.PuntajeMaximo,
        OrdenEnEvaluacion = p.OrdenEnEvaluacion,
        TiempoLimiteSegundos = p.TiempoLimiteSegundos,
        EvaluacionId = p.EvaluacionId,
        CreadoEn = p.CreadoEn,
        EstaActivo = p.EstaActivo
    };
}
