using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using PPAI_backend.datos.dtos;


namespace PPAI_backend.models.entities
{
    public class GestorCerrarOrdenDeInspeccion
    {
        private Sesion actualSesion = new Sesion
        {
            Usuario = new Usuario()
        };

        private Empleado empleado;



        public Empleado BuscarEmpleadoRI()
        {
            return actualSesion.Usuario.ObtenerEmpleado();
        }


        private List<OrdenDeInspeccion> ordenesInspeccion = new List<OrdenDeInspeccion>();
        private List<Motivo> motivosSeleccionados = new();




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

        //Esto hay que borrar! 
        public Sesion ObtenerSesion()
        {
            return actualSesion;
        }


        private OrdenDeInspeccion ordenSeleccionada;
        public void tomarOrdenSeleccionada(int numeroOrden)
        {
            ordenSeleccionada = ordenesInspeccion.FirstOrDefault(oi => oi.getNumeroOrden() == numeroOrden);

            if (ordenSeleccionada == null)
                throw new Exception($"No se encontró la orden número: {numeroOrden} en la lista mostrada anteriormente.");
        }

        public void tomarObservacion(string observacion)
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para tomar la observación.");
            ordenSeleccionada.ObservacionCierre = observacion;

        }
        public List<MotivoDTO> ObtenerMotivosDesdeJson()
        {
            string jsonPath = "datos/datos.json";
            string json = File.ReadAllText(jsonPath);

            using var doc = JsonDocument.Parse(json);
            var motivosJson = doc.RootElement.GetProperty("motivosFueraServicio");

            var motivos = new List<MotivoDTO>();

            foreach (var m in motivosJson.EnumerateArray())
            {
                motivos.Add(new MotivoDTO
                {
                    Id = m.GetProperty("id").GetInt32(),
                    Descripcion = m.GetProperty("descripcion").GetString()!
                });
            }

            return motivos;
        }


        public void tomarMotivoFueraDeServicio(List<MotivoDTO> seleccionados)
        {
            var todosLosMotivos = ObtenerMotivosDesdeJson();

            foreach (var dto in seleccionados)
            {
                var baseMotivo = todosLosMotivos.FirstOrDefault(m => m.Id == dto.Id);
                if (baseMotivo == null)
                    throw new Exception($"Motivo con ID {dto.Id} no encontrado.");

                // Crear un nuevo objeto motivo (el seleccionado) con el comentario agregado por el usuario.
                var motivo = new Motivo
                {
                    Id = baseMotivo.Id,
                    Descripcion = baseMotivo.Descripcion,
                    Comentario = dto.Comentario ?? ""
                };

                motivosSeleccionados.Add(motivo);
            }

            Console.WriteLine("Los motivos seleccionados han sido registrados con éxito!");
        }

        public string confirmar()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No hay una orden seleccionada para cerrar.");

            // ordenSeleccionada.FechaHoraCierre = DateTime.Now; // Toma la hora de cierre de la orden de inspeccion

            return $"Orden N° {ordenSeleccionada.NumeroOrden} cerrada correctamente.";
        }

        public void validarObsYComentario()
        {
            if (ordenSeleccionada == null)
                throw new Exception("No ha seleccionado ninguna orden de inspeccion.");

            if (string.IsNullOrWhiteSpace(ordenSeleccionada.ObservacionCierre))
                throw new Exception("Debe ingresar una observación para cerrar la orden.");

            if (motivosSeleccionados == null) //|| !motivosSeleccionados.Any()
                throw new Exception("Debe seleccionar al menos un motivo.");
        }

        public void buscarEstadoCerrada(List<Estado> estados)
        {
            var estadoCerrada = estados.FirstOrDefault(e => e.Nombre == "Cerrada" && e.Ambito == "OrdenInspeccion");

            if (estadoCerrada == null)
                throw new Exception("No se encontró el estado 'Cerrada' con ámbito 'OrdenInspeccion'.");
        }




    }
}
