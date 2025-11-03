using PPAI_backend.models.entities;
using PPAI_backend.models.interfaces;
using PPAI_backend.models.monitores; // Asumo que aquí está PantallaCCRS
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace PPAI_backend.models.observador
{
    public class ObservadorPantallaCRSS : IObservadorNotificacion
    {
        // 1. ESTADO INTERNO (Campos privados, como en el ejemplo)
        // Estos guardarán la información de la última actualización.
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
            // Creamos UNA instancia de la pantalla que este observador va a gestionar.
            _pantalla = new PantallaCCRS();
            _motivos = new List<string>();
            _comentarios = new List<string>();
            _destinatarios = new List<string>();
            _nombreEstado = string.Empty;
        }

        // 2. IMPLEMENTACIÓN DE LA INTERFAZ (El método que el Gestor llamará)
        public void Actualizar(OrdenDeInspeccion orden)
        {
            // 3. EXTRAER DATOS de la orden (esto es lo que no hacías)
            // Asumiré la estructura de tu orden. Ajusta si es necesario.
            var sismografo = orden.EstacionSismologica?.Sismografo;
            var cambioEstadoFS = sismografo?.CambioEstado
                .Where(ce => ce.Estado.Nombre.ToLower() == "fuera de servicio")
                .OrderByDescending(ce => ce.FechaHoraInicio)
                .FirstOrDefault();

            // 4. USAR LOS "SETTERS" (¡Como en el ejemplo!)
            // Llenamos los campos privados de esta clase.
            SetIdentificadorSismografo(sismografo?.IdentificadorSismografo ?? 0);
            SetNombreEstado(cambioEstadoFS?.Estado.Nombre ?? "Desconocido");
            SetFechaCambioEstado(cambioEstadoFS?.FechaHoraInicio ?? DateTime.MinValue);
            SetMotivos(cambioEstadoFS?.Motivos.Select(m => m.TipoMotivo.Descripcion).ToList() ?? new List<string>());
            SetComentarios(cambioEstadoFS?.Motivos.Select(m => m.Comentario).ToList() ?? new List<string>());
            SetDestinatarios(orden.EmailsResponsables ?? new List<string>()); // Asumo que esto existe en la orden
            SetFechaCierre(DateTime.Now); // La fecha de "ahora"

            // 5. GENERAR EL JSON
            // Ahora creamos el JSON usando los campos privados (this._...)
            var mensaje = new
            {
                tipo = "cierre_orden_inspeccion",
                timestamp = this._fechaCierre.ToString("yyyy-MM-dd HH:mm:ss"),
                datos = new
                {
                    sismografo = new
                    {
                        identificador = this._identificadorSismografo,
                        estado = this._nombreEstado,
                        fechaCambioEstado = this._fechaCambioEstado.ToString("yyyy-MM-dd HH:mm:ss")
                    },
                    cierre = new
                    {
                        fechaCierre = this._fechaCierre.ToString("yyyy-MM-dd HH:mm:ss"),
                        motivos = this._motivos,
                        comentarios = this._comentarios,
                        destinatarios = this._destinatarios
                    },
                    notificacion = new { /* ... */ }
                },
                metadatos = new { /* ... */ }
            };

            var options = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var mensajeJson = JsonSerializer.Serialize(mensaje, options);

            Console.WriteLine($"[PANTALLA CRSS] JSON generado:");
            Console.WriteLine(mensajeJson);

            // 6. ACTUALIZAR EL MODELO DE PANTALLA
            // (Esto es lo que hacía tu método estático, pero ahora en el objeto real)
            this._pantalla.SetFecha(this._fechaCierre);
            this._pantalla.SetMensaje($"Sismógrafo #{this._identificadorSismografo} cambió al estado: {this._nombreEstado}");

            // 7. ENVIAR AL FRONTEND (El paso que falta)
            // Aquí es donde deberías enviar el JSON al frontend.
            // `Console.WriteLine` NO lo envía.
            // Necesitarías un servicio (como SignalR) inyectado aquí para hacerlo.
            // Ejemplo: _miServicioDeSignalR.EnviarMensaje(mensajeJson);
        }

        // --- 8. MÉTODOS SET (Ahora sí son setters reales) ---
        // Son 'private' porque solo esta clase necesita llamarlos.
        private void SetIdentificadorSismografo(int identificador)
        {
            this._identificadorSismografo = identificador;
        }

        private void SetNombreEstado(string nombre)
        {
            this._nombreEstado = nombre ?? "";
        }

        private void SetFechaCambioEstado(DateTime fecha)
        {
            this._fechaCambioEstado = fecha;
        }

        private void SetMotivos(List<string> motivos)
        {
            this._motivos = motivos ?? new List<string>();
        }

        private void SetComentarios(List<string> comentarios)
        {
            this._comentarios = comentarios ?? new List<string>();
        }

        private void SetDestinatarios(List<string> destinatarios)
        {
            this._destinatarios = destinatarios ?? new List<string>();
        }

        private void SetFechaCierre(DateTime fechaActual)
        {
            this._fechaCierre = fechaActual;
        }
    }
}