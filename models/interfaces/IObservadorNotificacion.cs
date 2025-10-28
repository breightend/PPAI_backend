using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.interfaces
{
    public interface IObservadorNotificacion
    {
        Task NotificarCierreOrdenInspeccion(string mensaje, List<string> destinatarios);
        Task NotificarCierreOrdenInspeccion(string asunto, string mensaje, List<string> destinatarios);
    }
}