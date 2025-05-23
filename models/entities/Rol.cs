using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Rol
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public bool esReparacion()
        {
            return Nombre == "Tecnico de Reparaciones";
        }
    }
}