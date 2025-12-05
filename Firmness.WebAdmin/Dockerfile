# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official ASP.NET Core SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Firmness.WebAdmin/Firmness.WebAdmin.csproj", "Firmness.WebAdmin/"]
COPY ["Firmness.Application/Firmness.Application.csproj", "Firmness.Application/"]
COPY ["Firmness.Domain/Firmness.Domain.csproj", "Firmness.Domain/"]
COPY ["Firmness.Infrastructure/Firmness.Infrastructure.csproj", "Firmness.Infrastructure/"]
RUN dotnet restore "Firmness.WebAdmin/Firmness.WebAdmin.csproj"
COPY . .
WORKDIR "/src/Firmness.WebAdmin"
RUN dotnet build "Firmness.WebAdmin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Firmness.WebAdmin.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmness.WebAdmin.dll"]
