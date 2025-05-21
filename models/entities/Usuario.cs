using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Usuario
    {
        public string Correo { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public  Empleado Empleado { get; set; }

        public Empleado obtenerEmpleado()
        {
            return Empleado;
        }
    }
}