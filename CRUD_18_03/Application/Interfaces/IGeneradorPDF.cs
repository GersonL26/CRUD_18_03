using CRUD_18_03.Application.DTOs.ResultadoEvaluacion;

namespace CRUD_18_03.Application.Interfaces;

public interface IGeneradorPDF
{
    byte[] GenerarReporteCandidato(ResultadoCompletoDto resultado);
}
