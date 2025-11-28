# Estágio 1: Build da Aplicação ASP.NET Core (BUILD)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 1. Copia o .sln
COPY BancoDeItens_V2.sln .
# NOTA: Ajuste o nome do arquivo se for BancoDeItens.sln

# 2. Copia a pasta 'src' inteira e o NuGet.config
COPY src/ src/
COPY NuGet.config .

# 3. Restaura explicitamente a Solução
RUN dotnet restore BancoDeItens_V2.sln

# 4. Publica explicitamente com o Runtime
# Adicionamos o RuntimeIdentifier para garantir que o buildpack não falhe
RUN dotnet publish "src/BancoItens.Api/BancoItens.Api.csproj" -c Release -o /publish /p:UseAppHost=false /p:RuntimeIdentifier=linux-x64

# Estágio 2: Imagem de Produção Final (RUNTIME)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Copia os arquivos publicados
COPY --from=build /publish .

# Comando final para rodar a aplicação
CMD ["dotnet", "BancoItens.Api.dll", "--urls", "http://0.0.0.0:8080"]