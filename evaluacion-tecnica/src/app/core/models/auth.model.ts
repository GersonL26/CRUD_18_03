export interface LoginDto {
  email: string;
  password: string;
}

export interface RegistroDto {
  nombreCompleto: string;
  email: string;
  password: string;
  rol: number;
}

export interface AuthResponseDto {
  usuarioId: string;
  nombreCompleto: string;
  email: string;
  rol: number;
  token: string;
}
