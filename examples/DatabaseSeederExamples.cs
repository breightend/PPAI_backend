// 📝 EJEMPLOS DE PERSONALIZACIÓN
// Copia y modifica estos ejemplos en DatabaseSeederConfig.cs según tus necesidades

using PPAI_backend.services;

namespace PPAI_backend.examples
{
    public static class DatabaseSeederExamples
    {
        /// <summary>
        /// Configuración para desarrollo local - Pocos datos, generación rápida
        /// </summary>
        public static DatabaseSeederConfig ConfiguracionDesarrollo => new()
        {
            // Cantidades pequeñas para desarrollo
            NumeroEmpleados = 8,
            NumeroUsuarios = 5,
            NumeroSismografos = 10,
            NumeroEstaciones = 6,
            NumeroOrdenes = 15,
            NumeroSesiones = 3,
            NumeroMotivos = 8,

            // Fechas recientes para pruebas
            MesesAtrasOrdenes = 2,
            DiasAtrasSesiones = 7,

            // Limpiar datos siempre en desarrollo
            LimpiarDatosExistentes = true,

            // Datos específicos para Argentina
            CiudadesArgentinas = new() { "Buenos Aires", "Córdoba", "Rosario", "Mendoza", "La Plata" }
        };

        /// <summary>
        /// Configuración para testing automatizado
        /// </summary>
        public static DatabaseSeederConfig ConfiguracionTesting => new()
        {
            // Datos mínimos para tests rápidos
            NumeroEmpleados = 3,
            NumeroUsuarios = 2,
            NumeroSismografos = 3,
            NumeroEstaciones = 2,
            NumeroOrdenes = 5,
            NumeroSesiones = 1,
            NumeroMotivos = 3,

            // Contraseñas conocidas para testing
            ContraseñaDefecto = "test123",

            // Siempre limpiar en tests
            LimpiarDatosExistentes = true
        };

        /// <summary>
        /// Configuración para demo - Datos representativos pero no excesivos
        /// </summary>
        public static DatabaseSeederConfig ConfiguracionDemo => new()
        {
            NumeroEmpleados = 25,
            NumeroUsuarios = 15,
            NumeroSismografos = 30,
            NumeroEstaciones = 20,
            NumeroOrdenes = 60,
            NumeroSesiones = 20,
            NumeroMotivos = 20,

            // Datos más antiguos para mostrar historial
            AñosAtrasAdquisicion = 15,
            MesesAtrasOrdenes = 12,
            DiasAtrasSesiones = 90,

            // Más certificaciones para demo
            PorcentajeCertificacion = 0.9f
        };

        /// <summary>
        /// Configuración personalizada con roles específicos
        /// </summary>
        public static DatabaseSeederConfig ConfiguracionRolesPersonalizados => new()
        {
            NumeroEmpleados = 20,
            
            // Roles específicos de tu organización
            RolesPredefinidos = new()
            {
                ("Supervisor Regional", "Supervisa múltiples estaciones en una región"),
                ("Técnico Senior", "Técnico con experiencia avanzada"),
                ("Técnico Junior", "Técnico en entrenamiento"),
                ("Ingeniero de Campo", "Ingeniero especializado en trabajo de campo"),
                ("Coordinador Logístico", "Coordina recursos y materiales"),
                ("Analista de Datos", "Especialista en análisis de datos sísmicos"),
                ("Responsable de Calidad", "Encargado del control de calidad"),
                ("Administrador de Sistema", "Administra la infraestructura técnica")
            }
        };

        /// <summary>
        /// Configuración con motivos específicos de tu dominio
        /// </summary>
        public static DatabaseSeederConfig ConfiguracionMotivosPersonalizados => new()
        {
            NumeroEmpleados = 15,

            TiposMotivosPredefinidos = new()
            {
                (1, "Falla del sensor principal"),
                (2, "Interferencia sísmica externa"),
                (3, "Calibración anual obligatoria"),
                (4, "Actualización de firmware"),
                (5, "Mantenimiento preventivo trimestral"),
                (6, "Reparación post-sismo"),
                (7, "Instalación de mejoras"),
                (8, "Verificación de precisión"),
                (9, "Cambio de ubicación del sensor"),
                (10, "Pruebas de rendimiento"),
                (11, "Limpieza profunda del equipo"),
                (12, "Reemplazo de batería de respaldo")
            }
        };

        /// <summary>
        /// Configuración con ubicaciones geográficas específicas
        /// </summary>
        public static DatabaseSeederConfig ConfiguracionRegionEspecifica => new()
        {
            // Enfocado en región pampeana
            CiudadesArgentinas = new()
            {
                "Buenos Aires", "La Plata", "Mar del Plata", "Tandil", 
                "Olavarría", "Bahía Blanca", "Santa Rosa", "General Pico",
                "Pergamino", "Junín", "Trenque Lauquen", "Necochea"
            },

            // Coordenadas para región pampeana
            LatitudMinima = -39.0,
            LatitudMaxima = -33.0,

            // Más estaciones en esta región
            NumeroEstaciones = 25,
            NumeroSismografos = 30
        };

        /// <summary>
        /// Configuración para pruebas de rendimiento
        /// </summary>
        public static DatabaseSeederConfig ConfiguracionRendimiento => new()
        {
            // Muchos datos para probar rendimiento
            NumeroEmpleados = 200,
            NumeroUsuarios = 100,
            NumeroSismografos = 500,
            NumeroEstaciones = 150,
            NumeroOrdenes = 1000,
            NumeroSesiones = 500,
            NumeroMotivos = 100,

            // No limpiar para agregar a datos existentes
            LimpiarDatosExistentes = false,

            // Rango amplio de fechas
            AñosAtrasAdquisicion = 20,
            MesesAtrasOrdenes = 24,
            DiasAtrasSesiones = 365
        };
    }

    /// <summary>
    /// Ejemplos de cómo aplicar configuraciones personalizadas
    /// </summary>
    public static class EjemplosDeUso
    {
        // EJEMPLO 1: Cambiar configuración en DatabaseSeeder.cs
        /*
        public DatabaseSeeder(ApplicationDbContext context)
        {
            _context = context;
            
            // Usar una configuración específica
            _config = DatabaseSeederExamples.ConfiguracionDesarrollo;
            
            _faker = new Faker(_config.IdiomaFaker);
        }
        */

        // EJEMPLO 2: Crear configuración híbrida
        /*
        public DatabaseSeeder(ApplicationDbContext context)
        {
            _context = context;
            
            // Combinar configuraciones
            _config = DatabaseSeederConfig.Default;
            _config.NumeroEmpleados = 50; // Más empleados
            _config.LimpiarDatosExistentes = false; // No limpiar
            _config.RolesPredefinidos = DatabaseSeederExamples
                .ConfiguracionRolesPersonalizados.RolesPredefinidos; // Roles custom
                
            _faker = new Faker(_config.IdiomaFaker);
        }
        */

        // EJEMPLO 3: Configuración basada en entorno
        /*
        public DatabaseSeeder(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            
            if (env.IsDevelopment())
                _config = DatabaseSeederExamples.ConfiguracionDesarrollo;
            else if (env.IsStaging())
                _config = DatabaseSeederExamples.ConfiguracionDemo;
            else
                _config = DatabaseSeederExamples.ConfiguracionRendimiento;
                
            _faker = new Faker(_config.IdiomaFaker);
        }
        */
    }
}