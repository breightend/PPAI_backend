using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.gestor
{
    public class InterfazMail
    {
        public void EnviarMails(List<string> destinatarios, string mensaje)
        {
            Console.WriteLine("=== ENVÍO DE CORREOS ===");
            Console.WriteLine($"Destinatarios: {string.Join(", ", destinatarios)}");
            Console.WriteLine($"Mensaje: {mensaje}");
            Console.WriteLine("Correos enviados correctamente.");
        }

    }
}