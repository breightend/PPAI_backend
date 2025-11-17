using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.models.entities;
using PPAI_backend.models.interfaces;
using PPAI_backend.services;
using SendGrid.Helpers.Mail;

namespace PPAI_backend.models.observador
{

    public class ObservadorMail : IObservadorNotificacion
    {
        private readonly EmailService _emailService;

        public ObservadorMail(EmailService emailService)
        {
            _emailService = emailService;
        }


        public void Actualizar(int identificadorSismografo, string nombreEstado, DateTime fecha, List<string> motivos, List<string> comentarios, List<string> destinatarios)
        {
            Console.WriteLine("=== ObservadorMail.Actualizar ===");
            Console.WriteLine($"Identificador Sismógrafo: {identificadorSismografo}");
            Console.WriteLine($"Estado: {nombreEstado}");
            Console.WriteLine($"Fecha: {fecha:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Motivos: [{string.Join(", ", motivos)}]");
            Console.WriteLine($"Comentarios: [{string.Join(", ", comentarios)}]");
            Console.WriteLine($"Destinatarios: [{string.Join(", ", destinatarios)}]");
            var mensaje = $"NOTIFICACIÓN DE CAMBIO DE ESTADO - SISMÓGRAFO\n\n" +
                         $"Identificador del Sismógrafo: {identificadorSismografo}\n" +
                         $"Nuevo Estado: {nombreEstado}\n" +
                         $"Fecha y Hora del Cambio: {fecha:yyyy-MM-dd HH:mm:ss}\n\n" +
                         $"MOTIVOS:\n{string.Join("\n• ", motivos.Select(m => "• " + m))}\n\n" +
                         $"COMENTARIOS ADICIONALES:\n{string.Join("\n• ", comentarios.Select(c => "• " + c))}\n\n" +
                         $"Este es un mensaje automático del Sistema de Gestión Sismológica.\n" +
                         $"Por favor, tome las acciones necesarias.";

            Console.WriteLine($"Mensaje a enviar:\n{mensaje}");

            Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine($" Iniciando envío de email a {destinatarios.Count} destinatarios...");
                    await _emailService.NotificarCierreOrdenInspeccionAsync(mensaje, destinatarios);
                    Console.WriteLine(" Email enviado exitosamente");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Error al enviar notificación por email: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($" Error interno: {ex.InnerException.Message}");
                    }
                }
            });
        }

    }
}
