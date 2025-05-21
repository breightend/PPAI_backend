using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class CambioEstado
    {
        public DateTime FechaHoraFin { get; set; }
        public required Motivo Motivo { get; set; }
    }
}