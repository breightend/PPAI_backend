using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.models.entities;
using PPAI_backend.models.gestor;
using PPAI_backend.models.interfaces;

namespace PPAI_backend.models.observador
{

    public class ObservadorMail : IObservadorNotificacion
    {
        private readonly InterfazMail _interfazMail;
        public ObservadorMail(InterfazMail interfazMail)
        {
            _interfazMail = interfazMail;
        }
        public void Actualizar(OrdenDeInspeccion orden)
        {
            //TODO: cambiar lofica que envie a todos los mails correspondientes
            var destinatarios = new List<string>
            {
                orden.Empleado.Mail
            };

            var mensaje = $"La orden de inspección número {orden.NumeroOrden} ha sido cerrada.";

            _interfazMail.EnviarMails(destinatarios, mensaje).Wait();
        }
    }
}
