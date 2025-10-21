using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class CambioEstado
    {
        // Atributos:
        [Key]
        public int Id { get; set; }

        public required Estado Estado { get; set; }


        public DateTime? FechaHoraFin { get; set; }
        public DateTime FechaHoraInicio { get; set; }

        // Relacion con la clase Estado:

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