FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Sonixy.EmailService/Sonixy.EmailService.csproj", "Sonixy.EmailService/"]
COPY ["Sonixy.Shared/Sonixy.Shared.csproj", "Sonixy.Shared/"]
RUN dotnet restore "Sonixy.EmailService/Sonixy.EmailService.csproj"

COPY . .
WORKDIR "/src/Sonixy.EmailService"
RUN dotnet publish "Sonixy.EmailService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sonixy.EmailService.dll"]
