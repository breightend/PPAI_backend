using PPAI_backend.datos.dtos;
using PPAI_backend.models.entities;
using PPAI_backend.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar nuestros servicios
builder.Services.AddSingleton<JsonMappingService>();
builder.Services.AddSingleton<DataLoaderService>();
builder.Services.AddScoped<GestorCerrarOrdenDeInspeccion>();

var app = builder.Build();

// Cargar todos los datos al inicio de la aplicación
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dataLoader = scope.ServiceProvider.GetRequiredService<DataLoaderService>();
        await dataLoader.LoadAllDataAsync("datos/datos.json");
        Console.WriteLine("✅ Datos cargados exitosamente al iniciar la aplicación");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error al cargar datos iniciales: {ex.Message}");
    throw; // Stop application if data loading fails
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();





// Endpoint que carga todos los datos y devuelve todos los objetos generados
app.MapGet("/todos-los-datos", async (DataLoaderService dataLoader) =>
{
    try
    {
        await dataLoader.LoadAllDataAsync("datos/datos.json");

        return Results.Ok(new
        {
            message = "Todos los datos cargados y mapeados exitosamente",
            datos = new
            {
                empleados = dataLoader.Empleados,
                usuarios = dataLoader.Usuarios,
                estados = dataLoader.Estados,
                motivos = dataLoader.Motivos,
                sismografos = dataLoader.Sismografos,
                estacionesSismologicas = dataLoader.EstacionesSismologicas,
                ordenesDeInspeccion = dataLoader.OrdenesDeInspeccion,
                sesiones = dataLoader.Sesiones
            },
            estadisticas = new
            {
                totalEmpleados = dataLoader.Empleados.Count,
                totalUsuarios = dataLoader.Usuarios.Count,
                totalEstados = dataLoader.Estados.Count,
                totalMotivos = dataLoader.Motivos.Count,
                totalSismografos = dataLoader.Sismografos.Count,
                totalEstacionesSismologicas = dataLoader.EstacionesSismologicas.Count,
                totalOrdenesDeInspeccion = dataLoader.OrdenesDeInspeccion.Count,
                totalSesiones = dataLoader.Sesiones.Count
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al cargar y mapear los datos: {ex.Message}");
    }
});


app.MapGet("/empleado-logueado", (GestorCerrarOrdenDeInspeccion gestor) =>
{
    var empleado = gestor.BuscarEmpleadoRI();
    if (empleado == null)
    {
        return Results.NotFound("No se encontró empleado logueado.");
    }
    return Results.Ok(empleado);
});

app.MapGet("/ordenes-inspeccion", (GestorCerrarOrdenDeInspeccion gestor) => // Endpoint para mostrar ordenes de inspeccion:
{
    var empleado = gestor.BuscarEmpleadoRI();
    if (empleado == null)
    {
        return Results.NotFound("Empleado no encontrado.");
    }
    var ordenes = gestor.BuscarOrdenInspeccion(empleado);
    if (ordenes == null || ordenes.Count == 0)
    {
        return Results.NotFound("No se encontraron órdenes de inspección.");
    }
    var ordenesOrdenadas = gestor.OrdenarOrdenInspeccion(ordenes);
    if (ordenesOrdenadas == null || ordenesOrdenadas.Count == 0)
    {
        return Results.NotFound("No se encontraron órdenes de inspección.");
    }
    return Results.Ok(ordenesOrdenadas);
});

app.MapGet("/motivos", (GestorCerrarOrdenDeInspeccion gestor) => // Endpoint para mostrar Motivos:
{
    var motivos = gestor.BuscarMotivoFueraDeServicio();
    return Results.Ok(motivos);
});

app.MapPost("/motivos-seleccionados", (MotivosSeleccionadosDTO dto, DataLoaderService dataLoader) =>
{
    // Mapear los DTOs a entidades Motivo usando los datos base de motivos
    var motivosBase = dataLoader.Motivos;
    var motivosSeleccionados = dto.Motivos
        .Select(m =>
        {
            var baseMotivo = motivosBase.FirstOrDefault(b => b.Id == m.IdMotivo);
            return baseMotivo == null
                ? null
                : new Motivo
                {
                    Id = baseMotivo.Id,
                    Descripcion = baseMotivo.Descripcion,
                    Comentario = m.Comentario
                };
        })
        .Where(m => m != null)
        .ToList();

    dataLoader.GuardarMotivosSeleccionados(motivosSeleccionados!);
    return Results.Ok("Motivos registrados correctamente.");
});

// Nuevos endpoints para el flujo completo de cierre de órdenes

/* app.MapPost("/seleccionar-orden", (int numeroOrden, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        gestor.TomarMotivoFueraDeServicio(numeroOrden);
        return Results.Ok($"Orden {numeroOrden} seleccionada correctamente.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al seleccionar orden: {ex.Message}");
    }
}); */

app.MapPost("/agregar-observacion", (string observacion, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        gestor.tomarObservacion(observacion);
        return Results.Ok("Observación agregada correctamente.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al agregar observación: {ex.Message}");
    }
});

/* app.MapPost("/registrar-motivos", (List<MotivoDTO> motivosSeleccionados, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        gestor.TomarMotivoFueraDeServicio(motivosSeleccionados);
        return Results.Ok("Motivos registrados correctamente.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al registrar motivos: {ex.Message}");
    }
});
 */
app.MapPost("/cerrar-orden", (GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        gestor.ValidarObsYComentario();
        var resultado = gestor.CerrarOrdenSeleccionada();
        return Results.Ok(resultado);
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al cerrar orden: {ex.Message}");
    }
});


// Nuevo endpoint para cargar y mostrar datos desde JSON
app.MapGet("/datos-json", async (DataLoaderService dataLoader) =>
{
    try
    {
        await dataLoader.LoadAllDataAsync("datos/datos.json");

        return Results.Ok(new
        {
            message = "Datos cargados exitosamente",
            empleados = dataLoader.Empleados.Count,
            usuarios = dataLoader.Usuarios.Count,
            estados = dataLoader.Estados.Count,
            motivos = dataLoader.Motivos.Count,
            sismografos = dataLoader.Sismografos.Count,
            estacionesSismologicas = dataLoader.EstacionesSismologicas.Count,
            ordenesDeInspeccion = dataLoader.OrdenesDeInspeccion.Count,
            sesiones = dataLoader.Sesiones.Count
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al cargar datos: {ex.Message}");
    }
});


app.Run();

async Task CargarDatosIniciales(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var dataLoader = scope.ServiceProvider.GetRequiredService<DataLoaderService>();

    try
    {
        await dataLoader.LoadAllDataAsync("datos/datos.json");
        Console.WriteLine("✅ Datos iniciales cargados correctamente al iniciar la aplicación");
        Console.WriteLine($"📊 Resumen de datos cargados:");
        Console.WriteLine($"   - {dataLoader.Empleados.Count} empleados");
        Console.WriteLine($"   - {dataLoader.Usuarios.Count} usuarios");
        Console.WriteLine($"   - {dataLoader.Estados.Count} estados");
        Console.WriteLine($"   - {dataLoader.Motivos.Count} motivos");
        Console.WriteLine($"   - {dataLoader.Sismografos.Count} sismógrafos");
        Console.WriteLine($"   - {dataLoader.EstacionesSismologicas.Count} estaciones sismológicas");
        Console.WriteLine($"   - {dataLoader.OrdenesDeInspeccion.Count} órdenes de inspección");
        Console.WriteLine($"   - {dataLoader.Sesiones.Count} sesiones");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al cargar datos iniciales: {ex.Message}");
        throw; // This will stop the application if data loading fails
    }
}
