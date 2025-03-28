# Base runtime do .NET
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# SDK para compilar o projeto
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia e restaura dependências
COPY ["PersonalFinances.Server/PersonalFinances.Server.csproj", "PersonalFinances.Server/"]
COPY ["PersonalFinances.BLL/PersonalFinances.BLL.csproj", "PersonalFinances.BLL/"]
COPY ["PersonalFinances.DAL/PersonalFinances.DAL.csproj", "PersonalFinances.DAL/"]
COPY ["personalfinances.client/personalfinances.client.esproj", "personalfinances.client/"]
RUN dotnet restore "PersonalFinances.Server/PersonalFinances.Server.csproj"

# Copia código e compila
COPY . .
WORKDIR "/src/PersonalFinances.Server"
RUN dotnet build "PersonalFinances.Server.csproj" -c Release -o /app/build

# Publica a aplicação (inclui o Angular dentro do wwwroot)
FROM build AS publish
RUN dotnet publish "PersonalFinances.Server.csproj" -c Release -o /app/publish

# Imagem final para rodar a aplicação
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PersonalFinances.Server.dll"]
