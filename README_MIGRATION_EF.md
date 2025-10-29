# Migración de GestorCerrarOrdenDeInspeccion a Entity Framework

## Resumen de Cambios

Se ha migrado la clase `GestorCerrarOrdenDeInspeccion` para usar Entity Framework Core en lugar del `DataLoaderService` que leía datos desde JSON.

## Principales Cambios Realizados

### 1. **Dependencias Actualizadas**

- Removido: `DataLoaderService`
- Agregado: `ApplicationDbContext` de Entity Framework
- Agregado: `Microsoft.EntityFrameworkCore` usando directiva

### 2. **Constructor Modificado**

```csharp
// Antes
public GestorCerrarOrdenDeInspeccion(DataLoaderService dataLoader, IObservadorNotificacion emailService)

// Después
public GestorCerrarOrdenDeInspeccion(ApplicationDbContext context, IObservadorNotificacion emailService)
```

### 3. **Métodos Convertidos a Asíncronos**

Todos los métodos que acceden a la base de datos ahora son asíncronos:

| Método Original                           | Método Nuevo                                   |
| ----------------------------------------- | ---------------------------------------------- |
| `BuscarEmpleadoRI()`                      | `BuscarEmpleadoRIAsync()`                      |
| `BuscarOrdenInspeccion()`                 | `BuscarOrdenInspeccionAsync()`                 |
| `TomarOrdenSeleccionada()`                | `TomarOrdenSeleccionadaAsync()`                |
| `BuscarMotivoFueraDeServicio()`           | `BuscarMotivoFueraDeServicioAsync()`           |
| `CerrarOrdenInspeccion()`                 | `CerrarOrdenInspeccionAsync()`                 |
| `BuscarEstadoFueraServicio()`             | `BuscarEstadoFueraServicioAsync()`             |
| `TomarMotivoFueraDeServicioYComentario()` | `TomarMotivoFueraDeServicioYComentarioAsync()` |
| `ObtenerMailsResponsableReparacion()`     | `ObtenerMailsResponsableReparacionAsync()`     |
| `EnviarNotificacionPorMail()`             | `EnviarNotificacionPorMailAsync()`             |

### 4. **Consultas Entity Framework**

#### Ejemplo de Migración - BuscarOrdenInspeccionAsync:

```csharp
// Antes (JSON)
foreach (var oi in _dataLoader.OrdenesDeInspeccion)
{
    if (oi.EsDelEmpleado(empleado) && oi.EstaRealizada())
    {
        // lógica...
    }
}

// Después (Entity Framework)
var ordenesDeInspeccion = await _context.OrdenesDeInspeccion
    .Include(oi => oi.Empleado)
    .Include(oi => oi.Estado)
    .Include(oi => oi.EstacionSismologica)
    .ThenInclude(es => es.Sismografo)
    .Where(oi => oi.Empleado.Mail == empleado.Mail)
    .ToListAsync();
```

### 5. **Includes para Cargar Relaciones**

Se agregaron `.Include()` y `.ThenInclude()` para cargar las entidades relacionadas:

- **OrdenesDeInspeccion**: Empleado, Estado, EstacionSismologica, Sismografo, CambioEstado
- **Empleados**: Rol
- **MotivosFueraDeServicio**: TipoMotivo
- **Sesiones**: Usuario, Empleado

### 6. **Persistencia de Cambios**

Se agregó `await _context.SaveChangesAsync()` en métodos que modifican datos:

- `CerrarOrdenInspeccionAsync()`
- `BuscarEstadoFueraServicioAsync()`

### 7. **Implementación de Interfaz**

Se implementaron los métodos de `IObservadorNotificacion`:

- `NotificarCierreOrdenInspeccion(string mensaje, List<string> destinatarios)`
- `NotificarCierreOrdenInspeccion(string asunto, string mensaje, List<string> destinatarios)`

## Nuevas Funcionalidades

### Controlador de API

Se creó `GestorCerrarOrdenController` que expone endpoints REST:

- `GET /api/gestorcerrarorden/empleado-ri`
- `GET /api/gestorcerrarorden/ordenes-inspeccion/{empleadoMail}`
- `GET /api/gestorcerrarorden/motivos-fuera-servicio`
- `POST /api/gestorcerrarorden/cerrar-orden`
- `GET /api/gestorcerrarorden/responsables-reparacion`

## Cómo Usar

### 1. Inyección de Dependencias

```csharp
// En Program.cs o Startup.cs
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
services.AddScoped<IObservadorNotificacion, EmailService>();
```

### 2. Uso en Controlador

```csharp
public class MiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IObservadorNotificacion _emailService;

    public MiController(ApplicationDbContext context, IObservadorNotificacion emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpPost("cerrar-orden")]
    public async Task<ActionResult> CerrarOrden(CerrarOrdenRequest request)
    {
        var gestor = new GestorCerrarOrdenDeInspeccion(_context, _emailService);

        await gestor.TomarOrdenSeleccionadaAsync(request.NumeroOrden);
        gestor.TomarObservacion(request.Observacion);
        await gestor.TomarMotivoFueraDeServicioYComentarioAsync(request.Motivos);
        gestor.ValidarObsYComentario();

        var resultado = await gestor.CerrarOrdenInspeccionAsync();
        await gestor.EnviarNotificacionPorMailAsync();

        return Ok(resultado);
    }
}
```

## Beneficios de la Migración

1. **Performance**: Las consultas EF son más eficientes y pueden usar índices de base de datos
2. **Escalabilidad**: Soporte para múltiples usuarios concurrentes
3. **Integridad**: Transacciones y validaciones a nivel de base de datos
4. **Mantenibilidad**: Consultas LINQ más legibles y mantenibles
5. **Consistencia**: Datos siempre actualizados en tiempo real

## Notas Importantes

- Todos los métodos de acceso a datos son ahora asíncronos
- Es necesario usar `await` al llamar estos métodos
- Los cambios se persisten automáticamente con `SaveChangesAsync()`
- Las relaciones se cargan explícitamente con `Include()`
