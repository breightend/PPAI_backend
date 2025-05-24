using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.services;

namespace PPAI_backend.models.entities
{
    public class Usuario
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;

        public  Empleado Empleado { get; set; }

        public Empleado ObtenerEmpleado()
        {
            if (Empleado == null)
                throw new Exception($"No se encontró empleado para el usuario {NombreUsuario}");
            
            return Empleado;
        }
    }
}