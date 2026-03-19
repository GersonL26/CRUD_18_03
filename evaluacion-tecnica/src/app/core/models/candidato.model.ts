export interface CandidatoDto {
  id: string;
  nombre: string;
  email: string;
  token: string;
  fechaInicioRespuesta?: string;
  fechaFinRespuesta?: string;
  evaluacionId: string;
  creadoEn: string;
  estaActivo: boolean;
}

export interface InvitarCandidatoDto {
  nombre: string;
  email: string;
}

export interface EvaluacionCandidatoDto {
  evaluacionId: string;
  titulo: string;
  descripcion?: string;
  tecnologia: string;
  nivel: number;
  tiempoLimiteTotalMinutos: number;
  requiereCamara: boolean;
  requiereMicrofono: boolean;
  candidatoId: string;
  nombreCandidato: string;
  fechaInicioRespuesta?: string;
  yaRespondio: boolean;
  preguntas: PreguntaCandidatoDto[];
}

export interface PreguntaCandidatoDto {
  id: string;
  texto: string;
  tipo: number;
  puntajeMaximo: number;
  ordenEnEvaluacion: number;
  tiempoLimiteSegundos: number;
}

export interface EnviarRespuestasDto {
  respuestas: RespuestaItemDto[];
}

export interface RespuestaItemDto {
  preguntaId: string;
  contenido: string;
  tiempoUsadoSegundos?: number;
}
