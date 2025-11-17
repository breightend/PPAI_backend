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
        private PantallaCCRS _pantalla;
        private int _identificadorSismografo;
        private string _nombreEstado;
        private DateTime _fechaCambioEstado;
        private DateTime _fechaCierre;
        private List<string> _motivos;
        private List<string> _comentarios;
        private List<string> _destinatarios;

        public ObservadorPantallaCRSS()
        {
            _pantalla = new PantallaCCRS();
            _motivos = new List<string>();
            _comentarios = new List<string>();
            _destinatarios = new List<string>();
            _nombreEstado = string.Empty;
        }

        public void Actualizar(int identificadorSismografo, string nombreEstado, DateTime fecha, List<string> motivos, List<string> comentarios, List<string> destinatarios)
        {
            SetIdentificadorSismografo(identificadorSismografo);
            SetNombreEstado(nombreEstado);
            SetFechaCambioEstado(fecha);
            SetMotivos(motivos ?? new List<string>());
            SetComentarios(comentarios ?? new List<string>());
            SetDestinatarios(destinatarios ?? new List<string>());
            SetFechaCierre(DateTime.Now);

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

            var mensajeJson = JsonSerializer.Serialize(mensaje, options);

            ActualizarPantallaCCRS();

            Console.WriteLine($"[PANTALLA CRSS] Notificación JSON generada:");
            Console.WriteLine(mensajeJson);
        }



        private void ActualizarPantallaCCRS()
        {
            _pantalla.SetMensaje($"Sismógrafo #{_identificadorSismografo} cambió al estado: {_nombreEstado}");
            _pantalla.SetFecha(_fechaCierre);
            _pantalla.SetMotivos(_motivos);
            _pantalla.SetComentarios(_comentarios);
            _pantalla.SetResponsablesReparacion(_destinatarios);

            _pantalla.NotificarOrdenDeInspeccion($"Actualización de estado para sismógrafo #{_identificadorSismografo}");
        }

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

        public int GetIdentificadorSismografo() => _identificadorSismografo;
        public string GetNombreEstado() => _nombreEstado;
        public DateTime GetFechaCambioEstado() => _fechaCambioEstado;
        public DateTime GetFechaCierre() => _fechaCierre;
        public List<string> GetMotivos() => new List<string>(_motivos);
        public List<string> GetComentarios() => new List<string>(_comentarios);
        public List<string> GetDestinatarios() => new List<string>(_destinatarios);

        public PantallaCCRSResponseDTO GetPantallaResponseDTO()
        {
            return _pantalla.GetResponseDTO();
        }

        public PantallaCCRS GetPantalla()
        {
            return _pantalla;
        }

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