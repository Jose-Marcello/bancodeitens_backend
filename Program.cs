// Localização: Program.cs (CÓDIGO FINAL PARA ACA)

using BancoDeItensWebApi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; // Necessário para CreateScope

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    // 🛑 ADICIONE ESTA LINHA: Força o pipeline a retornar 406 (Not Acceptable)
    // se o cliente não aceitar um formato disponível, prevenindo que retorne HTML.
    options.ReturnHttpNotAcceptable = true;

    // Adicione a checagem que o ASP.NET Core exige:
    options.Filters.Add(new ProducesAttribute("application/json"));
});

// === CONFIGURAÇÃO DO DBCONTEXT (COCKROACHDB/POSTGRESQL) ===

// 1. A Connection String será lida da variável de ambiente AZURE-DB-CONN
var connectionString = Environment.GetEnvironmentVariable("AZURE-DB-CONN");

if (string.IsNullOrEmpty(connectionString))
{
    // Fallback de desenvolvimento (se rodar localmente e não definir a variável)
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("A Connection String não foi encontrada.");
    }
}

// 2. Injeção do DbContext
builder.Services.AddDbContext<BancoDeItensContext>(options =>
{
    options.UseNpgsql(connectionString,
        npgsqlOptionsAction: sqlOptions =>
        {
            // CORREÇÃO CRÍTICA DO COCKROACHDB: Força a execução sem transações longas
            sqlOptions.MinBatchSize(1);

            // Ativa a retentativa padrão do EF Core (Resolve a falha de conexão temporária)
            // A linha 41 no seu código
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                // ADICIONE ESTA LINHA: O C# 8/9+ exige que você defina a lista.
                errorCodesToAdd: null
            );
        })
        .LogTo(Console.WriteLine, LogLevel.Information); // Útil para logs de startup
});

// === FIM DA CONFIGURAÇÃO DE DBCONTEXT ===


// Configuração do CORS: Permitir que o Front-end Angular acesse esta API
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy.WithOrigins("http://localhost:4200", "https://orange-mud-08279831e.3.azurestaticapps.net")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Adiciona o Swagger/OpenAPI (Opcional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Removendo o bloco de migração automática (causa o crash em containers)
/*
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<BancoDeItensContext>();
        db.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao tentar aplicar as migrações.");
    }
}
*/


// Configura o pipeline de requisição HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Comentado para ACA Proxy

// Usa a política de CORS configurada
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();