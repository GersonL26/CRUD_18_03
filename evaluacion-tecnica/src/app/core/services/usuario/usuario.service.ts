import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';

export interface UsuarioDto {
  id: string;
  nombreCompleto: string;
  email: string;
  rol: number;
  creadoEn: string;
  estaActivo: boolean;
}

export interface ActualizarUsuarioDto {
  nombreCompleto?: string;
  rol?: number;
  estaActivo?: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class UsuarioService {
  private url = `${environment.apiUrl}/usuarios`;

  constructor(private http: HttpClient) {}

  listar() {
    return this.http.get<UsuarioDto[]>(this.url);
  }

  actualizar(id: string, dto: ActualizarUsuarioDto) {
    return this.http.put<{ message: string }>(`${this.url}/${id}`, dto);
  }
}
