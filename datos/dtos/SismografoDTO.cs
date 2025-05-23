using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class SismografoDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("FechaAdquisicion")]
        public DateTime FechaAdquisicion { get; set; }
        
        [JsonProperty("IdentificadorSismografo")]
        public int IdentificadorSismografo { get; set; }
        
        [JsonProperty("NroSerie")]
        public int NroSerie { get; set; }
        
        [JsonProperty("CambioEstado")]
        public List<CambioEstadoDto> CambioEstado { get; set; } = new List<CambioEstadoDto>();
    }
} 