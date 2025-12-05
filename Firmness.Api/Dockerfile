# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official ASP.NET Core SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Firmness.Api/Firmness.Api.csproj", "Firmness.Api/"]
COPY ["Firmness.Application/Firmness.Application.csproj", "Firmness.Application/"]
COPY ["Firmness.Domain/Firmness.Domain.csproj", "Firmness.Domain/"]
COPY ["Firmness.Infrastructure/Firmness.Infrastructure.csproj", "Firmness.Infrastructure/"]
RUN dotnet restore "Firmness.Api/Firmness.Api.csproj"
COPY . .
WORKDIR "/src/Firmness.Api"
RUN dotnet build "Firmness.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Firmness.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmness.Api.dll"]
