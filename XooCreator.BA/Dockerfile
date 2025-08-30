# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers for better caching
COPY XooCreator.BA.csproj ./
RUN dotnet restore "XooCreator.BA.csproj"

# Copy everything and build
COPY . .
RUN dotnet publish "XooCreator.BA.csproj" -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .

# Listen on the port provided by Railway (default 8080)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "XooCreator.BA.dll"]