FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ServiceBusListener/ServiceBusListener.csproj ServiceBusListener/
COPY AzureServiceBusManager/AzureServiceBusManager.csproj AzureServiceBusManager/
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
