// Localiza��o: Program.cs (C�DIGO FINAL PARA ACA)

using BancoDeItensWebApi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; // Necess�rio para CreateScope

var builder = WebApplication.CreateBuilder(args);

// Adiciona servi�os ao cont�iner.
builder.Services.AddControllers();

// === CONFIGURA��O DO DBCONTEXT (COCKROACHDB/POSTGRESQL) ===

// 1. A Connection String ser� lida da vari�vel de ambiente AZURE-DB-CONN
var connectionString = Environment.GetEnvironmentVariable("AZURE-DB-CONN");

if (string.IsNullOrEmpty(connectionString))
{
    // Fallback de desenvolvimento (se rodar localmente e n�o definir a vari�vel)
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("A Connection String n�o foi encontrada.");
    }
}

// 2. Inje��o do DbContext
builder.Services.AddDbContext<BancoDeItensContext>(options =>
{
    options.UseNpgsql(connectionString,
        npgsqlOptionsAction: sqlOptions =>
        {
            // CORRE��O CR�TICA DO COCKROACHDB: For�a a execu��o sem transa��es longas
            sqlOptions.MinBatchSize(1);

            // Ativa a retentativa padr�o do EF Core (Resolve a falha de conex�o tempor�ria)
            // A linha 41 no seu c�digo
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                // ADICIONE ESTA LINHA: O C# 8/9+ exige que voc� defina a lista.
                errorCodesToAdd: null
            );
        })
        .LogTo(Console.WriteLine, LogLevel.Information); // �til para logs de startup
});

// === FIM DA CONFIGURA��O DE DBCONTEXT ===


// Configura��o do CORS: Permitir que o Front-end Angular acesse esta API
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

// Removendo o bloco de migra��o autom�tica (causa o crash em containers)
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
        logger.LogError(ex, "Ocorreu um erro ao tentar aplicar as migra��es.");
    }
}
*/


// Configura o pipeline de requisi��o HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Comentado para ACA Proxy

// Usa a pol�tica de CORS configurada
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();