FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./

# Ensure correct case for MayNghien.Common
RUN if [ -d "Maynghien.Common" ]; then mv Maynghien.Common MayNghien.Common; fi
RUN if [ -d "maynghien.common" ]; then mv maynghien.common MayNghien.Common; fi

# Restore as distinct layers
RUN dotnet restore BudgetManBackEnd.sln

# Build and publish
RUN dotnet publish BudgetManBackEnd.API/BudgetManBackEnd.API.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Configure the port and environment
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

# Set the entry point
ENTRYPOINT ["dotnet", "BudgetManBackEnd.API.dll"]
