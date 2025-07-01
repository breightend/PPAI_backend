using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Sismografo
    {
        public DateTime FechaAdquisicion { get; set; }
        public int IdentificadorSismografo { get; set; }
        public int NroSerie { get; set; }
        public required List<CambioEstado> CambioEstado { get; set; }


        public int getIdentificadorSismografo()
        {
            return IdentificadorSismografo;
        }

        public void setEstadoActual(CambioEstado nuevoEstado)
        {
            CambioEstado.Add(nuevoEstado);
        }

        public void ActualizarSismografo( DateTime horaActual, Estado estado, List<MotivoFueraDeServicio> motivos)
        {
            if (estado == null)
                throw new ArgumentNullException(nameof(estado), "El estado no puede ser nulo.");
            if (motivos == null || !motivos.Any())
                throw new ArgumentNullException(nameof(motivos), "La lista de motivos no puede ser nula o vacía.");

            crearCambioEstadoSismografo(estado, motivos, horaActual);
        }


        public void crearCambioEstadoSismografo(Estado nuevoEstado, List<MotivoFueraDeServicio> motivos, DateTime horaActual)
        {
            var cambioActual = CambioEstado
                .FirstOrDefault(ce => ce.esEstadoActual() && ce.Estado.esAmbitoSismografo());

            if (cambioActual != null)
                cambioActual.setFechaHoraFin(horaActual);

            var nuevoCambio = new CambioEstado
            {
                FechaHoraInicio = horaActual,
                Estado = nuevoEstado,
                Motivos = motivos
            };

            CambioEstado.Add(nuevoCambio);
        }


    }
}