// Localização: Program.cs (CÓDIGO FINAL PARA AZURE SQL DB)

using BancoDeItensWebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// === CONFIGURAÇÃO DE SERVIÇOS INICIAIS ===

// Adiciona o serviço de controladores (MVC/API)
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
    options.Filters.Add(new ProducesAttribute("application/json"));
});

// Adiciona o Swagger/OpenAPI (Opcional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adiciona o serviço de autorização (necessário para app.UseAuthorization)
builder.Services.AddAuthorization();


// === CONFIGURAÇÃO DO DBCONTEXT (AZURE SQL SERVER) ===

// LER A CONNECTION STRING DIRETAMENTE DA CONFIGURAÇÃO 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // 🛑 CORREÇÃO 1: A chave buscada deve ser "DefaultConnection" (anteriormente estava "BancoDeItensDB")
    // Se a string não for encontrada (ex: no ACA sem Segredo), esta exceção ocorre.
    throw new InvalidOperationException("A Connection String 'DefaultConnection' não foi encontrada. Verifique o appsettings.json ou os Segredos do Azure.");
}

// 2. Injeção do DbContext
// 🟢 AQUI ESTÁ DESCOMENTADO para que o Add-Migration funcione.
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


// === CONFIGURAÇÃO DO CORS (MANTENHA ATIVO PARA A URL DIRETA) ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy.WithOrigins("http://localhost:4200",
                                     "https://icy-stone-049318c1e.3.azurestaticapps.net")
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

// mapControllers precisa ser chamado após UseAuthorization.
app.MapControllers();

app.Run();
