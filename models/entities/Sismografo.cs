using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Sismografo
    {
        // Atributos:
        public DateTime FechaAdquisicion { get; set; }
        public int IdentificadorSismografo { get; set; }
        public int NroSerie { get; set; }
        public required List<CambioEstado> CambioEstado { get; set; }


        // Metodos: 
        public int getIdentificadorSismografo()
        {
            return IdentificadorSismografo;
        }

        public void setEstadoActual(CambioEstado nuevoEstado)
        {
            CambioEstado.Add(nuevoEstado);
        }

        public void crearCambioEstadoSismografo(Estado nuevoEstado, List<Motivo> motivos)
        {
            // Buscar cambio de estado actual con ámbito "Sismógrafo"
            var cambioActual = CambioEstado
                .FirstOrDefault(ce => ce.esEstadoActual() && ce.Estado.esAmbitoSismografo());

            // Cerrar el estado actual si corresponde
            if (cambioActual != null)
                cambioActual.setFechaHoraFin(DateTime.Now);

            // Crear nuevo cambio de estado con los motivos
            var nuevoCambio = new CambioEstado
            {
                FechaHoraInicio = DateTime.Now,
                Estado = nuevoEstado,
                Motivos = motivos
            };

            CambioEstado.Add(nuevoCambio);
        }


    }
}