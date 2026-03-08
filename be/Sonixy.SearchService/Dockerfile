FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8096

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Sonixy.SearchService/Api/Sonixy.SearchService.Api.csproj", "Sonixy.SearchService/Api/"]
COPY ["Sonixy.SearchService/Application/Sonixy.SearchService.Application.csproj", "Sonixy.SearchService/Application/"]
COPY ["Sonixy.SearchService/Infrastructure/Sonixy.SearchService.Infrastructure.csproj", "Sonixy.SearchService/Infrastructure/"]
COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]
RUN dotnet restore "Sonixy.SearchService/Api/Sonixy.SearchService.Api.csproj"
COPY . .
WORKDIR "/src/Sonixy.SearchService/Api"
RUN dotnet build "Sonixy.SearchService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sonixy.SearchService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.SearchService.Api.dll"]
