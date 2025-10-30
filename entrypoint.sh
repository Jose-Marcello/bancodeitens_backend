#!/bin/bash

# --- 1. Rodar Migrações do Entity Framework Core ---
echo "Aplicando migrações do banco de dados..."
# Garante que o ef está no PATH e aplica migrações
/usr/local/bin/dotnet-ef database update --project BancoDeItensWebApi.csproj --startup-project BancoDeItensWebApi.csproj

# --- 2. Iniciar o Servidor Kestrel na porta correta do Heroku ---
echo "Iniciando Kestrel na porta $PORT..."

# O comando CRÍTICO: --urls http://0.0.0.0:$PORT força o Kestrel a escutar a porta injetada pelo Heroku
dotnet BancoDeItensWebApi.dll --urls http://0.0.0.0:$PORT 