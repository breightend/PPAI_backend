using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class EstadoDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("Nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; } = string.Empty;
        
        [JsonProperty("Ambito")]
        public string Ambito { get; set; } = string.Empty;
    }
} 