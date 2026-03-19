import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';
import { CandidatoDto, InvitarCandidatoDto } from '../../models/candidato.model';

@Injectable({
  providedIn: 'root',
})
export class CandidatoService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  listar(evaluacionId: string) {
    return this.http.get<CandidatoDto[]>(`${this.baseUrl}/evaluaciones/${evaluacionId}/candidatos`);
  }

  invitar(evaluacionId: string, dto: InvitarCandidatoDto) {
    return this.http.post<CandidatoDto>(`${this.baseUrl}/evaluaciones/${evaluacionId}/candidatos`, dto);
  }

  eliminar(evaluacionId: string, candidatoId: string) {
    return this.http.delete<{ message: string }>(`${this.baseUrl}/evaluaciones/${evaluacionId}/candidatos/${candidatoId}`);
  }
}
