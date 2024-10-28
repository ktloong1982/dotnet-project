# Use the official .NET 7 SDK image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy everything and restore as distinct layers
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /publish

# Use a smaller runtime image to reduce the final image size
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /publish .

# Expose port 80
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "MyApi.dll"]
