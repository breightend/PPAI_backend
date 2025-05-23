using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class EmpleadoDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("Apellido")]
        public string Apellido { get; set; } = string.Empty;
        
        [JsonProperty("Nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [JsonProperty("Mail")]
        public string Mail { get; set; } = string.Empty;
        
        [JsonProperty("Telefono")]
        public string Telefono { get; set; } = string.Empty;
        
        [JsonProperty("RolId")]
        public int RolId { get; set; }
    }
} 