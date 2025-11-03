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
            
            var mensaje = $"El sismógrafo con ID {identificadorSismografo} ha cambiado al estado '{nombreEstado}' el {fecha:yyyy-MM-dd HH:mm:ss}.\n" +
                         $"Motivos: {string.Join(", ", motivos)}\n" +
                         $"Comentarios: {string.Join(", ", comentarios)}";

            Task.Run(async () =>
            {
                try
                {
                    await _emailService.NotificarCierreOrdenInspeccionAsync(mensaje, destinatarios);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar notificación por email: {ex.Message}");
                }
            });
        }

    }
}
