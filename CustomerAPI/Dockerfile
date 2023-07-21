FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["CustomerAPI/CustomerAPI.csproj", "CustomerAPI/"]
COPY ["CQRSFramework/CQRSFramework.csproj", "CQRSFramework/"]
COPY ["AzureServiceBusManager/AzureServiceBusManager.csproj", "AzureServiceBusManager/"]
COPY ["Broker.Core/Brokers.Core.csproj", "Broker.Core/"]
COPY ["AzureADAuthenticationUtilities/AzureADAuthenticationUtilities.csproj", "AzureADAuthenticationUtilities/"]
COPY ["MongoDBUtilities/MongoDBUtilities.csproj", "MongoDBUtilities/"]
COPY ["Claims.Core/Claims.Core.csproj", "Claims.Core/"]

RUN dotnet restore "CustomerAPI/CustomerAPI.csproj"
COPY . .
WORKDIR "/src/CustomerAPI"
RUN dotnet build "CustomerAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CustomerAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CustomerAPI.dll"]