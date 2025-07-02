# Use official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy csproj files and restore as distinct layers
COPY TruckLink.API/*.csproj TruckLink.API/
COPY TruckLink.Core/*.csproj TruckLink.Core/
COPY TruckLink.Infrastructure/*.csproj TruckLink.Infrastructure/
COPY TruckLink.Logic/*.csproj TruckLink.Logic/
RUN dotnet restore TruckLink.API/TruckLink.API.csproj

# Copy everything else and build publish
COPY . .
RUN dotnet publish TruckLink.API/TruckLink.API.csproj -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Bind to port specified by Render (default fallback 5000)
ENV ASPNETCORE_URLS=http://+:5000
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 5000

ENTRYPOINT ["dotnet", "TruckLink.API.dll"]
