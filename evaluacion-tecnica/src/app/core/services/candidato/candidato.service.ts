import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';
import { AsignarCandidatoDto, CandidatoDto, EvaluacionAsignadaDto, RespuestaCrudaDto, UsuarioResumenDto } from '../../models/candidato.model';

@Injectable({
  providedIn: 'root',
})
export class CandidatoService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  listar(evaluacionId: string) {
    return this.http.get<CandidatoDto[]>(`${this.baseUrl}/evaluaciones/${evaluacionId}/candidatos`);
  }

  asignar(evaluacionId: string, dto: AsignarCandidatoDto) {
    return this.http.post<CandidatoDto>(`${this.baseUrl}/evaluaciones/${evaluacionId}/candidatos`, dto);
  }

  eliminar(evaluacionId: string, candidatoId: string) {
    return this.http.delete<{ message: string }>(`${this.baseUrl}/evaluaciones/${evaluacionId}/candidatos/${candidatoId}`);
  }

  listarUsuariosCandidatos(evaluacionId: string) {
    return this.http.get<UsuarioResumenDto[]>(`${this.baseUrl}/evaluaciones/${evaluacionId}/candidatos/usuarios-candidatos`);
  }

  obtenerRespuestasCrudas(evaluacionId: string, candidatoId: string) {
    return this.http.get<RespuestaCrudaDto[]>(
      `${this.baseUrl}/evaluaciones/${evaluacionId}/candidatos/${candidatoId}/respuestas`
    );
  }

  misEvaluaciones() {
    return this.http.get<EvaluacionAsignadaDto[]>(`${this.baseUrl}/mis-evaluaciones`);
  }
}
