using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.datos.dtos
{
    public class MotivoSeleccionadoConComentarioDTO
    {
        public int IdMotivo { get; set; }
        public string Comentario { get; set; }
    }
    public class MotivosSeleccionadosDTO
    {
        public List<MotivoSeleccionadoConComentarioDTO> Motivos { get; set; }
    }
}