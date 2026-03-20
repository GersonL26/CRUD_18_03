import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';
import { EvaluacionCandidatoDto, EnviarRespuestasDto } from '../../models/candidato.model';
import { ResultadoCandidatoPublicoDto } from '../../models/resultado.model';

@Injectable({
  providedIn: 'root',
})
export class PruebaService {
  private url = `${environment.apiUrl}/prueba`;

  constructor(private http: HttpClient) {}

  obtenerPorToken(token: string) {
    return this.http.get<EvaluacionCandidatoDto>(`${this.url}/${token}`);
  }

  iniciar(token: string) {
    return this.http.post<{ message: string }>(`${this.url}/${token}/iniciar`, {});
  }

  enviarRespuestas(token: string, dto: EnviarRespuestasDto) {
    return this.http.post<{ message: string }>(`${this.url}/${token}/respuestas`, dto);
  }

  obtenerResultado(token: string) {
    return this.http.get<ResultadoCandidatoPublicoDto>(`${this.url}/${token}/resultado`);
  }

  reportarPerdidaFoco(token: string) {
    return this.http.post<void>(`${this.url}/${token}/focus-lost`, {});
  }

  registrarEventoProctoring(token: string, tipo: string, detalle?: string) {
    return this.http.post<void>(`${this.url}/${token}/proctoring`, { tipo, detalle });
  }

  enviarAudio(token: string, audio: Blob) {
    const formData = new FormData();
    formData.append('audio', audio, 'audio.webm');
    return this.http.post<{ transcripcion: string }>(`${this.url}/${token}/audio`, formData);
  }
}
