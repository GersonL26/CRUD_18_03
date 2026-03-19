import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'tiempoInvertido',
})
export class TiempoInvertidoPipe implements PipeTransform {
  transform(value: unknown, ...args: unknown[]): unknown {
    return null;
  }
}
