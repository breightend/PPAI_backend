using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class CambioEstado
    {
        // Atributos:
        public DateTime FechaHoraFin { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        
        // Relacion con la clase Estado:
        public required List<Motivo> Motivo { get; set; }
        public required Estado Estado { get; set; }


        public CambioEstado() { }

        // Métodos:
        public DateTime getFechaHoraFin()
        {
            return FechaHoraFin;
        }

        public Estado getEstado()
        {
            return Estado;
        }
        public bool esEstadoActual()
        {
            return FechaHoraFin == null;
            // Que este subrayado es solo una advertencia pero esta bien
        }
    }
}