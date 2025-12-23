# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]
COPY ["Sonixy.AnalyticsService/Sonixy.AnalyticsService.csproj", "Sonixy.AnalyticsService/"]

RUN dotnet restore "Sonixy.AnalyticsService/Sonixy.AnalyticsService.csproj"

COPY . .
WORKDIR "/src/Sonixy.AnalyticsService"
RUN dotnet build "Sonixy.AnalyticsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sonixy.AnalyticsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8092

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.AnalyticsService.dll"]
