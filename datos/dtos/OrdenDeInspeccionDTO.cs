using BackendAPI.datos.dtos;
using PPAI_backend.models.entities;

public class DatosOI
{
    public int Numero { get; set; }
    public DateTime FechaFin { get; set; }
    public string NombreEstacion { get; set; }
    public int IdSismografo { get; set; } //Por que?
    public string EmpleadoId { get; set; }
    public string EstadoId { get; set; }

    public List<OrdenDeInspeccion> OrdenesDeInspeccion { get; set; } 
    public List<CambioEstadoDto> CambioEstado { get; set; }
    
}

