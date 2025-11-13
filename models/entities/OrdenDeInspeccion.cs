using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class OrdenDeInspeccion
    {
        [Key]
        public int NumeroOrden { get; set; }


        // Atributos:
        public DateTime? FechaHoraCierre { get; set; }
        public DateTime? FechaHoraFinalizacion { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public required string ObservacionCierre { get; set; }
        public required Empleado Empleado { get; set; }
        public required Estado Estado { get; set; }

        public required EstacionSismologica EstacionSismologica { get; set; }

        public required List<CambioEstado> CambioEstado { get; set; }




        // Metodos:
        public DateTime? getFechaFin() { return FechaHoraFinalizacion; }

        public int getNumeroOrden() { return NumeroOrden; }
        public string getObservacionCierre() { return ObservacionCierre; }

        public bool EsDelEmpleado(Empleado empleadoLogueado)
        {
            if (Empleado == null || empleadoLogueado == null) return false;
            return Empleado.Mail == empleadoLogueado.Mail;
        }
        public DateTime? getFechaHoraCierre() { return FechaHoraCierre; }
        public bool EstaRealizada()
        {
            return Estado.esFinalizada() == true;
        }

        public (string NombreEstacion, int IdentificadorSismografo) getDatosEstacion()
        {
            return EstacionSismologica.getNombreEIdentificador();
        }

        public void cerrar(Estado estadoCerrada, List<MotivoFueraDeServicio> motivosSeleccionados, DateTime horaActual, string observacion)
        {
            var estadoActualHistorial = CambioEstado.OrderByDescending(ce => ce.FechaHoraInicio).FirstOrDefault();
            if (estadoActualHistorial != null)
            {
                estadoActualHistorial.setFechaHoraFin(horaActual);
            }
            CrearCambioEnLaOrdenDeInspeccion(estadoCerrada, horaActual, motivosSeleccionados);

            this.Estado = estadoCerrada;
            this.FechaHoraCierre = horaActual;
            this.ObservacionCierre = observacion;
        }

        public void CrearCambioEnLaOrdenDeInspeccion(Estado estado, DateTime horaActual, List<MotivoFueraDeServicio> motivosSeleccionados)
        {
            var nuevoCambio = new CambioEstado
            {
                FechaHoraInicio = horaActual,
                FechaHoraFin = null,
                Estado = estado,
                Motivos = motivosSeleccionados
            };
            CambioEstado.Add(nuevoCambio);
            FechaHoraFinalizacion = horaActual;
        }
    }
}