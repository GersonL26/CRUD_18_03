export interface EstadoSesionDto {
  sesionId: string;
  evaluacionId: string;
  candidatoId: string;
  nombreCandidato: string;
  preguntaActualIndex: number;
  totalPreguntas: number;
  segundosTranscurridos: number;
  tiempoLimitePreguntaSegundos: number;
  sesionActiva: boolean;
  fueCompletada: boolean;
}

export interface PreguntaEnVivoDto {
  preguntaId: string;
  index: number;
  texto: string;
  tipo: number;
  puntajeMaximo: number;
  tiempoLimiteSegundos: number;
  totalPreguntas: number;
}

export interface ResponderPreguntaEnVivoDto {
  preguntaId: string;
  contenido: string;
  tiempoUsadoSegundos: number;
}

export interface CrearSesionEnVivoDto {
  evaluacionId: string;
  candidatoId: string;
}
