import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'nivelBadge',
  standalone: true,
})
export class NivelBadgePipe implements PipeTransform {
  private readonly niveles: Record<number, string> = {
    1: 'Junior',
    2: 'Mid',
    3: 'Senior',
    4: 'Lead'
  };

  transform(nivel: number | null | undefined): string {
    if (nivel === null || nivel === undefined) return '—';
    return this.niveles[nivel] ?? '—';
  }
}
