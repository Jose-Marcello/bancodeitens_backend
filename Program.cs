using BancoDeItensWebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Builder;
using BancoDeItensWebApi.Interfaces;
using BancoDeItensWebApi.Repositories;
using BancoDeItensWebApi.Services;
using BancoDeItensWebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// === CONFIGURAÇÃO DE SERVIÇOS INICIAIS ===

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
    options.Filters.Add(new ProducesAttribute("application/json"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

// 🟢 REGISTRO DA INJEÇÃO DE DEPENDÊNCIA (Application Layer e Infrastructure Layer)

// 1. Repositório: Liga IQuestaoRepository à QuestaoRepository (Persistence/Infrastructure)
// O Repositório é responsável pela camada de acesso a dados.
builder.Services.AddScoped<IQuestaoRepository, QuestaoRepository>();

// 2. Serviço: Liga IQuestaoService à QuestaoService (Business/Application)
// O Serviço é responsável pela lógica de negócio e coordena o Repositório.
builder.Services.AddScoped<IQuestaoService, QuestaoService>();


// === CONFIGURAÇÃO DO DBCONTEXT (POSTGRESQL) ===

// 1. Prioriza a leitura da variável de ambiente comum do Railway/Cloud
var railwayConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
var connectionString = "";

if (!string.IsNullOrEmpty(railwayConnectionString))
{
    // Se DATABASE_URL for encontrada, ela será a principal string.
    connectionString = railwayConnectionString;
    Console.WriteLine("Usando DATABASE_URL do ambiente (Railway).");
}
else
{
    // Caso contrário, tenta ler a DefaultConnection (que será sobrescrita pela convenção do .NET ou pelo appsettings)
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"Usando ConnectionStrings:DefaultConnection. Lida: {connectionString?.Substring(0, connectionString.IndexOf(';') + 1)}...");
}


if (string.IsNullOrEmpty(connectionString))
{
    // Se a string ainda for nula/vazia, lança exceção.
    throw new InvalidOperationException("A Connection String 'DefaultConnection' ou 'DATABASE_URL' não foi encontrada. O servidor não pode iniciar.");
}

// 2. Injeção do DbContext
builder.Services.AddDbContext<BancoDeItensContext>(options =>
{
    // CRÍTICO: Usa a string de conexão determinada acima.
    options.UseNpgsql(connectionString,
        npgsqlOptionsAction: sqlOptions =>
        {
            // Ativa a retentativa padrão (Execution Strategy) para o PostgreSQL
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null
            );
        })
        .LogTo(Console.WriteLine, LogLevel.Information);
});

// === FIM DA CONFIGURAÇÃO DE DBCONTEXT ===

// === BLOCO DE CORS (CONFIGURAÇÃO SEGURA - Permite apenas as URLs do Frontend) ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy.WithOrigins(
            "http://localhost:4200", // Para desenvolvimento local
                                     // URL de produção do frontend no Railway (com HTTPS)
            "https://app-bancodeitens-angular-front-production.up.railway.app")
            .AllowAnyMethod()
            .AllowAnyHeader()
        );
});


var app = builder.Build();

app.ApplyMigrations();


// === CONFIGURAÇÃO DO PIPELINE DE REQUISIÇÃO HTTP ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();