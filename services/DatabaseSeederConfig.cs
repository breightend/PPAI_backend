using System.ComponentModel;
using Bogus;

namespace PPAI_backend.services
{
    /// <summary>
    /// Configuración para el generador de datos aleatorios de la base de datos
    /// </summary>
    public class DatabaseSeederConfig
    {
        #region Configuración de Cantidad de Registros

        [Description("Número de empleados a generar")]
        public int NumeroEmpleados { get; set; } = 20;

        [Description("Número de usuarios a generar (subset de empleados)")]
        public int NumeroUsuarios { get; set; } = 10;

        [Description("Número de sismógrafos a generar")]
        public int NumeroSismografos { get; set; } = 25;

        [Description("Número de estaciones sismológicas a generar")]
        public int NumeroEstaciones { get; set; } = 15;

        [Description("Número de órdenes de inspección a generar")]
        public int NumeroOrdenes { get; set; } = 30;

        [Description("Número de sesiones históricas a generar")]
        public int NumeroSesiones { get; set; } = 10;

        [Description("Número de motivos fuera de servicio a generar")]
        public int NumeroMotivos { get; set; } = 15;

        #endregion

        #region Configuración de Fechas

        [Description("Años hacia atrás para fechas de adquisición de sismógrafos")]
        public int AñosAtrasAdquisicion { get; set; } = 10;

        [Description("Meses hacia atrás para órdenes de inspección")]
        public int MesesAtrasOrdenes { get; set; } = 6;

        [Description("Días hacia atrás para sesiones")]
        public int DiasAtrasSesiones { get; set; } = 30;

        #endregion

        #region Configuración de Datos Específicos

        [Description("Idioma para generar datos falsos (es, en, etc.)")]
        public string IdiomaFaker { get; set; } = "es";

        [Description("Porcentaje de estaciones que tienen certificación (0.0 - 1.0)")]
        public float PorcentajeCertificacion { get; set; } = 0.8f;

        [Description("Rango mínimo de latitud para Argentina")]
        public double LatitudMinima { get; set; } = -56.0;

        [Description("Rango máximo de latitud para Argentina")]
        public double LatitudMaxima { get; set; } = -17.0;

        [Description("Contraseña por defecto para usuarios (en producción usar hash)")]
        public string ContraseñaDefecto { get; set; } = "123456";

        [Description("¿Limpiar datos existentes antes de generar nuevos?")]
        public bool LimpiarDatosExistentes { get; set; } = true;

        [Description("Empleados con emails reales para testing de notificaciones")]
        public List<(string Email, string Nombre, string Apellido, string RolNombre)> EmpleadosReales { get; set; } = new()
        {
            ("brendatapa6@gmail.com", "Brenda", "Desarrolladora", "Responsable de Reparación"),
            ("ramondelligonzalo5@gmail.com", "Gonzalo", "Técnico Senior", "Responsable de Reparación"),
            ("mikaelaherrero26@gmail.com", "Mikaela", "Supervisor", "Responsable de Reparación"),
            ("mikaelaherrero26@gmail.com", "Ana", "Técnico", "Responsable de Reparación"),
            ("mikaelaherrero26@gmail.com", "Ana", "Técnico Junior", "Responsable de Reparación"),
            ("braian@sumail.com", "Braian", "Técnico Junior", "Responsable de Detección"),
        };

        #endregion

        #region Listas de Datos Personalizables

        [Description("Roles predefinidos del sistema")]
        public List<(string Nombre, string Descripcion)> RolesPredefinidos { get; set; } = new()
        {
            ("Responsable de Inspección", "Responsable de realizar y supervisar inspecciones"),
            ("Tecnico de Reparaciones", "Técnico especializado en reparación de equipos"),
            ("Administrador", "Administrador del sistema"),
            ("Supervisor", "Supervisor de operaciones"),
            ("Analista", "Analista de datos sísmicos")
        };

        [Description("Tipos de motivo predefinidos")]
        public List<(int Id, string Descripcion)> TiposMotivosPredefinidos { get; set; } = new()
        {
            (1, "Falla técnica del equipo"),
            (2, "Mantenimiento preventivo programado"),
            (3, "Calibración requerida"),
            (4, "Condiciones climáticas adversas"),
            (5, "Falta de suministro eléctrico"),
            (6, "Daño por vandalismo"),
            (7, "Actualización de software"),
            (8, "Reemplazo de componentes"),
            (9, "Interferencia electromagnética"),
            (10, "Problemas de conectividad")
        };

        [Description("Estados predefinidos del sistema")]
        public List<(string Nombre, string Descripcion, string Ambito)> EstadosPredefinidos { get; set; } = new()
        {
            // Estados para Órdenes de Inspección
            ("Pendiente", "Orden pendiente de ejecución", "OrdenDeInspeccion"),
            ("En Progreso", "Orden en proceso de ejecución", "OrdenDeInspeccion"),
            ("Finalizada", "Orden completada exitosamente", "OrdenDeInspeccion"),
            ("Cerrada", "Orden cerrada y archivada", "OrdenDeInspeccion"),
            ("Cancelada", "Orden cancelada por motivos externos", "OrdenDeInspeccion"),

            // Estados para Sismógrafos
            ("Operativo", "Equipo funcionando correctamente", "Sismografo"),
            ("Fuera de Servicio", "Equipo no operativo", "Sismografo"),
            ("En Mantenimiento", "Equipo en proceso de mantenimiento", "Sismografo"),
            ("En Calibración", "Equipo siendo calibrado", "Sismografo"),
            ("Dañado", "Equipo con daños reportados", "Sismografo"),

            // Estados para Estaciones
            ("Activa", "Estación en funcionamiento normal", "EstacionSismologica"),
            ("Inactiva", "Estación temporalmente inactiva", "EstacionSismologica"),
            ("En Construcción", "Estación en proceso de instalación", "EstacionSismologica")
        };

        [Description("Ciudades argentinas para nombres de estaciones")]
        public List<string> CiudadesArgentinas { get; set; } = new()
        {
            "Buenos Aires", "Córdoba", "Rosario", "Mendoza", "Tucumán", "La Plata",
            "Mar del Plata", "Salta", "San Juan", "Resistencia", "Neuquén",
            "Santiago del Estero", "Corrientes", "Posadas", "Bahía Blanca",
            "Paraná", "Formosa", "San Luis", "Catamarca", "La Rioja",
            "Santa Rosa", "Rawson", "Viedma", "San Salvador de Jujuy", "Ushuaia"
        };

        #endregion

        #region Configuración de Rangos

        [Description("Rango mínimo para números de serie")]
        public int NumeroSerieMinimo { get; set; } = 100000;

        [Description("Rango máximo para números de serie")]
        public int NumeroSerieMaximo { get; set; } = 999999;

        [Description("Rango mínimo para números de certificación")]
        public int NumeroCertificacionMinimo { get; set; } = 1000;

        [Description("Rango máximo para números de certificación")]
        public int NumeroCertificacionMaximo { get; set; } = 9999;

        [Description("Número mínimo de motivos por cambio de estado")]
        public int MotivosMinimos { get; set; } = 1;

        [Description("Número máximo de motivos por cambio de estado")]
        public int MotivosMaximos { get; set; } = 3;

        #endregion

        #region Constructor y Configuración de Semilla

        /// <summary>
        /// Constructor que configura la semilla para datos consistentes
        /// </summary>
        public DatabaseSeederConfig()
        {
            // ✅ IMPORTANTE: Seed fijo para que todos generen los mismos datos
            // Esto asegura que todos los desarrolladores obtengan exactamente los mismos registros
            Randomizer.Seed = new Random(12345);
        }

        #endregion

        #region Métodos de Utilidad

        /// <summary>
        /// Configuración estándar para compartir entre el equipo
        /// </summary>
        public static DatabaseSeederConfig TeamShared => new()
        {
            NumeroEmpleados = 20,
            NumeroUsuarios = 10,
            NumeroSismografos = 25,
            NumeroEstaciones = 15,
            NumeroOrdenes = 30,
            NumeroSesiones = 10,
            NumeroMotivos = 15,
            LimpiarDatosExistentes = true, // Siempre empezar limpio
            ContraseñaDefecto = "123456", // Password común para testing
            IdiomaFaker = "es"
        };

        /// <summary>
        /// Obtiene la configuración por defecto
        /// </summary>
        public static DatabaseSeederConfig Default => TeamShared;

        /// <summary>
        /// Configuración para testing (menos datos)
        /// </summary>
        public static DatabaseSeederConfig Testing => new()
        {
            NumeroEmpleados = 5,
            NumeroUsuarios = 3,
            NumeroSismografos = 5,
            NumeroEstaciones = 3,
            NumeroOrdenes = 8,
            NumeroSesiones = 2,
            NumeroMotivos = 5
        };

        /// <summary>
        /// Configuración para producción (más datos)
        /// </summary>
        public static DatabaseSeederConfig Production => new()
        {
            NumeroEmpleados = 100,
            NumeroUsuarios = 50,
            NumeroSismografos = 200,
            NumeroEstaciones = 80,
            NumeroOrdenes = 500,
            NumeroSesiones = 100,
            NumeroMotivos = 50,
            LimpiarDatosExistentes = false // Más seguro en producción
        };

        #endregion
    }
}