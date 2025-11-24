// Localização: Program.cs (CÓDIGO FINAL PARA AZURE SQL DB)

using BancoDeItensWebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Builder; // Necessário para ApplicationBuilder

var builder = WebApplication.CreateBuilder(args);

// === CONFIGURAÇÃO DE SERVIÇOS INICIAIS ===

// 🟢 CORREÇÃO 1: Adiciona o serviço de controladores (MVC/API)
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
    options.Filters.Add(new ProducesAttribute("application/json"));
});

// Adiciona o Swagger/OpenAPI (Opcional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🟢 CORREÇÃO 2: Adiciona o serviço de autorização (necessário para app.UseAuthorization)
builder.Services.AddAuthorization();


// === CONFIGURAÇÃO DO DBCONTEXT (POSTGRESQL) ===

// 1. A Connection String será lida do sistema de configuração (appsettings ou Variáveis do ACA).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // Se a string não for encontrada, lança exceção.
    throw new InvalidOperationException("A Connection String 'DefaultConnection' não foi encontrada. Verifique o appsettings.json ou as Variáveis de Ambiente/Segredos do Azure (usando a chave ConnectionStrings__DefaultConnection).");
}

builder.Services.AddHealthChecks();

// 2. Injeção do DbContext
builder.Services.AddDbContext<BancoDeItensContext>(options =>
{
    options.UseNpgsql(connectionString,
        npgsqlOptionsAction: sqlOptions =>
        {
            // REMOVIDA: A linha sqlOptions.MinBatchSize(1) (Específica do CockroachDB)

            // Ativa a retentativa padrão (Execution Strategy) para o PostgreSQL
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null // Usa o conjunto padrão de erros transientes do Postgree
            );
        })
        .LogTo(Console.WriteLine, LogLevel.Information);
});

// === FIM DA CONFIGURAÇÃO DE DBCONTEXT ===




// === CONFIGURAÇÃO DO DBCONTEXT (AZURE SQL SERVER) ===
/*
// LER A CONNECTION STRING DIRETAMENTE DA CONFIGURAÇÃO 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // Este erro causará o crash-loop se o segredo do ACA não for configurado corretamente.
    throw new InvalidOperationException("A Connection String 'BancoDeItensDB' não foi encontrada. Verifique as Variáveis de Ambiente/Segredos do Azure.");
}

// 2. Injeção do DbContext
/*
builder.Services.AddDbContext<BancoDeItensContext>(options =>
{
    // 🟢 MUDANÇA CRÍTICA: Trocando para UseSqlServer
    options.UseSqlServer(connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            // Ativa a retentativa padrão do EF Core (Resiliência de Rede)
            // 🛑 CORREÇÃO DA SINTAXE: Removendo o parâmetro errorCodesToAdd problemático
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10
               // maxRetryDelay: TimeSpan.FromSeconds(30)
            );
        })
        .LogTo(Console.WriteLine, LogLevel.Information);
});
*/


// === CONFIGURAÇÃO DO CORS (MANTENHA ATIVO PARA A URL DIRETA) ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy.WithOrigins("http://localhost:4200",
                                     "https://polite-dune-053c7490f.3.azurestaticapps.net")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});


var app = builder.Build();

// *** IMPORTANTE: BLOCO DE MIGRAÇÃO COMENTADO/REMOVIDO ***

// === CONFIGURAÇÃO DO PIPELINE DE REQUISIÇÃO HTTP ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseAuthorization();

// 🟢 CORREÇÃO 3: mapControllers precisa ser chamado após UseAuthorization.
app.MapControllers();

app.Run();
