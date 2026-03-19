import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';
import {
  EvaluacionDto,
  EvaluacionConPreguntasDto,
  CrearEvaluacionConPreguntasDto,
  ActualizarEvaluacionDto,
  PreguntaDto,
  AgregarPreguntaDto,
  ActualizarPreguntaDto
} from '../../models/evaluacion.model';

@Injectable({
  providedIn: 'root',
})
export class EvaluacionService {
  private url = `${environment.apiUrl}/evaluaciones`;

  constructor(private http: HttpClient) {}

  listar() {
    return this.http.get<EvaluacionDto[]>(this.url);
  }

  obtener(id: string) {
    return this.http.get<EvaluacionConPreguntasDto>(`${this.url}/${id}`);
  }

  crear(dto: CrearEvaluacionConPreguntasDto) {
    return this.http.post<EvaluacionConPreguntasDto>(this.url, dto);
  }

  actualizar(id: string, dto: ActualizarEvaluacionDto) {
    return this.http.put<{ message: string }>(`${this.url}/${id}`, dto);
  }

  eliminar(id: string) {
    return this.http.delete<{ message: string }>(`${this.url}/${id}`);
  }

  activar(id: string) {
    return this.http.post<{ message: string }>(`${this.url}/${id}/activar`, {});
  }

  cerrar(id: string) {
    return this.http.post<{ message: string }>(`${this.url}/${id}/cerrar`, {});
  }

  agregarPregunta(evaluacionId: string, dto: AgregarPreguntaDto) {
    return this.http.post<PreguntaDto>(`${this.url}/${evaluacionId}/preguntas`, dto);
  }

  actualizarPregunta(evaluacionId: string, preguntaId: string, dto: ActualizarPreguntaDto) {
    return this.http.put<{ message: string }>(`${this.url}/${evaluacionId}/preguntas/${preguntaId}`, dto);
  }

  eliminarPregunta(evaluacionId: string, preguntaId: string) {
    return this.http.delete<{ message: string }>(`${this.url}/${evaluacionId}/preguntas/${preguntaId}`);
  }
}
