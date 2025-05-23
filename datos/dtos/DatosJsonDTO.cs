using System.Collections.Generic;
using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class DatosJsonDTO
    {
        [JsonProperty("usuarios")]
        public List<UsuarioDTO> Usuarios { get; set; } = new List<UsuarioDTO>();
        
        [JsonProperty("empleados")]
        public List<EmpleadoDTO> Empleados { get; set; } = new List<EmpleadoDTO>();
        
        [JsonProperty("roles")]
        public List<RolDTO> Roles { get; set; } = new List<RolDTO>();
        
        [JsonProperty("estados")]
        public List<EstadoDTO> Estados { get; set; } = new List<EstadoDTO>();
        
        [JsonProperty("motivos")]
        public List<MotivoDTO> Motivos { get; set; } = new List<MotivoDTO>();
        
        [JsonProperty("sismografos")]
        public List<SismografoDTO> Sismografos { get; set; } = new List<SismografoDTO>();
        
        [JsonProperty("estacionesSismologicas")]
        public List<EstacionSismologicaDTO> EstacionesSismologicas { get; set; } = new List<EstacionSismologicaDTO>();
        
        [JsonProperty("ordenesDeInspeccion")]
        public List<OrdenDeInspeccionJsonDTO> OrdenesDeInspeccion { get; set; } = new List<OrdenDeInspeccionJsonDTO>();
        
        [JsonProperty("sesiones")]
        public List<SesionDTO> Sesiones { get; set; } = new List<SesionDTO>();
    }
} 