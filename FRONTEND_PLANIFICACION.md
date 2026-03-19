# Frontend — Sistema de Evaluación Técnica Asistida por IA

## Stack Tecnológico

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| Angular | 19+ | Framework principal |
| Angular Material | 19+ | Componentes UI |
| TailwindCSS | 4 | Utilidades CSS, layout, colores |
| @microsoft/signalr | latest | Comunicación real-time (sesiones en vivo) |
| chart.js + ng2-charts | latest | Gráficas de score y comparaciones |

> **Convenciones**: standalone components, lazy loading, signals, functional guards/interceptors, sin comentarios en el código.

---

## API Base URL

```
http://localhost:5145/api
```

SignalR Hub: `http://localhost:5145/hubs/sesion`

---

## Estructura del Proyecto

```
src/
├── app/
│   ├── app.component.ts
│   ├── app.config.ts
│   ├── app.routes.ts
│   │
│   ├── core/
│   │   ├── guards/
│   │   │   ├── auth.guard.ts
│   │   │   └── evaluador.guard.ts
│   │   ├── interceptors/
│   │   │   ├── jwt.interceptor.ts
│   │   │   └── error.interceptor.ts
│   │   ├── services/
│   │   │   ├── auth.service.ts
│   │   │   ├── evaluacion.service.ts
│   │   │   ├── candidato.service.ts
│   │   │   ├── resultado.service.ts
│   │   │   ├── reporte.service.ts
│   │   │   └── sesion.service.ts
│   │   └── models/
│   │       ├── auth.model.ts
│   │       ├── evaluacion.model.ts
│   │       ├── candidato.model.ts
│   │       ├── resultado.model.ts
│   │       └── sesion.model.ts
│   │
│   ├── auth/
│   │   ├── login/
│   │   │   └── login.component.ts
│   │   └── registro/
│   │       └── registro.component.ts
│   │
│   ├── evaluador/
│   │   ├── evaluador.routes.ts
│   │   ├── layout/
│   │   │   └── evaluador-layout.component.ts
│   │   ├── dashboard/
│   │   │   └── dashboard.component.ts
│   │   ├── crear-evaluacion/
│   │   │   └── crear-evaluacion.component.ts
│   │   ├── detalle-evaluacion/
│   │   │   └── detalle-evaluacion.component.ts
│   │   ├── candidatos/
│   │   │   └── candidatos.component.ts
│   │   ├── resultado/
│   │   │   └── resultado.component.ts
│   │   ├── ranking/
│   │   │   └── ranking.component.ts
│   │   ├── comparar/
│   │   │   └── comparar.component.ts
│   │   └── sesion-vivo/
│   │       └── sesion-vivo.component.ts
│   │
│   ├── candidato/
│   │   ├── candidato.routes.ts
│   │   ├── instrucciones/
│   │   │   └── instrucciones.component.ts
│   │   ├── responder/
│   │   │   └── responder.component.ts
│   │   ├── sesion-vivo/
│   │   │   └── sesion-vivo-candidato.component.ts
│   │   └── mi-resultado/
│   │       └── mi-resultado.component.ts
│   │
│   └── shared/
│       ├── components/
│       │   ├── score-badge/
│       │   │   └── score-badge.component.ts
│       │   ├── countdown-timer/
│       │   │   └── countdown-timer.component.ts
│       │   ├── loading-spinner/
│       │   │   └── loading-spinner.component.ts
│       │   ├── confirm-dialog/
│       │   │   └── confirm-dialog.component.ts
│       │   └── empty-state/
│       │       └── empty-state.component.ts
│       └── pipes/
│           ├── tiempo-invertido.pipe.ts
│           └── nivel-badge.pipe.ts
│
├── environments/
│   ├── environment.ts
│   └── environment.development.ts
│
├── styles.scss
└── index.html
```

---

## Catálogo de Endpoints del Backend

### Auth (público)
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/auth/login` | Login → `AuthResponseDto { token, nombre, email, rol }` |
| POST | `/auth/registro` | Registro → `AuthResponseDto` |

### Evaluaciones (Evaluador/Admin)
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/evaluaciones` | Crear evaluación con preguntas |
| GET | `/evaluaciones` | Listar evaluaciones del evaluador |
| GET | `/evaluaciones/{id}` | Detalle con preguntas |
| PUT | `/evaluaciones/{id}` | Actualizar evaluación |
| DELETE | `/evaluaciones/{id}` | Eliminar evaluación |
| POST | `/evaluaciones/{id}/activar` | Cambiar estado a Activa |
| POST | `/evaluaciones/{id}/cerrar` | Cambiar estado a Cerrada |
| POST | `/evaluaciones/{eId}/preguntas` | Agregar pregunta |
| PUT | `/evaluaciones/{eId}/preguntas/{pId}` | Editar pregunta |
| DELETE | `/evaluaciones/{eId}/preguntas/{pId}` | Eliminar pregunta |

### Candidatos (Evaluador/Admin)
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/evaluaciones/{eId}/candidatos` | Invitar candidato |
| GET | `/evaluaciones/{eId}/candidatos` | Listar candidatos |
| DELETE | `/evaluaciones/{eId}/candidatos/{cId}` | Eliminar candidato |

### Prueba — Candidato (público, por token)
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/prueba/{token}` | Obtener evaluación por token |
| POST | `/prueba/{token}/iniciar` | Iniciar prueba |
| POST | `/prueba/{token}/respuestas` | Enviar respuestas |
| GET | `/prueba/{token}/resultado` | Ver resultado público |

### Análisis IA (Evaluador/Admin)
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/analisis/{candidatoId}` | Ejecutar análisis IA |

### Resultados (Evaluador/Admin)
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/resultados/candidato/{cId}` | Resultado de un candidato |
| GET | `/resultados/evaluacion/{eId}` | Resumen de evaluación |
| GET | `/resultados/evaluacion/{eId}/ranking` | Ranking de candidatos |
| POST | `/resultados/evaluacion/{eId}/comparar` | Comparar candidatos |

### Reportes PDF (Evaluador/Admin)
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/reportes/{cId}/pdf` | PDF de resultado de candidato |
| GET | `/reportes/evaluacion/{eId}/ranking-pdf` | PDF de ranking |

### Sesiones en Vivo (Evaluador/Admin)
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/sesiones` | Crear sesión en vivo |
| GET | `/sesiones/{sId}` | Estado de sesión |

### Sesión en Vivo — Candidato (público, header `X-Candidato-Token`)
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/sesion-vivo/{sId}/iniciar` | Iniciar sesión en vivo |
| GET | `/sesion-vivo/{sId}/pregunta-actual` | Pregunta actual |
| POST | `/sesion-vivo/{sId}/responder` | Responder y avanzar |
| POST | `/sesion-vivo/{sId}/finalizar` | Finalizar sesión |

### SignalR Hub — `/hubs/sesion`
| Método/Evento | Tipo | Descripción |
|---------------|------|-------------|
| `UnirseASesion(sesionId)` | Client → Server | Unirse al grupo |
| `SalirDeSesion(sesionId)` | Client → Server | Salir del grupo |
| `SesionIniciada` | Server → Client | Sesión arrancó |
| `PreguntaCambiada` | Server → Client | Nueva pregunta disponible |
| `SesionCompletada` | Server → Client | Sesión terminada |

---

## Paleta de Colores y Diseño

| Elemento | Color | Uso |
|----------|-------|-----|
| Primary | `#1e3a5f` (azul oscuro) | Navbar, botones principales, headers |
| Accent | `#00b4d8` (cyan) | Links, badges, highlights |
| Success | `#10b981` (verde) | Score alto (≥80), estados activos |
| Warning | `#f59e0b` (ámbar) | Score medio (50-79), estados borrador |
| Danger | `#ef4444` (rojo) | Score bajo (<50), errores, eliminar |
| Background | `#f8fafc` | Fondo general |
| Surface | `#ffffff` | Cards, dialogs |
| Text Primary | `#1e293b` | Texto principal |
| Text Secondary | `#64748b` | Texto secundario, labels |

### Reglas de Score Visual
- **≥ 80**: Verde — badge con icono ✓, glow sutil
- **50 – 79**: Ámbar — badge con icono !, sin glow
- **< 50**: Rojo — badge con icono ✗, borde sutil

---

## Fases de Desarrollo

### Fase F0 — Setup y Arquitectura Base

**Objetivo**: Proyecto Angular listo con Material + Tailwind, rutas base, interceptors y guards.

**Tareas**:
1. Crear proyecto Angular: `ng new evaluacion-tecnica --style=scss --routing --ssr=false`
2. Instalar dependencias:
   - `ng add @angular/material` (tema custom con paleta definida)
   - `npm install -D tailwindcss @tailwindcss/postcss postcss`
   - `npm install @microsoft/signalr`
   - `npm install chart.js ng2-charts`
3. Configurar `tailwind.config.js` con colores custom
4. Crear `environment.ts` y `environment.development.ts` con `apiUrl`
5. Crear modelos TypeScript (interfaces) en `core/models/`
6. Crear `jwt.interceptor.ts` y `error.interceptor.ts`
7. Crear `auth.guard.ts` y `evaluador.guard.ts`
8. Crear `auth.service.ts` con login/registro/logout/isAuthenticated
9. Configurar `app.config.ts` con `provideHttpClient(withInterceptors(...))`
10. Crear `app.routes.ts` con rutas base y lazy loading
11. Crear layout base: navbar con logo, menú lateral colapsable, router-outlet

**Entregable**: App corre, navbar visible, login redirige, interceptors registrados.

**Verificación**:
```
- ng serve corre sin errores
- Navbar se muestra con logo y título
- Ruta /login muestra placeholder
- Ruta /evaluador redirige a /login (guard funciona)
- Console.log del interceptor muestra que agrega token
```

---

### Fase F1 — Autenticación (Login / Registro)

**Objetivo**: Pantallas de Login y Registro funcionales, token almacenado, redirección por rol.

**Pantallas**:

#### Login (`/login`)
- Card centrada con fondo degradado sutil
- Logo arriba
- Campos: Email, Contraseña (con toggle visibilidad)
- Botón "Iniciar Sesión" (Material raised, color primary)
- Link a registro
- Snackbar de error si falla
- Después del login: redirige a `/evaluador/dashboard`

#### Registro (`/registro`)
- Card centrada
- Campos: Nombre, Email, Contraseña, Confirmar contraseña
- Validaciones reactivas (email válido, contraseña min 6 chars, passwords match)
- Botón "Crear Cuenta"
- Snackbar de éxito → redirige a login

**Entregable**: Login y registro funcionan contra la API. Token se guarda en localStorage. Guards redirigen correctamente.

**Verificación**:
```
- Login con credenciales válidas → redirige a /evaluador/dashboard
- Login con credenciales inválidas → snackbar de error
- Registro crea usuario → redirige a login
- Refresh de página → token persiste, no vuelve a login
- Logout → limpia token, redirige a login
- /evaluador/* sin token → redirige a /login
```

---

### Fase F2 — Dashboard del Evaluador y CRUD de Evaluaciones

**Objetivo**: Dashboard con estadísticas, lista de evaluaciones, crear/editar evaluación con preguntas.

**Pantallas**:

#### Dashboard (`/evaluador/dashboard`)
- Cards de resumen: Total evaluaciones, Activas, Candidatos pendientes, Analizadas
- Tabla con últimas evaluaciones (columnas: Título, Tecnología, Nivel, Estado, Candidatos, Acciones)
- Chip de estado con color: Borrador=ámbar, Activa=verde, Cerrada=gris
- Botón FAB "Nueva Evaluación" flotante abajo-derecha

#### Crear Evaluación (`/evaluador/crear`)
- Stepper de Material (3 pasos):
  1. **Datos Generales**: Título, Descripción, Tecnología (autocomplete), Nivel (select), Tiempo límite
  2. **Preguntas**: Lista dinámica. Cada pregunta: Texto (textarea), Tipo (select: Abierta/OpcionMultiple/Codigo), Puntaje máximo. Botón "+ Agregar Pregunta". Arrastrar para reordenar (CDK drag-drop). Máximo 30 preguntas.
  3. **Resumen**: Preview de la evaluación antes de guardar
- Botón "Guardar como Borrador"

#### Detalle/Editar Evaluación (`/evaluador/evaluacion/:id`)
- Tabs de Material:
  - **Info General**: Datos editables si estado = Borrador
  - **Preguntas**: CRUD de preguntas (agregar, editar inline, eliminar)
  - **Candidatos**: Lista con acciones (ver resultado)
- Toolbar con acciones: Activar, Cerrar, Eliminar (con dialog de confirmación)
- Estado de la evaluación visible con chip de color

**Entregable**: CRUD completo de evaluaciones, stepper de creación, gestión de preguntas.

**Verificación**:
```
- Dashboard muestra estadísticas calculadas
- Crear evaluación con 3 preguntas → aparece en dashboard
- Editar título de evaluación → se refleja
- Agregar pregunta a evaluación existente → aparece en lista
- Eliminar pregunta → desaparece con confirmación
- Activar evaluación → chip cambia a verde
- Cerrar evaluación → chip cambia a gris
- Eliminar evaluación → desaparece del dashboard
```

---

### Fase F3 — Gestión de Candidatos e Invitaciones

**Objetivo**: Invitar candidatos a evaluaciones, ver lista, copiar link de prueba.

**Pantallas**:

#### Tab Candidatos (dentro de Detalle Evaluación)
- Tabla: Nombre, Email, Estado (Invitado/EnProgreso/Completado/Analizado), Fecha invitación, Acciones
- Botón "Invitar Candidato" → Dialog con campos: Nombre, Email
- Al invitar: se genera link `http://localhost:4200/candidato/{token}`
- Botón "Copiar Link" con feedback visual (icono check temporal)
- Acciones por candidato:
  - Copiar link
  - Ejecutar análisis IA (si estado = Completado)
  - Ver resultado (si estado = Analizado)
  - Eliminar candidato (con confirmación)
- Indicador visual de estado con iconos y colores

**Entregable**: Invitaciones funcionales, links generados, acciones contextuales.

**Verificación**:
```
- Invitar candidato → aparece en tabla con estado "Invitado"
- Copiar link → clipboard tiene el URL correcto
- Link funciona en nueva pestaña (candidato ve instrucciones)
- Eliminar candidato → desaparece con confirmación
```

---

### Fase F4 — Flujo del Candidato (Prueba Asíncrona)

**Objetivo**: El candidato accede por token, ve instrucciones, responde preguntas y envía.

**Pantallas**:

#### Instrucciones (`/candidato/:token`)
- Card con: Título de evaluación, Tecnología, Nivel, Tiempo límite, Número de preguntas
- Nombre del evaluador
- Reglas: "Una vez iniciada no puede pausarse", "Todas las preguntas son obligatorias"
- Botón "Iniciar Prueba" (grande, color primary)
- Diseño limpio, sin navbar del evaluador

#### Responder Prueba (`/candidato/:token/responder`)
- Navegación: barra de progreso arriba (pregunta X de Y)
- Countdown timer visible (si hay tiempo límite)
- Cada pregunta: número, texto, campo de respuesta según tipo:
  - Abierta: textarea
  - Código: textarea monospace con fondo oscuro
  - Opción múltiple: radio buttons
- Navegación: "Anterior" / "Siguiente"
- Botón "Enviar Prueba" en la última pregunta (con confirmación)
- Si el tiempo se agota: envío automático con lo respondido

#### Mi Resultado (`/candidato/:token/resultado`)
- Si no hay resultado aún: mensaje "Prueba en revisión, pronto recibirás tus resultados"
- Si hay resultado:
  - Score general con `score-badge` grande
  - Gráfica radar con scores por pregunta
  - Lista de fortalezas (verde) y brechas (rojo)
  - Recomendación destacada en card especial
  - Botón "Descargar PDF" (descarga desde API)

**Entregable**: Flujo completo del candidato: instrucciones → responder → resultado.

**Verificación**:
```
- Candidato accede con token → ve instrucciones con datos correctos
- Iniciar prueba → muestra primera pregunta con timer
- Responder todas y enviar → mensaje de éxito
- Intentar enviar de nuevo → error "ya respondiste"
- Ver resultado (después de análisis) → score y detalles visibles
- Descargar PDF → archivo se descarga
```

---

### Fase F5 — Análisis IA y Resultados del Evaluador

**Objetivo**: El evaluador ejecuta el análisis IA y ve resultados detallados.

**Pantallas**:

#### Ejecución del Análisis (desde Tab Candidatos)
- Botón "Analizar con IA" junto a candidatos con estado Completado
- Loading overlay mientras se procesa
- Snackbar de éxito al terminar → estado cambia a "Analizado"

#### Ver Resultado (`/evaluador/evaluacion/:eId/resultado/:cId`)
- Header: Nombre del candidato, Email, Fecha de prueba, Tiempo invertido
- Score general grande con `score-badge`
- Gráfica de barras: score por pregunta
- Tabla detallada: Pregunta | Respuesta del candidato | Score | Justificación IA
- Sección "Fortalezas" con chips verdes
- Sección "Brechas" con chips rojos
- Recomendación en card destacada
- Botones: "Descargar PDF", "Volver"

#### Resumen de Evaluación (`/evaluador/evaluacion/:eId/resultados`)
- Card con estadísticas: Promedio general, Candidatos analizados, Score más alto, Score más bajo
- Tabla resumen: Candidato | Score | Estado | Acciones (ver detalle)
- Gráfica de distribución de scores (bar chart)

**Entregable**: Análisis IA ejecutable desde UI, resultados con gráficas y detalle.

**Verificación**:
```
- Ejecutar análisis → loading → resultado aparece
- Ver resultado muestra score, gráficas, fortalezas, brechas
- PDF se descarga con datos correctos
- Resumen de evaluación muestra estadísticas
```

---

### Fase F6 — Ranking y Comparación de Candidatos

**Objetivo**: Tabla de ranking con posiciones, comparación side-by-side.

**Pantallas**:

#### Ranking (`/evaluador/evaluacion/:eId/ranking`)
- Tabla con posiciones:
  - Columnas: Posición, Candidato, Score, Fortalezas top, Acciones
  - Fila #1 con fondo dorado sutil, #2 plata, #3 bronce
  - Score con `score-badge`
- Botón "Descargar Ranking PDF"
- Botón "Comparar Seleccionados" (checkbox de selección múltiple, mínimo 2)

#### Comparar Candidatos (`/evaluador/evaluacion/:eId/comparar`)
- Cards side-by-side (2-4 candidatos)
- Cada card: Nombre, Score general, Fortalezas, Brechas
- Gráfica radar superpuesta comparando scores por pregunta
- Tabla comparativa: Pregunta | Score candidato A | Score candidato B | ...
- Indicador visual de quién ganó en cada pregunta (highlight verde)

**Entregable**: Ranking visual con posiciones, comparación side-by-side con gráficas.

**Verificación**:
```
- Ranking muestra posiciones ordenadas
- Colores de podio en top 3
- Descargar ranking PDF funciona
- Seleccionar 2+ candidatos → comparar muestra cards
- Gráfica radar muestra diferencias
```

---

### Fase F7 — Sesiones en Vivo (SignalR)

**Objetivo**: Evaluador crea sesión en vivo, candidato responde pregunta por pregunta en tiempo real.

**Pantallas**:

#### Panel del Evaluador (`/evaluador/evaluacion/:eId/sesion-vivo/:sId`)
- Estado de la sesión: Creada / EnProgreso / Completada
- Pregunta actual visible con número y texto
- Indicador: "Esperando respuesta del candidato..."
- Timer de la pregunta actual
- Historial de preguntas respondidas con mini-score
- Al completarse: botón "Ver Resultado"
- Conexión SignalR: escucha `PreguntaCambiada` y `SesionCompletada`

#### Vista del Candidato (`/candidato/:token/sesion-vivo/:sId`)
- Sin navbar, diseño limpio
- Pregunta actual con número y timer
- Campo de respuesta (textarea o code)
- Botón "Enviar Respuesta" → avanza a siguiente
- Progreso: "Pregunta X de Y"
- Al completarse: mensaje de agradecimiento
- Conexión SignalR: recibe actualizaciones en tiempo real

**Entregable**: Sesión en vivo funcional con SignalR, ambas vistas sincronizadas.

**Verificación**:
```
- Crear sesión → estado "Creada"
- Candidato inicia → evaluador ve "EnProgreso" en tiempo real
- Candidato responde → evaluador ve pregunta avanzar
- Sesión completada → ambos ven feedback
- Conexión perdida → reconexión automática
```

---

### Fase F8 — Pulido Final y UX

**Objetivo**: Animaciones, responsive, estados vacíos, errores, accesibilidad.

**Tareas**:
1. **Responsive**: Sidebar colapsable en mobile, tablas scroll horizontal, cards stack vertical
2. **Estados vacíos**: Ilustración + mensaje cuando no hay evaluaciones/candidatos/resultados
3. **Loading states**: Skeleton loaders en tablas y cards
4. **Animaciones**: Transiciones de ruta (fade), hover en cards (elevación), entrada de elementos (slide-up)
5. **Snackbar global**: Mensajes de éxito/error consistentes via `error.interceptor`
6. **Accesibilidad**: aria-labels, focus management, keyboard navigation
7. **Tema oscuro** (opcional): Toggle en navbar, persiste en localStorage
8. **Favicon y título** dinámico por ruta

**Verificación**:
```
- App se ve bien en 320px, 768px, 1024px, 1440px
- Estados vacíos con ilustración cuando no hay datos
- Skeleton loaders durante cargas
- Navegación completa con teclado
- Errores HTTP muestran snackbar
```

---

## Resumen de Fases

| Fase | Nombre | Componentes Clave |
|------|--------|-------------------|
| F0 | Setup y Arquitectura | Proyecto, Material, Tailwind, guards, interceptors, rutas |
| F1 | Autenticación | Login, Registro, JWT, guards funcionales |
| F2 | Dashboard y Evaluaciones | Dashboard, Crear/Editar/Detalle evaluación, CRUD preguntas |
| F3 | Candidatos | Invitaciones, tabla candidatos, copiar link |
| F4 | Flujo del Candidato | Instrucciones, responder prueba, ver resultado |
| F5 | Análisis IA y Resultados | Ejecutar análisis, resultado detallado, gráficas |
| F6 | Ranking y Comparación | Tabla ranking, comparación side-by-side, radar chart |
| F7 | Sesiones en Vivo | SignalR, panel evaluador, vista candidato en vivo |
| F8 | Pulido Final | Responsive, animaciones, estados vacíos, accesibilidad |

---

## Reglas de Desarrollo

1. **No pasar a la siguiente fase sin verificar la anterior**
2. **SOLID y Clean Architecture**: servicios con responsabilidad única, separación de concerns
3. **Sin comentarios**: el código debe entenderse solo
4. **Standalone components**: no usar NgModules
5. **Reactive Forms**: para todos los formularios
6. **Signals**: preferir signals sobre BehaviorSubject donde aplique
7. **OnPush**: todas las estrategias de detección de cambios
8. **Lazy loading**: cada módulo de rutas se carga bajo demanda
9. **Manejo de errores centralizado**: via interceptor
10. **No hardcodear URLs**: usar `environment.apiUrl`
