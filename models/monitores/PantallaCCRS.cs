using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.monitores
{
    public class PantallaCCRS
    {
        public string estadoActual { get; set; } = string.Empty;

        public DateTime? fechaUltimaActualizacion { get; set; }

        public DateTime? horaUltimaActualizacion { get; set; }

        public List<string> comentarios { get; set; } = new List<string>();

        public List<string> motivos { get; set; } = new List<string>();

        public void NotificarOrdenDeInspeccion(string mensaje)
        {
            //Aca le tengo que enviar a una pantalla del frontend la informacion que necesito
        }
    }
}