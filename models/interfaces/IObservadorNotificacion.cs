using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.models.entities;

namespace PPAI_backend.models.interfaces
{
    public interface IObservadorNotificacion
    {
        void Actualizar(OrdenDeInspeccion orden);
    }
}