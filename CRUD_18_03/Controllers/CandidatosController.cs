using System.Security.Claims;
using CRUD_18_03.Application.DTOs.Auth;
using CRUD_18_03.Application.DTOs.Candidato;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Domain.Enums;
using CRUD_18_03.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUD_18_03.Controllers;

[ApiController]
[Route("api/evaluaciones/{evaluacionId:guid}/candidatos")]
[Authorize(Roles = "Admin,Evaluador")]
public class CandidatosController : ControllerBase
{
    private readonly ICandidatoService _candidatoService;
    private readonly ApplicationDbContext _dbContext;

    public CandidatosController(ICandidatoService candidatoService, ApplicationDbContext dbContext)
    {
        _candidatoService = candidatoService;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Asignar(Guid evaluacionId, [FromBody] AsignarCandidatoDto dto)
    {
        var evaluadorId = ObtenerUsuarioId();
        var candidato = await _candidatoService.AsignarAsync(evaluacionId, dto, evaluadorId);
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

    [HttpGet("usuarios-candidatos")]
    public async Task<IActionResult> ListarUsuariosCandidatos()
    {
        var usuarios = await _dbContext.Usuarios
            .Where(u => u.Rol == RolUsuario.Candidato && u.EstaActivo)
            .AsNoTracking()
            .OrderBy(u => u.NombreCompleto)
            .Select(u => new UsuarioResumenDto
            {
                Id = u.Id,
                NombreCompleto = u.NombreCompleto,
                Email = u.Email
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    private Guid ObtenerUsuarioId()
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Token de usuario inválido."));
}

[ApiController]
[Route("api/mis-evaluaciones")]
[Authorize(Roles = "Candidato")]
public class MisEvaluacionesController : ControllerBase
{
    private readonly ICandidatoService _candidatoService;

    public MisEvaluacionesController(ICandidatoService candidatoService)
    {
        _candidatoService = candidatoService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var usuarioId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Token inválido."));
        var evaluaciones = await _candidatoService.ListarEvaluacionesPorUsuarioAsync(usuarioId);
        return Ok(evaluaciones);
    }
}

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
