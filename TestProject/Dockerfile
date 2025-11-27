# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj first to restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the code
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# ASP.NET Core listens on port 8080 for Koyeb
ENV ASPNETCORE_URLS=http://+:8080

# Copy published files from build stage
COPY --from=build /app/publish .

# Start the app
ENTRYPOINT ["dotnet", "TestProject.dll"]
