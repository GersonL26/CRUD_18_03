import { Injectable, signal } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EvaluacionService } from '../evaluacion/evaluacion.service';
import { CandidatoService } from '../candidato/candidato.service';

export interface NotificacionItem {
  candidatoNombre: string;
  evaluacionTitulo: string;
  evaluacionId: string;
  candidatoId: string;
  fecha: string;
  tieneResultado: boolean;
}

@Injectable({ providedIn: 'root' })
export class NotificacionesService {
  private _count = signal(0);
  private _items = signal<NotificacionItem[]>([]);
  readonly count = this._count.asReadonly();
  readonly items = this._items.asReadonly();

  constructor(
    private evaluacionService: EvaluacionService,
    private candidatoService: CandidatoService
  ) {}

  setNotificaciones(items: NotificacionItem[]): void {
    this._items.set(items);
    this._count.set(items.length);
  }

  cargar(): void {
    this.evaluacionService.listar().subscribe({
      next: evaluaciones => {
        const activas = evaluaciones.filter(e => e.estado === 2);
        if (activas.length === 0) {
          this.setNotificaciones([]);
          return;
        }
        const requests = activas.map(ev =>
          this.candidatoService.listar(ev.id).pipe(catchError(() => of([])))
        );
        forkJoin(requests).subscribe({
          next: candidatosPorEval => {
            const notifItems = activas.flatMap((ev, i) =>
              (candidatosPorEval[i] as any[])
                .filter(c => !!c.fechaFinRespuesta)
                .map(c => ({
                  candidatoNombre: c.nombre,
                  evaluacionTitulo: ev.titulo,
                  evaluacionId: ev.id,
                  candidatoId: c.id,
                  fecha: c.fechaFinRespuesta!,
                  tieneResultado: c.tieneResultado
                }))
            );
            this.setNotificaciones(notifItems);
          }
        });
      }
    });
  }
}
