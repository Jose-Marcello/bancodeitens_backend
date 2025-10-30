// Localiza��o: Program.cs
//using BancoDeItensWebApi.Data;
//using Microsoft.EntityFrameworkCore;
// Npgsql; // Necess�rio para a convers�o da URL do Heroku
using System;

var builder = WebApplication.CreateBuilder(args);

// Adiciona servi�os ao cont�iner.
builder.Services.AddControllers();

/*NOVA CONFIGURA��O*/
// === CONFIGURA��O CR�TICA KESTREL (Adicionar este bloco!) ===
/*
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port) && int.TryParse(port, out int herokuPort))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // For�a o Kestrel a escutar qualquer IP na porta Heroku injetada.
        options.ListenAnyIP(herokuPort);
    });
}
*/

// === CONFIGURA��O CORRIGIDA DO DBCONTEXT COM POSTGRESQL (SUPORTE HEROKU) ===

// 1. Ler a Connection String no formato URL (DATABASE_URL no appsettings.json)
/*
var databaseUrl = builder.Configuration.GetValue<string>("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // 2. Converter a URL do Heroku (postgres://user:pass@host:port/db) para a string padr�o do Npgsql (chave=valor)
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    // Npgsql exige uma Connection String no formato chave=valor.
    connectionString = $"Host={uri.Host};Port={uri.Port};Username={userInfo[0]};Password={userInfo[1]};Database={uri.LocalPath.Substring(1)};Pooling=true;SSL Mode=Prefer;Trust Server Certificate=true";
}
else
{
    // 3. Fallback: Se DATABASE_URL n�o existir (ambiente local sem Heroku), tenta ler a DefaultConnection (Se fosse um projeto mais complexo)
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DATABASE_URL' or 'DefaultConnection' not found.");
}

// 4. Inje��o do DbContext
builder.Services.AddDbContext<BancoDeItensContext>(options =>
    options.UseNpgsql(connectionString));
/*

// === FIM DA CONFIGURA��O DE DBCONTEXT ===*/


// Configura��o do CORS: Permitir que o Front-end Angular (localhost:4200) acesse esta API
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy.WithOrigins("http://localhost:4200") // URL do Angular
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Adiciona o Swagger/OpenAPI (Opcional, mas �til para testes)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline de requisi��o HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Usa a pol�tica de CORS configurada
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
