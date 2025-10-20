using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.observador
{

    public class ObservadorMail
    {
        public void NotificarOrdenDeInspeccion(string mensaje, int IdentificadorSismografo, List<string> mailResponsableReparacion, List<string> mailsResponsables, string nombreEstado, DateTime fechaEstado, DateTime tiempo_estado, List<string> comentario, List<string> motivos)
        {
            // Lógica para enviar notificación por mail.
        }
    }
}
