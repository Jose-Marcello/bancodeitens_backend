// Nome do arquivo: Program.cs
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation;
//using FluentValidation.DependencyInjection.Extensions;
using BancoDeItensWebApi.Data;
using BancoDeItensWebApi.Interfaces;
using BancoDeItensWebApi.Repositories;
using BancoDeItensWebApi.Services;
using BancoDeItensWebApi.Extensions;
using BancoDeItensWebApi.Profiles; // NECESSÁRIO para resolver o AutoMapperProfile
using AutoMapper; // NECESSÁRIO para usar o IConfigurationExpression

var builder = WebApplication.CreateBuilder(args);

// === CONFIGURAÇÃO DE SERVIÇOS INICIAIS ===

// 🛑 REGISTRO DO MVC E FLUENTVALIDATION
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
    options.Filters.Add(new ProducesAttribute("application/json"));
});

// 🟢 REGISTRO MANUAL DO FLUENTVALIDATION
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

// 🟢 REGISTRO DA INJEÇÃO DE DEPENDÊNCIA (Application Layer e Infrastructure Layer)

// 🛑 CORREÇÃO FINAL DO AUTOMAPPER: Usando a sintaxe de configuração explícita (Universal)
// A sintaxe de configuração explícita resolve o erro CS1503 no seu ambiente.
builder.Services.AddAutoMapper(cfg =>
{
    // Adiciona o perfil ao pipeline de configuração.
    cfg.AddProfile(new AutoMapperProfile());
}, Assembly.GetExecutingAssembly());


builder.Services.AddScoped<IQuestaoRepository, QuestaoRepository>();
builder.Services.AddScoped<IDisciplinaRepository, DisciplinaRepository>();
builder.Services.AddScoped<IQuestaoService, QuestaoService>();


// === CONFIGURAÇÃO DO DBCONTEXT (POSTGRESQL) ===

var railwayConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = "";

if (string.IsNullOrEmpty(railwayConnectionString))
{
    railwayConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

if (string.IsNullOrEmpty(railwayConnectionString))
{
    throw new InvalidOperationException("A Connection String 'DefaultConnection' ou 'DATABASE_URL' não foi encontrada.");
}

// 🛑 CORREÇÃO DA CONNECTION STRING: Conversão de URL (postgresql://...) para Chave/Valor
// Isso resolve o System.ArgumentException: Format of the initialization string...
if (railwayConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
{
    var match = Regex.Match(railwayConnectionString,
        @"postgresql://(?<user>[^:]+):(?<password>[^@]+)@(?<host>[^:]+):(?<port>\d+)/(?<database>.+)");

    if (match.Success)
    {
        connectionString = $"Host={match.Groups["host"].Value};" +
                           $"Port={match.Groups["port"].Value};" +
                           $"Username={match.Groups["user"].Value};" +
                           $"Password={match.Groups["password"].Value};" +
                           $"Database={match.Groups["database"].Value}";
    }
    else
    {
        throw new InvalidOperationException("A Connection String RAILWAY não está no formato URL esperado.");
    }
}
else
{
    connectionString = railwayConnectionString;
}
// FIM DA CORREÇÃO CRÍTICA


builder.Services.AddDbContext<BancoDeItensContext>(options =>
{
    options.UseNpgsql(connectionString,
        npgsqlOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null
            );
        })
        .LogTo(Console.WriteLine, LogLevel.Information);
});


var app = builder.Build();

// 🛑 BLOCo DE MIGRATIONS: Executa a aplicação da Migration
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

