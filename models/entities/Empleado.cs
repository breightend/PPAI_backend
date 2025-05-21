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

        //Relaciones
        public required Rol Rol { get; set; }

        // Metodos:
        public string getApellido() { return Apellido; }
        public string getNombre() { return Nombre; }
        public string getMail() { return Mail; }
        public string getTelefono() { return Telefono; }
        public Rol getRol() { return Rol; }
        public bool esResponsableDeReparacion()
        {
            return Rol.Descripcion == "ResponsableDeReparacion";
        }

    }
}