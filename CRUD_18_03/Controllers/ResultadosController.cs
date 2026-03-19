using System.Security.Claims;
using CRUD_18_03.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_18_03.Controllers;

[ApiController]
[Route("api/resultados")]
[Authorize(Roles = "Admin,Evaluador")]
public class ResultadosController : ControllerBase
{
    private readonly IResultadoService _resultadoService;

    public ResultadosController(IResultadoService resultadoService)
    {
        _resultadoService = resultadoService;
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
