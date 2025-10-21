using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Sesion
    {
        [Key]
        public int Id { get; set; }

        
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }

        public required Usuario Usuario { get; set; }

        

        public Empleado BuscarEmpleadoRI()
        {
            if (Usuario == null)
                throw new Exception("No se encontró el usuario en la sesión.");
            return Usuario.ObtenerEmpleado();
        } 
    }
}