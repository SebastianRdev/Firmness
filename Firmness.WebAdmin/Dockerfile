# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar los archivos de solución
COPY *.sln ./
COPY Firmness.Infrastructure/Firmness.Infrastructure.csproj Firmness.Infrastructure/
COPY Firmness.WebAdmin/Firmness.WebAdmin.csproj Firmness.WebAdmin/
RUN dotnet restore

# Copiar el resto del código y compilar
COPY . .
RUN dotnet publish Firmness.WebAdmin -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Variables de entorno (se pueden reemplazar en producción)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=firmnessdb;Username=postgres;Password=postgres"

EXPOSE 8080
ENTRYPOINT ["dotnet", "Firmness.WebAdmin.dll"]
