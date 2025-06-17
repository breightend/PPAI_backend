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
        //TODO: mandar mail para saber que es lo que quieren de mi :(
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
        var motivos = gestor.BuscarMotivoFueraDeServicio();

        var motivosResponse = motivos.Select(m => new
        {
            id = m.Id,
            descripcion = m.Descripcion,
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

app.MapPost("/agregar-observacion", (ObservationRequest request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        // El gestor maneja toda la lógica interna
        gestor.TomarOrdenSeleccionada(request.OrderId);
        gestor.TomarObservacion(request.Observation);
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
        gestor.TomarOrdenSeleccionada(request.OrdenId);
        gestor.TomarObservacion(request.Observation);
        gestor.TomarMotivosSeleccionados(request.Motivos);
        gestor.ValidarObsYComentario();

        var resultado = gestor.CerrarOrdenDeInspeccion();
        // Obtener la orden cerrada y sus datos relevantes
        var orden = gestor.GetOrdenSeleccionada();
        if (orden == null)
            return Results.BadRequest("No se encontró la orden seleccionada.");
        var sismografo = orden.EstacionSismologica?.Sismografo;
        if (sismografo == null)
            return Results.BadRequest("No se encontró el sismógrafo asociado a la orden.");
        var cambioEstadoFS = sismografo.CambioEstado
            .Where(ce => ce.Estado.Nombre.ToLower() == "fuera de servicio")
            .OrderByDescending(ce => ce.FechaHoraInicio)
            .FirstOrDefault();

        var motivos = cambioEstadoFS?.Motivos.Select(m => new
        {
            id = m.Id,
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
            message = resultado,
            cierre = new
            {
                identificadorSismografo = sismografo.IdentificadorSismografo,
                estado = cambioEstadoFS?.Estado.Nombre,
                fechaHoraRegistro = cambioEstadoFS?.FechaHoraInicio,
                motivos = motivos
            }
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
        await dataLoader.LoadAllDataAsync("datos/datos.json");

        foreach (var usuario in dataLoader.Usuarios)
        {
            if (usuario.Empleado == null)
            {
                Console.WriteLine($"⚠️ Usuario {usuario.NombreUsuario} no tiene empleado asignado");
            }
        }

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
