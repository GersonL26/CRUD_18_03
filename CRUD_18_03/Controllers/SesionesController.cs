using System.Security.Claims;
using CRUD_18_03.Application.DTOs.SesionEnVivo;
using CRUD_18_03.Application.Interfaces;
using CRUD_18_03.Infrastructure.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CRUD_18_03.Controllers;

[ApiController]
[Route("api/sesiones")]
[Authorize(Roles = "Admin,Evaluador")]
public class SesionesController : ControllerBase
{
    private readonly ISesionEnVivoService _sesionService;
    private readonly IHubContext<SesionHub> _hubContext;
    private readonly ITranscripcionService _transcripcionService;

    public SesionesController(
        ISesionEnVivoService sesionService,
        IHubContext<SesionHub> hubContext,
        ITranscripcionService transcripcionService)
    {
        _sesionService = sesionService;
        _hubContext = hubContext;
        _transcripcionService = transcripcionService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearSesion([FromBody] CrearSesionEnVivoDto dto)
    {
        var evaluadorId = ObtenerUsuarioId();
        var estado = await _sesionService.CrearSesionAsync(dto.EvaluacionId, dto.CandidatoId, evaluadorId);
        return StatusCode(201, estado);
    }

    [HttpGet("{sesionId:guid}")]
    public async Task<IActionResult> ObtenerEstado(Guid sesionId)
    {
        var estado = await _sesionService.ObtenerEstadoAsync(sesionId);
        return Ok(estado);
    }

    [HttpPost("{sesionId:guid}/iniciar")]
    public async Task<IActionResult> IniciarSesion(Guid sesionId)
    {
        var evaluadorId = ObtenerUsuarioId();
        var pregunta = await _sesionService.IniciarSesionPorEvaluadorAsync(sesionId, evaluadorId);
        return Ok(pregunta);
    }

    [HttpGet("{sesionId:guid}/pregunta-actual")]
    public async Task<IActionResult> ObtenerPreguntaActual(Guid sesionId)
    {
        var evaluadorId = ObtenerUsuarioId();
        var pregunta = await _sesionService.ObtenerPreguntaActualPorEvaluadorAsync(sesionId, evaluadorId);
        return Ok(pregunta);
    }

    [HttpPost("{sesionId:guid}/responder")]
    public async Task<IActionResult> Responder(Guid sesionId, [FromBody] ResponderPreguntaEnVivoDto dto)
    {
        var evaluadorId = ObtenerUsuarioId();
        var siguiente = await _sesionService.ResponderYAvanzarPorEvaluadorAsync(sesionId, evaluadorId, dto);

        if (siguiente is null)
            return Ok(new { message = "Sesión completada. Todas las preguntas fueron respondidas.", completada = true });

        return Ok(siguiente);
    }

    [HttpPost("{sesionId:guid}/transcribir")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Transcribir(Guid sesionId, IFormFile audio)
    {
        using var stream = audio.OpenReadStream();
        var transcripcion = await _transcripcionService.TranscribirAudioAsync(stream, audio.FileName);
        return Ok(new { transcripcion });
    }

    [HttpPost("{sesionId:guid}/finalizar")]
    public async Task<IActionResult> FinalizarSesion(Guid sesionId)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _sesionService.FinalizarSesionPorEvaluadorAsync(sesionId, evaluadorId);
        return Ok(new { message = "Sesión finalizada." });
    }

    [HttpPost("{sesionId:guid}/cancelar")]
    public async Task<IActionResult> CancelarSesion(Guid sesionId)
    {
        var evaluadorId = ObtenerUsuarioId();
        await _sesionService.CancelarSesionPorEvaluadorAsync(sesionId, evaluadorId);
        return Ok(new { message = "Sesión cancelada." });
    }

    private Guid ObtenerUsuarioId()
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Token de usuario inválido."));
}

[ApiController]
[Route("api/sesion-vivo")]
[AllowAnonymous]
public class SesionVivoPublicaController : ControllerBase
{
    private readonly ISesionEnVivoService _sesionService;
    private readonly IHubContext<SesionHub> _hubContext;

    public SesionVivoPublicaController(ISesionEnVivoService sesionService, IHubContext<SesionHub> hubContext)
    {
        _sesionService = sesionService;
        _hubContext = hubContext;
    }

    [HttpPost("{sesionId:guid}/iniciar")]
    public async Task<IActionResult> Iniciar(Guid sesionId, [FromHeader(Name = "X-Candidato-Token")] string token)
    {
        var pregunta = await _sesionService.IniciarSesionAsync(sesionId, token);

        await _hubContext.Clients.Group($"sesion-{sesionId}")
            .SendAsync("SesionIniciada", pregunta);

        return Ok(pregunta);
    }

    [HttpGet("{sesionId:guid}/pregunta-actual")]
    public async Task<IActionResult> ObtenerPreguntaActual(
        Guid sesionId, [FromHeader(Name = "X-Candidato-Token")] string token)
    {
        var pregunta = await _sesionService.ObtenerPreguntaActualAsync(sesionId, token);
        return Ok(pregunta);
    }

    [HttpPost("{sesionId:guid}/responder")]
    public async Task<IActionResult> Responder(
        Guid sesionId,
        [FromHeader(Name = "X-Candidato-Token")] string token,
        [FromBody] ResponderPreguntaEnVivoDto dto)
    {
        var siguientePregunta = await _sesionService.ResponderYAvanzarAsync(sesionId, token, dto);

        if (siguientePregunta is null)
        {
            await _hubContext.Clients.Group($"sesion-{sesionId}")
                .SendAsync("SesionCompletada");

            return Ok(new { message = "Sesión completada. Todas las preguntas fueron respondidas." });
        }

        await _hubContext.Clients.Group($"sesion-{sesionId}")
            .SendAsync("PreguntaCambiada", siguientePregunta);

        return Ok(siguientePregunta);
    }

    [HttpPost("{sesionId:guid}/finalizar")]
    public async Task<IActionResult> Finalizar(
        Guid sesionId, [FromHeader(Name = "X-Candidato-Token")] string token)
    {
        await _sesionService.FinalizarSesionAsync(sesionId, token);

        await _hubContext.Clients.Group($"sesion-{sesionId}")
            .SendAsync("SesionCompletada");

        return Ok(new { message = "Sesión finalizada." });
    }

    [HttpPost("{sesionId:guid}/audio/{preguntaId:guid}")]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> SubirAudio(
        Guid sesionId,
        Guid preguntaId,
        [FromHeader(Name = "X-Candidato-Token")] string token,
        IFormFile audio)
    {
        await _sesionService.GuardarAudioRespuestaAsync(sesionId, token, preguntaId, audio);
        return Ok(new { message = "Audio guardado correctamente." });
    }
}
