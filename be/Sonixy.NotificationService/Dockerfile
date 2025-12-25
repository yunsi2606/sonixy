# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]
COPY ["Sonixy.NotificationService/Domain/Sonixy.NotificationService.Domain.csproj", "Sonixy.NotificationService/Domain/"]
COPY ["Sonixy.NotificationService/Application/Sonixy.NotificationService.Application.csproj", "Sonixy.NotificationService/Application/"]
COPY ["Sonixy.NotificationService/Infrastructure/Sonixy.NotificationService.Infrastructure.csproj", "Sonixy.NotificationService/Infrastructure/"]
COPY ["Sonixy.NotificationService/Api/Sonixy.NotificationService.Api.csproj", "Sonixy.NotificationService/Api/"]

RUN dotnet restore "Sonixy.NotificationService/Api/Sonixy.NotificationService.Api.csproj"

COPY . .

WORKDIR "/src/Sonixy.NotificationService/Api"
RUN dotnet build "Sonixy.NotificationService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sonixy.NotificationService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8094

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.NotificationService.Api.dll"]
