using Domain.Entities;
using Domain.ValueTypes;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infraestructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PartnerMeshDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<PartnerMeshDbContext>>();

        try
        {
            // Aplicar migrations pendentes
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations aplicadas com sucesso");

            // Verificar se já existem dados
            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("Banco de dados já contém dados. Seeding ignorado.");
                return;
            }

            logger.LogInformation("Iniciando seeding do banco de dados...");

            // 1. Criar Vetor Padrão
            var vetorPrincipal = new Vetor("Vetor Principal", "vetor@partnermesh.com");
            await context.Vetores.AddAsync(vetorPrincipal);
            await context.SaveChangesAsync();
            logger.LogInformation($"Vetor Principal criado: {vetorPrincipal.Id}");

            // 2. Criar Admin Global
            var adminGlobal = new User(
                "Admin Global",
                "admin@partnermesh.com",
                BCrypt.Net.BCrypt.HashPassword("123456"),
                PermissionEnum.AdminGlobal
            );
            await context.Users.AddAsync(adminGlobal);
            await context.SaveChangesAsync();
            logger.LogInformation($"Admin Global criado: {adminGlobal.Email}");

            // 3. Criar Admin Vetor
            var adminVetor = new User(
                "Admin Vetor",
                "adminvetor@partnermesh.com",
                BCrypt.Net.BCrypt.HashPassword("123456"),
                PermissionEnum.AdminVetor
            );
            adminVetor.AddVetor(vetorPrincipal.Id);
            await context.Users.AddAsync(adminVetor);
            await context.SaveChangesAsync();
            logger.LogInformation($"Admin Vetor criado: {adminVetor.Email}");

            // 4. Criar Operador
            var operador = new User(
                "Operador Sistema",
                "operador@partnermesh.com",
                BCrypt.Net.BCrypt.HashPassword("123456"),
                PermissionEnum.Operador
            );
            operador.AddVetor(vetorPrincipal.Id);
            await context.Users.AddAsync(operador);
            await context.SaveChangesAsync();
            logger.LogInformation($"Operador criado: {operador.Email}");

            // 5. Criar Vetor Secundário para testes multi-vetor
            var vetorSecundario = new Vetor("Vetor Secundário", "vetor2@partnermesh.com");
            await context.Vetores.AddAsync(vetorSecundario);
            await context.SaveChangesAsync();
            logger.LogInformation($"Vetor Secundário criado: {vetorSecundario.Id}");

            logger.LogInformation("Seeding concluído com sucesso!");
            logger.LogInformation("=".PadRight(50, '='));
            logger.LogInformation("CREDENCIAIS DE ACESSO:");
            logger.LogInformation("Admin Global: admin@partnermesh.com / 123456");
            logger.LogInformation("Admin Vetor: adminvetor@partnermesh.com / 123456");
            logger.LogInformation("Operador: operador@partnermesh.com / 123456");
            logger.LogInformation("=".PadRight(50, '='));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro durante o seeding do banco de dados");
            throw;
        }
    }
}
