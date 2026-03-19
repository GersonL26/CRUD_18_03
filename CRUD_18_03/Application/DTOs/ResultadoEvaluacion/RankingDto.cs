namespace CRUD_18_03.Application.DTOs.ResultadoEvaluacion;

public class RankingItemDto
{
    public int Posicion { get; set; }
    public Guid CandidatoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public double ScoreTotal { get; set; }
    public string Recomendacion { get; set; } = string.Empty;
    public string? TiempoInvertido { get; set; }
    public DateTime FechaAnalisis { get; set; }
}

public class RankingEvaluacionDto
{
    public Guid EvaluacionId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Tecnologia { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public int TotalAnalizados { get; set; }
    public double? ScorePromedio { get; set; }
    public double? ScoreMaximo { get; set; }
    public double? ScoreMinimo { get; set; }
    public List<RankingItemDto> Ranking { get; set; } = new();
}

public class ComparacionCandidatosDto
{
    public string Tecnologia { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string TituloEvaluacion { get; set; } = string.Empty;
    public List<CandidatoComparadoDto> Candidatos { get; set; } = new();
}

public class CandidatoComparadoDto
{
    public Guid CandidatoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public double ScoreTotal { get; set; }
    public string Recomendacion { get; set; } = string.Empty;
    public string? TiempoInvertido { get; set; }
    public string ResumenIA { get; set; } = string.Empty;
    public List<string> Fortalezas { get; set; } = new();
    public List<string> Brechas { get; set; } = new();
    public List<ScorePorPreguntaDto> ScoresPorPregunta { get; set; } = new();
}

public class ScorePorPreguntaDto
{
    public int OrdenEnEvaluacion { get; set; }
    public string TextoPregunta { get; set; } = string.Empty;
    public int PuntajeMaximo { get; set; }
    public double? ScoreIA { get; set; }
}
