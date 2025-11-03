using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus.DataSets;
using PPAI_backend.models.entities;

namespace PPAI_backend.models.interfaces
{
    public interface IObservadorNotificacion
    {
        void Actualizar(int identificadorSismografo, string nombreEstado, DateTime fecha, List<string> motivos, List<string> comentarios, List<string> destinatarios);
    }
}