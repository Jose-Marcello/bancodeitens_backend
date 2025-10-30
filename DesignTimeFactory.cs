using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using BancoDeItensWebApi.Data;
using System;

// Esta classe instrui o EF Core como construir o DbContext em tempo de design (quando rodamos 'dotnet ef')
public class DesignTimeFactory : IDesignTimeDbContextFactory<BancoDeItensContext>
{
    public BancoDeItensContext CreateDbContext(string[] args)
    {
        // A Connection String � lida da vari�vel de ambiente que definimos no PowerShell.
        var connectionString = Environment.GetEnvironmentVariable("AZURE_POSTGRES_CONSTR");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "A Connection String 'AZURE_POSTGRES_CONSTR' n�o foi definida. Defina-a no PowerShell para rodar a Migration.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<BancoDeItensContext>();

        // Aqui est� a instru��o de como usar o provedor Npgsql e a Connection String.
        optionsBuilder.UseNpgsql(connectionString);

        return new BancoDeItensContext(optionsBuilder.Options);
    }
}