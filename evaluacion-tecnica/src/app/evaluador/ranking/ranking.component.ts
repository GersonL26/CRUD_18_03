import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ResultadoService } from '../../core/services/resultado/resultado.service';
import { ReporteService } from '../../core/services/reporte/reporte.service';
import { RankingEvaluacionDto, RankingItemDto } from '../../core/models/resultado.model';
import { ScoreBadgeComponent } from '../../shared/components/score-badge/score-badge.component';

@Component({
  selector: 'app-ranking',
  imports: [
    MatCardModule, MatTableModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatTooltipModule, MatCheckboxModule,
    ScoreBadgeComponent
  ],
  templateUrl: './ranking.component.html',
  styleUrl: './ranking.component.scss',
})
export class RankingComponent implements OnInit {
  ranking = signal<RankingEvaluacionDto | null>(null);
  cargando = signal(true);
  descargando = signal(false);
  seleccionados = signal<Set<string>>(new Set());
  columnas = ['posicion', 'nombre', 'score', 'recomendacion', 'tiempo', 'acciones'];

  private evaluacionId = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private resultadoService: ResultadoService,
    private reporteService: ReporteService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.evaluacionId = this.route.snapshot.paramMap.get('eId') ?? '';
    this.cargar();
  }

  private cargar(): void {
    this.resultadoService.obtenerRanking(this.evaluacionId).subscribe({
      next: data => {
        this.ranking.set(data);
        this.cargando.set(false);
      },
      error: () => {
        this.snackBar.open('Error al cargar ranking', 'OK', { duration: 3000 });
        this.cargando.set(false);
      }
    });
  }

  toggleSeleccion(id: string): void {
    const set = new Set(this.seleccionados());
    if (set.has(id)) set.delete(id); else set.add(id);
    this.seleccionados.set(set);
  }

  estaSeleccionado(id: string): boolean {
    return this.seleccionados().has(id);
  }

  compararSeleccionados(): void {
    const ids = Array.from(this.seleccionados());
    this.router.navigate(
      ['/evaluador/evaluacion', this.evaluacionId, 'comparar'],
      { queryParams: { ids: ids.join(',') } }
    );
  }

  verResultado(candidatoId: string): void {
    this.router.navigate(['/evaluador/evaluacion', this.evaluacionId, 'resultado', candidatoId]);
  }

  descargarPdf(): void {
    this.descargando.set(true);
    this.reporteService.descargarRankingPdf(this.evaluacionId).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `ranking-${this.evaluacionId}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
        this.descargando.set(false);
      },
      error: () => {
        this.snackBar.open('Error al descargar PDF', 'OK', { duration: 3000 });
        this.descargando.set(false);
      }
    });
  }

  volver(): void {
    this.router.navigate(['/evaluador/rankings']);
  }

  clasePodio(pos: number): string {
    if (pos === 1) return 'podio-oro';
    if (pos === 2) return 'podio-plata';
    if (pos === 3) return 'podio-bronce';
    return '';
  }
}
