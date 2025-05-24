using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Empleado
    {
        public string Apellido { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public required Rol Rol { get; set; }

        // Metodos:
        public string GetApellido() { return Apellido; }
        public string GetNombre() { return Nombre; }
        public string GetMail() { return Mail; }
        public string GetTelefono() { return Telefono; }
        public Rol GetRol() { return Rol; }
        public bool EsResponsableDeReparacion()
        {
            return Rol.Descripcion == "ResponsableDeReparacion";
        }
        



    }
}