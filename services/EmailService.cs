using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using PPAI_backend.models.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PPAI_backend.services
{
    public class EmailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(ISendGridClient sendGridClient, IConfiguration configuration, ILogger<EmailService> logger)
        {
            _sendGridClient = sendGridClient;
            _configuration = configuration;
            _logger = logger;


            _fromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL")
                ?? _configuration["SendGrid:FromEmail"]
                ?? "noreply@example.com";
            _fromName = Environment.GetEnvironmentVariable("SENDGRID_FROM_NAME")
                ?? _configuration["SendGrid:FromName"]
                ?? "Sistema de Sismología";
        }

        public async Task NotificarCierreOrdenInspeccionAsync(string mensaje, List<string> destinatarios)
        {
            const string asuntoDefault = "Notificación de Cierre de Orden de Inspección";
            await NotificarCierreOrdenInspeccionAsync(asuntoDefault, mensaje, destinatarios);
        }

        public async Task NotificarCierreOrdenInspeccionAsync(string asunto, string mensaje, List<string> destinatarios)
        {
            try
            {
                if (destinatarios == null || destinatarios.Count == 0)
                {
                    _logger.LogWarning("No hay destinatarios para enviar la notificación de cierre de orden de inspección");
                    return;
                }

                var from = new EmailAddress(_fromEmail, _fromName);
                var subject = asunto;
                var plainTextContent = mensaje;
                var htmlContent = GenerarContenidoHtml(mensaje);

                foreach (var destinatario in destinatarios)
                {
                    if (string.IsNullOrWhiteSpace(destinatario) || !EsEmailValido(destinatario))
                    {
                        _logger.LogWarning($"Email destinatario inválido: {destinatario}");
                        continue;
                    }

                    var to = new EmailAddress(destinatario);
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                    var response = await _sendGridClient.SendEmailAsync(msg);

                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        _logger.LogInformation($"Email enviado exitosamente a: {destinatario}");
                    }
                    else
                    {
                        var responseBody = await response.Body.ReadAsStringAsync();
                        _logger.LogError($"Error al enviar email a {destinatario}. Código: {response.StatusCode}, Respuesta: {responseBody}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación de cierre de orden de inspección");
                throw;
            }
        }

        private string GenerarContenidoHtml(string mensaje)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 20px; }}
                        .header {{ background-color: #f4f4f4; padding: 10px; text-align: center; }}
                        .content {{ margin: 20px 0; }}
                        .footer {{ font-size: 12px; color: #666; margin-top: 20px; }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h2>Sistema de Sismología - Notificación Automática</h2>
                    </div>
                    <div class='content'>
                        <p>Estimado responsable,</p>
                        <p>{mensaje}</p>
                        <p>Esta es una notificación automática del sistema de gestión de órdenes de inspección sismológica.</p>
                    </div>
                    <div class='footer'>
                        <p>Este es un mensaje automático, por favor no responda a este correo.</p>
                        <p>Fecha y hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
                    </div>
                </body>
                </html>";
        }

        private bool EsEmailValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}