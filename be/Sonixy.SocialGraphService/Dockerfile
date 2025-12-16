# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]
COPY ["Sonixy.SocialGraphService/Domain/Sonixy.SocialGraphService.Domain.csproj", "Sonixy.SocialGraphService/Domain/"]
COPY ["Sonixy.SocialGraphService/Application/Sonixy.SocialGraphService.Application.csproj", "Sonixy.SocialGraphService/Application/"]
COPY ["Sonixy.SocialGraphService/Infrastructure/Sonixy.SocialGraphService.Infrastructure.csproj", "Sonixy.SocialGraphService/Infrastructure/"]
COPY ["Sonixy.SocialGraphService/Api/Sonixy.SocialGraphService.csproj", "Sonixy.SocialGraphService/Api/"]

RUN dotnet restore "Sonixy.SocialGraphService/Api/Sonixy.SocialGraphService.csproj"

COPY . .

WORKDIR "/src/Sonixy.SocialGraphService/Api"
RUN dotnet build "Sonixy.SocialGraphService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sonixy.SocialGraphService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5003

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.SocialGraphService.dll"]
