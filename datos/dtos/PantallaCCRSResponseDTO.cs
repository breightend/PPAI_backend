using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class PantallaCCRSResponseDTO
    {
        [JsonProperty("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [JsonProperty("fecha")]
        public DateTime? Fecha { get; set; }

        [JsonProperty("comentarios")]
        public List<string> Comentarios { get; set; } = new List<string>();

        [JsonProperty("motivos")]
        public List<string> Motivos { get; set; } = new List<string>();

        [JsonProperty("responsablesReparacion")]
        public List<string> ResponsablesReparacion { get; set; } = new List<string>();
    }
}