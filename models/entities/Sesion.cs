using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class Sesion
    {
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }

        public required Usuario Usuario { get; set; }

        

        public Empleado buscarEmpleadoRI()
        {
            return Usuario.obtenerEmpleado();
        } 
    }
}