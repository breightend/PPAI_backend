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
        public required Estado Estado { get; set; }

        public required EstacionSismologica EstacionSismologica { get; set; }

        public required List<CambioEstado> CambioEstado { get; set; }




        // Metodos:
        public DateTime getFechaFin() { return FechaHoraFinalizacion; }

        public int getNumeroOrden() { return NumeroOrden; }
        public string getObservacionCierre() { return ObservacionCierre; }

        public Boolean EsDelEmpleado(Empleado empleadoLogueado)
        {
            return Empleado.Nombre == empleadoLogueado.Nombre && Empleado.Apellido == empleadoLogueado.Apellido;
        }
        public DateTime getFechaHoraCierre() { return FechaHoraCierre; }
        public bool EstaRealizada()
        {
            return Estado.esFinalizada() == true;
        }

        public (string NombreEstacion, int IdentificadorSismografo) getDatosEstacion()
        {
            return EstacionSismologica.getNombreEIdentificador();
        }

        public void cerrar(Estado estadoCerrada,  List<Motivo> motivosSeleccionados)
        {
            var estadoActual = CambioEstado.FirstOrDefault(ce => ce.esEstadoActual());

            if (estadoActual != null)
            {
                estadoActual.setFechaHoraFin(DateTime.Now);
            }
            var nuevoCambio = new CambioEstado // Inicializa un nuevo objeto cambio de estado
            {
                FechaHoraInicio = DateTime.Now,
                FechaHoraFin = null,
                Estado = estadoCerrada,
                Motivos = motivosSeleccionados
            };
            CambioEstado.Add(nuevoCambio);
            FechaHoraCierre = DateTime.Now;
        }


    }
}