import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'nivelBadge',
})
export class NivelBadgePipe implements PipeTransform {
  transform(value: unknown, ...args: unknown[]): unknown {
    return null;
  }
}
