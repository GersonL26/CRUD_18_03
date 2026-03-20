import { Component, computed, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { UsuarioService, UsuarioDto } from '../../core/services/usuario/usuario.service';

@Component({
  selector: 'app-gestion-usuarios',
  imports: [
    FormsModule,
    MatCardModule,
    MatTableModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatTooltipModule,
    MatChipsModule,
    DatePipe,
    LoadingSpinnerComponent,
    EmptyStateComponent
  ],
  templateUrl: './gestion-usuarios.component.html',
  styleUrl: './gestion-usuarios.component.scss',
})
export class GestionUsuariosComponent implements OnInit {
  usuarios = signal<UsuarioDto[]>([]);
  cargando = signal(true);
  filtroTexto = signal('');
  filtroRol = signal('');

  columnas = ['nombre', 'email', 'rol', 'fecha', 'estado', 'acciones'];

  roles = [
    { value: 1, label: 'Admin', icon: 'shield', color: 'role-admin' },
    { value: 2, label: 'Evaluador', icon: 'school', color: 'role-evaluador' },
    { value: 3, label: 'Candidato', icon: 'person', color: 'role-candidato' }
  ];

  usuariosFiltrados = computed(() => {
    let lista = this.usuarios();
    const texto = this.filtroTexto().toLowerCase();
    const rol = this.filtroRol();

    if (texto) {
      lista = lista.filter(u =>
        u.nombreCompleto.toLowerCase().includes(texto) ||
        u.email.toLowerCase().includes(texto)
      );
    }
    if (rol) {
      lista = lista.filter(u => u.rol === +rol);
    }
    return lista;
  });

  totalActivos = computed(() => this.usuarios().filter(u => u.estaActivo).length);
  totalEvaluadores = computed(() => this.usuarios().filter(u => u.rol === 2).length);
  totalCandidatos = computed(() => this.usuarios().filter(u => u.rol === 3).length);

  constructor(
    private usuarioService: UsuarioService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.usuarioService.listar().subscribe({
      next: data => {
        this.usuarios.set(data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  getRol(valor: number) {
    return this.roles.find(r => r.value === valor) ?? this.roles[2];
  }

  cambiarRol(usuario: UsuarioDto, nuevoRol: number): void {
    this.usuarioService.actualizar(usuario.id, { rol: nuevoRol }).subscribe({
      next: () => {
        usuario.rol = nuevoRol;
        this.usuarios.update(list => [...list]);
        this.snackBar.open('Rol actualizado', 'Cerrar', { duration: 2500, horizontalPosition: 'end', verticalPosition: 'top' });
      }
    });
  }

  toggleActivo(usuario: UsuarioDto): void {
    const nuevoEstado = !usuario.estaActivo;
    const accion = nuevoEstado ? 'activar' : 'desactivar';

    this.dialog.open(ConfirmDialogComponent, {
      data: {
        titulo: `${nuevoEstado ? 'Activar' : 'Desactivar'} usuario`,
        mensaje: `¿Estás seguro de ${accion} a ${usuario.nombreCompleto}?`,
        textoConfirmar: nuevoEstado ? 'Activar' : 'Desactivar',
        color: nuevoEstado ? 'primary' : 'warn'
      } as ConfirmDialogData,
      width: '400px'
    }).afterClosed().subscribe(result => {
      if (!result) return;

      this.usuarioService.actualizar(usuario.id, { estaActivo: nuevoEstado }).subscribe({
        next: () => {
          usuario.estaActivo = nuevoEstado;
          this.usuarios.update(list => [...list]);
          this.snackBar.open(`Usuario ${accion}do`, 'Cerrar', { duration: 2500, horizontalPosition: 'end', verticalPosition: 'top' });
        }
      });
    });
  }

  limpiarFiltros(): void {
    this.filtroTexto.set('');
    this.filtroRol.set('');
  }
}
