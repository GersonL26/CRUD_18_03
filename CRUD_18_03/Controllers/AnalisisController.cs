using System.Security.Claims;
using CRUD_18_03.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_18_03.Controllers;

[ApiController]
[Route("api/analisis")]
[Authorize(Roles = "Admin,Evaluador")]
public class AnalisisController : ControllerBase
{
    private readonly IAnalisisService _analisisService;

    public AnalisisController(IAnalisisService analisisService)
    {
        _analisisService = analisisService;
    }

    [HttpPost("{candidatoId:guid}")]
    public async Task<IActionResult> EjecutarAnalisis(Guid candidatoId)
    {
        var evaluadorId = ObtenerUsuarioId();
        var resultado = await _analisisService.EjecutarAnalisisAsync(candidatoId, evaluadorId);
        return StatusCode(201, resultado);
    }

    private Guid ObtenerUsuarioId()
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Token de usuario inválido."));
}
