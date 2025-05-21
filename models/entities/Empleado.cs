using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Empleado
    {
        public required string Apellido { get; set; }
        public required string Mail { get; set; }
        public required string Nombre { get; set; }
        public required string Telefono { get; set; }
    }
}