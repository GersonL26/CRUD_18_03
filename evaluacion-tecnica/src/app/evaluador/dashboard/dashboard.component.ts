import { Component, OnInit, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EvaluacionService } from '../../core/services/evaluacion/evaluacion.service';
import { EvaluacionDto, EstadoEvaluacion, NivelTecnico } from '../../core/models/evaluacion.model';

@Component({
  selector: 'app-dashboard',
  imports: [
    RouterLink,
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatTooltipModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  evaluaciones = signal<EvaluacionDto[]>([]);
  cargando = signal(true);

  columnas = ['titulo', 'tecnologia', 'nivel', 'estado', 'fecha', 'acciones'];

  total = computed(() => this.evaluaciones().length);
  activas = computed(() => this.evaluaciones().filter(e => e.estado === EstadoEvaluacion.Activa).length);
  borradores = computed(() => this.evaluaciones().filter(e => e.estado === EstadoEvaluacion.Borrador).length);
  cerradas = computed(() => this.evaluaciones().filter(e => e.estado === EstadoEvaluacion.Cerrada).length);

  constructor(
    private evaluacionService: EvaluacionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.evaluacionService.listar().subscribe({
      next: data => {
        this.evaluaciones.set(data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  nombreNivel(nivel: number): string {
    const map: Record<number, string> = { 0: 'Junior', 1: 'Junior', 2: 'Mid', 3: 'Senior', 4: 'Lead' };
    return map[nivel] ?? 'Junior';
  }

  nombreEstado(estado: number): string {
    const map: Record<number, string> = { 1: 'Borrador', 2: 'Activa', 3: 'Cerrada' };
    return map[estado] ?? '';
  }

  colorEstado(estado: number): string {
    const map: Record<number, string> = { 1: 'warn', 2: 'primary', 3: '' };
    return map[estado] ?? '';
  }

  verDetalle(id: string): void {
    this.router.navigate(['/evaluador/evaluacion', id]);
  }
}
