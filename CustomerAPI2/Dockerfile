FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["CustomerAPI2/CustomerAPI2.csproj", "CustomerAPI2/"]
COPY ["CQRSFramework/CQRSFramework.csproj", "CQRSFramework/"]
COPY ["AzureServiceBusManager/AzureServiceBusManager.csproj", "AzureServiceBusManager/"]
COPY ["Broker.Core/Brokers.Core.csproj", "Broker.Core/"]
COPY ["AzureADAuthenticationUtilities/AzureADAuthenticationUtilities.csproj", "AzureADAuthenticationUtilities/"]
COPY ["MongoDBUtilities/MongoDBUtilities.csproj", "MongoDBUtilities/"]
COPY ["Claims.Core/Claims.Core.csproj", "Claims.Core/"]

RUN dotnet restore "CustomerAPI2/CustomerAPI2.csproj"
COPY . .
WORKDIR "/src/CustomerAPI2"
RUN dotnet build "CustomerAPI2.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CustomerAPI2.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CustomerAPI2.dll"]