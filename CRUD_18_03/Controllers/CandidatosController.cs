using System.Security.Claims;
using CRUD_18_03.Application.DTOs.Candidato;
using CRUD_18_03.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_18_03.Controllers;

[ApiController]
[Route("api/evaluaciones/{evaluacionId:guid}/candidatos")]
[Authorize(Roles = "Admin,Evaluador")]
public class CandidatosController : ControllerBase
{
    private readonly ICandidatoService _candidatoService;

    public CandidatosController(ICandidatoService candidatoService)
    {
        _candidatoService = candidatoService;
    }

    [HttpPost]
    public async Task<IActionResult> Invitar(Guid evaluacionId, [FromBody] InvitarCandidatoDto dto)
    {
        var evaluadorId = ObtenerUsuarioId();
        var candidato = await _candidatoService.InvitarAsync(evaluacionId, dto, evaluadorId);
        return StatusCode(201, candidato);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(Guid evaluacionId)
    {
        var evaluadorId = ObtenerUsuarioId();
        var candidatos = await _candidatoService.ListarPorEvaluacionAsync(evaluacionId, evaluadorId);
        return Ok(candidatos);
    }

    [HttpDelete("{candidatoId:guid}")]
    public async Task<IActionResult> Eliminar(Guid evaluacionId, Guid candidatoId)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _candidatoService.EliminarAsync(evaluacionId, candidatoId, evaluadorId);
        return Ok(new { message = "Candidato eliminado correctamente." });
    }

    private Guid ObtenerUsuarioId()
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Token de usuario inválido."));
}

/// <summary>
/// Controlador público para candidatos (acceso con token, sin JWT).
/// </summary>
[ApiController]
[Route("api/prueba")]
[AllowAnonymous]
public class PruebaController : ControllerBase
{
    private readonly ICandidatoService _candidatoService;

    public PruebaController(ICandidatoService candidatoService)
    {
        _candidatoService = candidatoService;
    }

    [HttpGet("{token}")]
    public async Task<IActionResult> ObtenerPorToken(string token)
    {
        var evaluacion = await _candidatoService.ObtenerPorTokenAsync(token);
        return Ok(evaluacion);
    }

    [HttpPost("{token}/iniciar")]
    public async Task<IActionResult> Iniciar(string token)
    {
        await _candidatoService.IniciarRespuestaAsync(token);
        return Ok(new { message = "Prueba iniciada correctamente." });
    }

    [HttpPost("{token}/respuestas")]
    public async Task<IActionResult> EnviarRespuestas(string token, [FromBody] EnviarRespuestasDto dto)
    {
        await _candidatoService.EnviarRespuestasAsync(token, dto);
        return Ok(new { message = "Respuestas enviadas correctamente." });
    }
}
