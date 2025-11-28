# Estágio 1: Build da Aplicação ASP.NET Core (BUILD)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia e restaura a Solução inteira (incluindo todos os projetos e .sln)
# Copia o .sln
COPY BancoDeItens.sln . 

# Copia os arquivos de projeto (todos os .csproj)
COPY src/BancoItens.Api/*.csproj src/BancoItens.Api/
COPY src/BancoItens.Application/*.csproj src/BancoItens.Application/
COPY src/BancoItens.Domain/*.csproj src/BancoItens.Domain/
COPY src/BancoItens.Infrastructure/*.csproj src/BancoItens.Infrastructure/

# Restaura as dependências (agora usando a solução)
RUN dotnet restore

# Copia o restante dos arquivos (código-fonte)
COPY . .

# Publica a Solução focando no projeto de API
# Atenção: 'BancoItens.Api.csproj' é o novo nome do projeto!
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
# Atenção: O nome da DLL final é o nome do projeto (BancoItens.Api.dll)!
CMD ["dotnet", "BancoItens.Api.dll", "--urls", "http://0.0.0.0:8080"]