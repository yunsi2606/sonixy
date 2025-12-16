# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Sonixy.Gateway/Sonixy.Gateway.csproj", "Sonixy.Gateway/"]

RUN dotnet restore "Sonixy.Gateway/Sonixy.Gateway.csproj"

COPY . .

WORKDIR "/src/Sonixy.Gateway"
RUN dotnet build "Sonixy.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sonixy.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5050

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.Gateway.dll"]
