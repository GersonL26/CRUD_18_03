using System.Net.Http.Headers;
using System.Text.Json;
using CRUD_18_03.Application.Interfaces;

namespace CRUD_18_03.Infrastructure.AI;

public class TranscripcionWhisperService : ITranscripcionService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    public TranscripcionWhisperService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"];
    }

    public async Task<string> TranscribirAudioAsync(Stream audioStream, string nombreArchivo)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return "[Whisper API key no configurada — audio recibido pero no transcrito]";

        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(audioStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/webm");
        content.Add(streamContent, "file", nombreArchivo);
        content.Add(new StringContent("whisper-1"), "model");
        content.Add(new StringContent("es"), "language");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsync(
            "https://api.openai.com/v1/audio/transcriptions", content);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return $"[Error Whisper: {response.StatusCode}]";

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("text").GetString() ?? string.Empty;
    }
}
