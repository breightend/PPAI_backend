using PPAI_backend.datos.dtos;
using PPAI_backend.models.entities;
using PPAI_backend.models.interfaces;
using PPAI_backend.models.observador;
using PPAI_backend.services;
using Microsoft.EntityFrameworkCore;
using SendGrid;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


DotNetEnv.Env.Load();

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


var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
    ?? builder.Configuration["SendGrid:ApiKey"];

if (!string.IsNullOrEmpty(sendGridApiKey))
{
    builder.Services.AddSingleton<ISendGridClient>(provider => new SendGridClient(sendGridApiKey));
}
else
{
    builder.Services.AddSingleton<ISendGridClient>(provider => new SendGridClient("mock-api-key"));
}


builder.Services.AddScoped<GestorCerrarOrdenDeInspeccion>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ObservadorMail>();
builder.Services.AddScoped<ObservadorPantallaCRSS>();
builder.Services.AddScoped<DatabaseSeeder>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddControllers();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();


if (args.Contains("--seed"))
{
    Console.WriteLine("Iniciando seeding de base de datos...");
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedDatabaseAsync();
    await seeder.MostrarEstadisticas();
    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapPost("/seed-database", async (DatabaseSeeder seeder) =>
    {
        try
        {
            await seeder.SeedDatabaseAsync();
            await seeder.MostrarEstadisticas();

            return Results.Ok(new
            {
                success = true,
                message = "Base de datos poblada exitosamente con datos aleatorios",
                timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al poblar la base de datos",
                error = ex.Message,
                timestamp = DateTime.Now
            });
        }
    });
}

app.MapGet("/database-stats", async (DatabaseSeeder seeder) =>
{
    try
    {
        await seeder.MostrarEstadisticas();

        return Results.Ok(new
        {
            success = true,
            message = "Estadísticas mostradas en consola",
            timestamp = DateTime.Now
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "Error al obtener estadísticas",
            error = ex.Message
        });
    }
});




app.MapGet("/empleado-logueado", async (GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {

        var empleado = await gestor.BuscarEmpleadoRI();
        if (empleado == null)
        {
            return Results.NotFound(new
            {
                success = false,
                message = "Empleado no encontrado"
            });
        }

        return Results.Ok(new
        {
            success = true,
            message = "Empleado obtenido",
            data = new
            {
                nombre = empleado.Nombre,
                apellido = empleado.Apellido,
                mail = empleado.Mail,
                telefono = empleado.Telefono,
                rol = empleado.Rol?.Descripcion
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error: {ex.Message}");
    }
});

app.MapGet("/motivos", async (GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        var motivos = await gestor.BuscarMotivoFueraDeServicio();

        if (motivos == null || motivos.Count == 0)
        {
            return Results.Ok(new
            {
                success = true,
                message = "No se encontraron motivos fuera de servicio",
                data = new List<object>()
            });
        }

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

app.MapGet("/ordenes-inspeccion", async (GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {

        var empleado = await gestor.BuscarEmpleadoRI();
        var ordenes = await gestor.BuscarOrdenInspeccion(empleado);

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

app.MapPost("/motivos-seleccionados", async (MotivosSeleccionadosDTO request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        foreach (var motivo in request.Motivos)
        {
            Console.WriteLine($"ID: {motivo.IdMotivo}, Comentario: {motivo.Comentario}");
        }
        await gestor.TomarMotivoFueraDeServicioYComentario(request.Motivos);

        return Results.Ok("Motivos recibidos correctamente.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al recibir los motivos: {ex.Message}");
    }
});

app.MapPost("/confirmar-cierre", async (ConfirmarRequest request, GestorCerrarOrdenDeInspeccion gestor) =>
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

        await gestor.EnviarNotificacionPorMail();

        return Results.Ok("Cierre confirmado correctamente y notificaciones enviadas.");


    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al confirmar cierre: {ex.Message}");
    }
});



app.MapPost("/agregar-observacion", async (ObservationRequest request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        await gestor.TomarOrdenSeleccionada(request.OrderId);
        gestor.TomarObservacion(request.Observation);
        return Results.Ok("Observación agregada correctamente.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error al agregar observación: {ex.Message}");
    }
});

app.MapPost("/cerrar-orden", async (CerrarOrdenRequest request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        await gestor.TomarOrdenSeleccionada(request.OrdenId);
        gestor.TomarObservacion(request.Observation);
        await gestor.TomarMotivoFueraDeServicioYComentario(request.Motivos);
        gestor.ValidarObsYComentario();
        await gestor.BuscarEstadoCerrada();

        var resultado = await gestor.CerrarOrdenInspeccion();
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
        var nombreEstado = await gestor.BuscarEstadoFueraServicio(sismografo);
        var obtenerResponsablesReparacion = await gestor.ObtenerResponsablesReparacion();
        await gestor.EnviarNotificacionPorMail();


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





//Proximamente se va a borrar esta parte
app.MapPost("/buscar-estado-fuera-servicio", async (BuscarEstadoFueraServicioRequest request, GestorCerrarOrdenDeInspeccion gestor) =>
{
    try
    {
        await gestor.TomarOrdenSeleccionada(request.OrdenId);

        var sismografo = new Sismografo
        {
            IdentificadorSismografo = request.Sismografo.IdentificadorSismografo,
            NroSerie = request.Sismografo.NroSerie,
            FechaAdquisicion = request.Sismografo.FechaAdquisicion,
            CambioEstado = new List<CambioEstado>()
        };

        await gestor.BuscarEstadoFueraServicio(sismografo);

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


app.MapPost("/observadores/registrar-email", async (
    string email,
    GestorCerrarOrdenDeInspeccion gestor,
    EmailService emailService) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "El email es requerido"
            });
        }

        var observador = new ObservadorMail(emailService);

        return Results.Ok(new
        {
            success = true,
            message = "Observador de email registrado exitosamente",
            email = email,
            tipo = "Email"
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "Error al registrar observador",
            error = ex.Message
        });
    }
});


app.MapGet("/empleados/{mail}/notificaciones-pendientes", async (
    string mail,
    ApplicationDbContext context) =>
{
    try
    {
        var empleado = await context.Empleados
            .FirstOrDefaultAsync(e => e.Mail == mail);

        if (empleado == null)
        {
            return Results.NotFound(new
            {
                success = false,
                message = "Empleado no encontrado"
            });
        }

        var ordenesActivas = await context.OrdenesDeInspeccion
            .Include(o => o.EstacionSismologica)
            .ThenInclude(e => e.Sismografo)
            .Include(o => o.Estado)
            .Where(o => o.Empleado.Mail == mail && o.Estado.Nombre != "Cerrada")
            .OrderByDescending(o => o.FechaHoraInicio)
            .Select(o => new
            {
                numeroOrden = o.NumeroOrden,
                estado = o.Estado.Nombre,
                fechaInicio = o.FechaHoraInicio,
                estacion = o.EstacionSismologica.Nombre,
                sismografo = o.EstacionSismologica.Sismografo.IdentificadorSismografo
            })
            .ToListAsync();

        return Results.Ok(new
        {
            success = true,
            message = $"Notificaciones pendientes para {empleado.Nombre} {empleado.Apellido}",
            totalOrdenes = ordenesActivas.Count,
            data = ordenesActivas
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "Error al obtener notificaciones",
            error = ex.Message
        });
    }
});


// Endpoint para enviar notificación manual a un grupo de empleados
app.MapPost("/notificaciones/enviar", async (
    string asunto,
    string mensaje,
    List<string> destinatarios,
    EmailService emailService) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(asunto) || string.IsNullOrWhiteSpace(mensaje))
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Asunto y mensaje son requeridos"
            });
        }

        await emailService.NotificarCierreOrdenInspeccionAsync(asunto, mensaje, destinatarios);

        return Results.Ok(new
        {
            success = true,
            message = "Notificaciones enviadas exitosamente",
            destinatariosCount = destinatarios.Count,
            timestamp = DateTime.Now
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "Error al enviar notificaciones",
            error = ex.Message
        });
    }
});

// FUNCIÓN COMENTADA - Ya no se usa porque ahora usamos DatabaseSeeder
// async Task ConfigurarRelacionesEntidades(IServiceProvider services)
// {
//     using var scope = services.CreateScope();
//     var dataLoader = scope.ServiceProvider.GetRequiredService<DataLoaderService>();
// 
//     try
//     {
//         await dataLoader.LoadAllDataAsync("datos/datos.json");
// 
//         foreach (var usuario in dataLoader.Usuarios)
//         {
//             if (usuario.Empleado == null)
//             {
//                 Console.WriteLine($" Usuario {usuario.NombreUsuario} no tiene empleado asignado");
//             }
//         }
// 
//         var usuarioRI = dataLoader.Usuarios.FirstOrDefault(u =>
//             u.Empleado?.Rol?.Descripcion == "Responsable de Inspección");
// 
//         if (usuarioRI != null)
//         {
//             var sesionActiva = new Sesion
//             {
//                 FechaHoraInicio = DateTime.Now.AddHours(-2),
//                 FechaHoraFin = default,
//                 Usuario = usuarioRI
//             };
// 
//             dataLoader.Sesiones.Add(sesionActiva);
//         }
// 
//         Console.WriteLine(" Relaciones entre entidades configuradas correctamente");
//         Console.WriteLine($"Sesión activa configurada para: {usuarioRI?.Empleado?.Nombre} {usuarioRI?.Empleado?.Apellido}");
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($" Error al configurar relaciones: {ex.Message}");
//         throw;
//     }
// }



app.Run();
