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
            return Ambito.ToLower() == "sismógrafo"; // O como esté escrito exactamente en tus datos
        }


    }
}