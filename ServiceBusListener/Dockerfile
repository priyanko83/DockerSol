FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ServiceBusListener/ServiceBusListener.csproj ServiceBusListener/
COPY MongoDBUtilities/MongoDBUtilities.csproj MongoDBUtilities/
COPY Claims.Core/Claims.Core.csproj Claims.Core/
COPY CQRSFramework/CQRSFramework.csproj CQRSFramework/
COPY Broker.Core/Brokers.Core.csproj Broker.Core/
COPY AzureServiceBusManager/AzureServiceBusManager.csproj AzureServiceBusManager/
COPY AzureADAuthenticationUtilities/AzureADAuthenticationUtilities.csproj AzureADAuthenticationUtilities/
RUN dotnet restore ServiceBusListener/ServiceBusListener.csproj
COPY . .
WORKDIR /src/ServiceBusListener
RUN dotnet build ServiceBusListener.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ServiceBusListener.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ServiceBusListener.dll"]
