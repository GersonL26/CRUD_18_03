using System.Security.Claims;
using CRUD_18_03.Application.DTOs.Proctoring;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Controllers;

[ApiController]
[Route("api/resultados")]
[Authorize(Roles = "Admin,Evaluador")]
public class ResultadosController : ControllerBase
{
    private readonly IResultadoService _resultadoService;
    private readonly ApplicationDbContext _dbContext;

    public ResultadosController(IResultadoService resultadoService, ApplicationDbContext dbContext)
    {
        _resultadoService = resultadoService;
        _dbContext = dbContext;
    }

    [HttpGet("candidato/{candidatoId:guid}")]
    public async Task<IActionResult> ObtenerResultado(Guid candidatoId)
    {
        var evaluadorId = ObtenerUsuarioId();
        var resultado = await _resultadoService.ObtenerResultadoAsync(candidatoId, evaluadorId);

        if (resultado is null)
            return NotFound(new { message = "El análisis aún no ha sido generado para este candidato." });

        return Ok(resultado);
    }

    [HttpGet("evaluacion/{evaluacionId:guid}")]
    public async Task<IActionResult> ObtenerResumenEvaluacion(Guid evaluacionId)
    {
        var evaluadorId = ObtenerUsuarioId();
        var resumen = await _resultadoService.ObtenerResumenEvaluacionAsync(evaluacionId, evaluadorId);
        return Ok(resumen);
    }

    [HttpGet("evaluacion/{evaluacionId:guid}/ranking")]
    public async Task<IActionResult> ObtenerRanking(Guid evaluacionId)
    {
        var evaluadorId = ObtenerUsuarioId();
        var ranking = await _resultadoService.ObtenerRankingAsync(evaluacionId, evaluadorId);
        return Ok(ranking);
    }

    [HttpPost("evaluacion/{evaluacionId:guid}/comparar")]
    public async Task<IActionResult> CompararCandidatos(
        Guid evaluacionId, [FromBody] CompararCandidatosRequest request)
    {
        var evaluadorId = ObtenerUsuarioId();
        var comparacion = await _resultadoService.CompararCandidatosAsync(
            evaluacionId, request.CandidatoIds, evaluadorId);
        return Ok(comparacion);
    }

    [HttpPost("candidato/{candidatoId:guid}/liberar")]
    public async Task<IActionResult> LiberarResultado(Guid candidatoId)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _resultadoService.LiberarResultadoAsync(candidatoId, evaluadorId);
        return Ok(new { message = "Resultado liberado para el candidato." });
    }

    [HttpGet("candidato/{candidatoId:guid}/proctoring")]
    public async Task<IActionResult> ObtenerProctoring(Guid candidatoId)
    {
        var evaluadorId = ObtenerUsuarioId();

        var candidato = await _dbContext.Candidatos
            .Include(c => c.Evaluacion)
            .FirstOrDefaultAsync(c => c.Id == candidatoId && c.EstaActivo)
            ?? throw new KeyNotFoundException("Candidato no encontrado.");

        if (candidato.Evaluacion!.EvaluadorId != evaluadorId)
            throw new UnauthorizedAccessException("No tiene permiso.");

        var eventos = await _dbContext.EventosProctoring
            .Where(e => e.CandidatoId == candidatoId && e.EstaActivo)
            .OrderBy(e => e.Timestamp)
            .Select(e => new EventoProctoringDto
            {
                Id = e.Id,
                Tipo = e.Tipo,
                Detalle = e.Detalle,
                Timestamp = e.Timestamp
            })
            .ToListAsync();

        var resumen = new ResumenProctoringDto
        {
            VecesSalioFoco = candidato.VecesSalioFoco,
            VecesCopyPaste = eventos.Count(e => e.Tipo == "CopyPaste"),
            TranscripcionesAudio = eventos.Count(e => e.Tipo == "TranscripcionAudio"),
            Eventos = eventos
        };

        return Ok(resumen);
    }

    private Guid ObtenerUsuarioId()
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Token de usuario inválido."));
}

[ApiController]
[Route("api/prueba")]
[AllowAnonymous]
public class ResultadoPublicoController : ControllerBase
{
    private readonly IResultadoService _resultadoService;

    public ResultadoPublicoController(IResultadoService resultadoService)
    {
        _resultadoService = resultadoService;
    }

    [HttpGet("{token}/resultado")]
    public async Task<IActionResult> ObtenerResultadoPorToken(string token)
    {
        var resultado = await _resultadoService.ObtenerResultadoPorTokenAsync(token);
        return Ok(resultado);
    }
}

public class CompararCandidatosRequest
{
    public List<Guid> CandidatoIds { get; set; } = new();
}
