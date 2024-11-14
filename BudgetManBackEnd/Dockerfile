# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BudgetManBackEnd.API/BudgetManBackEnd.API.csproj", "BudgetManBackEnd.API/"]
COPY ["BudgetManBackEnd.Common/BudgetManBackEnd.Common.csproj", "BudgetManBackEnd.Common/"]
COPY ["BudgetManBackEnd.Model/BudgetManBackEnd.Model.csproj", "BudgetManBackEnd.Model/"]
COPY ["MayNghien.Common/MayNghien.Common.csproj", "MayNghien.Common/"]
COPY ["BudgetManBackEnd.Service/BudgetManBackEnd.Service.csproj", "BudgetManBackEnd.Service/"]
COPY ["BudgetManBackEnd.DAL/BudgetManBackEnd.DAL.csproj", "BudgetManBackEnd.DAL/"]
RUN dotnet restore "./BudgetManBackEnd.API/BudgetManBackEnd.API.csproj"
COPY . .
WORKDIR "/src/BudgetManBackEnd.API"
RUN dotnet build "./BudgetManBackEnd.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BudgetManBackEnd.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BudgetManBackEnd.API.dll"]