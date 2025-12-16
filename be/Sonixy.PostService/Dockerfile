# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]
COPY ["Sonixy.PostService/Domain/Sonixy.PostService.Domain.csproj", "Sonixy.PostService/Domain/"]
COPY ["Sonixy.PostService/Application/Sonixy.PostService.Application.csproj", "Sonixy.PostService/Application/"]
COPY ["Sonixy.PostService/Infrastructure/Sonixy.PostService.Infrastructure.csproj", "Sonixy.PostService/Infrastructure/"]
COPY ["Sonixy.PostService/Api/Sonixy.PostService.csproj", "Sonixy.PostService/Api/"]

RUN dotnet restore "Sonixy.PostService/Api/Sonixy.PostService.csproj"

COPY . .

WORKDIR "/src/Sonixy.PostService/Api"
RUN dotnet build "Sonixy.PostService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sonixy.PostService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5002

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.PostService.dll"]
