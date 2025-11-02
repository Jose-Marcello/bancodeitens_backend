# Baseia-se na imagem SDK mais recente para compilação (BUILD)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de solução e restaura dependências
COPY ["BancoDeItensV2.sln", "."]
COPY ["BancoDeItensWebApi/BancoDeItensWebApi.csproj", "BancoDeItensWebApi/"]
RUN dotnet restore "BancoDeItensWebApi/BancoDeItensWebApi.csproj"

# Copia todos os arquivos do projeto e compila
COPY . .
WORKDIR "/src/BancoDeItensWebApi"
RUN dotnet build "BancoDeItensWebApi.csproj" -c Release -o /app/build

# Baseia-se na imagem de Runtime mais leve (FINAL)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/build .

# Configurações de produção
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Comando de entrada
ENTRYPOINT ["dotnet", "BancoDeItensWebApi.dll"]
