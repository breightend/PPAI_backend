using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PPAI_backend.datos.dtos;
using PPAI_backend.models.interfaces;
using PPAI_backend.models.observador;

namespace PPAI_backend.models.entities
{
    public class GestorCerrarOrdenDeInspeccion : IObservadorNotificacion
    {
        private readonly ApplicationDbContext _context;
        private readonly IObservadorNotificacion _emailService;

        private Sesion actualSesion = new Sesion
        {
            Usuario = new Usuario()
        };
        private List<MotivoFueraDeServicio> motivosSeleccionados = new();
        private List<IObservadorNotificacion> observadores = new();

        private Empleado? empleado;

        public GestorCerrarOrdenDeInspeccion(ApplicationDbContext context, IObservadorNotificacion emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        public async Task<Empleado> BuscarEmpleadoRIAsync()
        {
            var sesionActiva = await _context.Sesiones
                .Include(s => s.Usuario)
                .ThenInclude(u => u.Empleado)
                .ThenInclude(e => e.Rol)
                .FirstOrDefaultAsync(s => s.FechaHoraFin == default);

            if (sesionActiva == null)
                throw new Exception("No hay sesión activa");

            return sesionActiva.BuscarEmpleadoRI();
        }

        public async Task<List<DatosOI>> BuscarOrdenInspeccionAsync(Empleado empleado)
        {
            var ordenesDeInspeccion = await _context.OrdenesDeInspeccion
                .Include(oi => oi.Empleado)
                .Include(oi => oi.Estado)
                .Include(oi => oi.EstacionSismologica)
                .ThenInclude(es => es.Sismografo)
                .Where(oi => oi.Empleado.Mail == empleado.Mail)
                .ToListAsync();

            List<DatosOI> resultado = new List<DatosOI>();

            foreach (var oi in ordenesDeInspeccion)
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

        public async Task TomarOrdenSeleccionadaAsync(int numeroOrden)
        {
            ordenSeleccionada = await _context.OrdenesDeInspeccion
                .Include(oi => oi.Empleado)
                .Include(oi => oi.Estado)
                .Include(oi => oi.EstacionSismologica)
                .ThenInclude(es => es.Sismografo)
                .Include(oi => oi.CambioEstado)
                .ThenInclude(ce => ce.Estado)
                .FirstOrDefaultAsync(oi => oi.NumeroOrden == numeroOrden);

            if (ordenSeleccionada == null)
                throw new Exception($"No se encontró la orden número: {numeroOrden} en la base de datos.");
        }

        public void TomarObservacion(string observacion)
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para tomar la observación.");
            ordenSeleccionada.ObservacionCierre = observacion;

        }

        public async Task<List<MotivoFueraDeServicio>> BuscarMotivoFueraDeServicioAsync()
        {
            var motivos = await _context.MotivosFueraDeServicio
                .Include(m => m.TipoMotivo)
                .ToListAsync();

            List<MotivoFueraDeServicio> motivosFueraDeServicio = new List<MotivoFueraDeServicio>();

            foreach (var motivo in motivos)
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
        public async Task BuscarEstadoCerradaAsync()
        {
            var estadoCerrada = await _context.Estados
                .FirstOrDefaultAsync(e => e.esAmbitoOrden() && e.esEstadoCerrada());

            if (estadoCerrada == null)
                throw new Exception("No se encontró el estado 'Cerrada' con ámbito 'OrdenDeInspeccion'.");
        }

        public async Task<string> CerrarOrdenInspeccionAsync()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para cerrar.");

            var estadoCerrada = await _context.Estados
                .FirstOrDefaultAsync(e => e.Ambito == "OrdenDeInspeccion" && e.Nombre == "Cerrada");

            if (estadoCerrada == null)
                throw new Exception("No se encontró el estado 'Cerrada'.");

            ordenSeleccionada.cerrar(estadoCerrada, motivosSeleccionados, DateTime.Now);

            await _context.SaveChangesAsync();

            return $"Orden N° {ordenSeleccionada.NumeroOrden} cerrada correctamente.";
        }

        public async Task BuscarEstadoFueraServicioAsync(Sismografo sismografo)
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay orden seleccionada.");

            var estadoFueraServicio = await _context.Estados
                .FirstOrDefaultAsync(e => e.esAmbitoSismografo() && e.estadoFueraServicio());

            if (estadoFueraServicio == null)
                throw new Exception("No se encontró el estado 'Fuera de servicio' con ámbito 'Sismógrafo'.");

            ordenSeleccionada.EstacionSismologica.ActualizarSismografo(sismografo, DateTime.Now,
                estadoFueraServicio, motivosSeleccionados);

            await _context.SaveChangesAsync();
        }


        public async Task TomarMotivoFueraDeServicioYComentarioAsync(List<MotivoSeleccionadoConComentarioDTO> motivosDto)
        {
            motivosSeleccionados.Clear();
            var todosLosMotivos = await _context.MotivosFueraDeServicio
                .Include(m => m.TipoMotivo)
                .ToListAsync();

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

        public async Task<List<string>> ObtenerMailsResponsableReparacionAsync()
        {
            var empleados = await _context.Empleados
                .Include(e => e.Rol)
                .Where(e => e.Rol != null)
                .ToListAsync();

            List<string> mailsResponsableReparacion = new List<string>();

            foreach (var emp in empleados)
            {
                if (emp.EsResponsableDeReparacion())
                {
                    mailsResponsableReparacion.Add(emp.GetMail());
                }
            }
            return mailsResponsableReparacion;
        }

        public async Task EnviarNotificacionPorMailAsync()
        {
            var mailsResponsables = await ObtenerMailsResponsableReparacionAsync();

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

            await _emailService.NotificarCierreOrdenInspeccion(asunto, mensaje, mailsResponsables);
        }


        public void Suscribir(IObservadorNotificacion observador)
        {
            observadores.Add(observador);
        }

        public async Task Notificar()
        {
            foreach (var observador in observadores)
            {
                await observador.NotificarCierreOrdenInspeccion("La orden de inspección ha sido cerrada.", new List<string>());
            }
        }

        // Implementación de la interfaz IObservadorNotificacion
        public async Task NotificarCierreOrdenInspeccion(string mensaje, List<string> destinatarios)
        {
            await _emailService.NotificarCierreOrdenInspeccion(mensaje, destinatarios);
        }

        public async Task NotificarCierreOrdenInspeccion(string asunto, string mensaje, List<string> destinatarios)
        {
            await _emailService.NotificarCierreOrdenInspeccion(asunto, mensaje, destinatarios);
        }



        // Métodos auxiliares para verificar el DTO faltante
        public class DatosOI
        {
            public int Numero { get; set; }
            public DateTime FechaFin { get; set; }
            public string NombreEstacion { get; set; } = string.Empty;
            public int IdSismografo { get; set; }
        }
    }
}