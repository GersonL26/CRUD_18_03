export interface ResultadoCompletoDto {
  resultadoId: string;
  candidatoNombre: string;
  candidatoEmail: string;
  tecnologia: string;
  nivel: string;
  tituloEvaluacion: string;
  scoreTotal: number;
  recomendacion: string;
  resumenIA: string;
  brechasDetectadas: string;
  fortalezasDetectadas: string;
  generadoEn: string;
  tiempoInvertido?: string;
  respuestas: DetalleRespuestaDto[];
}

export interface DetalleRespuestaDto {
  preguntaId: string;
  textoPregunta: string;
  ordenEnEvaluacion: number;
  puntajeMaximo: number;
  contenidoRespuesta: string;
  tiempoUsadoSegundos?: number;
  scoreIA?: number;
  feedbackIA?: string;
  brechasIdentificadas?: string;
}

export interface ResultadoCandidatoPublicoDto {
  candidatoNombre: string;
  tecnologia: string;
  nivel: string;
  tituloEvaluacion: string;
  scoreTotal: number;
  fortalezas: string[];
  brechas: string[];
  tiempoInvertido?: string;
  generadoEn: string;
  respuestas: DetallePreguntaPublicoDto[];
}

export interface DetallePreguntaPublicoDto {
  ordenEnEvaluacion: number;
  textoPregunta: string;
  puntajeMaximo: number;
  scoreIA?: number;
  feedbackIA?: string;
}

export interface RankingItemDto {
  posicion: number;
  candidatoId: string;
  nombre: string;
  email: string;
  scoreTotal: number;
  recomendacion: string;
  tiempoInvertido?: string;
  fechaAnalisis: string;
}

export interface RankingEvaluacionDto {
  evaluacionId: string;
  titulo: string;
  tecnologia: string;
  nivel: string;
  totalAnalizados: number;
  scorePromedio?: number;
  scoreMaximo?: number;
  scoreMinimo?: number;
  ranking: RankingItemDto[];
}

export interface ComparacionCandidatosDto {
  tecnologia: string;
  nivel: string;
  tituloEvaluacion: string;
  candidatos: CandidatoComparadoDto[];
}

export interface CandidatoComparadoDto {
  candidatoId: string;
  nombre: string;
  scoreTotal: number;
  recomendacion: string;
  tiempoInvertido?: string;
  resumenIA: string;
  fortalezas: string[];
  brechas: string[];
  scoresPorPregunta: ScorePorPreguntaDto[];
}

export interface ScorePorPreguntaDto {
  ordenEnEvaluacion: number;
  textoPregunta: string;
  puntajeMaximo: number;
  scoreIA?: number;
}

export interface ResumenCandidatoDto {
  candidatoId: string;
  nombre: string;
  email: string;
  estado: string;
  scoreTotal?: number;
  recomendacion?: string;
  resultadoLiberado: boolean;
  tiempoInvertido?: string;
  fechaFinRespuesta?: string;
  fechaAnalisis?: string;
}

export interface ResumenEvaluacionResultadosDto {
  evaluacionId: string;
  titulo: string;
  tecnologia: string;
  nivel: string;
  totalCandidatos: number;
  candidatosRespondieron: number;
  candidatosAnalizados: number;
  scorePromedio?: number;
  candidatos: ResumenCandidatoDto[];
}
