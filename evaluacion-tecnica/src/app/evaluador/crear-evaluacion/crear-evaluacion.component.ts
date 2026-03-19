import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
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
import { EvaluacionService } from '../../core/services/evaluacion/evaluacion.service';
import { TipoPregunta } from '../../core/models/evaluacion.model';

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
    MatSlideToggleModule
  ],
  templateUrl: './crear-evaluacion.component.html',
  styleUrl: './crear-evaluacion.component.scss',
})
export class CrearEvaluacionComponent {
  guardando = signal(false);

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
      nivel: [0, [Validators.required]],
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
      tipo: [0, [Validators.required]],
      puntajeMaximo: [10, [Validators.required, Validators.min(1), Validators.max(100)]],
      tiempoLimiteSegundos: [300, [Validators.required, Validators.min(30), Validators.max(3600)]]
    }));
  }

  eliminarPregunta(index: number): void {
    this.preguntas.removeAt(index);
  }

  nombreTipo(tipo: number): string {
    return this.tiposPregunta.find(t => t.value === tipo)?.label ?? '';
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
      preguntas: preguntas.length > 0 ? preguntas : undefined
    }).subscribe({
      next: (ev) => {
        this.snackBar.open('Evaluación creada correctamente', 'OK', { duration: 3000 });
        this.router.navigate(['/evaluador/evaluacion', ev.id]);
      },
      error: () => this.guardando.set(false)
    });
  }
}
