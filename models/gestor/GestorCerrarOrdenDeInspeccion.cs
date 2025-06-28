using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using PPAI_backend.datos.dtos;
using PPAI_backend.services;
using PPAI_backend.models.gestor;
using PPAI_backend.models.monitores;


namespace PPAI_backend.models.entities
{
    public class GestorCerrarOrdenDeInspeccion
    {
        private readonly DataLoaderService _dataLoader;

        private Sesion actualSesion = new Sesion
        {
            Usuario = new Usuario()
        };
        private List<Motivo> motivosSeleccionados = new();

        private Empleado? empleado;

        public GestorCerrarOrdenDeInspeccion(DataLoaderService dataLoader)
        {
            _dataLoader = dataLoader;
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
        public List<Motivo> BuscarMotivoFueraDeServicio()
        {
            //TODO: metodo fuera de servicio tener que ir al motivo 
            return _dataLoader.Motivos.ToList();
        }
        //TODO: referenciar
        public void TomarMotivoFueraDeServicio(List<MotivoDTO> seleccionados)
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para tomar los motivos.");

            if (seleccionados == null || !seleccionados.Any())
                throw new Exception("Debe seleccionar al menos un motivo.");

            motivosSeleccionados.Clear();

            // Usar el _dataLoader para obtener los motivos, igual que BuscarEmpleadoRI()
            var todosLosMotivos = _dataLoader.Motivos.ToList();

            foreach (var dto in seleccionados)
            {
                var baseMotivo = todosLosMotivos.FirstOrDefault(m => m.Id == dto.Id);
                if (baseMotivo == null)
                    throw new Exception($"Motivo con ID {dto.Id} no encontrado.");

                var motivo = new Motivo
                {
                    Id = baseMotivo.Id,
                    TipoMotivo = baseMotivo.TipoMotivo,
                    Comentario = dto.Comentario ?? ""
                };

                motivosSeleccionados.Add(motivo);
            }

            Console.WriteLine("Los motivos seleccionados han sido registrados con éxito!");
        }


        public string Confirmar()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para cerrar.");

            // ordenSeleccionada.FechaHoraCierre = DateTime.Now; // Toma la hora de cierre de la orden de inspeccion

            return $"Orden N° {ordenSeleccionada.NumeroOrden} cerrada correctamente.";
        }

        public void ValidarObsYComentario()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No ha seleccionado ninguna orden de inspeccion.");

            if (string.IsNullOrWhiteSpace(ordenSeleccionada.ObservacionCierre))
                throw new Exception("Debe ingresar una observación para cerrar la orden.");

            if (motivosSeleccionados == null) //|| !motivosSeleccionados.Any()
                throw new Exception("Debe seleccionar al menos un motivo.");
        }
        public void BuscarEstadoCerrada()
        {
            var estadoCerrada = _dataLoader.Estados.FirstOrDefault(e => e.esEstadoCerrada() && e.esAmbitoOrden());

            if (estadoCerrada == null)
                throw new Exception("No se encontró el estado 'Cerrada' con ámbito 'OrdenDeInspeccion'.");
        }

        public string CerrarOrdenInspeccion()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para cerrar.");

            var estadoCerrada = _dataLoader.Estados.FirstOrDefault(e => e.Nombre == "Cerrada" && e.Ambito == "OrdenDeInspeccion");
            if (estadoCerrada == null)
                throw new Exception("No se encontró el estado 'Cerrada'.");

            ordenSeleccionada.cerrar(estadoCerrada, motivosSeleccionados, DateTime.Now);

            return $"Orden N° {ordenSeleccionada.NumeroOrden} cerrada correctamente.";
        }
        public void BuscarEstadoFueraServicio()
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

            var sismografo = ordenSeleccionada.EstacionSismologica.Sismografo;
            sismografo.crearCambioEstadoSismografo(estadoFueraServicio, motivosSeleccionados, DateTime.Now);
        }


        public void TomarMotivosSeleccionados(List<MotivoSeleccionadoConComentarioDTO> motivosDto)
        {
            motivosSeleccionados.Clear();
            // Usar el _dataLoader para obtener los motivos, igual que BuscarEmpleadoRI()
            var todosLosMotivos = _dataLoader.Motivos.ToList();

            foreach (var dto in motivosDto)
            {
                var baseMotivo = todosLosMotivos.FirstOrDefault(m => m.Id == dto.IdMotivo);
                if (baseMotivo == null)
                    throw new Exception($"Motivo con ID {dto.IdMotivo} no encontrado.");

                // Crear un nuevo objeto motivo (el seleccionado) con el comentario agregado por el usuario.
                var motivo = new Motivo
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
        public void EnviarNotificacionPorMail()
        {
            var mailsResponsables = ObtenerMailsResponsableReparacion();

            if (ordenSeleccionada == null)
                throw new Exception("No hay orden seleccionada para enviar notificación.");

            if (motivosSeleccionados == null || !motivosSeleccionados.Any())
                throw new Exception("No hay motivos seleccionados para la notificación.");

            var sismografo = ordenSeleccionada.EstacionSismologica.Sismografo;

            var motivosComenterios = motivosSeleccionados.Select(m =>
                $"{m.TipoMotivo.Descripcion}: {m.Comentario}").ToList();


            string mensaje = $"Sismografo N° {sismografo.IdentificadorSismografo} " +
                $"con el estado '{ordenSeleccionada.Estado.Nombre}' y los siguientes motivos y comentarios: " +
                $"{string.Join(", ", motivosComenterios)}.";
            InterfazMail interfazMail = new InterfazMail();

            interfazMail.EnviarMails(mailsResponsables, mensaje);
        }

        public void PublicarMonitores()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay orden seleccionada para publicar monitores.");

            var sismografo = ordenSeleccionada.EstacionSismologica.Sismografo;

            var motivosComenterios = motivosSeleccionados.Select(m =>
                $"{m.TipoMotivo.Descripcion}: {m.Comentario}").ToList();

            string mensaje = $"Sismógrafo N° {sismografo.IdentificadorSismografo} " +
                $"con el estado '{ordenSeleccionada.Estado.Nombre}' y los siguientes motivos y comentarios: " +
                $"{string.Join(", ", motivosComenterios)}.";
            PantallaCCRS pantallaCCRS = new PantallaCCRS();
            pantallaCCRS.publicar(mensaje);
        }
    }
}