using System;
using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class EstacionSismologicaDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("CodigoEstacion")]
        public int CodigoEstacion { get; set; }
        
        [JsonProperty("DocumentoCertificacionAdq")]
        public bool DocumentoCertificacionAdq { get; set; }
        
        [JsonProperty("FechaSolicitudCertificacion")]
        public DateTime FechaSolicitudCertificacion { get; set; }
        
        [JsonProperty("Latitud")]
        public double Latitud { get; set; }
        
        [JsonProperty("Nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [JsonProperty("NroCertificacionAdquirida")]
        public int NroCertificacionAdquirida { get; set; }
        
        [JsonProperty("SismografoId")]
        public string SismografoId { get; set; } = string.Empty;
        
        [JsonProperty("EmpleadoId")]
        public string EmpleadoId { get; set; } = string.Empty;
        
        [JsonProperty("EstadoId")]
        public string EstadoId { get; set; } = string.Empty;
    }
} 