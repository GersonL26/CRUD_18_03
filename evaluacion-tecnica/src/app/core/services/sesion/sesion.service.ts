import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';
import {
  EstadoSesionDto,
  PreguntaEnVivoDto,
  CrearSesionEnVivoDto,
  ResponderPreguntaEnVivoDto
} from '../../models/sesion.model';

@Injectable({ providedIn: 'root' })
export class SesionService {
  private url = `${environment.apiUrl}/sesiones`;
  private vivoUrl = `${environment.apiUrl}/sesion-vivo`;

  constructor(private http: HttpClient) {}

  crear(dto: CrearSesionEnVivoDto) {
    return this.http.post<EstadoSesionDto>(this.url, dto);
  }

  obtenerEstado(sesionId: string) {
    return this.http.get<EstadoSesionDto>(`${this.url}/${sesionId}`);
  }

  iniciar(sesionId: string, token: string) {
    return this.http.post<PreguntaEnVivoDto>(
      `${this.vivoUrl}/${sesionId}/iniciar`,
      {},
      { headers: { 'X-Candidato-Token': token } }
    );
  }

  obtenerPreguntaActual(sesionId: string, token: string) {
    return this.http.get<PreguntaEnVivoDto>(
      `${this.vivoUrl}/${sesionId}/pregunta-actual`,
      { headers: { 'X-Candidato-Token': token } }
    );
  }

  responder(sesionId: string, dto: ResponderPreguntaEnVivoDto, token: string) {
    return this.http.post<PreguntaEnVivoDto | null>(
      `${this.vivoUrl}/${sesionId}/responder`,
      dto,
      { headers: { 'X-Candidato-Token': token } }
    );
  }

  finalizar(sesionId: string, token: string) {
    return this.http.post(
      `${this.vivoUrl}/${sesionId}/finalizar`,
      {},
      { headers: { 'X-Candidato-Token': token } }
    );
  }

  subirAudio(sesionId: string, preguntaId: string, audioBlob: Blob, token: string) {
    const formData = new FormData();
    formData.append('audio', audioBlob, 'grabacion.webm');
    return this.http.post(
      `${this.vivoUrl}/${sesionId}/audio/${preguntaId}`,
      formData,
      { headers: { 'X-Candidato-Token': token } }
    );
  }

  // Evaluator-driven (in-person interview)
  iniciarPorEvaluador(sesionId: string) {
    return this.http.post<PreguntaEnVivoDto>(`${this.url}/${sesionId}/iniciar`, {});
  }

  obtenerPreguntaActualEvaluador(sesionId: string) {
    return this.http.get<PreguntaEnVivoDto>(`${this.url}/${sesionId}/pregunta-actual`);
  }

  responderPorEvaluador(sesionId: string, dto: ResponderPreguntaEnVivoDto) {
    return this.http.post<PreguntaEnVivoDto | { message: string; completada: boolean }>(
      `${this.url}/${sesionId}/responder`, dto
    );
  }

  transcribir(sesionId: string, audio: Blob) {
    const formData = new FormData();
    formData.append('audio', audio, 'grabacion.webm');
    return this.http.post<{ transcripcion: string }>(`${this.url}/${sesionId}/transcribir`, formData);
  }

  finalizarPorEvaluador(sesionId: string) {
    return this.http.post<{ message: string }>(`${this.url}/${sesionId}/finalizar`, {});
  }

  cancelarPorEvaluador(sesionId: string) {
    return this.http.post<{ message: string }>(`${this.url}/${sesionId}/cancelar`, {});
  }
}
