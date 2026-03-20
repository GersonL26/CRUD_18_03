export interface EventoProctoringDto {
  id: string;
  tipo: string;
  detalle?: string;
  timestamp: string;
}

export interface ResumenProctoringDto {
  vecesSalioFoco: number;
  vecesCopyPaste: number;
  transcripcionesAudio: number;
  eventos: EventoProctoringDto[];
}
