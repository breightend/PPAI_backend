using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.gestor
{
    public class PantallaCierreOrdenInspeccion
    {
        public class PantallaOrdenInspeccion
        {
            public void presentarOrdenDeInspeccion(List<DatosOI> ordenes)
            {
                foreach (var oi in ordenes)
                {
                    Console.WriteLine($"Orden #{oi.Numero} - Estación: {oi.NombreEstacion} - Sismógrafo: {oi.IdSismografo} - Fecha fin: {oi.FechaFin}");
                }
            }

        }

    }
}