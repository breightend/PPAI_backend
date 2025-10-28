using Microsoft.AspNetCore.Mvc;
using PPAI_backend.models.monitores;
using PPAI_backend.datos.dtos;

namespace PPAI_backend.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PantallaCCRSController : ControllerBase
    {
        private readonly PantallaCCRS _pantallaCCRS;

        public PantallaCCRSController()
        {
            _pantallaCCRS = new PantallaCCRS();
        }

        [HttpGet("estado")]
        public ActionResult<PantallaCCRSResponseDTO> GetEstado()
        {
            // Ejemplo de uso de los métodos
            _pantallaCCRS.SetMensaje("Sistema funcionando correctamente");
            _pantallaCCRS.SetFecha(DateTime.Now);

            _pantallaCCRS.AddComentario("Revisión técnica completada");
            _pantallaCCRS.AddComentario("Mantenimiento preventivo realizado");

            _pantallaCCRS.AddMotivo("Mantenimiento programado");
            _pantallaCCRS.AddMotivo("Calibración de sensores");

            _pantallaCCRS.AddResponsableReparacion("Juan Pérez - Técnico Senior");
            _pantallaCCRS.AddResponsableReparacion("María González - Ingeniera de Sistemas");

            return Ok(_pantallaCCRS.GetResponseDTO());
        }

        [HttpPost("actualizar")]
        public ActionResult<PantallaCCRSResponseDTO> ActualizarEstado([FromBody] PantallaCCRSUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Datos inválidos");
            }

            // Usar los métodos Set
            if (!string.IsNullOrEmpty(request.Mensaje))
                _pantallaCCRS.SetMensaje(request.Mensaje);

            if (request.Fecha.HasValue)
                _pantallaCCRS.SetFecha(request.Fecha.Value);

            if (request.Comentarios != null)
                _pantallaCCRS.SetComentarios(request.Comentarios);

            if (request.Motivos != null)
                _pantallaCCRS.SetMotivos(request.Motivos);

            if (request.ResponsablesReparacion != null)
                _pantallaCCRS.SetResponsablesReparacion(request.ResponsablesReparacion);

            return Ok(_pantallaCCRS.GetResponseDTO());
        }
    }

    public class PantallaCCRSUpdateRequest
    {
        public string? Mensaje { get; set; }
        public DateTime? Fecha { get; set; }
        public List<string>? Comentarios { get; set; }
        public List<string>? Motivos { get; set; }
        public List<string>? ResponsablesReparacion { get; set; }
    }
}