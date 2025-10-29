using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPAI_backend.models.entities;
using PPAI_backend.datos.dtos;
using PPAI_backend.models.interfaces;

namespace PPAI_backend.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GestorCerrarOrdenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IObservadorNotificacion _emailService;

        public GestorCerrarOrdenController(ApplicationDbContext context, IObservadorNotificacion emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet("empleado-ri")]
        public async Task<ActionResult<Empleado>> BuscarEmpleadoRI()
        {
            try
            {
                var gestor = new GestorCerrarOrdenDeInspeccion(_context, _emailService);
                var empleado = await gestor.BuscarEmpleadoRIAsync();
                return Ok(empleado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ordenes-inspeccion/{empleadoMail}")]
        public async Task<ActionResult<List<GestorCerrarOrdenDeInspeccion.DatosOI>>> BuscarOrdenesInspeccion(string empleadoMail)
        {
            try
            {
                var empleado = await _context.Empleados
                    .Include(e => e.Rol)
                    .FirstOrDefaultAsync(e => e.Mail == empleadoMail);

                if (empleado == null)
                    return NotFound("Empleado no encontrado");

                var gestor = new GestorCerrarOrdenDeInspeccion(_context, _emailService);
                var ordenes = await gestor.BuscarOrdenInspeccionAsync(empleado);
                var ordenesOrdenadas = gestor.OrdenarOrdenInspeccion(ordenes);

                return Ok(ordenesOrdenadas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("motivos-fuera-servicio")]
        public async Task<ActionResult<List<MotivoFueraDeServicio>>> BuscarMotivos()
        {
            try
            {
                var gestor = new GestorCerrarOrdenDeInspeccion(_context, _emailService);
                var motivos = await gestor.BuscarMotivoFueraDeServicioAsync();
                return Ok(motivos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("cerrar-orden")]
        public async Task<ActionResult<string>> CerrarOrden([FromBody] CerrarOrdenRequest request)
        {
            try
            {
                var gestor = new GestorCerrarOrdenDeInspeccion(_context, _emailService);

                // Tomar la orden seleccionada
                await gestor.TomarOrdenSeleccionadaAsync(request.NumeroOrden);

                // Tomar observación
                gestor.TomarObservacion(request.Observacion);

                // Tomar motivos y comentarios
                await gestor.TomarMotivoFueraDeServicioYComentarioAsync(request.Motivos);

                // Validar
                gestor.ValidarObsYComentario();

                // Cerrar orden
                var resultado = await gestor.CerrarOrdenInspeccionAsync();

                // Enviar notificación por mail
                await gestor.EnviarNotificacionPorMailAsync();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("responsables-reparacion")]
        public async Task<ActionResult<List<string>>> ObtenerResponsablesReparacion()
        {
            try
            {
                var gestor = new GestorCerrarOrdenDeInspeccion(_context, _emailService);
                var mails = await gestor.ObtenerMailsResponsableReparacionAsync();
                return Ok(mails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}