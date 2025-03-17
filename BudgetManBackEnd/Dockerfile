FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy solution and project files first
COPY BudgetManBackEnd.sln ./
COPY */*.csproj ./
COPY */*/BudgetManBackEnd.API.csproj ./BudgetManBackEnd.API/
COPY */*/BudgetManBackEnd.DAL.csproj ./BudgetManBackEnd.DAL/
COPY */*/BudgetManBackEnd.Service.csproj ./BudgetManBackEnd.Service/
COPY */*/BudgetManBackEnd.Common.csproj ./BudgetManBackEnd.Common/
COPY */*/BudgetManBackEnd.Model.csproj ./BudgetManBackEnd.Model/
COPY */*/Maynghien.Common.csproj ./MayNghien.Common/
COPY */*/BudgetManBackEnd.BotFramework.csproj ./MessageCronJob/

# Restore as distinct layers
RUN dotnet restore BudgetManBackEnd.sln

# Copy everything else
COPY . ./

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
