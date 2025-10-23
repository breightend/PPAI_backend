using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.models.interfaces;

namespace PPAI_backend.models.gestor
{
    public class InterfazMail
    {
        private readonly IObservadorNotificacion _emailService;

        public InterfazMail(IObservadorNotificacion emailService)
        {
            _emailService = emailService;
        }

        // Constructor sin parámetros para compatibilidad (usar con precaución)
        public InterfazMail()
        {
            // Este constructor se mantiene para compatibilidad, pero idealmente no debería usarse
        }

        public async Task EnviarMails(List<string> destinatarios, string mensaje)
        {
            if (_emailService != null)
            {
                await _emailService.NotificarCierreOrdenInspeccionAsync(mensaje, destinatarios);
                Console.WriteLine("=== CORREOS ENVIADOS CORRECTAMENTE ===");
                Console.WriteLine($"Destinatarios: {string.Join(", ", destinatarios)}");
            }
            else
            {
                // Fallback para cuando se usa el constructor sin parámetros
                Console.WriteLine("=== SIMULACIÓN DE ENVÍO DE CORREOS ===");
                Console.WriteLine($"⚠️ EmailService no configurado. Usando simulación:");
                Console.WriteLine($"Destinatarios: {string.Join(", ", destinatarios)}");
                Console.WriteLine($"Mensaje: {mensaje}");
                Console.WriteLine("Simulación de correos enviados correctamente.");
            }
        }

        public async Task EnviarMails(List<string> destinatarios, string asunto, string mensaje)
        {
            if (_emailService != null)
            {
                await _emailService.NotificarCierreOrdenInspeccionAsync(asunto, mensaje, destinatarios);
                Console.WriteLine("=== CORREOS ENVIADOS CORRECTAMENTE ===");
                Console.WriteLine($"Asunto: {asunto}");
                Console.WriteLine($"Destinatarios: {string.Join(", ", destinatarios)}");
            }
            else
            {
                // Fallback para cuando se usa el constructor sin parámetros
                Console.WriteLine("=== SIMULACIÓN DE ENVÍO DE CORREOS ===");
                Console.WriteLine($"⚠️ EmailService no configurado. Usando simulación:");
                Console.WriteLine($"Asunto: {asunto}");
                Console.WriteLine($"Destinatarios: {string.Join(", ", destinatarios)}");
                Console.WriteLine($"Mensaje: {mensaje}");
                Console.WriteLine("Simulación de correos enviados correctamente.");
            }
        }
    }
}