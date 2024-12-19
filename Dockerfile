# Use a base image with the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

WORKDIR /app

# Copy the csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Build the application
RUN dotnet publish -c Release -o /app/publish

# Use a smaller runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app
COPY --from=build-env /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "ucsUpdatedApp.dll"]