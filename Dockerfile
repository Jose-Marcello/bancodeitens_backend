#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core (BUILD)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src/app

# Copia o arquivo de projeto e restaura dependências
COPY *.csproj .
RUN dotnet restore

# Copia o restante dos arquivos (incluindo Controllers, Models, etc.)
COPY . .

# Publica a aplicação
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

#------------------------------------------------------------------
# Estágio 2: Imagem de Produção Final (RUNTIME)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080 # Porta definida no ACA Ingress

# Copia os arquivos publicados
COPY --from=build /app/publish .

# Comando final para rodar a aplicação
CMD ["dotnet", "BancoDeItensWebApi.dll", "--urls", "http://0.0.0.0:8080"]