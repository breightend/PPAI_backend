using Microsoft.EntityFrameworkCore;
using PPAI_backend.models.entities;

public class ApplicationDbContext : DbContext
{
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<CambioEstado> CambiosEstado { get; set; }
    public DbSet<EstacionSismologica> EstacionesSismologicas { get; set; }
    public DbSet<Estado> Estados { get; set; }
    public DbSet<MotivoFueraDeServicio> MotivosFueraDeServicio { get; set; }
    public DbSet<OrdenDeInspeccion> OrdenesDeInspeccion { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Sesion> Sesiones { get; set; }
    public DbSet<TipoMotivo> TiposMotivo { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Sismografo> Sismografos { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}