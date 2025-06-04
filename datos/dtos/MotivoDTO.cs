using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PPAI_backend.datos.dtos
{
    public class MotivoDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("tipoMotivo")]
        public TipoMotivoDTO TipoMotivo { get; set; } = new();

        [JsonProperty("Comentario")]
        public string? Comentario { get; set; }
    }

}