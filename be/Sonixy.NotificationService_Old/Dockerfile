# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]
COPY ["Sonixy.NotificationService/Sonixy.NotificationService.csproj", "Sonixy.NotificationService/"]

RUN dotnet restore "Sonixy.NotificationService/Sonixy.NotificationService.csproj"

COPY . .

WORKDIR "/src/Sonixy.NotificationService"
RUN dotnet build "Sonixy.NotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sonixy.NotificationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8094

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.NotificationService.dll"]
