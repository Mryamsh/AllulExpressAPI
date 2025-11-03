# -------------------------------
# 1️⃣  Build stage
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore

# Build and publish to /app/out
RUN dotnet publish -c Release -o /app/out

# -------------------------------
# 2️⃣  Runtime stage
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/out .

# Tell ASP.NET Core to listen on Railway’s dynamic port
ENV ASPNETCORE_URLS=http://+:$PORT

# Run your API
ENTRYPOINT ["dotnet", "AllulExpressAPI.dll"]
