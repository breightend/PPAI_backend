using System;
using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class SesionDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("FechaHoraInicio")]
        public DateTime FechaHoraInicio { get; set; }
        
        [JsonProperty("FechaHoraFin")]
        public DateTime? FechaHoraFin { get; set; }
        
        [JsonProperty("UsuarioId")]
        public string UsuarioId { get; set; } = string.Empty;
    }
} 