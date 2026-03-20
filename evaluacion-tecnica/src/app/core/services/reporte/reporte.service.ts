import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class ReporteService {
  private url = `${environment.apiUrl}/reportes`;

  constructor(private http: HttpClient) {}

  descargarPdfCandidato(candidatoId: string) {
    return this.http.get(`${this.url}/${candidatoId}/pdf`, {
      responseType: 'blob'
    });
  }

  descargarRankingPdf(evaluacionId: string) {
    return this.http.get(`${this.url}/evaluacion/${evaluacionId}/ranking-pdf`, {
      responseType: 'blob'
    });
  }
}
