using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class OrdenDeInspeccion
    {
        // Atributos:
        public DateTime FechaHoraCierre { get; set; }
        public DateTime FechaHoraFinalizacion { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public int NumeroOrden { get; set; }
        public required string ObservacionCierre { get; set; }
        public required Empleado Empleado { get; set; }

        // Metodos:
        public DateTime getFechaHoraFinalizacion() { return FechaHoraFinalizacion ; }

        public int getNumeroOrden() { return NumeroOrden; } 
        public string getObservacionCierre() { return ObservacionCierre; }

        public DateTime getFechaHoraCierre() { return FechaHoraCierre; }

    }
}