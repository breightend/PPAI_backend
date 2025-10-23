using Microsoft.EntityFrameworkCore;
using PPAI_backend.models.entities;
using Bogus;

namespace PPAI_backend.services
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly Faker _faker;
        private readonly DatabaseSeederConfig _config;

        public DatabaseSeeder(ApplicationDbContext context)
        {
            _context = context;
            _config = DatabaseSeederConfig.Default; // Puedes cambiar a Testing o Production
            _faker = new Faker(_config.IdiomaFaker);
        }

        public async Task SeedDatabaseAsync()
        {
            Console.WriteLine("🌱 Iniciando generación de datos aleatorios...");
            Console.WriteLine($"📋 Configuración: {_config.NumeroEmpleados} empleados, {_config.NumeroOrdenes} órdenes, {_config.NumeroEstaciones} estaciones");

            // Limpiar datos existentes si está configurado
            if (_config.LimpiarDatosExistentes)
            {
                await LimpiarDatosExistentes();
            }

            // Generar datos en el orden correcto (respetando dependencias)
            await GenerarRoles();
            await GenerarEmpleados();
            await GenerarUsuarios();
            await GenerarTiposMotivo();
            await GenerarMotivosFueraDeServicio();
            await GenerarEstados();
            await GenerarSismografos();
            await GenerarEstacionesSismologicas();
            await GenerarOrdenesDeInspeccion();
            await GenerarSesiones();

            Console.WriteLine("✅ Generación de datos completada exitosamente!");
        }

        private async Task LimpiarDatosExistentes()
        {
            Console.WriteLine("🧹 Limpiando datos existentes...");
            
            // Eliminar en orden inverso a las dependencias
            _context.Sesiones.RemoveRange(_context.Sesiones);
            _context.OrdenesDeInspeccion.RemoveRange(_context.OrdenesDeInspeccion);
            _context.EstacionesSismologicas.RemoveRange(_context.EstacionesSismologicas);
            _context.CambiosEstado.RemoveRange(_context.CambiosEstado);
            _context.Sismografos.RemoveRange(_context.Sismografos);
            _context.MotivosFueraDeServicio.RemoveRange(_context.MotivosFueraDeServicio);
            _context.TiposMotivo.RemoveRange(_context.TiposMotivo);
            _context.Estados.RemoveRange(_context.Estados);
            _context.Usuarios.RemoveRange(_context.Usuarios);
            _context.Empleados.RemoveRange(_context.Empleados);
            _context.Roles.RemoveRange(_context.Roles);
            
            await _context.SaveChangesAsync();
        }

        private async Task GenerarRoles()
        {
            Console.WriteLine("👥 Generando roles...");

            var roles = _config.RolesPredefinidos.Select(r => new Rol 
            { 
                Nombre = r.Nombre, 
                Descripcion = r.Descripcion 
            }).ToList();

            _context.Roles.AddRange(roles);
            await _context.SaveChangesAsync();
        }

        private async Task GenerarEmpleados()
        {
            Console.WriteLine("👨‍💼 Generando empleados...");

            var roles = await _context.Roles.ToListAsync();
            var empleados = new List<Empleado>();

            // 🔥 EMPLEADOS CON EMAILS REALES PARA TESTING
            // Los emails reales se configuran en DatabaseSeederConfig.cs
            var empleadosReales = _config.EmpleadosReales;

            // Primero crear empleados con emails reales
            Console.WriteLine("📧 Creando empleados con emails reales para testing...");
            foreach (var (email, nombre, apellido, rolNombre) in empleadosReales)
            {
                var rol = roles.FirstOrDefault(r => r.Nombre == rolNombre) ?? roles.First();
                
                var empleadoReal = new Empleado
                {
                    Mail = email, // 📧 EMAIL REAL
                    Nombre = nombre,
                    Apellido = apellido,
                    Telefono = _faker.Phone.PhoneNumber("9########"),
                    Rol = rol
                };

                empleados.Add(empleadoReal);
                Console.WriteLine($"  ✅ {nombre} {apellido} ({email}) - Rol: {rol.Nombre}");
            }

            // Luego generar el resto con emails falsos
            var empleadosRestantes = _config.NumeroEmpleados - empleadosReales.Count;
            Console.WriteLine($"📝 Generando {empleadosRestantes} empleados adicionales con emails aleatorios...");
            
            for (int i = 0; i < empleadosRestantes; i++)
            {
                var empleado = new Empleado
                {
                    Mail = _faker.Internet.Email(), // Email falso para el resto
                    Nombre = _faker.Name.FirstName(),
                    Apellido = _faker.Name.LastName(),
                    Telefono = _faker.Phone.PhoneNumber("9########"),
                    Rol = _faker.PickRandom(roles)
                };

                empleados.Add(empleado);
            }

            _context.Empleados.AddRange(empleados);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Total empleados creados: {empleados.Count}");
            Console.WriteLine($"📧 Empleados con emails reales: {empleadosReales.Count}");
        }

        private async Task GenerarUsuarios()
        {
            Console.WriteLine("👤 Generando usuarios...");

            var empleados = await _context.Empleados.Include(e => e.Rol).ToListAsync();
            var usuarios = new List<Usuario>();

            // Generar usuarios según configuración
            var empleadosSeleccionados = empleados.Take(_config.NumeroUsuarios);

            foreach (var empleado in empleadosSeleccionados)
            {
                var usuario = new Usuario
                {
                    NombreUsuario = $"{empleado.Nombre.ToLower()}.{empleado.Apellido.ToLower()}",
                    Contraseña = _config.ContraseñaDefecto,
                    Empleado = empleado
                };

                usuarios.Add(usuario);
            }

            _context.Usuarios.AddRange(usuarios);
            await _context.SaveChangesAsync();
        }

        private async Task GenerarTiposMotivo()
        {
            Console.WriteLine("📋 Generando tipos de motivo...");

            var tiposMotivo = new List<TipoMotivo>
            {
                new TipoMotivo { Id = 1, Descripcion = "Falla técnica del equipo" },
                new TipoMotivo { Id = 2, Descripcion = "Mantenimiento preventivo programado" },
                new TipoMotivo { Id = 3, Descripcion = "Calibración requerida" },
                new TipoMotivo { Id = 4, Descripcion = "Condiciones climáticas adversas" },
                new TipoMotivo { Id = 5, Descripcion = "Falta de suministro eléctrico" },
                new TipoMotivo { Id = 6, Descripcion = "Daño por vandalismo" },
                new TipoMotivo { Id = 7, Descripcion = "Actualización de software" },
                new TipoMotivo { Id = 8, Descripcion = "Reemplazo de componentes" },
                new TipoMotivo { Id = 9, Descripcion = "Interferencia electromagnética" },
                new TipoMotivo { Id = 10, Descripcion = "Problemas de conectividad" }
            };

            _context.TiposMotivo.AddRange(tiposMotivo);
            await _context.SaveChangesAsync();
        }

        private async Task GenerarMotivosFueraDeServicio()
        {
            Console.WriteLine("⚠️ Generando motivos fuera de servicio...");

            var tiposMotivo = await _context.TiposMotivo.ToListAsync();
            var motivos = new List<MotivoFueraDeServicio>();

            for (int i = 1; i <= 15; i++)
            {
                var motivo = new MotivoFueraDeServicio
                {
                    Id = i,
                    TipoMotivo = _faker.PickRandom(tiposMotivo),
                    Comentario = _faker.Lorem.Sentence(10, 5)
                };

                motivos.Add(motivo);
            }

            _context.MotivosFueraDeServicio.AddRange(motivos);
            await _context.SaveChangesAsync();
        }

        private async Task GenerarEstados()
        {
            Console.WriteLine("📊 Generando estados...");

            var estados = new List<Estado>
            {
                // Estados para Órdenes de Inspección
                new Estado { Nombre = "Pendiente", Descripcion = "Orden pendiente de ejecución", Ambito = "OrdenDeInspeccion" },
                new Estado { Nombre = "En Progreso", Descripcion = "Orden en proceso de ejecución", Ambito = "OrdenDeInspeccion" },
                new Estado { Nombre = "Finalizada", Descripcion = "Orden completada exitosamente", Ambito = "OrdenDeInspeccion" },
                new Estado { Nombre = "Cerrada", Descripcion = "Orden cerrada y archivada", Ambito = "OrdenDeInspeccion" },
                new Estado { Nombre = "Cancelada", Descripcion = "Orden cancelada por motivos externos", Ambito = "OrdenDeInspeccion" },

                // Estados para Sismógrafos
                new Estado { Nombre = "Operativo", Descripcion = "Equipo funcionando correctamente", Ambito = "Sismografo" },
                new Estado { Nombre = "Fuera de Servicio", Descripcion = "Equipo no operativo", Ambito = "Sismografo" },
                new Estado { Nombre = "En Mantenimiento", Descripcion = "Equipo en proceso de mantenimiento", Ambito = "Sismografo" },
                new Estado { Nombre = "En Calibración", Descripcion = "Equipo siendo calibrado", Ambito = "Sismografo" },
                new Estado { Nombre = "Dañado", Descripcion = "Equipo con daños reportados", Ambito = "Sismografo" },

                // Estados para Estaciones
                new Estado { Nombre = "Activa", Descripcion = "Estación en funcionamiento normal", Ambito = "EstacionSismologica" },
                new Estado { Nombre = "Inactiva", Descripcion = "Estación temporalmente inactiva", Ambito = "EstacionSismologica" },
                new Estado { Nombre = "En Construcción", Descripcion = "Estación en proceso de instalación", Ambito = "EstacionSismologica" }
            };

            _context.Estados.AddRange(estados);
            await _context.SaveChangesAsync();
        }

        private async Task GenerarSismografos()
        {
            Console.WriteLine("📡 Generando sismógrafos...");

            var sismografos = new List<Sismografo>();

            for (int i = 1; i <= 25; i++)
            {
                var sismografo = new Sismografo
                {
                    IdentificadorSismografo = i,
                    FechaAdquisicion = _faker.Date.Between(DateTime.Now.AddYears(-10), DateTime.Now.AddYears(-1)),
                    NroSerie = _faker.Random.Number(100000, 999999),
                    CambioEstado = new List<CambioEstado>()
                };

                sismografos.Add(sismografo);
            }

            _context.Sismografos.AddRange(sismografos);
            await _context.SaveChangesAsync();
        }

        private async Task GenerarEstacionesSismologicas()
        {
            Console.WriteLine("🏢 Generando estaciones sismológicas...");

            var empleados = await _context.Empleados.Include(e => e.Rol).ToListAsync();
            var estados = await _context.Estados.Where(e => e.Ambito == "EstacionSismologica").ToListAsync();
            var sismografos = await _context.Sismografos.ToListAsync();
            var estaciones = new List<EstacionSismologica>();

            for (int i = 1; i <= 15; i++)
            {
                var estacion = new EstacionSismologica
                {
                    CodigoEstacion = i,
                    Nombre = $"Estación Sísmica {_faker.Address.City()} - {i:D3}",
                    DocumentoCertificacionAdq = _faker.Random.Bool(0.8f), // 80% tienen certificación
                    FechaSolicitudCertificacion = _faker.Date.Between(DateTime.Now.AddYears(-2), DateTime.Now),
                    Latitud = _faker.Random.Double(-56.0, -17.0), // Latitud aproximada de Argentina
                    NroCertificacionAdquirida = _faker.Random.Number(1000, 9999),
                    Sismografo = _faker.PickRandom(sismografos),
                    Empleado = _faker.PickRandom(empleados),
                    Estado = _faker.PickRandom(estados)
                };

                estaciones.Add(estacion);
                sismografos.Remove(estacion.Sismografo); // Evitar duplicados
            }

            _context.EstacionesSismologicas.AddRange(estaciones);
            await _context.SaveChangesAsync();
        }

        private async Task GenerarOrdenesDeInspeccion()
        {
            Console.WriteLine("📋 Generando órdenes de inspección...");

            var empleados = await _context.Empleados.Include(e => e.Rol).ToListAsync();
            var estados = await _context.Estados.Where(e => e.Ambito == "OrdenDeInspeccion").ToListAsync();
            var estaciones = await _context.EstacionesSismologicas
                .Include(e => e.Sismografo)
                .Include(e => e.Empleado)
                .Include(e => e.Estado)
                .ToListAsync();

            var ordenes = new List<OrdenDeInspeccion>();

            for (int i = 1; i <= 30; i++)
            {
                var fechaInicio = _faker.Date.Between(DateTime.Now.AddMonths(-6), DateTime.Now.AddDays(-1));
                var fechaFin = _faker.Date.Between(fechaInicio, fechaInicio.AddDays(7));
                var estadoAleatorio = _faker.PickRandom(estados);

                var orden = new OrdenDeInspeccion
                {
                    NumeroOrden = i,
                    FechaHoraInicio = fechaInicio,
                    FechaHoraFinalizacion = estadoAleatorio.Nombre == "Finalizada" || estadoAleatorio.Nombre == "Cerrada" ? fechaFin : default,
                    FechaHoraCierre = estadoAleatorio.Nombre == "Cerrada" ? fechaFin.AddHours(_faker.Random.Number(1, 48)) : default,
                    ObservacionCierre = estadoAleatorio.Nombre == "Cerrada" ? 
                        _faker.Lorem.Paragraph(3) : 
                        "Sin observaciones de cierre",
                    Empleado = _faker.PickRandom(empleados),
                    Estado = estadoAleatorio,
                    EstacionSismologica = _faker.PickRandom(estaciones),
                    CambioEstado = new List<CambioEstado>()
                };

                ordenes.Add(orden);
            }

            _context.OrdenesDeInspeccion.AddRange(ordenes);
            await _context.SaveChangesAsync();

            // Generar cambios de estado para las órdenes
            await GenerarCambiosEstadoParaOrdenes();
        }

        private async Task GenerarCambiosEstadoParaOrdenes()
        {
            Console.WriteLine("🔄 Generando cambios de estado para órdenes...");

            var ordenes = await _context.OrdenesDeInspeccion
                .Include(o => o.Estado)
                .Include(o => o.CambioEstado)
                .ToListAsync();

            var estadosOrden = await _context.Estados.Where(e => e.Ambito == "OrdenDeInspeccion").ToListAsync();
            var motivos = await _context.MotivosFueraDeServicio.Include(m => m.TipoMotivo).ToListAsync();

            foreach (var orden in ordenes)
            {
                // Estado inicial (siempre Pendiente)
                var estadoInicial = estadosOrden.First(e => e.Nombre == "Pendiente");
                var cambioInicial = new CambioEstado
                {
                    Estado = estadoInicial,
                    FechaHoraInicio = orden.FechaHoraInicio,
                    FechaHoraFin = orden.Estado.Nombre != "Pendiente" ? orden.FechaHoraInicio.AddHours(_faker.Random.Number(1, 24)) : null,
                    Motivos = new List<MotivoFueraDeServicio>()
                };

                orden.CambioEstado.Add(cambioInicial);

                // Si no está pendiente, agregar cambio al estado actual
                if (orden.Estado.Nombre != "Pendiente")
                {
                    var cambioActual = new CambioEstado
                    {
                        Estado = orden.Estado,
                        FechaHoraInicio = cambioInicial.FechaHoraFin ?? orden.FechaHoraInicio.AddHours(1),
                        FechaHoraFin = orden.Estado.Nombre == "Finalizada" || orden.Estado.Nombre == "Cerrada" ? 
                            orden.FechaHoraFinalizacion : null,
                        Motivos = orden.Estado.Nombre == "Cerrada" ? 
                            _faker.PickRandom(motivos, _faker.Random.Number(1, 3)).ToList() : 
                            new List<MotivoFueraDeServicio>()
                    };

                    orden.CambioEstado.Add(cambioActual);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task GenerarSesiones()
        {
            Console.WriteLine("🔐 Generando sesiones...");

            var usuarios = await _context.Usuarios.Include(u => u.Empleado).ThenInclude(e => e.Rol).ToListAsync();
            var sesiones = new List<Sesion>();

            // Crear algunas sesiones históricas
            for (int i = 0; i < 10; i++)
            {
                var fechaInicio = _faker.Date.Between(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(-1));
                var sesion = new Sesion
                {
                    FechaHoraInicio = fechaInicio,
                    FechaHoraFin = fechaInicio.AddHours(_faker.Random.Number(1, 8)),
                    Usuario = _faker.PickRandom(usuarios)
                };

                sesiones.Add(sesion);
            }

            // Crear una sesión activa para el responsable de inspección
            var responsable = usuarios.FirstOrDefault(u => u.Empleado.Rol.Nombre == "Responsable de Inspección");
            if (responsable != null)
            {
                var sesionActiva = new Sesion
                {
                    FechaHoraInicio = DateTime.Now.AddHours(-2),
                    FechaHoraFin = default, // Sin fecha de fin = sesión activa
                    Usuario = responsable
                };

                sesiones.Add(sesionActiva);
            }

            _context.Sesiones.AddRange(sesiones);
            await _context.SaveChangesAsync();
        }

        public async Task MostrarEstadisticas()
        {
            Console.WriteLine("\n📊 ESTADÍSTICAS DE LA BASE DE DATOS:");
            Console.WriteLine($"👥 Roles: {await _context.Roles.CountAsync()}");
            Console.WriteLine($"👨‍💼 Empleados: {await _context.Empleados.CountAsync()}");
            Console.WriteLine($"👤 Usuarios: {await _context.Usuarios.CountAsync()}");
            Console.WriteLine($"📋 Tipos de Motivo: {await _context.TiposMotivo.CountAsync()}");
            Console.WriteLine($"⚠️ Motivos Fuera de Servicio: {await _context.MotivosFueraDeServicio.CountAsync()}");
            Console.WriteLine($"📊 Estados: {await _context.Estados.CountAsync()}");
            Console.WriteLine($"📡 Sismógrafos: {await _context.Sismografos.CountAsync()}");
            Console.WriteLine($"🏢 Estaciones Sismológicas: {await _context.EstacionesSismologicas.CountAsync()}");
            Console.WriteLine($"📋 Órdenes de Inspección: {await _context.OrdenesDeInspeccion.CountAsync()}");
            Console.WriteLine($"🔄 Cambios de Estado: {await _context.CambiosEstado.CountAsync()}");
            Console.WriteLine($"🔐 Sesiones: {await _context.Sesiones.CountAsync()}");
        }
    }
}