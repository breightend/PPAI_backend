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

        public void ActualizarSismografo(Sismografo sismografo, DateTime horaActual)
        {
            if (sismografo == null)
                throw new ArgumentNullException(nameof(sismografo), "El sismógrafo no puede ser nulo.");

            FechaAdquisicion = sismografo.FechaAdquisicion;
            IdentificadorSismografo = sismografo.IdentificadorSismografo;
            NroSerie = sismografo.NroSerie;
            CambioEstado = sismografo.CambioEstado;
            crearCambioEstadoSismografo(sismografo.CambioEstado.Last().Estado, sismografo.CambioEstado.Last().Motivos, horaActual);
        }

        public void crearCambioEstadoSismografo(Estado nuevoEstado, List<Motivo> motivos, DateTime horaActual)
        {
            // Buscar cambio de estado actual con ámbito "Sismógrafo"
            var cambioActual = CambioEstado
                .FirstOrDefault(ce => ce.esEstadoActual() && ce.Estado.esAmbitoSismografo());

            // Cerrar el estado actual si corresponde
            if (cambioActual != null)
                cambioActual.setFechaHoraFin(horaActual);

            // Crear nuevo cambio de estado con los motivos
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