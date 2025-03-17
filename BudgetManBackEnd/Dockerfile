FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./

# Restore as distinct layers
RUN dotnet restore BudgetManBackEnd/BudgetManBackEnd/BudgetManBackEnd.sln

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Configure the port and environment
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

# Set the entry point
ENTRYPOINT ["dotnet", "BudgetManBackEnd.API.dll"]
