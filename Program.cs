using PPAI_backend.datos.dtos;
using PPAI_backend.models.entities;
using PPAI_backend.services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5199");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddSingleton<JsonMappingService>();
builder.Services.AddSingleton<DataLoaderService>();
builder.Services.AddScoped<GestorCerrarOrdenDeInspeccion>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();


// Cargar todos los datos y configurar relaciones al inicio de la aplicación
try
{
    await ConfigurarRelacionesEntidades(app.Services);
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error al cargar datos iniciales: {ex.Message}");
    throw;
}

// Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ========== ENDPOINTS QUE SOLO USAN EL GESTOR ==========

// Ejemplo perfecto: Navegación Sesion -> Usuario -> Empleado
app.MapGet("/empleado-logueado", (GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        // El gestor maneja toda la lógica: 
        // 1. Busca la sesión activa
        // 2. De la sesión obtiene el usuario
        // 3. Del usuario obtiene el empleado
        var empleado = gestor.BuscarEmpleadoRI();

        return Results.Ok(new
        {
            success = true,
            message = "Empleado obtenido navegando: Sesion -> Usuario -> Empleado",
            data = new
            {
                nombre = empleado.Nombre,
                apellido = empleado.Apellido,
                mail = empleado.Mail,
                telefono = empleado.Telefono,
                rol = empleado.Rol.Descripcion
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error: {ex.Message}");
    }
});

app.MapGet("/motivos", (GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        // El gestor se encarga de buscar los motivos desde los datos cargados
        var motivos = gestor.BuscarMotivoFueraDeServicio();

        // Mapear a una estructura más clara para el frontend
        var motivosResponse = motivos.Select(m => new
        {
            id = m.Id,
            descripcion = m.Descripcion, // Ahora viene de TipoMotivo.Descripcion
            tipoMotivo = new
            {
                id = m.TipoMotivo.Id,
                descripcion = m.TipoMotivo.Descripcion
            },
            comentario = m.Comentario
        }).ToList();

        return Results.Ok(new
        {
            success = true,
            message = "Motivos obtenidos desde el gestor con estructura TipoMotivo",
            data = motivosResponse
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error: {ex.Message}");
    }
});

app.MapGet("/ordenes-inspeccion", (GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        // El gestor maneja toda la lógica:
        // 1. Obtiene el empleado logueado (Sesion -> Usuario -> Empleado)
        // 2. Busca las órdenes de ese empleado
        // 3. Las ordena por fecha
        var empleado = gestor.BuscarEmpleadoRI();
        var ordenes = gestor.BuscarOrdenInspeccion(empleado);

        if (ordenes == null || ordenes.Count == 0)
        {
            return Results.Ok(new
            {
                success = true,
                message = "No se encontraron órdenes de inspección para el empleado",
                data = new List<object>()
            });
        }

        var ordenesOrdenadas = gestor.OrdenarOrdenInspeccion(ordenes);

        return Results.Ok(new
        {
            success = true,
            message = $"Órdenes encontradas para {empleado.Nombre} {empleado.Apellido}",
            data = ordenesOrdenadas
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error: {ex.Message}");
    }
});

// ========== ENDPOINTS QUE SOLO USAN EL GESTOR ==========

// ========== ENDPOINTS QUE SOLO USAN EL GESTOR ==========

app.MapPost("/agregar-observacion", (ObservationRequest request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        // El gestor maneja toda la lógica interna
        gestor.tomarOrdenSeleccionada(request.OrderId);
        gestor.tomarObservacion(request.Observation);
        return Results.Ok("Observación agregada correctamente.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al agregar observación: {ex.Message}");
    }
});

app.MapPost("/cerrar-orden", (CerrarOrdenRequest request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        // El gestor maneja toda la lógica de cierre de orden
        gestor.tomarOrdenSeleccionada(request.OrdenId);
        gestor.tomarObservacion(request.Observation);
        gestor.TomarMotivosSeleccionados(request.Motivos);
        gestor.ValidarObsYComentario();

        var resultado = gestor.CerrarOrdenSeleccionada();
        return Results.Ok(new
        {
            success = true,
            message = resultado
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al cerrar orden: {ex.Message}");
    }
});

// Función para inicializar relaciones entre entidades
async Task ConfigurarRelacionesEntidades(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var dataLoader = scope.ServiceProvider.GetRequiredService<DataLoaderService>();

    try
    {
        // Cargar los datos primero
        await dataLoader.LoadAllDataAsync("datos/datos.json");

        // Configurar relaciones Usuario -> Empleado
        foreach (var usuario in dataLoader.Usuarios)
        {
            // El usuario ya tiene la relación con empleado establecida por JsonMappingService
            // Solo verificamos que la relación exista
            if (usuario.Empleado == null)
            {
                Console.WriteLine($"⚠️ Usuario {usuario.NombreUsuario} no tiene empleado asignado");
            }
        }

        // Crear sesión activa simulada para pruebas
        // Buscar un usuario con rol "Responsable de Inspección" 
        var usuarioRI = dataLoader.Usuarios.FirstOrDefault(u =>
            u.Empleado?.Rol?.Descripcion == "Responsable de Inspección");

        if (usuarioRI != null)
        {
            var sesionActiva = new Sesion
            {
                FechaHoraInicio = DateTime.Now.AddHours(-2),
                FechaHoraFin = default, // Sin fecha fin = sesión activa
                Usuario = usuarioRI
            };

            // Agregar la sesión activa a la lista
            dataLoader.Sesiones.Add(sesionActiva);
        }

        Console.WriteLine("✅ Relaciones entre entidades configuradas correctamente");
        Console.WriteLine($"📊 Sesión activa configurada para: {usuarioRI?.Empleado?.Nombre} {usuarioRI?.Empleado?.Apellido}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al configurar relaciones: {ex.Message}");
        throw;
    }
}

app.Run();
