import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { EvaluacionService } from '../../core/services/evaluacion/evaluacion.service';
import { CandidatoService } from '../../core/services/candidato/candidato.service';
import { EvaluacionConPreguntasDto, PreguntaDto, EstadoEvaluacion } from '../../core/models/evaluacion.model';
import { CandidatoDto } from '../../core/models/candidato.model';

@Component({
  selector: 'app-detalle-evaluacion',
  imports: [
    ReactiveFormsModule,
    MatTabsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatSlideToggleModule,
    MatTooltipModule
  ],
  templateUrl: './detalle-evaluacion.component.html',
  styleUrl: './detalle-evaluacion.component.scss',
})
export class DetalleEvaluacionComponent implements OnInit {
  evaluacion = signal<EvaluacionConPreguntasDto | null>(null);
  candidatos = signal<CandidatoDto[]>([]);
  cargando = signal(true);
  guardandoInfo = signal(false);
  agregandoPregunta = signal(false);
  invitando = signal(false);

  esBorrador = () => this.evaluacion()?.estado === EstadoEvaluacion.Borrador;
  esActiva = () => this.evaluacion()?.estado === EstadoEvaluacion.Activa;

  columnasPreguntas = ['orden', 'texto', 'tipo', 'puntaje', 'tiempo', 'acciones'];
  columnasCandidatos = ['nombre', 'email', 'estado', 'acciones'];

  niveles = [
    { value: 0, label: 'Junior' },
    { value: 1, label: 'Mid' },
    { value: 2, label: 'Senior' }
  ];

  tiposPregunta = [
    { value: 0, label: 'Abierta' },
    { value: 1, label: 'Opción Múltiple' },
    { value: 2, label: 'Código' }
  ];

  infoForm;
  preguntaForm;
  candidatoForm;

  private evaluacionId = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private evaluacionService: EvaluacionService,
    private candidatoService: CandidatoService,
    private snackBar: MatSnackBar
  ) {
    this.infoForm = this.fb.group({
      titulo: ['', [Validators.required, Validators.maxLength(200)]],
      descripcion: ['', [Validators.maxLength(1000)]],
      tecnologia: ['', [Validators.required, Validators.maxLength(100)]],
      nivel: [0],
      tiempoLimiteTotalMinutos: [60, [Validators.min(10), Validators.max(300)]],
      requiereCamara: [false],
      requiereMicrofono: [false]
    });

    this.preguntaForm = this.fb.group({
      texto: ['', [Validators.required, Validators.maxLength(2000)]],
      tipo: [0, [Validators.required]],
      puntajeMaximo: [10, [Validators.required, Validators.min(1), Validators.max(100)]],
      tiempoLimiteSegundos: [300, [Validators.required, Validators.min(30), Validators.max(3600)]]
    });

    this.candidatoForm = this.fb.group({
      nombre: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnInit(): void {
    this.evaluacionId = this.route.snapshot.paramMap.get('id')!;
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.evaluacionService.obtener(this.evaluacionId).subscribe({
      next: ev => {
        this.evaluacion.set(ev);
        this.infoForm.patchValue(ev);
        if (ev.estado !== EstadoEvaluacion.Borrador) {
          this.infoForm.disable();
        }
        this.cargando.set(false);
      },
      error: () => {
        this.snackBar.open('Error al cargar evaluación', 'OK', { duration: 3000 });
        this.router.navigate(['/evaluador/dashboard']);
      }
    });

    this.candidatoService.listar(this.evaluacionId).subscribe({
      next: data => this.candidatos.set(data)
    });
  }

  nombreEstado(estado: number): string {
    return ['Borrador', 'Activa', 'Cerrada'][estado] ?? '';
  }

  claseEstado(estado: number): string {
    return [
      'bg-amber-50 text-amber-700',
      'bg-green-50 text-green-700',
      'bg-gray-100 text-gray-600'
    ][estado] ?? '';
  }

  nombreTipo(tipo: number): string {
    return this.tiposPregunta.find(t => t.value === tipo)?.label ?? '';
  }

  guardarInfo(): void {
    if (this.infoForm.invalid || !this.esBorrador()) return;
    this.guardandoInfo.set(true);

    this.evaluacionService.actualizar(this.evaluacionId, {
      titulo: this.infoForm.value.titulo ?? undefined,
      descripcion: this.infoForm.value.descripcion ?? undefined,
      tecnologia: this.infoForm.value.tecnologia ?? undefined,
      nivel: this.infoForm.value.nivel ?? undefined,
      tiempoLimiteTotalMinutos: this.infoForm.value.tiempoLimiteTotalMinutos ?? undefined,
      requiereCamara: this.infoForm.value.requiereCamara ?? undefined,
      requiereMicrofono: this.infoForm.value.requiereMicrofono ?? undefined
    }).subscribe({
      next: () => {
        this.snackBar.open('Información actualizada', 'OK', { duration: 2000 });
        this.guardandoInfo.set(false);
        this.cargar();
      },
      error: () => this.guardandoInfo.set(false)
    });
  }

  agregarPregunta(): void {
    if (this.preguntaForm.invalid) return;
    this.agregandoPregunta.set(true);

    const pv = this.preguntaForm.getRawValue();
    this.evaluacionService.agregarPregunta(this.evaluacionId, {
      texto: pv.texto!,
      tipo: pv.tipo!,
      puntajeMaximo: pv.puntajeMaximo!,
      tiempoLimiteSegundos: pv.tiempoLimiteSegundos!
    }).subscribe({
      next: () => {
        this.preguntaForm.reset({ tipo: 0, puntajeMaximo: 10, tiempoLimiteSegundos: 300 });
        this.agregandoPregunta.set(false);
        this.cargar();
      },
      error: () => this.agregandoPregunta.set(false)
    });
  }

  eliminarPregunta(preguntaId: string): void {
    this.evaluacionService.eliminarPregunta(this.evaluacionId, preguntaId).subscribe({
      next: () => this.cargar()
    });
  }

  invitarCandidato(): void {
    if (this.candidatoForm.invalid) return;
    this.invitando.set(true);

    const cv = this.candidatoForm.getRawValue();
    this.candidatoService.invitar(this.evaluacionId, {
      nombre: cv.nombre!,
      email: cv.email!
    }).subscribe({
      next: () => {
        this.snackBar.open('Candidato invitado', 'OK', { duration: 2000 });
        this.candidatoForm.reset();
        this.invitando.set(false);
        this.candidatoService.listar(this.evaluacionId).subscribe({
          next: data => this.candidatos.set(data)
        });
      },
      error: () => this.invitando.set(false)
    });
  }

  eliminarCandidato(candidatoId: string): void {
    this.candidatoService.eliminar(this.evaluacionId, candidatoId).subscribe({
      next: () => {
        this.candidatoService.listar(this.evaluacionId).subscribe({
          next: data => this.candidatos.set(data)
        });
      }
    });
  }

  activar(): void {
    this.evaluacionService.activar(this.evaluacionId).subscribe({
      next: () => {
        this.snackBar.open('Evaluación activada', 'OK', { duration: 2000 });
        this.cargar();
      }
    });
  }

  cerrar(): void {
    this.evaluacionService.cerrar(this.evaluacionId).subscribe({
      next: () => {
        this.snackBar.open('Evaluación cerrada', 'OK', { duration: 2000 });
        this.cargar();
      }
    });
  }

  eliminarEvaluacion(): void {
    if (!confirm('¿Eliminar esta evaluación? Esta acción no se puede deshacer.')) return;
    this.evaluacionService.eliminar(this.evaluacionId).subscribe({
      next: () => {
        this.snackBar.open('Evaluación eliminada', 'OK', { duration: 2000 });
        this.router.navigate(['/evaluador/dashboard']);
      }
    });
  }
}
