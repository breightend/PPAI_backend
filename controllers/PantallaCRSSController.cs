using Microsoft.AspNetCore.Mvc;
using PPAI_backend.models.observador;
using PPAI_backend.datos.dtos;
using System;
using System.Collections.Generic;

namespace PPAI_backend.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PantallaCRSSController : ControllerBase
    {
        private static readonly List<ObservadorPantallaCRSS> _observadores = new();

        [HttpPost("crear-observador")]
        public ActionResult<int> CrearObservador()
        {
            var nuevoObservador = new ObservadorPantallaCRSS();
            _observadores.Add(nuevoObservador);

            return Ok(new
            {
                observadorId = _observadores.Count - 1,
                mensaje = "Observador creado exitosamente"
            });
        }

        [HttpPost("actualizar/{observadorId}")]
        public ActionResult ActualizarObservador(int observadorId, [FromBody] ActualizarObservadorRequest request)
        {
            try
            {
                if (observadorId < 0 || observadorId >= _observadores.Count)
                    return NotFound("Observador no encontrado");

                var observador = _observadores[observadorId];

                observador.Actualizar(
                    request.IdentificadorSismografo,
                    request.NombreEstado,
                    request.Fecha,
                    request.Motivos ?? new List<string>(),
                    request.Comentarios ?? new List<string>(),
                    request.Destinatarios ?? new List<string>()
                );

                return Ok(new
                {
                    mensaje = "Observador actualizado exitosamente",
                    observadorId = observadorId,
                    estadoActual = observador.GetNombreEstado()
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar observador: {ex.Message}");
            }
        }

        [HttpGet("estado/{observadorId}")]
        public ActionResult<ObservadorEstadoResponse> ObtenerEstado(int observadorId)
        {
            try
            {
                if (observadorId < 0 || observadorId >= _observadores.Count)
                    return NotFound("Observador no encontrado");

                var observador = _observadores[observadorId];

                var estado = new ObservadorEstadoResponse
                {
                    IdentificadorSismografo = observador.GetIdentificadorSismografo(),
                    NombreEstado = observador.GetNombreEstado(),
                    FechaCambioEstado = observador.GetFechaCambioEstado(),
                    FechaCierre = observador.GetFechaCierre(),
                    Motivos = observador.GetMotivos(),
                    Comentarios = observador.GetComentarios(),
                    Destinatarios = observador.GetDestinatarios(),
                    PantallaDTO = observador.GetPantallaResponseDTO()
                };

                return Ok(estado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener estado: {ex.Message}");
            }
        }

        [HttpPost("notificacion-especifica/{observadorId}")]
        public ActionResult EnviarNotificacionEspecifica(int observadorId, [FromBody] NotificacionEspecificaRequest request)
        {
            try
            {
                if (observadorId < 0 || observadorId >= _observadores.Count)
                    return NotFound("Observador no encontrado");

                var observador = _observadores[observadorId];
                observador.EnviarNotificacionEspecifica(request.Mensaje);

                return Ok(new
                {
                    mensaje = "Notificación específica enviada",
                    observadorId = observadorId
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al enviar notificación: {ex.Message}");
            }
        }

        [HttpGet("listar-observadores")]
        public ActionResult<List<ObservadorResumenResponse>> ListarObservadores()
        {
            var resumen = new List<ObservadorResumenResponse>();

            for (int i = 0; i < _observadores.Count; i++)
            {
                var obs = _observadores[i];
                resumen.Add(new ObservadorResumenResponse
                {
                    ObservadorId = i,
                    IdentificadorSismografo = obs.GetIdentificadorSismografo(),
                    NombreEstado = obs.GetNombreEstado(),
                    FechaUltimaActualizacion = obs.GetFechaCierre(),
                    CantidadMotivos = obs.GetMotivos().Count,
                    CantidadComentarios = obs.GetComentarios().Count,
                    CantidadDestinatarios = obs.GetDestinatarios().Count
                });
            }

            return Ok(resumen);
        }

        [HttpDelete("eliminar/{observadorId}")]
        public ActionResult EliminarObservador(int observadorId)
        {
            try
            {
                if (observadorId < 0 || observadorId >= _observadores.Count)
                    return NotFound("Observador no encontrado");

                _observadores.RemoveAt(observadorId);

                return Ok(new
                {
                    mensaje = "Observador eliminado exitosamente",
                    observadorId = observadorId
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar observador: {ex.Message}");
            }
        }
    }

    // DTOs para el controlador
    public class ActualizarObservadorRequest
    {
        public int IdentificadorSismografo { get; set; }
        public string NombreEstado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public List<string>? Motivos { get; set; }
        public List<string>? Comentarios { get; set; }
        public List<string>? Destinatarios { get; set; }
    }

    public class NotificacionEspecificaRequest
    {
        public string Mensaje { get; set; } = string.Empty;
    }

    public class ObservadorEstadoResponse
    {
        public int IdentificadorSismografo { get; set; }
        public string NombreEstado { get; set; } = string.Empty;
        public DateTime FechaCambioEstado { get; set; }
        public DateTime FechaCierre { get; set; }
        public List<string> Motivos { get; set; } = new();
        public List<string> Comentarios { get; set; } = new();
        public List<string> Destinatarios { get; set; } = new();
        public PantallaCCRSResponseDTO? PantallaDTO { get; set; }
    }

    public class ObservadorResumenResponse
    {
        public int ObservadorId { get; set; }
        public int IdentificadorSismografo { get; set; }
        public string NombreEstado { get; set; } = string.Empty;
        public DateTime FechaUltimaActualizacion { get; set; }
        public int CantidadMotivos { get; set; }
        public int CantidadComentarios { get; set; }
        public int CantidadDestinatarios { get; set; }
    }
}