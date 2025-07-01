using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class CambioEstado
    {
        // Atributos:
        public DateTime? FechaHoraFin { get; set; }
        public DateTime FechaHoraInicio { get; set; }

        // Relacion con la clase Estado:
        public required Estado Estado { get; set; }

        public required List<MotivoFueraDeServicio> Motivos { get; set; }

        public CambioEstado() { }

        // Métodos:
        public void setFechaHoraFin(DateTime fechaFin)
        {
            FechaHoraFin = fechaFin;
        }

        public Estado getEstado()
        {
            return Estado;
        }
        public bool esEstadoActual()
        {
            return FechaHoraFin == null;
        }



    }
}