import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'tiempoInvertido',
  standalone: true,
})
export class TiempoInvertidoPipe implements PipeTransform {
  transform(segundos: number | null | undefined): string {
    if (segundos === null || segundos === undefined || segundos < 0) return '—';

    const horas = Math.floor(segundos / 3600);
    const minutos = Math.floor((segundos % 3600) / 60);
    const segs = segundos % 60;

    if (horas > 0) return `${horas}h ${minutos}m ${segs}s`;
    if (minutos > 0) return `${minutos}m ${segs}s`;
    return `${segs}s`;
  }
}
