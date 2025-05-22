using PPAI_backend.datos.dtos;
using PPAI_backend.models.entities;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/brenda", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/ordenes-inspeccion", () => // Endpoint para mostrar ordenes de inspeccion:
{
    var gestor = new GestorCerrarOrdenDeInspeccion();
    var empleado = gestor.buscarEmpleadoRI(); // Esto tambien podemos pasarlo directamente como parametro
    var ordenes = gestor.BuscarOrdenInspeccion(empleado);
    var ordenesOrdenadas = gestor.OrdenarOrdenInspeccion(ordenes);
    return Results.Ok(ordenesOrdenadas);
});


app.MapGet("/motivos", () => // Endpoint para mostrar Motivos:
{
    var gestor = new GestorCerrarOrdenDeInspeccion();
    var motivos = gestor.ObtenerMotivosDesdeJson();
    return Results.Ok(motivos);
});

app.MapPost("/motivos-seleccionados", (MotivosSeleccionadosDTO dto) => // Endpoint para tomar y guardar los motivos seleccionados + comentarios en una lista 
{
    var gestor = new GestorCerrarOrdenDeInspeccion();
    gestor.tomarMotivoFueraDeServicio(dto.Motivos);
    return Results.Ok("Motivos registrados correctamente.");
});



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
