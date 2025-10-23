using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using PPAI_backend.datos.dtos;
using PPAI_backend.services;
using PPAI_backend.models.gestor;
using PPAI_backend.models.monitores;
using PPAI_backend.models.interfaces;
using PPAI_backend.models.observador;

namespace PPAI_backend.models.entities
{
    public class GestorCerrarOrdenDeInspeccion
    {
        private readonly DataLoaderService _dataLoader;
        private readonly IObservadorNotificacion _emailService;

        private Sesion actualSesion = new Sesion
        {
            Usuario = new Usuario()
        };
        private List<MotivoFueraDeServicio> motivosSeleccionados = new();

        private Empleado? empleado;

        public GestorCerrarOrdenDeInspeccion(DataLoaderService dataLoader, IObservadorNotificacion emailService)
        {
            _dataLoader = dataLoader;
            _emailService = emailService;
        }


        public Empleado BuscarEmpleadoRI()
        {
            Sesion? sesionActiva = _dataLoader.Sesiones.FirstOrDefault(s => s.FechaHoraFin == default);

            if (sesionActiva == null)
                throw new Exception("No hay sesión activa");

            return sesionActiva.BuscarEmpleadoRI();
        }


        public List<DatosOI> BuscarOrdenInspeccion(Empleado empleado)
        {
            List<DatosOI> resultado = new List<DatosOI>();

            foreach (var oi in _dataLoader.OrdenesDeInspeccion)
            {
                if (oi.EsDelEmpleado(empleado) && oi.EstaRealizada())
                {
                    var nombreEstacion = oi.getDatosEstacion().NombreEstacion;
                    var idSismografo = oi.getDatosEstacion().IdentificadorSismografo;

                    resultado.Add(new DatosOI
                    {
                        Numero = oi.getNumeroOrden(),
                        FechaFin = oi.getFechaFin(),
                        NombreEstacion = nombreEstacion,
                        IdSismografo = idSismografo
                    });
                }
            }

            return resultado;
        }

        public List<DatosOI> OrdenarOrdenInspeccion(List<DatosOI> ordenes)
        {
            return ordenes.OrderBy(o => o.FechaFin).ToList();
        }



        private OrdenDeInspeccion? ordenSeleccionada;
        public void TomarOrdenSeleccionada(int numeroOrden)
        {
            ordenSeleccionada = _dataLoader.OrdenesDeInspeccion.FirstOrDefault(oi => oi.getNumeroOrden() == numeroOrden);

            if (ordenSeleccionada == null)
                throw new Exception($"No se encontró la orden número: {numeroOrden} en la lista mostrada anteriormente.");
        }

        public void TomarObservacion(string observacion)
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para tomar la observación.");
            ordenSeleccionada.ObservacionCierre = observacion;

        }
        public List<MotivoFueraDeServicio> BuscarMotivoFueraDeServicio()
        {
            List<MotivoFueraDeServicio> motivosFueraDeServicio = new List<MotivoFueraDeServicio>();

            foreach (var motivo in _dataLoader.Motivos)
            {
                var motivoFueraServicio = motivo.ObtenerMotivoFueraServicio();
                motivosFueraDeServicio.Add(motivoFueraServicio);
            }

            return motivosFueraDeServicio;
        }


        public string Confirmar()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para cerrar.");


            return $"Orden N° {ordenSeleccionada.NumeroOrden} cerrada correctamente.";
        }

        public void ValidarObsYComentario()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No ha seleccionado ninguna orden de inspeccion.");

            if (string.IsNullOrWhiteSpace(ordenSeleccionada.ObservacionCierre))
                throw new Exception("Debe ingresar una observación para cerrar la orden.");

            if (motivosSeleccionados == null)
                throw new Exception("Debe seleccionar al menos un motivo.");
        }
        public void BuscarEstadoCerrada()
        {
            var estadoCerrada = _dataLoader.Estados.FirstOrDefault(e => e.esAmbitoOrden() && e.esEstadoCerrada());

            if (estadoCerrada == null)
                throw new Exception("No se encontró el estado 'Cerrada' con ámbito 'OrdenDeInspeccion'.");
        }

        public string CerrarOrdenInspeccion()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para cerrar.");

            var estadoCerrada = _dataLoader.Estados.FirstOrDefault(e => e.Ambito == "OrdenDeInspeccion" && e.Nombre == "Cerrada");
            if (estadoCerrada == null)
                throw new Exception("No se encontró el estado 'Cerrada'.");

            ordenSeleccionada.cerrar(estadoCerrada, motivosSeleccionados, DateTime.Now);

            return $"Orden N° {ordenSeleccionada.NumeroOrden} cerrada correctamente.";
        }
        public void BuscarEstadoFueraServicio(Sismografo sismografo)
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay orden seleccionada.");

            Estado? estadoFueraServicio = null;

            foreach (var estado in _dataLoader.Estados)
            {
                if (estado.esAmbitoSismografo() && estado.estadoFueraServicio())
                {
                    estadoFueraServicio = estado;
                    break;
                }
            }

            if (estadoFueraServicio == null)
                throw new Exception("No se encontró el estado 'Fuera de servicio' con ámbito 'Sismógrafo'.");

            ordenSeleccionada.EstacionSismologica.ActualizarSismografo(sismografo, DateTime.Now,
                estadoFueraServicio, motivosSeleccionados);
        }


        public void TomarMotivoFueraDeServicioYComentario(List<MotivoSeleccionadoConComentarioDTO> motivosDto)
        {
            motivosSeleccionados.Clear();
            var todosLosMotivos = _dataLoader.Motivos.ToList();

            foreach (var dto in motivosDto)
            {
                var baseMotivo = todosLosMotivos.FirstOrDefault(m => m.Id == dto.IdMotivo);
                if (baseMotivo == null)
                    throw new Exception($"Motivo con ID {dto.IdMotivo} no encontrado.");

                var motivo = new MotivoFueraDeServicio
                {
                    Id = baseMotivo.Id,
                    TipoMotivo = baseMotivo.TipoMotivo,
                    Comentario = dto.Comentario ?? ""
                };

                motivosSeleccionados.Add(motivo);
            }

            Console.WriteLine($"Se registraron {motivosSeleccionados.Count} motivos seleccionados.");
        }

        public OrdenDeInspeccion? GetOrdenSeleccionada()
        {
            return ordenSeleccionada;
        }

        public List<string> ObtenerMailsResponsableReparacion()
        {
            List<string> mailsResponsableReparacion = new List<string>();

            foreach (var emp in _dataLoader.Empleados)
            {
                if (emp.EsResponsableDeReparacion())
                {
                    mailsResponsableReparacion.Add(emp.GetMail());
                }
            }
            return mailsResponsableReparacion;
        }

        public async Task EnviarNotificacionPorMail()
        {
            var mailsResponsables = ObtenerMailsResponsableReparacion();

            if (ordenSeleccionada == null)
                throw new Exception("No hay orden seleccionada para enviar notificación.");

            if (motivosSeleccionados == null || !motivosSeleccionados.Any())
                throw new Exception("No hay motivos seleccionados para la notificación.");

            var sismografo = ordenSeleccionada.EstacionSismologica.Sismografo;

            var motivosComenterios = motivosSeleccionados.Select(m =>
                $"{m.TipoMotivo.Descripcion}: {m.Comentario}").ToList();

            string asunto = "Notificación de Cierre de Orden de Inspección Sismológica";
            string mensaje = $"Estimado responsable de reparación,\n\n" +
                $"Se ha cerrado una orden de inspección con los siguientes detalles:\n\n" +
                $"• Sismógrafo N°: {sismografo.IdentificadorSismografo}\n" +
                $"• Estado: {ordenSeleccionada.Estado.Nombre}\n" +
                $"• Fecha y hora de cierre: {ordenSeleccionada.FechaHoraCierre:dd/MM/yyyy HH:mm}\n" +
                $"• Observación de cierre: {ordenSeleccionada.ObservacionCierre}\n\n" +
                $"Motivos y comentarios:\n{string.Join("\n", motivosComenterios.Select(m => $"- {m}"))}\n\n" +
                $"Por favor, tome las acciones correspondientes según sea necesario.\n\n" +
                $"Saludos cordiales,\n" +
                $"Sistema de Gestión Sismológica";

            InterfazMail interfazMail = new InterfazMail(_emailService);
            await interfazMail.EnviarMails(mailsResponsables, asunto, mensaje);
        }
        public void Suscribir(ObservadorMail observador)
        {
            // Lógica para suscribir un observador
        }

        public void Notificar()
        {
            ObservadorMail observador = new ObservadorMail();
            //metodo notificar que esta en el observador 
            //metodo notificar que esta en la pantalla de los cors
        }

    }
}