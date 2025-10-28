using Microsoft.EntityFrameworkCore;
using PPAI_backend.services;

namespace PPAI_backend.scripts
{
    public class RunSeeder
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🌱 Iniciando Database Seeder...");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql("Host=localhost;Database=SismosDB;Username=postgres;Password=postgres")
                .Options;

            using var context = new ApplicationDbContext(options);
            var seeder = new DatabaseSeeder(context);

            await seeder.SeedDatabaseAsync();
            await seeder.MostrarEstadisticas();

            Console.WriteLine("✅ Seeding completado!");
        }
    }
}