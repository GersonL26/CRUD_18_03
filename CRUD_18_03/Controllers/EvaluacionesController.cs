using System.Security.Claims;
using CRUD_18_03.Application.DTOs.Evaluacion;
using CRUD_18_03.Application.DTOs.Pregunta;
using CRUD_18_03.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_18_03.Controllers;

[ApiController]
[Route("api/evaluaciones")]
[Authorize(Roles = "Admin,Evaluador")]
public class EvaluacionesController : ControllerBase
{
    private readonly IEvaluacionService _evaluacionService;

    public EvaluacionesController(IEvaluacionService evaluacionService)
    {
        _evaluacionService = evaluacionService;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEvaluacionConPreguntasDto dto)
    {
        var evaluadorId = ObtenerUsuarioId();
        var resultado = await _evaluacionService.CrearAsync(dto, evaluadorId);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        // Admin ve todas; Evaluador solo las propias
        var evaluadorId = User.IsInRole("Admin") ? Guid.Empty : ObtenerUsuarioId();
        var evaluaciones = await _evaluacionService.ListarPorEvaluadorAsync(evaluadorId);
        return Ok(evaluaciones);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var evaluadorId = ObtenerUsuarioId();
        var evaluacion = await _evaluacionService.ObtenerConPreguntasAsync(id, evaluadorId);

        if (evaluacion is null)
            return NotFound(new { message = $"Evaluación con ID '{id}' no encontrada." });

        return Ok(evaluacion);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarEvaluacionDto dto)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _evaluacionService.ActualizarAsync(id, dto, evaluadorId);
        return Ok(new { message = "Evaluación actualizada correctamente." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _evaluacionService.EliminarAsync(id, evaluadorId);
        return Ok(new { message = "Evaluación eliminada correctamente." });
    }

    [HttpPost("{id:guid}/activar")]
    public async Task<IActionResult> Activar(Guid id)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _evaluacionService.ActivarAsync(id, evaluadorId);
        return Ok(new { message = "Evaluación activada correctamente." });
    }

    [HttpPost("{id:guid}/cerrar")]
    public async Task<IActionResult> Cerrar(Guid id)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _evaluacionService.CerrarAsync(id, evaluadorId);
        return Ok(new { message = "Evaluación cerrada correctamente." });
    }

    [HttpPost("{id:guid}/reactivar")]
    public async Task<IActionResult> Reactivar(Guid id)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _evaluacionService.ReactivarAsync(id, evaluadorId);
        return Ok(new { message = "Evaluación reactivada correctamente." });
    }

    // --- Preguntas dentro de evaluación ---

    [HttpPost("{evaluacionId:guid}/preguntas")]
    public async Task<IActionResult> AgregarPregunta(Guid evaluacionId, [FromBody] AgregarPreguntaDto dto)
    {
        var evaluadorId = ObtenerUsuarioId();
        var pregunta = await _evaluacionService.AgregarPreguntaAsync(evaluacionId, dto, evaluadorId);
        return StatusCode(201, pregunta);
    }

    [HttpPut("{evaluacionId:guid}/preguntas/{preguntaId:guid}")]
    public async Task<IActionResult> ActualizarPregunta(Guid evaluacionId, Guid preguntaId, [FromBody] ActualizarPreguntaDto dto)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _evaluacionService.ActualizarPreguntaAsync(evaluacionId, preguntaId, dto, evaluadorId);
        return Ok(new { message = "Pregunta actualizada correctamente." });
    }

    [HttpDelete("{evaluacionId:guid}/preguntas/{preguntaId:guid}")]
    public async Task<IActionResult> EliminarPregunta(Guid evaluacionId, Guid preguntaId)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _evaluacionService.EliminarPreguntaAsync(evaluacionId, preguntaId, evaluadorId);
        return Ok(new { message = "Pregunta eliminada correctamente." });
    }

    // --- Helper ---

    private Guid ObtenerUsuarioId()
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Token de usuario inválido."));
}
