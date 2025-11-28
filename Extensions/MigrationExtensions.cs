using BancoDeItens.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoDeItensWebApi.Extensions
{
    // Classe estática para adicionar o método ApplyMigrations ao WebApplication
    public static class MigrationExtensions
    {
        public static WebApplication ApplyMigrations(this WebApplication app)
        {
            // O uso do 'using' garante que o escopo seja descartado após o uso
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Obtém o DbContext (BancoDeItensContext)
                    var context = services.GetRequiredService<BancoDeItensContext>();

                    // 🛑 APLICA A MIGRATION DE FATO NO BANCO DE DADOS 🛑
                    context.Database.Migrate();

                    // Adiciona um log simples para confirmar
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Migrations aplicadas com sucesso!");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ocorreu um erro ao aplicar as Migrations.");
                    // Lança a exceção para que o container falhe e o erro seja visível
                    throw;
                }
            }
            return app;
        }
    }
}