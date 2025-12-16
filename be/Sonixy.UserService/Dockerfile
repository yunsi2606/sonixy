# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]
COPY ["Sonixy.UserService/Domain/Sonixy.UserService.Domain.csproj", "Sonixy.UserService/Domain/"]
COPY ["Sonixy.UserService/Application/Sonixy.UserService.Application.csproj", "Sonixy.UserService/Application/"]
COPY ["Sonixy.UserService/Infrastructure/Sonixy.UserService.Infrastructure.csproj", "Sonixy.UserService/Infrastructure/"]
COPY ["Sonixy.UserService/Api/Sonixy.UserService.csproj", "Sonixy.UserService/Api/"]

RUN dotnet restore "Sonixy.UserService/Api/Sonixy.UserService.csproj"

COPY . .

WORKDIR "/src/Sonixy.UserService/Api"
RUN dotnet build "Sonixy.UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sonixy.UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5001

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.UserService.dll"]
