# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]
COPY ["Sonixy.ChatService/Domain/Sonixy.ChatService.Domain.csproj", "Sonixy.ChatService/Domain/"]
COPY ["Sonixy.ChatService/Application/Sonixy.ChatService.Application.csproj", "Sonixy.ChatService/Application/"]
COPY ["Sonixy.ChatService/Infrastructure/Sonixy.ChatService.Infrastructure.csproj", "Sonixy.ChatService/Infrastructure/"]
COPY ["Sonixy.ChatService/Api/Sonixy.ChatService.Api.csproj", "Sonixy.ChatService/Api/"]

RUN dotnet restore "Sonixy.ChatService/Api/Sonixy.ChatService.Api.csproj"

COPY . .

WORKDIR "/src/Sonixy.ChatService/Api"
RUN dotnet build "Sonixy.ChatService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sonixy.ChatService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8095

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.ChatService.Api.dll"]
