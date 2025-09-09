# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos csproj e restaura as dependências
COPY ["Curriculo-store.Server.csproj", "./"]
RUN dotnet restore "./Curriculo-store.Server.csproj"

# Copia o resto do código
COPY . .

# Builda e publica para uma pasta 'out'
RUN dotnet publish "Curriculo-store.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Atualiza os pacotes do sistema para corrigir vulnerabilidades conhecidas
RUN apt-get update && apt-get upgrade -y && apt-get clean && rm -rf /var/lib/apt/lists/*

# Copia os arquivos publicados do estágio de build
COPY --from=build /app/publish .

# Expõe a porta 80
EXPOSE 80

# Define o comando para rodar o app
ENTRYPOINT ["dotnet", "Curriculo-store.Server.dll"]