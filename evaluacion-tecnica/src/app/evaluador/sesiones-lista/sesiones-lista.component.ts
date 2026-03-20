import { Component, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { EvaluacionService } from '../../core/services/evaluacion/evaluacion.service';
import { EvaluacionDto } from '../../core/models/evaluacion.model';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';
import { NivelBadgePipe } from '../../shared/pipes/nivel-badge-pipe';

@Component({
  selector: 'app-sesiones-lista',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatTooltipModule,
    LoadingSpinnerComponent,
    EmptyStateComponent,
    NivelBadgePipe
  ],
  templateUrl: './sesiones-lista.component.html',
  styleUrl: './sesiones-lista.component.scss',
})
export class SesionesListaComponent implements OnInit {
  evaluaciones = signal<EvaluacionDto[]>([]);
  cargando = signal(true);
  columnas = ['titulo', 'tecnologia', 'nivel', 'estado', 'acciones'];

  constructor(
    private evaluacionService: EvaluacionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.evaluacionService.listar().subscribe({
      next: (data) => {
        this.evaluaciones.set(data.filter(e => e.estado === 2));
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  irADetalle(evaluacionId: string): void {
    this.router.navigate(['/evaluador/evaluacion', evaluacionId], { fragment: 'candidatos' });
  }
}
