using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.interfaces
{
    public interface IObservadorNotificacion
    {
        Task NotificarCierreOrdenInspeccionAsync(string mensaje, List<string> destinatarios);
        Task NotificarCierreOrdenInspeccionAsync(string asunto, string mensaje, List<string> destinatarios);
    }
}