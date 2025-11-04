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
// A chave buscada deve ser "DefaultConnection" (que é mapeada para ConnectionStrings__DefaultConnection no ACA)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // Se a string não for encontrada (ex: no ACA sem Segredo), esta exceção ocorre.
    throw new InvalidOperationException("A Connection String 'DefaultConnection' não foi encontrada. Verifique o appsettings.json ou os Segredos do Azure.");
}

// Injeção do DbContext
builder.Services.AddDbContext<BancoDeItensContext>(options =>
{
    // MUDANÇA CRÍTICA: Trocando para UseSqlServer
    options.UseSqlServer(connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            // Ativa a retentativa padrão do EF Core (Resiliência de Rede)
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10
            );
        })
        .LogTo(Console.WriteLine, LogLevel.Information);
});


// === CONFIGURAÇÃO DO CORS ===
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

// === AÇÃO FINAL: BLOCO DE APLICAÇÃO DE MIGRAÇÕES NA INICIALIZAÇÃO ===
// Isso garante que o banco de dados está pronto antes que o app tente consultá-lo.

/*
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BancoDeItensContext>();

        // Aplica migrações pendentes. Se o banco já estiver atualizado, esta linha não faz nada.
        // Se houver um problema com o DB, o erro 500 acontece AQUI, mas o ACA consegue logar.
        db.Database.Migrate();
    }
}
catch (Exception ex)
{
    // Se a migração falhar (por exemplo, por falta de permissão ou conexão), o log é gerado.
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Um erro ocorreu ao aplicar as migrações do banco de dados no startup do host.");
}
// =======================================================================
*/

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
