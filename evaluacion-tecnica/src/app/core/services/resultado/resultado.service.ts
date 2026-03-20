import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';
import {
  ResultadoCompletoDto,
  RankingEvaluacionDto,
  ComparacionCandidatosDto,
  ResumenEvaluacionResultadosDto
} from '../../models/resultado.model';
import { ResumenProctoringDto } from '../../models/proctoring.model';

@Injectable({ providedIn: 'root' })
export class ResultadoService {
  private url = `${environment.apiUrl}/resultados`;
  private analisisUrl = `${environment.apiUrl}/analisis`;

  constructor(private http: HttpClient) {}

  obtenerResultado(candidatoId: string) {
    return this.http.get<ResultadoCompletoDto>(`${this.url}/candidato/${candidatoId}`);
  }

  obtenerResumenEvaluacion(evaluacionId: string) {
    return this.http.get<ResumenEvaluacionResultadosDto>(`${this.url}/evaluacion/${evaluacionId}`);
  }

  obtenerRanking(evaluacionId: string) {
    return this.http.get<RankingEvaluacionDto>(`${this.url}/evaluacion/${evaluacionId}/ranking`);
  }

  compararCandidatos(evaluacionId: string, candidatoIds: string[]) {
    return this.http.post<ComparacionCandidatosDto>(
      `${this.url}/evaluacion/${evaluacionId}/comparar`,
      { candidatoIds }
    );
  }

  ejecutarAnalisis(candidatoId: string) {
    return this.http.post<ResultadoCompletoDto>(`${this.analisisUrl}/${candidatoId}`, {});
  }

  liberarResultado(candidatoId: string) {
    return this.http.post<{ message: string }>(`${this.url}/candidato/${candidatoId}/liberar`, {});
  }

  obtenerProctoring(candidatoId: string) {
    return this.http.get<ResumenProctoringDto>(`${this.url}/candidato/${candidatoId}/proctoring`);
  }
}
