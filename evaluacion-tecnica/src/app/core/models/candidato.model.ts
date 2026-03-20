export interface RespuestaCrudaDto {
  orden: number;
  pregunta: string;
  tipo: number;
  puntajeMaximo: number;
  contenido?: string;
  tiempoUsadoSegundos?: number;
  fechaRespuesta?: string;
}

export interface CandidatoDto {
  id: string;
  nombre: string;
  email: string;
  token: string;
  fechaInicioRespuesta?: string;
  fechaFinRespuesta?: string;
  evaluacionId: string;
  usuarioId?: string;
  tieneResultado: boolean;
  resultadoLiberado: boolean;
  vecesSalioFoco: number;
  creadoEn: string;
  estaActivo: boolean;
}

export interface AsignarCandidatoDto {
  usuarioId: string;
}

export interface UsuarioResumenDto {
  id: string;
  nombreCompleto: string;
  email: string;
}

export interface EvaluacionAsignadaDto {
  evaluacionId: string;
  titulo: string;
  tecnologia: string;
  nivel: number;
  tiempoLimiteTotalMinutos: number;
  totalPreguntas: number;
  token: string;
  fechaInicioRespuesta?: string;
  fechaFinRespuesta?: string;
  creadoEn: string;
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
