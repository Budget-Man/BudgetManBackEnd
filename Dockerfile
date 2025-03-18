FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything first to ensure all projects are available
COPY . ./

# Restore dependencies
RUN dotnet restore BudgetManBackEnd/BudgetManBackEnd/BudgetManBackEnd.sln

# Build and publish
RUN dotnet publish BudgetManBackEnd/BudgetManBackEnd/BudgetManBackEnd.API/BudgetManBackEnd.API.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Configure the port and environment
EXPOSE 80
EXPOSE 443
 
# Set the entry point
ENTRYPOINT ["dotnet", "BudgetManBackEnd.API.dll"]