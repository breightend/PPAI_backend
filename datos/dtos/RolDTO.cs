using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class RolDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
        
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; } = string.Empty;
    }
} 