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
            col.Item().Text("Reporte de Evaluación Técnica").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
            col.Item().PaddingTop(5).Text($"Candidato: {r.CandidatoNombre}").FontSize(14).SemiBold();
            col.Item().Text($"{r.Tecnologia} — Nivel {r.Nivel}").FontSize(11).FontColor(Colors.Grey.Darken1);
            col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
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
        col.Item().PaddingBottom(10).Background(ObtenerColorFondo(r.ScoreTotal)).Padding(15).Row(row =>
        {
            row.RelativeItem().Column(c =>
            {
                c.Item().Text("Score Total").FontSize(11).FontColor(Colors.White);
                c.Item().Text($"{r.ScoreTotal:F1} / 100").FontSize(24).Bold().FontColor(Colors.White);
            });

            row.RelativeItem().AlignRight().Column(c =>
            {
                c.Item().Text("Recomendación").FontSize(11).FontColor(Colors.White);
                c.Item().Text(r.Recomendacion).FontSize(16).Bold().FontColor(Colors.White);
            });
        });

        if (!string.IsNullOrEmpty(r.TiempoInvertido))
        {
            col.Item().PaddingBottom(5).Text($"Tiempo invertido: {r.TiempoInvertido}")
                .FontSize(9).FontColor(Colors.Grey.Darken1);
        }
    }

    private static void ConstruirSeccionResumen(ColumnDescriptor col, ResultadoCompletoDto r)
    {
        col.Item().PaddingTop(10).Text("Análisis de la IA").FontSize(13).Bold().FontColor(Colors.Blue.Darken2);
        col.Item().PaddingTop(3).PaddingBottom(10).Text(r.ResumenIA).FontSize(10).LineHeight(1.4f);
    }

    private static void ConstruirSeccionFortalezasYBrechas(ColumnDescriptor col, ResultadoCompletoDto r)
    {
        col.Item().Row(row =>
        {
            row.RelativeItem().PaddingRight(10).Column(c =>
            {
                c.Item().Text("Fortalezas").FontSize(12).Bold().FontColor("#2E7D32");
                foreach (var f in SplitLista(r.FortalezasDetectadas))
                    c.Item().PaddingTop(2).Text($"• {f}").FontSize(9);
            });

            row.RelativeItem().Column(c =>
            {
                c.Item().Text("Brechas detectadas").FontSize(12).Bold().FontColor("#C62828");
                foreach (var b in SplitLista(r.BrechasDetectadas))
                    c.Item().PaddingTop(2).Text($"• {b}").FontSize(9);
            });
        });
    }

    private static void ConstruirSeccionDetalle(ColumnDescriptor col, ResultadoCompletoDto r)
    {
        col.Item().PaddingTop(15).Text("Detalle por pregunta").FontSize(13).Bold().FontColor(Colors.Blue.Darken2);
        col.Item().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);

        foreach (var resp in r.Respuestas.OrderBy(x => x.OrdenEnEvaluacion))
        {
            col.Item().PaddingTop(10).Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(pregCol =>
            {
                pregCol.Item().Row(headerRow =>
                {
                    headerRow.RelativeItem()
                        .Text($"Pregunta {resp.OrdenEnEvaluacion}: {resp.TextoPregunta}")
                        .FontSize(10).SemiBold();
                    headerRow.ConstantItem(80).AlignRight()
                        .Text($"{resp.ScoreIA?.ToString("F1") ?? "—"} / {resp.PuntajeMaximo}")
                        .FontSize(10).Bold().FontColor(ObtenerColorScore(resp.ScoreIA, resp.PuntajeMaximo));
                });

                pregCol.Item().PaddingTop(5).Text("Respuesta:").FontSize(8).FontColor(Colors.Grey.Darken1);
                pregCol.Item().Text(resp.ContenidoRespuesta).FontSize(9).LineHeight(1.3f);

                if (!string.IsNullOrEmpty(resp.FeedbackIA))
                {
                    pregCol.Item().PaddingTop(5).Text("Feedback IA:").FontSize(8).FontColor(Colors.Blue.Darken1);
                    pregCol.Item().Text(resp.FeedbackIA).FontSize(9).Italic().LineHeight(1.3f);
                }

                if (!string.IsNullOrEmpty(resp.BrechasIdentificadas))
                {
                    pregCol.Item().PaddingTop(3).Text($"Brechas: {resp.BrechasIdentificadas}")
                        .FontSize(8).FontColor("#C62828");
                }

                if (resp.TiempoUsadoSegundos.HasValue)
                {
                    pregCol.Item().PaddingTop(3).Text($"Tiempo: {resp.TiempoUsadoSegundos}s")
                        .FontSize(8).FontColor(Colors.Grey.Darken1);
                }
            });
        }
    }

    private static void ConstruirPie(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text(t =>
            {
                t.Span("Generado el ").FontSize(8).FontColor(Colors.Grey.Medium);
                t.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
            });

            row.RelativeItem().AlignRight().Text(t =>
            {
                t.Span("Página ").FontSize(8).FontColor(Colors.Grey.Medium);
                t.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                t.Span(" de ").FontSize(8).FontColor(Colors.Grey.Medium);
                t.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
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

    private static string ObtenerColorFondo(double score) =>
        score >= 70 ? "#2E7D32" : score >= 50 ? "#F57F17" : "#C62828";

    private static string ObtenerColorScore(double? score, int maximo)
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
