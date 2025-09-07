# Multi-stage Dockerfile for Railway deployment of .NET 8 app
# Explicit because Nixpacks auto-detected .NET 6 previously

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore first (better layer caching)
COPY XooCreator.BA.csproj ./
RUN dotnet restore XooCreator.BA.csproj

# copy the remaining source
COPY . .
RUN dotnet publish XooCreator.BA.csproj -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
# Railway sets PORT variable; default to 8080 if absent
ENV PORT=8080
ENTRYPOINT ["dotnet", "XooCreator.BA.dll"]
