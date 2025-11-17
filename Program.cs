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

// app.UseHttpsRedirection();
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

app.MapPost("/cerrar-orden", async (CerrarOrdenRequest request, GestorCerrarOrdenDeInspeccion gestor, ObservadorMail obsMail, ObservadorPantallaCRSS obsPantalla) =>
{
    try
    {
        await gestor.TomarOrdenSeleccionada(request.OrdenId);
        gestor.TomarObservacion(request.Observation);
        await gestor.TomarMotivoFueraDeServicioYComentario(request.Motivos);
        gestor.ValidarObsYComentario();
        await gestor.BuscarEstadoCerrada();
        
        if (request.EnviarMail) {
            gestor.Suscribir(obsMail);
        }

        if (request.EnviarMonitores) {
            gestor.Suscribir(obsPantalla);
        }

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
        await gestor.BuscarEstadoFueraServicio(sismografo);
        var responsables = await gestor.ObtenerMailsResponsableReparacion();
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
        // ESTO ES NUEVO: Busca el error interno si existe
        var mensajeReal = ex.InnerException != null 
            ? $"{ex.Message} -> DETALLES: {ex.InnerException.Message}" 
            : ex.Message;

        Console.WriteLine($"🔥 ERROR CRÍTICO EN BD: {mensajeReal}"); // Para verlo en la terminal
        return Results.BadRequest($"Error al cerrar orden: {mensajeReal}");
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



app.MapGet("/monitores", async (ApplicationDbContext context) =>
{
    try
    {
        Console.WriteLine("🔍 Iniciando consulta a /monitores");
        
        // Obtener órdenes cerradas con todos los includes necesarios
        var ordenesCerradas = await context.OrdenesDeInspeccion
            .Include(oi => oi.Estado)
            .Include(oi => oi.EstacionSismologica)
                .ThenInclude(es => es.Sismografo)
                    .ThenInclude(s => s.CambioEstado)
                        .ThenInclude(ce => ce.Estado)
            .Include(oi => oi.EstacionSismologica)
                .ThenInclude(es => es.Sismografo)
                    .ThenInclude(s => s.CambioEstado)
                        .ThenInclude(ce => ce.Motivos)
                            .ThenInclude(m => m.TipoMotivo)
            .Where(oi => oi.Estado.Nombre == "Cerrada")
            .OrderByDescending(oi => oi.FechaHoraFinalizacion ?? oi.FechaHoraInicio)
            .Take(10)
            .ToListAsync();

        //Console.WriteLine($"📊 Órdenes cerradas encontradas: {ordenesCerradas.Count}");

        var monitores = new List<object>();

        foreach (var orden in ordenesCerradas)
        {
            //Console.WriteLine($"🔎 Procesando orden {orden.NumeroOrden}");
            
            var sismografo = orden.EstacionSismologica?.Sismografo;
            if (sismografo == null)
            {
                //Console.WriteLine($"⚠️ Orden {orden.NumeroOrden}: No tiene sismógrafo");
                continue;
            }

            //Console.WriteLine($"   Sismógrafo: {sismografo.IdentificadorSismografo}");
            //Console.WriteLine($"   Cambios de estado: {sismografo.CambioEstado?.Count ?? 0}");

            // Buscar el cambio de estado "Fuera de Servicio" más reciente
            var cambioEstadoFS = sismografo.CambioEstado?
                .Where(ce => ce.Estado != null && 
                            (ce.Estado.Nombre.ToLower() == "fuera de servicio" || 
                             ce.Estado.Nombre.ToLower() == "fueradeservicio"))
                .OrderByDescending(ce => ce.FechaHoraInicio)
                .FirstOrDefault();

            if (cambioEstadoFS == null)
            {
                //Console.WriteLine($"⚠️ Orden {orden.NumeroOrden}: No tiene cambio a 'Fuera de Servicio'");
                //Console.WriteLine($"   Estados disponibles: {string.Join(", ", sismografo.CambioEstado?.Select(ce => ce.Estado?.Nombre ?? "null") ?? new List<string>())}");
                continue;
            }

            //Console.WriteLine($"   ✅ Encontrado cambio FS: {cambioEstadoFS.FechaHoraInicio}");
            //Console.WriteLine($"   Motivos: {cambioEstadoFS.Motivos?.Count ?? 0}");

            var motivos = cambioEstadoFS.Motivos?
                .Select(m => m.TipoMotivo?.Descripcion ?? "Sin descripción")
                .ToList() ?? new List<string>();
                
            var comentarios = cambioEstadoFS.Motivos?
                .Select(m => m.Comentario ?? "")
                .ToList() ?? new List<string>();

            monitores.Add(new
            {
                tipo = "sismografo_fuera_de_servicio",
                timestamp = cambioEstadoFS.FechaHoraInicio,

                datos = new
                {
                    sismografo = new
                    {
                        identificador = sismografo.IdentificadorSismografo,
                        estado = cambioEstadoFS.Estado.Nombre,
                        fechaCambioEstado = cambioEstadoFS.FechaHoraInicio
                        },
                    cierre = new
                    {
                    fechaCierre = (DateTime?)null,
                    motivos = motivos,
                    comentarios = comentarios,
                    destinatarios = new List<string>()
                    },

                    notificacion = new
                    {
                        mensaje = $"El sismógrafo {sismografo.IdentificadorSismografo} fue marcado como 'Fuera de Servicio'.",
                        estado = "generada",
                        tipoNotificacion = "warning"
                    }
                },

                metadatos = new
                {
                    sistema = "Backend Sismológico",
                    modulo = "Monitores",
                    version = "1.0"
                }

                

            });
        }

        //Console.WriteLine($"✅ Total monitores a enviar: {monitores.Count}");

        // Obtener responsables de reparación
        var responsables = await context.Empleados
            .Include(e => e.Rol)
            .Where(e => e.Rol != null && e.Rol.Descripcion == "Tecnico de Reparaciones")
            .Select(e => e.Mail)
            .ToListAsync();

        //Console.WriteLine($"👥 Responsables encontrados: {responsables.Count}");

        return Results.Ok(new
        {
            success = true,
            message = "Datos de monitores obtenidos correctamente",
            data = new
            {
                monitores = monitores,
                responsablesReparacion = responsables
            }
        });
    }
    catch (Exception ex)
    {
        //Console.WriteLine($"❌ ERROR en /monitores: {ex.Message}");
        //Console.WriteLine($"   Stack: {ex.StackTrace}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"   Inner: {ex.InnerException.Message}");
        }
        
        return Results.BadRequest(new
        {
            success = false,
            message = "Error al obtener datos de monitores",
            error = ex.Message,
            innerError = ex.InnerException?.Message
        });
    }
});

app.Run();


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
