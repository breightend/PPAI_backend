using System;
using System.Collections.Generic;
using PPAI_backend.models.monitores;

namespace PPAI_backend.examples
{
    public class PantallaCCRSExample
    {
        public static void EjemploDeUso()
        {
            // Crear una instancia de la pantalla
            var pantallaCCRS = new PantallaCCRS();

            // Ejemplo 1: Usar métodos Set individuales
            pantallaCCRS.SetMensaje("Estación sismológica requiere mantenimiento");
            pantallaCCRS.SetFecha(DateTime.Now);

            // Agregar comentarios uno por uno
            pantallaCCRS.AddComentario("Se detectó ruido en el sensor principal");
            pantallaCCRS.AddComentario("Calibración necesaria en eje X");
            pantallaCCRS.AddComentario("Revisión de conectores completada");

            // Agregar motivos uno por uno
            pantallaCCRS.AddMotivo("Falla en sensor");
            pantallaCCRS.AddMotivo("Mantenimiento preventivo");

            // Agregar responsables uno por uno
            pantallaCCRS.AddResponsableReparacion("Carlos Mendoza - Técnico Especialista");
            pantallaCCRS.AddResponsableReparacion("Ana Rodriguez - Supervisora de Mantenimiento");

            // Ejemplo 2: Usar métodos Set con listas completas
            var nuevosComentarios = new List<string>
            {
                "Sistema reiniciado exitosamente",
                "Parámetros restaurados a valores por defecto",
                "Pruebas de funcionamiento completadas"
            };
            pantallaCCRS.SetComentarios(nuevosComentarios);

            var nuevosMotivos = new List<string>
            {
                "Actualización de firmware",
                "Reconfiguración de red"
            };
            pantallaCCRS.SetMotivos(nuevosMotivos);

            var nuevosResponsables = new List<string>
            {
                "Pedro Sanchez - Ingeniero de Software",
                "Laura Martinez - Técnica de Redes"
            };
            pantallaCCRS.SetResponsablesReparacion(nuevosResponsables);

            // Obtener datos usando métodos Get
            Console.WriteLine($"Mensaje: {pantallaCCRS.GetMensaje()}");
            Console.WriteLine($"Fecha: {pantallaCCRS.GetFecha()}");

            Console.WriteLine("Comentarios:");
            foreach (var comentario in pantallaCCRS.GetComentarios())
            {
                Console.WriteLine($"- {comentario}");
            }

            Console.WriteLine("Motivos:");
            foreach (var motivo in pantallaCCRS.GetMotivos())
            {
                Console.WriteLine($"- {motivo}");
            }

            Console.WriteLine("Responsables de Reparación:");
            foreach (var responsable in pantallaCCRS.GetResponsablesReparacion())
            {
                Console.WriteLine($"- {responsable}");
            }

            // Obtener DTO para enviar al frontend
            var responseDTO = pantallaCCRS.GetResponseDTO();
            Console.WriteLine($"DTO creado para frontend con {responseDTO.Comentarios.Count} comentarios, {responseDTO.Motivos.Count} motivos y {responseDTO.ResponsablesReparacion.Count} responsables");
        }
    }
}