using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.datos.dtos
{
    public class CerrarOrdenRequest
    {
        public int OrdenId { get; set; }
        public string Observation { get; set; } = string.Empty;
        public List<MotivoSeleccionadoConComentarioDTO> Motivos { get; set; } = new();
        public bool EnviarMail { get; set; }
        public bool EnviarMonitores { get; set; }
    }
}