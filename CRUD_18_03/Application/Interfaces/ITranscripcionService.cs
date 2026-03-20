namespace CRUD_18_03.Application.Interfaces;

public interface ITranscripcionService
{
    Task<string> TranscribirAudioAsync(Stream audioStream, string nombreArchivo);
}
