using System;
using System.Collections.Generic;
using PPAI_backend.models.observador;

namespace PPAI_backend.examples
{
    public class ObservadorPantallaCRSSExample
    {
        public static void EjemploDeUso()
        {
            Console.WriteLine("=== Ejemplo de uso del ObservadorPantallaCRSS ===\n");

            // 1. Crear una instancia del observador
            var observador = new ObservadorPantallaCRSS();

            // 2. Simular datos de una orden de inspección cerrada
            int identificadorSismografo = 12345;
            string nombreEstado = "Fuera de Servicio";
            DateTime fechaCambio = DateTime.Now.AddHours(-2);
            var motivos = new List<string>
            {
                "Falla en sensor principal",
                "Mantenimiento preventivo requerido",
                "Calibración necesaria"
            };
            var comentarios = new List<string>
            {
                "Se detectó ruido excesivo en las lecturas",
                "Requiere revisión técnica inmediata",
                "Contactar con el proveedor para piezas de repuesto"
            };
            var destinatarios = new List<string>
            {
                "tecnico1@sismologia.com",
                "supervisor@sismologia.com",
                "mantenimiento@sismologia.com"
            };

            // 3. Llamar al método Actualizar (como lo haría el Gestor)
            Console.WriteLine("📡 Actualizando observador con datos de la orden...\n");
            observador.Actualizar(identificadorSismografo, nombreEstado, fechaCambio, motivos, comentarios, destinatarios);

            Console.WriteLine("\n" + new string('=', 60) + "\n");

            // 4. Mostrar el estado interno del observador
            Console.WriteLine("📊 Estado interno del observador:");
            Console.WriteLine($"   • Sismógrafo ID: {observador.GetIdentificadorSismografo()}");
            Console.WriteLine($"   • Estado: {observador.GetNombreEstado()}");
            Console.WriteLine($"   • Fecha cambio: {observador.GetFechaCambioEstado():yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"   • Fecha cierre: {observador.GetFechaCierre():yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"   • Motivos: {string.Join(", ", observador.GetMotivos())}");
            Console.WriteLine($"   • Comentarios: {observador.GetComentarios().Count} comentarios");
            Console.WriteLine($"   • Destinatarios: {observador.GetDestinatarios().Count} destinatarios");

            Console.WriteLine("\n" + new string('-', 60) + "\n");

            // 5. Obtener el DTO de la pantalla CCRS
            Console.WriteLine("🖥️  DTO de PantallaCCRS:");
            var pantallaDTO = observador.GetPantallaResponseDTO();
            Console.WriteLine($"   • Mensaje: {pantallaDTO.Mensaje}");
            Console.WriteLine($"   • Fecha: {pantallaDTO.Fecha:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"   • Motivos: [{string.Join(", ", pantallaDTO.Motivos)}]");
            Console.WriteLine($"   • Comentarios: [{string.Join(", ", pantallaDTO.Comentarios)}]");
            Console.WriteLine($"   • Responsables: [{string.Join(", ", pantallaDTO.ResponsablesReparacion)}]");

            Console.WriteLine("\n" + new string('-', 60) + "\n");

            // 6. Enviar una notificación específica
            Console.WriteLine("📩 Enviando notificación específica...\n");
            observador.EnviarNotificacionEspecifica("ALERTA: Revisión técnica urgente requerida");

            Console.WriteLine("\n=== Fin del ejemplo ===");
        }

        public static void EjemploIntegracionConGestor()
        {
            Console.WriteLine("=== Ejemplo de integración con Gestor ===\n");

            // Simular cómo el Gestor usaría el observador
            var observadores = new List<ObservadorPantallaCRSS>
            {
                new ObservadorPantallaCRSS(),
                new ObservadorPantallaCRSS(),
                new ObservadorPantallaCRSS()
            };

            // Simular notificación a múltiples observadores
            Console.WriteLine("🔄 Notificando a múltiples observadores...\n");

            for (int i = 0; i < observadores.Count; i++)
            {
                Console.WriteLine($"--- Notificando observador #{i + 1} ---");

                observadores[i].Actualizar(
                    identificadorSismografo: 1000 + i,
                    nombreEstado: i % 2 == 0 ? "Fuera de Servicio" : "En Mantenimiento",
                    fecha: DateTime.Now.AddMinutes(-i * 30),
                    motivos: new List<string> { $"Motivo {i + 1}", $"Causa {i + 1}" },
                    comentarios: new List<string> { $"Comentario del sismógrafo {1000 + i}" },
                    destinatarios: new List<string> { $"responsable{i + 1}@sismologia.com" }
                );

                Console.WriteLine($"✅ Observador #{i + 1} actualizado\n");
            }

            Console.WriteLine("=== Integración completada ===");
        }
    }
}