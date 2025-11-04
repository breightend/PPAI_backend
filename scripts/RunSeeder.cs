using Microsoft.EntityFrameworkCore;
using PPAI_backend.services;

namespace PPAI_backend.scripts
{
    public class RunSeeder
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🌱 Iniciando Database Seeder...");

            // Read connection string from environment variable to avoid hardcoded secrets
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Environment variable 'ConnectionStrings__DefaultConnection' is not set. Aborting seeder.");
                return;
            }

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            using var context = new ApplicationDbContext(options);
            var seeder = new DatabaseSeeder(context);

            await seeder.SeedDatabaseAsync();
            await seeder.MostrarEstadisticas();

            Console.WriteLine("✅ Seeding completado!");
        }
    }
}