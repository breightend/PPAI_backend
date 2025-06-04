using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class TipoMotivoDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
        
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; } = string.Empty;
    }
}