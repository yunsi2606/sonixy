# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy shared project first
COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]

# Copy IdentityService projects
COPY ["Sonixy.IdentityService/Domain/Sonixy.IdentityService.Domain.csproj", "Sonixy.IdentityService/Domain/"]
COPY ["Sonixy.IdentityService/Application/Sonixy.IdentityService.Application.csproj", "Sonixy.IdentityService/Application/"]
COPY ["Sonixy.IdentityService/Infrastructure/Sonixy.IdentityService.Infrastructure.csproj", "Sonixy.IdentityService/Infrastructure/"]
COPY ["Sonixy.IdentityService/Api/Sonixy.IdentityService.csproj", "Sonixy.IdentityService/Api/"]

# Restore dependencies
RUN dotnet restore "Sonixy.IdentityService/Api/Sonixy.IdentityService.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/Sonixy.IdentityService/Api"
RUN dotnet build "Sonixy.IdentityService.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Sonixy.IdentityService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5000
EXPOSE 5000

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.IdentityService.dll"]
