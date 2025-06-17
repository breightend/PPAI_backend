using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class EstacionSismologica
    {
        public int CodigoEstacion { get; set; }
        public bool DocumentoCertificacionAdq { get; set; }
        public DateTime FechaSolicitudCertificacion { get; set; }
        public double Latitud { get; set; }
        public required string Nombre { get; set; }
        public int NroCertificacionAdquirida { get; set; }
        public required Sismografo Sismografo { get; set; }


        public required Empleado Empleado { get; set; }
        public required Estado Estado { get; set; }

        public (string Nombre, int IdentificadorSismografo) getNombreEIdentificador()
        {
            return (Nombre, Sismografo.getIdentificadorSismografo());
        }
        
        public void ActualizarSismografo(Sismografo sismografo, DateTime horaActual)
        {
            if (sismografo == null)
                throw new ArgumentNullException(nameof(sismografo), "El sismógrafo no puede ser nulo.");
            Sismografo.ActualizarSismografo(sismografo, horaActual);
        }
    }
}