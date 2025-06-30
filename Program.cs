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



try
{
    await ConfigurarRelacionesEntidades(app.Services);
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error al cargar datos iniciales: {ex.Message}");
    throw;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/empleado-logueado", (GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {

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

app.MapPost("/motivos-seleccionados", (MotivosSeleccionadosDTO request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        foreach (var motivo in request.Motivos)
        {
            Console.WriteLine($"ID: {motivo.IdMotivo}, Comentario: {motivo.Comentario}");
        }
        gestor.TomarMotivoFueraDeServicioYComentario(request.Motivos);

        return Results.Ok("Motivos recibidos correctamente.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al recibir los motivos: {ex.Message}");
    }
});

app.MapPost("/confirmar-cierre", (ConfirmarRequest request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        ConfirmarRequest confirmarRequest = new ConfirmarRequest
        {
            confirmado = request.confirmado
        };
        if (!confirmarRequest.confirmado)
        {
            return Results.BadRequest("El cierre no ha sido confirmado.");
        }
        gestor.Confirmar();

        gestor.EnviarNotificacionPorMail();

        gestor.PublicarMonitores();

        return Results.Ok("Cierre confirmado correctamente.");


    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al confirmar cierre: {ex.Message}");
    }
});



app.MapPost("/agregar-observacion", (ObservationRequest request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
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
        gestor.TomarMotivoFueraDeServicioYComentario(request.Motivos);
        gestor.ValidarObsYComentario();
        gestor.BuscarEstadoCerrada();

        var resultado = gestor.CerrarOrdenInspeccion();
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

app.MapPost("/buscar-estado-fuera-servicio", (BuscarEstadoFueraServicioRequest request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        gestor.TomarOrdenSeleccionada(request.OrdenId);

        var sismografo = new Sismografo
        {
            IdentificadorSismografo = request.Sismografo.IdentificadorSismografo,
            NroSerie = request.Sismografo.NroSerie,
            FechaAdquisicion = request.Sismografo.FechaAdquisicion,
            CambioEstado = new List<CambioEstado>()
        };

        gestor.BuscarEstadoFueraServicio(sismografo);

        var orden = gestor.GetOrdenSeleccionada();
        if (orden == null)
            return Results.BadRequest("No se encontró la orden seleccionada.");

        var sismografoOrden = orden.EstacionSismologica?.Sismografo;
        if (sismografoOrden == null)
            return Results.BadRequest("No se encontró el sismógrafo asociado a la orden.");

        var estadoActual = sismografoOrden.CambioEstado
            .Where(ce => ce.Estado.esAmbitoSismografo())
            .OrderByDescending(ce => ce.FechaHoraInicio)
            .FirstOrDefault();

        return Results.Ok(new
        {
            success = true,
            message = "Estado fuera de servicio procesado correctamente",
            data = new
            {
                ordenId = request.OrdenId,
                sismografo = new
                {
                    identificador = sismografoOrden.IdentificadorSismografo,
                    nroSerie = sismografoOrden.NroSerie,
                    fechaAdquisicion = sismografoOrden.FechaAdquisicion,
                    estadoActual = estadoActual != null ? new
                    {
                        nombre = estadoActual.Estado.Nombre,
                        descripcion = estadoActual.Estado.Descripcion,
                        ambito = estadoActual.Estado.Ambito,
                        fechaInicio = estadoActual.FechaHoraInicio,
                        fechaFin = estadoActual.FechaHoraFin
                    } : null
                }
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al buscar estado fuera de servicio: {ex.Message}");
    }
});

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
                Console.WriteLine($" Usuario {usuario.NombreUsuario} no tiene empleado asignado");
            }
        }

        var usuarioRI = dataLoader.Usuarios.FirstOrDefault(u =>
            u.Empleado?.Rol?.Descripcion == "Responsable de Inspección");

        if (usuarioRI != null)
        {
            var sesionActiva = new Sesion
            {
                FechaHoraInicio = DateTime.Now.AddHours(-2),
                FechaHoraFin = default, 
                Usuario = usuarioRI
            };

            dataLoader.Sesiones.Add(sesionActiva);
        }

        Console.WriteLine(" Relaciones entre entidades configuradas correctamente");
        Console.WriteLine($"Sesión activa configurada para: {usuarioRI?.Empleado?.Nombre} {usuarioRI?.Empleado?.Apellido}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Error al configurar relaciones: {ex.Message}");
        throw;
    }
}

app.Run();
