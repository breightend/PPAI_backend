using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class UsuarioDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("NombreUsuario")]
        public string NombreUsuario { get; set; } = string.Empty;
        
        [JsonProperty("Contraseña")]
        public string Contraseña { get; set; } = string.Empty;
        
        [JsonProperty("EmpleadoId")]
        public string EmpleadoId { get; set; } = string.Empty;
    }
} 