using CRUD_18_03.Application.DTOs.ResultadoEvaluacion;
using CRUD_18_03.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CRUD_18_03.Infrastructure.PDF;

public class GeneradorPDFQuestPDF : IGeneradorPDF
{
    public byte[] GenerarReporteCandidato(ResultadoCompletoDto resultado)
    {
        return Document.Create(contenedor =>
        {
            contenedor.Page(pagina =>
            {
                pagina.Size(PageSizes.Letter);
                pagina.Margin(40);
                pagina.DefaultTextStyle(x => x.FontSize(10));

                pagina.Header().Element(c => ConstruirEncabezado(c, resultado));
                pagina.Content().Element(c => ConstruirContenido(c, resultado));
                pagina.Footer().Element(ConstruirPie);
            });
        }).GeneratePdf();
    }

    private static void ConstruirEncabezado(IContainer container, ResultadoCompletoDto r)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("EvalTech IA").FontSize(10).FontColor("#3b82f6").Bold();
                    c.Item().PaddingTop(2).Text("Reporte de Evaluación Técnica").FontSize(20).Bold().FontColor("#1e293b");
                    c.Item().PaddingTop(4).Text($"Candidato: {r.CandidatoNombre}").FontSize(13).SemiBold().FontColor("#334155");
                    c.Item().PaddingTop(2).Text($"{r.Tecnologia} · Nivel {r.Nivel}").FontSize(10).FontColor("#64748b");
                });
                row.ConstantItem(80).AlignRight().AlignMiddle().Column(c =>
                {
                    c.Item().Text(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(9).FontColor("#94a3b8");
                });
            });
            col.Item().PaddingTop(8).LineHorizontal(2).LineColor("#3b82f6");
        });
    }

    private static void ConstruirContenido(IContainer container, ResultadoCompletoDto r)
    {
        container.PaddingTop(10).Column(col =>
        {
            ConstruirSeccionScore(col, r);
            ConstruirSeccionResumen(col, r);
            ConstruirSeccionFortalezasYBrechas(col, r);
            ConstruirSeccionDetalle(col, r);
        });
    }

    private static void ConstruirSeccionScore(ColumnDescriptor col, ResultadoCompletoDto r)
    {
        col.Item().PaddingBottom(12).Row(row =>
        {
            row.RelativeItem().Border(1).BorderColor("#e2e8f0").Background("#f8fafc").Padding(16).Column(c =>
            {
                c.Item().Text("SCORE TOTAL").FontSize(8).Bold().FontColor("#64748b").LetterSpacing(0.05f);
                c.Item().PaddingTop(4).Text($"{r.ScoreTotal:F1}").FontSize(28).Bold().FontColor(ObtenerColorFondo(r.ScoreTotal));
                c.Item().Text("de 100 puntos").FontSize(8).FontColor("#94a3b8");
            });

            row.ConstantItem(8);

            row.RelativeItem().Border(1).BorderColor("#e2e8f0").Background("#f8fafc").Padding(16).Column(c =>
            {
                c.Item().Text("RECOMENDACIÓN").FontSize(8).Bold().FontColor("#64748b").LetterSpacing(0.05f);
                c.Item().PaddingTop(4).Text(r.Recomendacion).FontSize(16).Bold().FontColor("#1e293b");
                c.Item().Text("según análisis IA").FontSize(8).FontColor("#94a3b8");
            });

            row.ConstantItem(8);

            row.RelativeItem().Border(1).BorderColor("#e2e8f0").Background("#f8fafc").Padding(16).Column(c =>
            {
                c.Item().Text("TIEMPO INVERTIDO").FontSize(8).Bold().FontColor("#64748b").LetterSpacing(0.05f);
                c.Item().PaddingTop(4).Text(r.TiempoInvertido ?? "—").FontSize(16).Bold().FontColor("#1e293b");
                c.Item().Text(r.GeneradoEn.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor("#94a3b8");
            });
        });
    }

    private static void ConstruirSeccionResumen(ColumnDescriptor col, ResultadoCompletoDto r)
    {
        if (string.IsNullOrWhiteSpace(r.ResumenIA)) return;
        col.Item().PaddingTop(12).Border(1).BorderColor("#e2e8f0").Padding(16).Column(c =>
        {
            c.Item().Text("RESUMEN IA").FontSize(9).Bold().FontColor("#3b82f6").LetterSpacing(0.05f);
            c.Item().PaddingTop(6).Text(r.ResumenIA).FontSize(10).FontColor("#334155").LineHeight(1.5f);
        });
    }

    private static void ConstruirSeccionFortalezasYBrechas(ColumnDescriptor col, ResultadoCompletoDto r)
    {
        col.Item().PaddingTop(12).Row(row =>
        {
            row.RelativeItem().PaddingRight(6).Border(1).BorderColor("#e2e8f0").Padding(14).Column(c =>
            {
                c.Item().Text("FORTALEZAS").FontSize(9).Bold().FontColor("#059669").LetterSpacing(0.05f);
                c.Item().PaddingTop(6);
                foreach (var f in SplitLista(r.FortalezasDetectadas))
                    c.Item().PaddingTop(3).Text($"✓  {f}").FontSize(9).FontColor("#334155");
                if (SplitLista(r.FortalezasDetectadas).Count == 0)
                    c.Item().PaddingTop(3).Text("Sin fortalezas identificadas").FontSize(9).FontColor("#94a3b8").Italic();
            });

            row.RelativeItem().PaddingLeft(6).Border(1).BorderColor("#e2e8f0").Padding(14).Column(c =>
            {
                c.Item().Text("BRECHAS DETECTADAS").FontSize(9).Bold().FontColor("#dc2626").LetterSpacing(0.05f);
                c.Item().PaddingTop(6);
                foreach (var b in SplitLista(r.BrechasDetectadas))
                    c.Item().PaddingTop(3).Text($"▸  {b}").FontSize(9).FontColor("#334155");
                if (SplitLista(r.BrechasDetectadas).Count == 0)
                    c.Item().PaddingTop(3).Text("Sin brechas detectadas").FontSize(9).FontColor("#94a3b8").Italic();
            });
        });
    }

    private static void ConstruirSeccionDetalle(ColumnDescriptor col, ResultadoCompletoDto r)
    {
        col.Item().PaddingTop(16).Text("DETALLE POR PREGUNTA").FontSize(11).Bold().FontColor("#1e293b").LetterSpacing(0.03f);
        col.Item().PaddingTop(4).LineHorizontal(1).LineColor("#e2e8f0");

        foreach (var resp in r.Respuestas.OrderBy(x => x.OrdenEnEvaluacion))
        {
            col.Item().PaddingTop(10).Border(1).BorderColor("#e2e8f0").Column(pregCol =>
            {
                // Header con número y score
                pregCol.Item().Background("#f8fafc").Padding(12).Row(headerRow =>
                {
                    headerRow.ConstantItem(24).AlignCenter()
                        .Text(resp.OrdenEnEvaluacion.ToString()).FontSize(10).Bold().FontColor("#3b82f6");
                    headerRow.ConstantItem(6);
                    headerRow.RelativeItem()
                        .Text(resp.TextoPregunta)
                        .FontSize(10).SemiBold().FontColor("#1e293b");
                    headerRow.ConstantItem(80).AlignRight()
                        .Text($"{resp.ScoreIA?.ToString("F1") ?? "—"} / {resp.PuntajeMaximo}")
                        .FontSize(10).Bold().FontColor(ObtenerColorScore(resp.ScoreIA, resp.PuntajeMaximo));
                });

                // Respuesta
                pregCol.Item().Padding(12).Column(bodyCol =>
                {
                    bodyCol.Item().Text("RESPUESTA DEL CANDIDATO").FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);
                    bodyCol.Item().PaddingTop(4).Text(resp.ContenidoRespuesta ?? "(sin respuesta)")
                        .FontSize(9).FontColor("#334155").LineHeight(1.4f);

                    if (!string.IsNullOrEmpty(resp.FeedbackIA))
                    {
                        bodyCol.Item().PaddingTop(8).Background("#eff6ff").Padding(10).Column(fbCol =>
                        {
                            fbCol.Item().Text("FEEDBACK IA").FontSize(7).Bold().FontColor("#3b82f6").LetterSpacing(0.05f);
                            fbCol.Item().PaddingTop(3).Text(resp.FeedbackIA)
                                .FontSize(9).FontColor("#1e40af").LineHeight(1.4f);
                        });
                    }

                    if (!string.IsNullOrEmpty(resp.BrechasIdentificadas))
                    {
                        bodyCol.Item().PaddingTop(6).Text($"Brechas: {resp.BrechasIdentificadas}")
                            .FontSize(8).FontColor("#dc2626");
                    }

                    if (resp.TiempoUsadoSegundos.HasValue)
                    {
                        bodyCol.Item().PaddingTop(4).Text($"⏱ {resp.TiempoUsadoSegundos}s")
                            .FontSize(8).FontColor("#94a3b8");
                    }
                });
            });
        }
    }

    private static void ConstruirPie(IContainer container)
    {
        container.Column(c =>
        {
            c.Item().LineHorizontal(1).LineColor("#e2e8f0");
            c.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text(t =>
                {
                    t.Span("EvalTech IA — Generado el ").FontSize(8).FontColor("#94a3b8");
                    t.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor("#64748b");
                });

                row.RelativeItem().AlignRight().Text(t =>
                {
                    t.Span("Página ").FontSize(8).FontColor("#94a3b8");
                    t.CurrentPageNumber().FontSize(8).FontColor("#64748b");
                    t.Span(" de ").FontSize(8).FontColor("#94a3b8");
                    t.TotalPages().FontSize(8).FontColor("#64748b");
                });
            });
        });
    }

    public byte[] GenerarReporteRanking(RankingEvaluacionDto ranking)
    {
        return Document.Create(contenedor =>
        {
            contenedor.Page(pagina =>
            {
                pagina.Size(PageSizes.Letter);
                pagina.Margin(40);
                pagina.DefaultTextStyle(x => x.FontSize(10));

                pagina.Header().Column(col =>
                {
                    col.Item().Text("Ranking de Candidatos").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                    col.Item().PaddingTop(3).Text($"{ranking.Titulo} — {ranking.Tecnologia} ({ranking.Nivel})")
                        .FontSize(12).FontColor(Colors.Grey.Darken1);
                    col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                });

                pagina.Content().PaddingTop(10).Column(col =>
                {
                    col.Item().PaddingBottom(10).Row(row =>
                    {
                        row.RelativeItem().Text($"Total analizados: {ranking.TotalAnalizados}").FontSize(10);
                        row.RelativeItem().Text($"Score promedio: {ranking.ScorePromedio?.ToString("F1") ?? "—"}").FontSize(10);
                        row.RelativeItem().Text($"Máx: {ranking.ScoreMaximo?.ToString("F1") ?? "—"} | Mín: {ranking.ScoreMinimo?.ToString("F1") ?? "—"}").FontSize(10);
                    });

                    col.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(40);
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(3);
                            cols.ConstantColumn(60);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                        });

                        tabla.Header(header =>
                        {
                            var estiloCabecera = TextStyle.Default.FontSize(9).Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("#").Style(estiloCabecera);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Nombre").Style(estiloCabecera);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Email").Style(estiloCabecera);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Score").Style(estiloCabecera);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Recomendación").Style(estiloCabecera);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Tiempo").Style(estiloCabecera);
                        });

                        foreach (var item in ranking.Ranking)
                        {
                            var bgColor = item.Posicion % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                            tabla.Cell().Background(bgColor).Padding(5).Text(item.Posicion.ToString()).FontSize(9);
                            tabla.Cell().Background(bgColor).Padding(5).Text(item.Nombre).FontSize(9);
                            tabla.Cell().Background(bgColor).Padding(5).Text(item.Email).FontSize(9);
                            tabla.Cell().Background(bgColor).Padding(5)
                                .Text($"{item.ScoreTotal:F1}").FontSize(9).Bold()
                                .FontColor(ObtenerColorFondo(item.ScoreTotal));
                            tabla.Cell().Background(bgColor).Padding(5).Text(item.Recomendacion).FontSize(8);
                            tabla.Cell().Background(bgColor).Padding(5).Text(item.TiempoInvertido ?? "—").FontSize(9);
                        }
                    });
                });

                pagina.Footer().Element(ConstruirPie);
            });
        }).GeneratePdf();
    }

    private static string ObtenerColorFondo(decimal score) =>
        score >= 70 ? "#2E7D32" : score >= 50 ? "#F57F17" : "#C62828";

    private static string ObtenerColorScore(decimal? score, int maximo)
    {
        if (!score.HasValue) return Colors.Grey.Medium;
        var porcentaje = score.Value / maximo * 100;
        return porcentaje >= 70 ? "#2E7D32" : porcentaje >= 50 ? "#F57F17" : "#C62828";
    }

    private static List<string> SplitLista(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return new List<string>();

        return texto.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }
}
