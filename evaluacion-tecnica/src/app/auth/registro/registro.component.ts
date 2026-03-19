import { Component, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../core/services/auth/auth.service';
import { RolUsuario } from '../../core/models/auth.model';

@Component({
  selector: 'app-registro',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './registro.component.html',
  styleUrl: './registro.component.scss',
})
export class RegistroComponent {
  ocultarPassword = signal(true);
  ocultarConfirmar = signal(true);
  cargando = signal(false);
  form;

  roles = [
    { value: RolUsuario.Evaluador, label: 'Evaluador' },
    { value: RolUsuario.Candidato, label: 'Candidato' }
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      nombreCompleto: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      rol: [RolUsuario.Candidato, [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmarPassword: ['', [Validators.required]]
    }, { validators: this.passwordsMatch });
  }

  registrar(): void {
    if (this.form.invalid) return;

    this.cargando.set(true);
    const { nombreCompleto, email, password, rol } = this.form.getRawValue();

    this.authService.registro({
      nombreCompleto: nombreCompleto!,
      email: email!,
      password: password!,
      rol: rol!
    }).subscribe({
      next: () => {
        this.snackBar.open('Cuenta creada exitosamente', 'Cerrar', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.router.navigate(['/login']);
      },
      error: () => {
        this.cargando.set(false);
      }
    });
  }

  private passwordsMatch(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmar = control.get('confirmarPassword');
    if (password?.value !== confirmar?.value) {
      confirmar?.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }
}
