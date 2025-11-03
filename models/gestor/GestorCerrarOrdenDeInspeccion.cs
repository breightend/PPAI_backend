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
    public class GestorCerrarOrdenDeInspeccion : ISujetoResponsableReparacion
    {
        private readonly ApplicationDbContext _context;
        private readonly IObservadorNotificacion? _emailService;

        private Sesion actualSesion = new Sesion
        {
            Usuario = new Usuario()
        };
        private List<MotivoFueraDeServicio> motivosSeleccionados = [];
        private List<IObservadorNotificacion> _observadores = new List<IObservadorNotificacion>();

        private List<string> _mailsResponsablesReparacion = new List<string>();

        private OrdenDeInspeccion? _ordenProcesada;

        private string _nombreEstadoObtenido = string.Empty;

        private Empleado? empleado;

        public GestorCerrarOrdenDeInspeccion(
                ApplicationDbContext context,
                IObservadorNotificacion? emailService = null)
        {
            _context = context;
            _emailService = emailService;
        }


        public async Task<Empleado> BuscarEmpleadoRI()
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

        public async Task<List<DatosOI>> BuscarOrdenInspeccion(Empleado empleado)
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

        public async Task TomarOrdenSeleccionada(int numeroOrden)
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

        public async Task<List<MotivoFueraDeServicio>> BuscarMotivoFueraDeServicio()
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
        public async Task BuscarEstadoCerrada()
        {
            var estadoCerrada = await _context.Estados
                .FirstOrDefaultAsync(e => e.esAmbitoOrden() && e.esEstadoCerrada());

            if (estadoCerrada == null)
                throw new Exception("No se encontró el estado 'Cerrada' con ámbito 'OrdenDeInspeccion'.");
        }

        public async Task<string> CerrarOrdenInspeccion()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para cerrar.");

            var estadoCerrada = await _context.Estados
                .FirstOrDefaultAsync(e => e.Ambito == "OrdenDeInspeccion" && e.Nombre == "Cerrada");

            if (estadoCerrada == null)
                throw new Exception("No se encontró el estado 'Cerrada'.");

            ordenSeleccionada.cerrar(estadoCerrada, motivosSeleccionados, DateTime.Now);
            _ordenProcesada = ordenSeleccionada; // Guardar la orden procesada

            await _context.SaveChangesAsync();

            return $"Orden N° {ordenSeleccionada.NumeroOrden} cerrada correctamente.";
        }

        public async Task BuscarEstadoFueraServicio(Sismografo sismografo)
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay orden seleccionada.");

            var estadoFueraServicio = await _context.Estados
                .FirstOrDefaultAsync(e => e.esAmbitoSismografo() && e.estadoFueraServicio());

            if (estadoFueraServicio == null)
                throw new Exception("No se encontró el estado 'Fuera de servicio' con ámbito 'Sismógrafo'.");

            _nombreEstadoObtenido = estadoFueraServicio.Nombre;

            ordenSeleccionada.EstacionSismologica.ActualizarSismografo(sismografo, DateTime.Now,
                estadoFueraServicio, motivosSeleccionados);

            await _context.SaveChangesAsync();
        }


        public async Task TomarMotivoFueraDeServicioYComentario(List<MotivoSeleccionadoConComentarioDTO> motivosDto)
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

        public async Task<List<string>> ObtenerMailsResponsableReparacion()
        {
            var empleados = await _context.Empleados
                .Include(e => e.Rol)
                .Where(e => e.Rol != null)
                .ToListAsync();

            _mailsResponsablesReparacion.Clear(); // Limpiar la lista antes de agregar nuevos mails

            foreach (var emp in empleados)
            {
                if (emp.EsResponsableDeReparacion())
                {
                    _mailsResponsablesReparacion.Add(emp.GetMail());
                }
            }

            return _mailsResponsablesReparacion;
        }


        public void Suscribir(IObservadorNotificacion observador)
        {
            _observadores.Add(observador);
        }

        public void Desuscribir(IObservadorNotificacion observador)
        {
            _observadores.Remove(observador);
        }

        public void Notificar()
        {
            if (_ordenProcesada == null)
                throw new Exception("No hay orden procesada para notificar.");

            foreach (var obs in _observadores)
            {
                obs.Actualizar(
                    _ordenProcesada.EstacionSismologica.Sismografo.IdentificadorSismografo,
                    _nombreEstadoObtenido,
                    DateTime.Now,
                    motivosSeleccionados.Select(m => m.TipoMotivo.Descripcion).ToList(),
                    motivosSeleccionados.Select(m => m.Comentario ?? "").ToList(),
                    _mailsResponsablesReparacion
                );
            }
        }

        public async Task EnviarNotificacionPorMail()
        {
            // Notificar a los observadores en lugar de enviar email directo
            if (_ordenProcesada != null)
            {
                await ObtenerMailsResponsableReparacion(); // Cargar los mails
                Notificar(); // Notificar a los observadores
            }
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