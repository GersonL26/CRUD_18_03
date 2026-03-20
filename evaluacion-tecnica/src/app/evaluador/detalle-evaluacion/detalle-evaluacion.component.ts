import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';
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
import { MatDialogModule } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Clipboard } from '@angular/cdk/clipboard';
import { EvaluacionService } from '../../core/services/evaluacion/evaluacion.service';
import { CandidatoService } from '../../core/services/candidato/candidato.service';
import { ResultadoService } from '../../core/services/resultado/resultado.service';
import { SesionService } from '../../core/services/sesion/sesion.service';
import { EvaluacionConPreguntasDto, PreguntaDto, EstadoEvaluacion, NivelTecnico, TipoPregunta } from '../../core/models/evaluacion.model';
import { CandidatoDto, UsuarioResumenDto } from '../../core/models/candidato.model';

@Component({
  selector: 'app-detalle-evaluacion',
  imports: [
    ReactiveFormsModule,
    DatePipe,
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
  usuariosCandidatos = signal<UsuarioResumenDto[]>([]);
  cargando = signal(true);
  guardandoInfo = signal(false);
  agregandoPregunta = signal(false);
  asignando = signal(false);
  analizandoId = signal<string | null>(null);
  liberandoId = signal<string | null>(null);
  creandoSesionId = signal<string | null>(null);

  esBorrador = () => this.evaluacion()?.estado === EstadoEvaluacion.Borrador;
  esActiva = () => this.evaluacion()?.estado === EstadoEvaluacion.Activa;
  esCerrada = () => this.evaluacion()?.estado === EstadoEvaluacion.Cerrada;

  columnasPreguntas = ['orden', 'texto', 'tipo', 'puntaje', 'tiempo', 'acciones'];
  columnasCandidatos = ['nombre', 'email', 'estado', 'fecha', 'acciones'];

  niveles = [
    { value: NivelTecnico.Junior, label: 'Junior' },
    { value: NivelTecnico.Mid, label: 'Mid' },
    { value: NivelTecnico.Senior, label: 'Senior' },
    { value: NivelTecnico.Lead, label: 'Lead' }
  ];

  tiposPregunta = [
    { value: TipoPregunta.TextoLibre, label: 'Texto Libre' },
    { value: TipoPregunta.Codigo, label: 'Código' },
    { value: TipoPregunta.OpcionMultiple, label: 'Opción Múltiple' }
  ];

  infoForm;
  preguntaForm;
  asignarForm;

  private evaluacionId = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private evaluacionService: EvaluacionService,
    private candidatoService: CandidatoService,
    private resultadoService: ResultadoService,
    private sesionService: SesionService,
    private clipboard: Clipboard,
    private snackBar: MatSnackBar
  ) {
    this.infoForm = this.fb.group({
      titulo: ['', [Validators.required, Validators.maxLength(200)]],
      descripcion: ['', [Validators.maxLength(1000)]],
      tecnologia: ['', [Validators.required, Validators.maxLength(100)]],
      nivel: [NivelTecnico.Junior],
      tiempoLimiteTotalMinutos: [60, [Validators.min(10), Validators.max(300)]],
      requiereCamara: [false],
      requiereMicrofono: [false]
    });

    this.preguntaForm = this.fb.group({
      texto: ['', [Validators.required, Validators.maxLength(2000)]],
      tipo: [TipoPregunta.TextoLibre, [Validators.required]],
      puntajeMaximo: [10, [Validators.required, Validators.min(1), Validators.max(100)]],
      tiempoLimiteSegundos: [300, [Validators.required, Validators.min(30), Validators.max(3600)]]
    });

    this.asignarForm = this.fb.group({
      usuarioId: ['', [Validators.required]]
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

    this.candidatoService.listarUsuariosCandidatos(this.evaluacionId).subscribe({
      next: data => this.usuariosCandidatos.set(data)
    });
  }

  nombreNivel(nivel: number): string {
    return this.niveles.find(n => n.value === nivel)?.label ?? 'Junior';
  }

  nombreEstado(estado: number): string {
    const map: Record<number, string> = { 1: 'Borrador', 2: 'Activa', 3: 'Cerrada' };
    return map[estado] ?? '';
  }

  claseEstado(estado: number): string {
    const map: Record<number, string> = {
      1: 'bg-amber-50 text-amber-700',
      2: 'bg-green-50 text-green-700',
      3: 'bg-gray-100 text-gray-600'
    };
    return map[estado] ?? '';
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
        this.preguntaForm.reset({ tipo: TipoPregunta.TextoLibre, puntajeMaximo: 10, tiempoLimiteSegundos: 300 });
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

  asignarCandidato(): void {
    if (this.asignarForm.invalid) return;
    this.asignando.set(true);

    const usuarioId = this.asignarForm.getRawValue().usuarioId!;
    this.candidatoService.asignar(this.evaluacionId, { usuarioId }).subscribe({
      next: () => {
        this.snackBar.open('Candidato asignado correctamente', 'OK', { duration: 3000 });
        this.asignarForm.reset();
        this.asignando.set(false);
        this.candidatoService.listar(this.evaluacionId).subscribe({
          next: data => this.candidatos.set(data)
        });
      },
      error: () => this.asignando.set(false)
    });
  }

  eliminarCandidato(candidatoId: string): void {
    if (!confirm('¿Eliminar este candidato? Esta acción no se puede deshacer.')) return;
    this.candidatoService.eliminar(this.evaluacionId, candidatoId).subscribe({
      next: () => {
        this.snackBar.open('Candidato eliminado', 'OK', { duration: 2000 });
        this.candidatoService.listar(this.evaluacionId).subscribe({
          next: data => this.candidatos.set(data)
        });
      }
    });
  }

  analizarCandidato(candidatoId: string): void {
    this.analizandoId.set(candidatoId);
    this.resultadoService.ejecutarAnalisis(candidatoId).subscribe({
      next: () => {
        this.snackBar.open('Análisis IA completado', 'OK', { duration: 3000 });
        this.analizandoId.set(null);
        this.candidatoService.listar(this.evaluacionId).subscribe({
          next: data => this.candidatos.set(data)
        });
      },
      error: (err) => {
        const msg = err?.error?.message || 'Error al ejecutar análisis IA';
        this.snackBar.open(typeof msg === 'string' ? msg : 'Error en análisis', 'Cerrar', { duration: 5000 });
        this.analizandoId.set(null);
      }
    });
  }

  liberarResultado(candidatoId: string): void {
    this.liberandoId.set(candidatoId);
    this.resultadoService.liberarResultado(candidatoId).subscribe({
      next: () => {
        this.snackBar.open('Resultado liberado para el candidato', 'OK', { duration: 3000 });
        this.liberandoId.set(null);
        this.candidatoService.listar(this.evaluacionId).subscribe({
          next: data => this.candidatos.set(data)
        });
      },
      error: (err) => {
        const msg = err?.error?.message || 'Error al liberar resultado';
        this.snackBar.open(typeof msg === 'string' ? msg : 'Error', 'Cerrar', { duration: 5000 });
        this.liberandoId.set(null);
      }
    });
  }

  verResultado(candidatoId: string): void {
    this.router.navigate(['/evaluador/evaluacion', this.evaluacionId, 'resultado', candidatoId]);
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

  reactivar(): void {
    this.evaluacionService.reactivar(this.evaluacionId).subscribe({
      next: () => {
        this.snackBar.open('Evaluación reactivada', 'OK', { duration: 2000 });
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

  crearSesionVivo(candidato: CandidatoDto): void {
    this.creandoSesionId.set(candidato.id);
    this.sesionService.crear({
      evaluacionId: this.evaluacionId,
      candidatoId: candidato.id
    }).subscribe({
      next: (estado) => {
        this.creandoSesionId.set(null);
        // Copy candidate link to clipboard
        const candidateUrl = `${window.location.origin}/candidato/${candidato.token}/sesion-vivo/${estado.sesionId}`;
        this.clipboard.copy(candidateUrl);
        this.snackBar.open('Sesión creada. Enlace del candidato copiado al portapapeles.', 'OK', { duration: 5000 });
        // Navigate to evaluador's live panel
        this.router.navigate(['/evaluador/evaluacion', this.evaluacionId, 'sesion-vivo', estado.sesionId]);
      },
      error: (err) => {
        const msg = err?.error?.message || 'Error al crear sesión en vivo';
        this.snackBar.open(typeof msg === 'string' ? msg : 'Error', 'Cerrar', { duration: 5000 });
        this.creandoSesionId.set(null);
      }
    });
  }
}
