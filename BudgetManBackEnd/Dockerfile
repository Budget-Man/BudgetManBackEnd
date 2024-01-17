FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
# Set the working directory in the container
WORKDIR /app


# Copy the remaining files
COPY . ./

# Restore NuGet packages for all projects

RUN dotnet clean
RUN dotnet restore
RUN dotnet build
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port the app will run on
EXPOSE 5000
ENV ASPNETCORE_URLS http://*:5000
# Set the entry point for the application
ENTRYPOINT ["dotnet", "BudgetManBackEnd.API.dll"]
