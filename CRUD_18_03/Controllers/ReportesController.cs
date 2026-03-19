using System.Security.Claims;
using CRUD_18_03.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_18_03.Controllers;

[ApiController]
[Route("api/reportes")]
[Authorize(Roles = "Admin,Evaluador")]
public class ReportesController : ControllerBase
{
    private readonly IResultadoService _resultadoService;
    private readonly IGeneradorPDF _generadorPDF;

    public ReportesController(IResultadoService resultadoService, IGeneradorPDF generadorPDF)
    {
        _resultadoService = resultadoService;
        _generadorPDF = generadorPDF;
    }

    [HttpGet("{candidatoId:guid}/pdf")]
    public async Task<IActionResult> DescargarPDF(Guid candidatoId)
    {
        var evaluadorId = ObtenerUsuarioId();
        var resultado = await _resultadoService.ObtenerResultadoAsync(candidatoId, evaluadorId);

        if (resultado is null)
            return NotFound(new { message = "El análisis aún no ha sido generado para este candidato." });

        var pdfBytes = _generadorPDF.GenerarReporteCandidato(resultado);
        var nombreArchivo = $"reporte-{resultado.CandidatoNombre.Replace(" ", "_")}.pdf";

        return File(pdfBytes, "application/pdf", nombreArchivo);
    }

    private Guid ObtenerUsuarioId()
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Token de usuario inválido."));
}
