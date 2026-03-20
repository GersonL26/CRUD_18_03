import { Component, Inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CandidatoService } from '../../core/services/candidato/candidato.service';
import { RespuestaCrudaDto } from '../../core/models/candidato.model';

export interface RespuestasCrudasDialogData {
  evaluacionId: string;
  candidatoId: string;
  nombreCandidato: string;
}

@Component({
  selector: 'app-respuestas-crudas-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  template: `
    <div class="dialog-header">
      <div class="flex items-center gap-3">
        <div class="w-9 h-9 rounded-lg bg-blue-50 flex items-center justify-center">
          <mat-icon class="text-blue-500 !text-lg">assignment</mat-icon>
        </div>
        <div>
          <h2 mat-dialog-title class="!text-base !font-bold !m-0">Respuestas originales</h2>
          <p class="text-xs text-slate-500 m-0">{{ data.nombreCandidato }} · sin procesamiento IA</p>
        </div>
      </div>
      <button mat-icon-button (click)="dialogRef.close()" class="!text-slate-400">
        <mat-icon>close</mat-icon>
      </button>
    </div>

    <mat-dialog-content class="!p-0 !max-h-[65vh] overflow-y-auto">
      @if (cargando()) {
        <div class="flex justify-center py-12">
          <mat-spinner diameter="36"></mat-spinner>
        </div>
      } @else if (respuestas().length === 0) {
        <div class="text-center py-10 text-slate-400">
          <mat-icon class="!text-4xl text-slate-300 block mx-auto mb-2">inbox</mat-icon>
          <p class="text-sm">Este candidato aún no tiene respuestas registradas.</p>
        </div>
      } @else {
        <div class="divide-y divide-slate-100">
          @for (r of respuestas(); track r.orden) {
            <div class="px-6 py-4">
              <div class="flex items-start gap-3">
                <span class="pregunta-num">{{ r.orden }}</span>
                <div class="flex-1 min-w-0">
                  <p class="text-sm font-semibold text-gray-800 mb-1 leading-snug">{{ r.pregunta }}</p>
                  <div class="flex items-center gap-3 mb-2">
                    <span class="tipo-badge">{{ nombreTipo(r.tipo) }}</span>
                    <span class="text-xs text-slate-400">{{ r.puntajeMaximo }} pts</span>
                    @if (r.tiempoUsadoSegundos) {
                      <span class="text-xs text-slate-400">
                        <mat-icon class="!text-xs inline align-middle">schedule</mat-icon>
                        {{ formatTiempo(r.tiempoUsadoSegundos) }}
                      </span>
                    }
                  </div>
                  @if (r.contenido) {
                    <div class="respuesta-box">{{ r.contenido }}</div>
                  } @else {
                    <div class="sin-respuesta">Sin respuesta registrada</div>
                  }
                </div>
              </div>
            </div>
          }
        </div>
      }
    </mat-dialog-content>

    <mat-dialog-actions class="!px-6 !py-4 !border-t !border-slate-100">
      <span class="text-xs text-slate-400 flex-1">{{ respuestas().length }} preguntas · respuestas sin modificar</span>
      <button mat-flat-button color="primary" (click)="dialogRef.close()">Cerrar</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .dialog-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 20px 24px 16px;
      border-bottom: 1px solid #f1f5f9;
    }
    .pregunta-num {
      width: 24px;
      height: 24px;
      border-radius: 50%;
      background: #eff6ff;
      color: #3b82f6;
      font-size: 11px;
      font-weight: 700;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
      margin-top: 2px;
    }
    .tipo-badge {
      font-size: 10px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      padding: 2px 8px;
      border-radius: 9999px;
      background: #f1f5f9;
      color: #64748b;
    }
    .respuesta-box {
      background: #f8fafc;
      border: 1px solid #e2e8f0;
      border-radius: 8px;
      padding: 10px 14px;
      font-size: 13px;
      color: #374151;
      white-space: pre-wrap;
      word-break: break-word;
      line-height: 1.5;
    }
    .sin-respuesta {
      font-size: 12px;
      color: #94a3b8;
      font-style: italic;
      padding: 6px 0;
    }
  `]
})
export class RespuestasCrudasDialogComponent {
  respuestas = signal<RespuestaCrudaDto[]>([]);
  cargando = signal(true);

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: RespuestasCrudasDialogData,
    public dialogRef: MatDialogRef<RespuestasCrudasDialogComponent>,
    private candidatoService: CandidatoService
  ) {
    this.candidatoService.obtenerRespuestasCrudas(data.evaluacionId, data.candidatoId).subscribe({
      next: r => { this.respuestas.set(r); this.cargando.set(false); },
      error: () => this.cargando.set(false)
    });
  }

  nombreTipo(tipo: number): string {
    return (['', 'Texto libre', 'Código', 'Opción múltiple'])[tipo] ?? 'Texto';
  }

  formatTiempo(seg: number): string {
    const m = Math.floor(seg / 60);
    const s = seg % 60;
    return m > 0 ? `${m}m ${s}s` : `${s}s`;
  }
}
