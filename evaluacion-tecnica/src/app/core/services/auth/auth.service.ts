import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { LoginDto, RegistroDto, AuthResponseDto } from '../../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private readonly tokenKey = 'jwt_token';
  private readonly userKey = 'user_data';

  private _usuario = signal<AuthResponseDto | null>(this.getStoredUser());

  readonly usuario = this._usuario.asReadonly();
  readonly isAuthenticated = computed(() => !!this._usuario());
  readonly nombreCompleto = computed(() => this._usuario()?.nombreCompleto ?? '');
  readonly rol = computed(() => this._usuario()?.rol);

  constructor(private http: HttpClient, private router: Router) {}

  login(dto: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, dto).pipe(
      tap(response => this.storeSession(response))
    );
  }

  registro(dto: RegistroDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/registro`, dto).pipe(
      tap(response => this.storeSession(response))
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this._usuario.set(null);
    this.router.navigate(['/login']);
  }

  private storeSession(response: AuthResponseDto): void {
    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.userKey, JSON.stringify(response));
    this._usuario.set(response);
  }

  private getStoredUser(): AuthResponseDto | null {
    const data = localStorage.getItem(this.userKey);
    return data ? JSON.parse(data) : null;
  }
}
