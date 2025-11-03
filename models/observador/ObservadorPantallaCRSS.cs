using PPAI_backend.models.entities;
using PPAI_backend.models.interfaces;
using PPAI_backend.models.monitores;
using PPAI_backend.datos.dtos;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace PPAI_backend.models.observador
{
    public class ObservadorPantallaCRSS : IObservadorNotificacion
    {
        // Estado interno del observador
        private PantallaCCRS _pantalla;
        private int _identificadorSismografo;
        private string _nombreEstado;
        private DateTime _fechaCambioEstado;
        private DateTime _fechaCierre;
        private List<string> _motivos;
        private List<string> _comentarios;
        private List<string> _destinatarios;

        // Constructor
        public ObservadorPantallaCRSS()
        {
            _pantalla = new PantallaCCRS();
            _motivos = new List<string>();
            _comentarios = new List<string>();
            _destinatarios = new List<string>();
            _nombreEstado = string.Empty;
        }

        // Implementación de la interfaz IObservadorNotificacion
        public void Actualizar(int identificadorSismografo, string nombreEstado, DateTime fecha, List<string> motivos, List<string> comentarios, List<string> destinatarios)
        {
            // Actualizar el estado interno usando los setters
            SetIdentificadorSismografo(identificadorSismografo);
            SetNombreEstado(nombreEstado);
            SetFechaCambioEstado(fecha);
            SetMotivos(motivos ?? new List<string>());
            SetComentarios(comentarios ?? new List<string>());
            SetDestinatarios(destinatarios ?? new List<string>());
            SetFechaCierre(DateTime.Now);

            // Generar el JSON con la información actualizada
            var mensajeJson = GenerarJsonNotificacion();

            // Actualizar la pantalla CCRS
            ActualizarPantallaCCRS();

            // Mostrar el JSON generado (en un escenario real se enviaría al frontend)
            Console.WriteLine($"[PANTALLA CRSS] Notificación JSON generada:");
            Console.WriteLine(mensajeJson);
        }

        // Método para generar el JSON de notificación
        private string GenerarJsonNotificacion()
        {
            var mensaje = new
            {
                tipo = "cierre_orden_inspeccion",
                timestamp = _fechaCierre.ToString("yyyy-MM-dd HH:mm:ss"),
                datos = new
                {
                    sismografo = new
                    {
                        identificador = _identificadorSismografo,
                        estado = _nombreEstado,
                        fechaCambioEstado = _fechaCambioEstado.ToString("yyyy-MM-dd HH:mm:ss")
                    },
                    cierre = new
                    {
                        fechaCierre = _fechaCierre.ToString("yyyy-MM-dd HH:mm:ss"),
                        motivos = _motivos,
                        comentarios = _comentarios,
                        destinatarios = _destinatarios
                    },
                    notificacion = new
                    {
                        mensaje = $"Sismógrafo #{_identificadorSismografo} cambió al estado: {_nombreEstado}",
                        requiereAccion = _nombreEstado.ToLower().Contains("fuera"),
                        prioridad = _nombreEstado.ToLower().Contains("fuera") ? "alta" : "normal"
                    }
                },
                metadatos = new
                {
                    origen = "Sistema de Gestión Sismológica",
                    version = "1.0",
                    generadoPor = "ObservadorPantallaCRSS"
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(mensaje, options);
        }

        // Método para actualizar la pantalla CCRS con los datos recibidos
        private void ActualizarPantallaCCRS()
        {
            // Usar los métodos Set de la pantalla CCRS
            _pantalla.SetMensaje($"Sismógrafo #{_identificadorSismografo} cambió al estado: {_nombreEstado}");
            _pantalla.SetFecha(_fechaCierre);
            _pantalla.SetMotivos(_motivos);
            _pantalla.SetComentarios(_comentarios);
            _pantalla.SetResponsablesReparacion(_destinatarios);

            // Notificar a la pantalla
            _pantalla.NotificarOrdenDeInspeccion($"Actualización de estado para sismógrafo #{_identificadorSismografo}");
        }

        // Métodos Set privados para actualizar el estado interno
        private void SetIdentificadorSismografo(int identificador)
        {
            _identificadorSismografo = identificador;
        }

        private void SetNombreEstado(string nombre)
        {
            _nombreEstado = nombre ?? string.Empty;
        }

        private void SetFechaCambioEstado(DateTime fecha)
        {
            _fechaCambioEstado = fecha;
        }

        private void SetMotivos(List<string> motivos)
        {
            _motivos = motivos ?? new List<string>();
        }

        private void SetComentarios(List<string> comentarios)
        {
            _comentarios = comentarios ?? new List<string>();
        }

        private void SetDestinatarios(List<string> destinatarios)
        {
            _destinatarios = destinatarios ?? new List<string>();
        }

        private void SetFechaCierre(DateTime fechaActual)
        {
            _fechaCierre = fechaActual;
        }

        // Métodos Get para acceder al estado interno
        public int GetIdentificadorSismografo() => _identificadorSismografo;
        public string GetNombreEstado() => _nombreEstado;
        public DateTime GetFechaCambioEstado() => _fechaCambioEstado;
        public DateTime GetFechaCierre() => _fechaCierre;
        public List<string> GetMotivos() => new List<string>(_motivos);
        public List<string> GetComentarios() => new List<string>(_comentarios);
        public List<string> GetDestinatarios() => new List<string>(_destinatarios);

        // Método para obtener el DTO de la pantalla CCRS
        public PantallaCCRSResponseDTO GetPantallaResponseDTO()
        {
            return _pantalla.GetResponseDTO();
        }

        // Método para obtener la instancia de la pantalla
        public PantallaCCRS GetPantalla()
        {
            return _pantalla;
        }

        // Método para enviar notificación específica (opcional)
        public void EnviarNotificacionEspecifica(string mensajePersonalizado)
        {
            _pantalla.SetMensaje(mensajePersonalizado);
            _pantalla.SetFecha(DateTime.Now);

            var notificacionEspecifica = new
            {
                tipo = "notificacion_especifica",
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                mensaje = mensajePersonalizado,
                sismografo = _identificadorSismografo,
                estado = _nombreEstado
            };

            var json = JsonSerializer.Serialize(notificacionEspecifica, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Console.WriteLine($"[PANTALLA CRSS] Notificación específica:");
            Console.WriteLine(json);
        }
    }
}