FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./

# Restore as distinct layers
RUN dotnet restore BudgetManBackEnd/BudgetManBackEnd/BudgetManBackEnd.sln

# Build and publish a release
RUN dotnet publish BudgetManBackEnd/BudgetManBackEnd/BudgetManBackEnd.API/BudgetManBackEnd.API.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "BudgetManBackEnd.API.dll"]