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
        private List<(MotivoDTO motivo, string comentario)> motivosSeleccionados = new();



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
            string jsonPath = "datos/datos.json"; // Ensure this path is correct relative to your execution directory
            string json = File.ReadAllText(jsonPath);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Check if the root element has a "motivos" property and it's an array
            if (root.TryGetProperty("motivos", out JsonElement motivosJsonArray) && motivosJsonArray.ValueKind == JsonValueKind.Array)
            {
                var motivos = new List<MotivoDTO>();

                foreach (var m in motivosJsonArray.EnumerateArray())
                {
                motivos.Add(new MotivoDTO
                {
                    Id = m.GetProperty("id").GetInt32(),
                    Descripcion = m.GetProperty("descripcion").GetString()!
                    // The "comentario" field from the JSON is not mapped to MotivoDTO
                });
                }
                return motivos;
            }
            else
            {
                // Handle the case where "motivos" property is not found or is not an array
                // This could be returning an empty list, throwing an exception, or logging an error
                // For now, returning an empty list or throwing an exception might be appropriate.
                // Let's throw an exception if the structure is not as expected.
                throw new JsonException("JSON structure does not contain a 'motivos' array or it's not in the expected format.");
            }
            }
            
        // Este metodo hay que cambiarlo pq ya hay un metodo obtenerMotivo en la clase Motivo
        public void tomarMotivoFueraDeServicio(List<MotivoSeleccionadoConComentarioDTO> seleccionados)
        {
            var todosLosMotivos = ObtenerMotivosDesdeJson();

            foreach (var item in seleccionados)
            {
                var motivo = todosLosMotivos.FirstOrDefault(m => m.Id == item.IdMotivo);
                if (motivo != null)
                {
                    motivosSeleccionados.Add((motivo, item.Comentario));
                }
                else
                {
                    throw new Exception($"Motivo con ID {item.IdMotivo} no encontrado.");
                }
            }
        }

        public string confirmar()
            {
                if (ordenSeleccionada == null)
                    throw new Exception("No hay una orden seleccionada para cerrar.");

                ordenSeleccionada.FechaHoraCierre = DateTime.Now; // Toma la hora de cierre de la orden de inspeccion

                return $"Orden N° {ordenSeleccionada.NumeroOrden} cerrada correctamente.";
            }


    }
}
