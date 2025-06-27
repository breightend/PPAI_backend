using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.gestor
{
    public class InterfazMail
    {
        public void EnviarMails(List<string> destinatarios, dynamic datosNotificacion)
        {
            Console.WriteLine("=== NOTIFICACIÓN ENVIADA ===");
            Console.WriteLine($"Destinatarios: {string.Join(", ", destinatarios)}");
            Console.WriteLine($"Sismógrafo: {datosNotificacion.dentificadorSismografo}");
            Console.WriteLine($"Estado: {datosNotificacion.NombreEstado}");
            Console.WriteLine($"Fecha y Hora: {datosNotificacion.FechaHoraRegistro}");
            Console.WriteLine($"Comentarios: {datosNotificacion.Comentarios}");
            Console.WriteLine("Motivos:");
            
            foreach (var motivo in datosNotificacion.Motivos)
            {
                Console.WriteLine($"  - {motivo}");
            }
            
            Console.WriteLine("=== FIN NOTIFICACIÓN ===");
        }

    }
}