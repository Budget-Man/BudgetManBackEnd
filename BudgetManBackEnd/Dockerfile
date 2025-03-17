FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy solution file
COPY BudgetManBackEnd.sln ./

# Create project directories
RUN mkdir -p BudgetManBackEnd.API \
    BudgetManBackEnd.DAL \
    BudgetManBackEnd.Service \
    BudgetManBackEnd.Common \
    BudgetManBackEnd.Model \
    MayNghien.Common \
    MessageCronJob \
    BudgetManBackEnd.TransferCronjob

# Copy project files
COPY BudgetManBackEnd.API/BudgetManBackEnd.API.csproj ./BudgetManBackEnd.API/
COPY BudgetManBackEnd.DAL/BudgetManBackEnd.DAL.csproj ./BudgetManBackEnd.DAL/
COPY BudgetManBackEnd.Service/BudgetManBackEnd.Service.csproj ./BudgetManBackEnd.Service/
COPY BudgetManBackEnd.Common/BudgetManBackEnd.Common.csproj ./BudgetManBackEnd.Common/
COPY BudgetManBackEnd.Model/BudgetManBackEnd.Model.csproj ./BudgetManBackEnd.Model/
COPY Maynghien.Common/Maynghien.Common.csproj ./Maynghien.Common/
COPY MessageCronJob/BudgetManBackEnd.BotFramework.csproj ./MessageCronJob/
COPY BudgetManBackEnd.TransferCronjob/BudgetManBackEnd.TransferCronjob.csproj ./BudgetManBackEnd.TransferCronjob/

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
