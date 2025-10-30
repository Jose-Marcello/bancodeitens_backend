#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core (BUILD)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia o arquivo .csproj e restaura dependências
COPY BancoDeItensWebApi.csproj .
RUN dotnet restore

# 2. Copia APENAS os arquivos de código-fonte necessários para a compilação.
# ISSO EVITA O ERRO DE TENTAR COMPILAR O Dockerfile, Procfile, etc.
COPY Controllers/ Controllers/
COPY Data/ Data/
COPY Models/ Models/
COPY Migrations/ Migrations/
COPY Program.cs .
COPY appsettings.json .
COPY appsettings.Development.json .
# Se tiver a classe WeatherForecast, adicione
COPY WeatherForecast.cs .

# 3. Publica a aplicação
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# 4. Instala o dotnet-ef no estágio com o SDK.
RUN dotnet tool install --global dotnet-ef --version 8.0.4

#------------------------------------------------------------------
# Estágio 2: Imagem de Produção Final (RUNTIME)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# [POSTGRESQL] Instala bibliotecas nativas
RUN apt-get update && apt-get install -y libpq-dev \
    && rm -rf /var/lib/apt/lists/*

# Copia a ferramenta dotnet-ef (instalada no estágio 'build')
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copia os arquivos publicados do backend
COPY --from=build /app/publish .


# OTIMIZAÇÃO PARA AZURE CONTAINER APPS
CMD ["dotnet", "BancoDeItensWebApi.dll", "--urls", "http://0.0.0.0:8080"]