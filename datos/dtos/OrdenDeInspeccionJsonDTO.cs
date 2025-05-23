using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class OrdenDeInspeccionJsonDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("NumeroOrden")]
        public int NumeroOrden { get; set; }
        
        [JsonProperty("FechaHoraInicio")]
        public DateTime FechaHoraInicio { get; set; }
        
        [JsonProperty("FechaHoraFinalizacion")]
        public DateTime? FechaHoraFinalizacion { get; set; }
        
        [JsonProperty("FechaHoraCierre")]
        public DateTime? FechaHoraCierre { get; set; }
        
        [JsonProperty("ObservacionCierre")]
        public string? ObservacionCierre { get; set; }
        
        [JsonProperty("EmpleadoId")]
        public string EmpleadoId { get; set; } = string.Empty;
        
        [JsonProperty("EstadoId")]
        public string EstadoId { get; set; } = string.Empty;
        
        [JsonProperty("EstacionSismologicaId")]
        public string EstacionSismologicaId { get; set; } = string.Empty;
        
        [JsonProperty("CambioEstado")]
        public List<CambioEstadoDto> CambioEstado { get; set; } = new List<CambioEstadoDto>();
    }
} 