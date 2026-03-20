import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormArray, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatRadioModule } from '@angular/material/radio';
import { MatDividerModule } from '@angular/material/divider';
import { EvaluacionService } from '../../core/services/evaluacion/evaluacion.service';
import { TipoPregunta, NivelTecnico } from '../../core/models/evaluacion.model';

@Component({
  selector: 'app-crear-evaluacion',
  imports: [
    ReactiveFormsModule,
    MatStepperModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatSlideToggleModule,
    MatTooltipModule,
    MatRadioModule,
    MatDividerModule
  ],
  templateUrl: './crear-evaluacion.component.html',
  styleUrl: './crear-evaluacion.component.scss',
})
export class CrearEvaluacionComponent {
  guardando = signal(false);

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

  datosForm;
  preguntasForm;

  constructor(
    private fb: FormBuilder,
    private evaluacionService: EvaluacionService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.datosForm = this.fb.group({
      titulo: ['', [Validators.required, Validators.maxLength(200)]],
      descripcion: ['', [Validators.maxLength(1000)]],
      tecnologia: ['', [Validators.required, Validators.maxLength(100)]],
      nivel: [NivelTecnico.Junior, [Validators.required]],
      tiempoLimiteTotalMinutos: [60, [Validators.required, Validators.min(10), Validators.max(300)]],
      requiereCamara: [false],
      requiereMicrofono: [false]
    });

    this.preguntasForm = this.fb.group({
      preguntas: this.fb.array([])
    });
  }

  get preguntas(): FormArray {
    return this.preguntasForm.get('preguntas') as FormArray;
  }

  agregarPregunta(): void {
    if (this.preguntas.length >= 30) return;
    this.preguntas.push(this.fb.group({
      texto: ['', [Validators.required, Validators.maxLength(2000)]],
      tipo: [TipoPregunta.TextoLibre, [Validators.required]],
      rubrica: ['', [Validators.maxLength(1000)]],
      puntajeMaximo: [10, [Validators.required, Validators.min(1), Validators.max(100)]],
      tiempoLimiteSegundos: [300, [Validators.required, Validators.min(30), Validators.max(3600)]],
      opciones: this.fb.array([]),
      opcionCorrecta: [0]
    }));
  }

  onTipoChange(preguntaIndex: number, nuevoTipo: TipoPregunta): void {
    const opciones = this.getOpciones(preguntaIndex);
    if (nuevoTipo === TipoPregunta.OpcionMultiple && opciones.length === 0) {
      this.agregarOpcion(preguntaIndex);
      this.agregarOpcion(preguntaIndex);
    }
  }

  getOpciones(preguntaIndex: number): FormArray {
    return this.preguntas.at(preguntaIndex).get('opciones') as FormArray;
  }

  agregarOpcion(preguntaIndex: number): void {
    const opciones = this.getOpciones(preguntaIndex);
    if (opciones.length >= 8) return;
    opciones.push(this.fb.control('', [Validators.required, Validators.maxLength(500)]));
  }

  eliminarOpcion(preguntaIndex: number, opcionIndex: number): void {
    const opciones = this.getOpciones(preguntaIndex);
    if (opciones.length <= 2) return;
    opciones.removeAt(opcionIndex);
    const correcta = this.preguntas.at(preguntaIndex).get('opcionCorrecta');
    if (correcta && correcta.value >= opciones.length) {
      correcta.setValue(0);
    }
  }

  getOpcionLetra(index: number): string {
    return String.fromCharCode(65 + index);
  }

  eliminarPregunta(index: number): void {
    this.preguntas.removeAt(index);
  }

  nombreTipo(tipo: number): string {
    return this.tiposPregunta.find(t => t.value === tipo)?.label ?? '';
  }

  nombreNivel(nivel: number | null | undefined): string {
    return this.niveles.find(n => n.value === nivel)?.label ?? '';
  }

  tieneErroresPreguntas(): boolean {
    for (let i = 0; i < this.preguntas.length; i++) {
      const p = this.preguntas.at(i);
      if (p.invalid) return true;
      if (p.get('tipo')?.value === TipoPregunta.OpcionMultiple) {
        const opciones = this.getOpciones(i);
        if (opciones.length < 2 || opciones.controls.some(c => c.invalid)) return true;
      }
    }
    return false;
  }

  guardar(): void {
    if (this.datosForm.invalid) return;

    this.guardando.set(true);
    const datos = this.datosForm.getRawValue();
    const preguntas = this.preguntas.getRawValue();

    this.evaluacionService.crear({
      titulo: datos.titulo!,
      descripcion: datos.descripcion || undefined,
      tecnologia: datos.tecnologia!,
      nivel: datos.nivel!,
      tiempoLimiteTotalMinutos: datos.tiempoLimiteTotalMinutos!,
      requiereCamara: datos.requiereCamara!,
      requiereMicrofono: datos.requiereMicrofono!,
      preguntas: preguntas.length > 0 ? preguntas.map((p: any) => ({
        texto: p.texto,
        tipo: p.tipo,
        rubrica: p.tipo === TipoPregunta.OpcionMultiple
          ? this.serializarOpciones(p.opciones, p.opcionCorrecta)
          : (p.rubrica || undefined),
        puntajeMaximo: p.puntajeMaximo,
        tiempoLimiteSegundos: p.tiempoLimiteSegundos
      })) : undefined
    }).subscribe({
      next: (ev) => {
        this.snackBar.open('Evaluación creada correctamente', 'OK', { duration: 3000 });
        this.router.navigate(['/evaluador/evaluacion', ev.id]);
      },
      error: (err) => {
        this.guardando.set(false);
        const mensaje = err?.error?.message || err?.error || 'Error al guardar la evaluación';
        this.snackBar.open(typeof mensaje === 'string' ? mensaje : 'Error al guardar', 'Cerrar', { duration: 5000 });
      }
    });
  }

  private serializarOpciones(opciones: string[], correctaIndex: number): string {
    return opciones
      .map((op, i) => i === correctaIndex ? `* ${op}` : op)
      .join('\n');
  }

  getOpcionesResumen(rubrica: string): { texto: string; correcta: boolean }[] {
    if (!rubrica) return [];
    return rubrica.split('\n').filter(l => l.trim()).map(linea => ({
      texto: linea.startsWith('* ') ? linea.substring(2) : linea,
      correcta: linea.startsWith('* ')
    }));
  }
}
