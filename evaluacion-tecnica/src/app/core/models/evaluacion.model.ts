export interface EvaluacionDto {
  id: string;
  titulo: string;
  descripcion?: string;
  tecnologia: string;
  nivel: number;
  estado: number;
  tiempoLimiteTotalMinutos: number;
  requiereCamara: boolean;
  requiereMicrofono: boolean;
  evaluadorId: string;
  creadoEn: string;
  estaActivo: boolean;
}

export interface CrearEvaluacionConPreguntasDto {
  titulo: string;
  descripcion?: string;
  tecnologia: string;
  nivel: number;
  tiempoLimiteTotalMinutos: number;
  requiereCamara: boolean;
  requiereMicrofono: boolean;
  preguntas?: AgregarPreguntaDto[];
}

export interface ActualizarEvaluacionDto {
  titulo?: string;
  descripcion?: string;
  tecnologia?: string;
  nivel?: number;
  tiempoLimiteTotalMinutos?: number;
  requiereCamara?: boolean;
  requiereMicrofono?: boolean;
}

export interface EvaluacionConPreguntasDto extends EvaluacionDto {
  preguntas: PreguntaDto[];
}

export interface PreguntaDto {
  id: string;
  texto: string;
  tipo: number;
  rubrica?: string;
  puntajeMaximo: number;
  ordenEnEvaluacion: number;
  tiempoLimiteSegundos: number;
  evaluacionId: string;
  creadoEn: string;
  estaActivo: boolean;
}

export interface AgregarPreguntaDto {
  texto: string;
  tipo: number;
  rubrica?: string;
  puntajeMaximo: number;
  tiempoLimiteSegundos: number;
}

export interface ActualizarPreguntaDto {
  texto?: string;
  tipo?: number;
  rubrica?: string;
  puntajeMaximo?: number;
  ordenEnEvaluacion?: number;
  tiempoLimiteSegundos?: number;
}

export enum NivelTecnico {
  Junior = 1,
  Mid = 2,
  Senior = 3,
  Lead = 4
}

export enum EstadoEvaluacion {
  Borrador = 1,
  Activa = 2,
  Cerrada = 3
}

export enum TipoPregunta {
  TextoLibre = 1,
  Codigo = 2,
  OpcionMultiple = 3
}
