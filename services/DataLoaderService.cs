using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PPAI_backend.models.entities;

namespace PPAI_backend.services
{
    public class DataLoaderService
    {
        private readonly JsonMappingService _mappingService;
        
        // Almacenamos las entidades cargadas
        public List<Empleado> Empleados { get; private set; } = new();
        public List<Usuario> Usuarios { get; private set; } = new();
        public List<Estado> Estados { get; private set; } = new();
        public List<Motivo> Motivos { get; private set; } = new();
        public List<Sismografo> Sismografos { get; private set; } = new();
        public List<EstacionSismologica> EstacionesSismologicas { get; private set; } = new();
        public List<OrdenDeInspeccion> OrdenesDeInspeccion { get; private set; } = new();
        public List<Sesion> Sesiones { get; private set; } = new();

        // Lista para almacenar motivos seleccionados temporalmente
        public List<Motivo> MotivosSeleccionados { get; private set; } = new();

        public DataLoaderService()
        {
            _mappingService = new JsonMappingService();
        }

        /// <summary>
        /// Carga todos los datos desde el archivo JSON y los mapea a entidades
        /// </summary>
        /// <param name="jsonFilePath">Ruta al archivo JSON</param>
        public async Task LoadAllDataAsync(string jsonFilePath = "datos/datos.json")
        {
            try
            {
                // 1. Cargar datos JSON
                await _mappingService.LoadJsonDataAsync(jsonFilePath);

                // 2. Mapear todas las entidades en orden de dependencias
                Estados = _mappingService.GetEstados();
                Motivos = _mappingService.GetMotivos();
                Empleados = _mappingService.GetEmpleados();
                Usuarios = _mappingService.GetUsuarios();
                Sismografos = _mappingService.GetSismografos();
                EstacionesSismologicas = _mappingService.GetEstacionesSismologicas();
                OrdenesDeInspeccion = _mappingService.GetOrdenesDeInspeccion();
                Sesiones = _mappingService.GetSesiones();

                Console.WriteLine($"Datos cargados exitosamente:");
                Console.WriteLine($"- {Empleados.Count} empleados");
                Console.WriteLine($"- {Usuarios.Count} usuarios");
                Console.WriteLine($"- {Estados.Count} estados");
                Console.WriteLine($"- {Motivos.Count} motivos");
                Console.WriteLine($"- {Sismografos.Count} sismógrafos");
                Console.WriteLine($"- {EstacionesSismologicas.Count} estaciones sismológicas");
                Console.WriteLine($"- {OrdenesDeInspeccion.Count} órdenes de inspección");
                Console.WriteLine($"- {Sesiones.Count} sesiones");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar los datos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Busca un empleado por su nombre de usuario
        /// </summary>
        public Empleado? BuscarEmpleadoPorUsuario(string nombreUsuario)
        {
            var usuario = Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
            return usuario?.Empleado;
        }

        /// <summary>
        /// Obtiene las órdenes de inspección de un empleado específico
        /// </summary>
        public List<OrdenDeInspeccion> GetOrdenesPorEmpleado(Empleado empleado)
        {
            return OrdenesDeInspeccion.Where(o => o.esDelEmpleado(empleado)).ToList();
        }

        /// <summary>
        /// Obtiene las órdenes finalizadas pero no cerradas
        /// </summary>
        public List<OrdenDeInspeccion> GetOrdenesFinalizadasSinCerrar()
        {
            return OrdenesDeInspeccion.Where(o => o.estaRealizada() && o.getFechaHoraCierre() == DateTime.MinValue).ToList();
        }

        /// <summary>
        /// Obtiene los sismógrafos con un estado específico
        /// </summary>
        public List<Sismografo> GetSismografosPorEstado(string nombreEstado)
        {
            return Sismografos.Where(s => 
                s.CambioEstado.Any(ce => ce.esEstadoActual() && ce.getEstado().Nombre == nombreEstado)
            ).ToList();
        }

        /// <summary>
        /// Refresca los datos desde el archivo JSON
        /// </summary>
        public async Task RefreshDataAsync(string jsonFilePath = "datos/datos.json")
        {
            _mappingService.ClearCache();
            await LoadAllDataAsync(jsonFilePath);
        }

        /// <summary>
        /// Guarda los motivos seleccionados (puedes adaptar la lógica según tu necesidad)
        /// </summary>
        public void GuardarMotivosSeleccionados(List<Motivo> motivos)
        {
            MotivosSeleccionados.Clear();
            MotivosSeleccionados.AddRange(motivos);
        }
    }
}