using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPAI_backend.models.entities
{
    public class GestorCerrarOrdenDeInspeccion
    {
        private Sesion actualSesion = new Sesion
        {
            Usuario = new Usuario()
        };

        private Empleado empleado;

        public Empleado buscarEmpleadoRI()
        {
            empleado = actualSesion.buscarEmpleadoRI();
            return empleado;
        }
        private List<OrdenDeInspeccion> ordenesInspeccion = new List<OrdenDeInspeccion>();


        public List<DatosOI> BuscarOrdenInspeccion(Empleado empleado)
        {
            List<DatosOI> resultado = new List<DatosOI>();

            foreach (var oi in ordenesInspeccion)
            {
                if (oi.esDelEmpleado(empleado) && oi.estaRealizada())
                {
                    var nombreEstacion = oi.getDatosEstacion().NombreEstacion;
                    var idSismografo = oi.getDatosEstacion().IdentificadorSismografo;

                    resultado.Add(new DatosOI
                    {
                        Numero = oi.getNumeroOrden(),
                        FechaFin = oi.getFechaFin(), // o fecha cierre según tu diseño
                        NombreEstacion = nombreEstacion,
                        IdSismografo = idSismografo
                    });
                }
            }

            return resultado;
        }
        public List<DatosOI> OrdenarOrdenInspeccion(List<DatosOI> ordenes)
        {
            return ordenes.OrderBy(o => o.FechaFin).ToList();
        }





    }
}
