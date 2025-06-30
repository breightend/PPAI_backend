using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Estado
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Ambito { get; set; } = string.Empty;

        public bool esFinalizada()
        {
            return Nombre == "Finalizada";
        }

        public bool esAmbitoSismografo()
        {
            return Ambito.ToLower() == "Sismografo".ToLower();
        }
        public bool esAmbitoOrden()
        {
            return Ambito.ToLower() == "OrdenDeInspeccion".ToLower();
        }

        public bool esEstadoCerrada()
        {
            return Nombre == "Cerrada";
        } 
        
        public bool estadoFueraServicio()
        {
            return Nombre == "Fuera de Servicio";
        }


    }
}