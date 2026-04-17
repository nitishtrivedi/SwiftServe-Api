# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SwiftServe-API/SwiftServe-API.csproj", "SwiftServe-API/"]
RUN dotnet restore "SwiftServe-API/SwiftServe-API.csproj"

COPY . .
WORKDIR "/src/SwiftServe-API"
RUN dotnet build "SwiftServe-API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "SwiftServe-API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SwiftServe-API.dll"]