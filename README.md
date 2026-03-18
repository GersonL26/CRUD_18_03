# CRUD Genérico Basado en Metadatos

## Descripción
Sistema CRUD genérico que gestiona múltiples entidades de dominio (Sucursal, Supervisor, Producto, Categoría, Proveedor) a través de un único controlador y servicio genérico, utilizando un esquema de metadatos para resolver dinámicamente los tipos de entidad y DTO.

## Arquitectura
**Clean Architecture** con separación en capas:

```
CRUD_18_03 (API)              → Controlador genérico, configuración, Program.cs
CRUD_18_03.Application        → Servicio genérico, interfaces, DTOs, metadatos
CRUD_18_03.Domain             → Entidades de dominio
CRUD_18_03.Infrastructure     → EF Core, DbContext, repositorio genérico
```

## Credenciales MySQL (DBeaver)

| Campo        | Valor                              |
|--------------|-------------------------------------|
| Host         | mysql-k3s-dev.grupofarsiman.io     |
| Puerto       | 3306                               |
| Base de datos| CrudGenerico                       |
| Usuario      | desarrollo                         |
| Contraseña   | dsrcorp                            |

## Endpoints

Todos los endpoints siguen la convención `/api/entity/{entityName}`:

| Método | Ruta                              | Descripción                    |
|--------|-----------------------------------|--------------------------------|
| GET    | /api/entity/{entityName}          | Obtener todas las entidades    |
| GET    | /api/entity/{entityName}/{id}     | Obtener entidad por ID         |
| POST   | /api/entity/{entityName}          | Crear nueva entidad            |
| PUT    | /api/entity/{entityName}/{id}     | Actualizar entidad (parcial)   |
| DELETE | /api/entity/{entityName}/{id}     | Eliminar entidad               |

### Entidades disponibles
- `sucursal` → Nombre, Ubicación, Activo
- `supervisor` → Nombre, Correo, SucursalId
- `producto` → Nombre, CategoríaId, Precio, Activo
- `categoria` → Descripción, Código
- `proveedor` → Nombre, Teléfono, Correo

## Plan de Desarrollo por Fases

### Fase 1: Estructura Clean Architecture
- [ ] Crear proyecto `CRUD_18_03.Domain`
- [ ] Crear proyecto `CRUD_18_03.Application`
- [ ] Crear proyecto `CRUD_18_03.Infrastructure`
- [x] Configurar referencias entre proyectos
- [x] Actualizar archivo de solución

### Fase 2: Entidades de Dominio
- [x] Crear entidad base `BaseEntity`
- [x] Crear entidad `Sucursal`
- [x] Crear entidad `Supervisor`
- [x] Crear entidad `Producto`
- [x] Crear entidad `Categoria`
- [x] Crear entidad `Proveedor`

### Fase 3: DTOs y Mapeo
- [x] Crear DTOs de creación para cada entidad
- [x] Crear DTOs de actualización para cada entidad
- [x] Crear DTOs de respuesta para cada entidad
- [x] Configurar mapeo entre entidades y DTOs

### Fase 4: MySQL + EF Core
- [x] Instalar paquetes NuGet necesarios
- [x] Crear DbContext con configuraciones
- [x] Configurar cadena de conexión (variables de entorno)
- [x] Crear script SQL y ejecutar en servidor

### Fase 5: Servicio Genérico (acceso a datos directo vía DbContext)
- [x] Crear interfaz `IGenericService`
- [x] Implementar `GenericService` con acceso directo a `DbContext`

### Fase 6: Proveedor de Metadatos
- [x] Crear clase `EntityMetadata`
- [x] Crear interfaz `IEntityMetadataProvider`
- [x] Implementar `EntityMetadataProvider`
- [x] Registrar todas las entidades con sus DTOs

### Fase 7: Servicio Genérico
- [x] Crear interfaz `IGenericService`
- [x] Implementar `GenericService`
- [x] Soporte para actualizaciones parciales

### Fase 8: Controlador Genérico
- [x] Crear `EntityController` único
- [x] Implementar los 5 endpoints REST
- [x] Integrar con metadatos y servicio genérico

### Fase 9: Manejo de Errores + Swagger
- [x] Middleware de manejo de excepciones
- [x] Respuestas HTTP estándar (200, 201, 400, 404, 500)
- [x] Configurar documentación OpenAPI (Scalar)

### Fase 10: Compilación y Validación
- [x] Compilar sin errores
- [x] Probar los 5 casos de prueba del ejercicio
- [x] Validar actualizaciones parciales
- [x] Verificar extensibilidad

## Casos de Prueba

1. **POST** `/api/entity/sucursal` → Crear sucursal
2. **PUT** `/api/entity/producto/{id}` → Actualización parcial (solo precio)
3. **GET** `/api/entity/supervisor` → Obtener todos los supervisores
4. **GET** `/api/entity/proveedor/{id}` → Obtener proveedor específico
5. **DELETE** `/api/entity/categoria/{id}` → Eliminar categoría
