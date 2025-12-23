# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]
COPY ["Sonixy.FeedService/Sonixy.FeedService.csproj", "Sonixy.FeedService/"]

RUN dotnet restore "Sonixy.FeedService/Sonixy.FeedService.csproj"

COPY . .
WORKDIR "/src/Sonixy.FeedService"
RUN dotnet build "Sonixy.FeedService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sonixy.FeedService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8093

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.FeedService.dll"]
