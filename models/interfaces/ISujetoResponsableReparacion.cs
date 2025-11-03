using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.interfaces
{
    public interface ISujetoResponsableReparacion
    {
        public void Suscribir(IObservadorNotificacion observador);
        public void Desuscribir(IObservadorNotificacion observador);

        public void Notificar();
    }
}