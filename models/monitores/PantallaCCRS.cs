using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.datos.dtos;

namespace PPAI_backend.models.monitores
{
    public class PantallaCCRS
    {
        
        public string estadoActual { get; set; } = string.Empty;
        public DateTime? fechaUltimaActualizacion { get; set; }
        public DateTime? horaUltimaActualizacion { get; set; }
        public List<string> comentarios { get; set; } = new List<string>();
        public List<string> motivos { get; set; } = new List<string>();

        public string mensaje { get; set; } = string.Empty;
        public List<string> responsablesReparacion { get; set; } = new List<string>();

        public void SetMensaje(string nuevoMensaje)
        {
            this.mensaje = nuevoMensaje;
        }

        public void SetFecha(DateTime nuevaFecha)
        {
            this.fechaUltimaActualizacion = nuevaFecha;
        }

        public void SetComentarios(List<string> nuevosComentarios)
        {
            this.comentarios = nuevosComentarios ?? new List<string>();
        }

        public void AddComentario(string comentario)
        {
            if (!string.IsNullOrWhiteSpace(comentario))
            {
                this.comentarios.Add(comentario);
            }
        }

        public void SetMotivos(List<string> nuevosMotivos)
        {
            this.motivos = nuevosMotivos ?? new List<string>();
        }

        public void AddMotivo(string motivo)
        {
            if (!string.IsNullOrWhiteSpace(motivo))
            {
                this.motivos.Add(motivo);
            }
        }

        public void SetResponsablesReparacion(List<string> responsables)
        {
            this.responsablesReparacion = responsables ?? new List<string>();
        }

        public void AddResponsableReparacion(string nombreResponsable)
        {
            if (!string.IsNullOrWhiteSpace(nombreResponsable))
            {
                this.responsablesReparacion.Add(nombreResponsable);
            }
        }

        public string GetMensaje()
        {
            return this.mensaje;
        }

        public DateTime? GetFecha()
        {
            return this.fechaUltimaActualizacion;
        }

        public List<string> GetComentarios()
        {
            return new List<string>(this.comentarios);
        }

        public List<string> GetMotivos()
        {
            return new List<string>(this.motivos);
        }

        public List<string> GetResponsablesReparacion()
        {
            return new List<string>(this.responsablesReparacion);
        }

        public PantallaCCRSResponseDTO GetResponseDTO()
        {
            return new PantallaCCRSResponseDTO
            {
                Mensaje = this.mensaje,
                Fecha = this.fechaUltimaActualizacion,
                Comentarios = this.GetComentarios(),
                Motivos = this.GetMotivos(),
                ResponsablesReparacion = this.GetResponsablesReparacion()
            };
        }

        public void NotificarOrdenDeInspeccion(string mensaje)
        {
            this.SetMensaje(mensaje);
            this.SetFecha(DateTime.Now);
            Console.WriteLine($"Notificación: {mensaje}");
        }
    }
}