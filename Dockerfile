# Estágio 1: Build da Aplicação ASP.NET Core (BUILD)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src 
# O Railway define a raiz do repo como o contexto.
# O WORKDIR /src AGORA É DENTRO DO CONTÊINER (A RAIZ DO BUILD).

# Copia o .sln da raiz do contexto para o /src do contêiner
COPY BancoDeItens.sln . 

# Copia todos os arquivos de projeto. Como o WORKDIR é /src, os paths funcionam
# Exemplo: Copia src/BancoItens.Api/BancoItens.Api.csproj
COPY src/BancoItens.Api/*.csproj src/BancoItens.Api/
COPY src/BancoItens.Application/*.csproj src/BancoItens.Application/
COPY src/BancoItens.Domain/*.csproj src/BancoItens.Domain/
COPY src/BancoItens.Infrastructure/*.csproj src/BancoItens.Infrastructure/

# Restaura as dependências (agora usando a solução)
RUN dotnet restore

# Copia o restante do código-fonte (para /src)
COPY . .

# Publica a Solução focando no projeto de API
RUN dotnet publish "src/BancoItens.Api/BancoItens.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


#------------------------------------------------------------------
# Estágio 2: Imagem de Produção Final (RUNTIME)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Copia os arquivos publicados
COPY --from=build /app/publish .

# Comando final para rodar a aplicação
CMD ["dotnet", "BancoItens.Api.dll", "--urls", "http://0.0.0.0:8080"]