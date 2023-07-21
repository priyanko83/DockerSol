FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["GatewayApi2/GatewayApi2.csproj", "GatewayApi2/"]
COPY ["AzureADAuthenticationUtilities/AzureADAuthenticationUtilities.csproj", "AzureADAuthenticationUtilities/"]

RUN dotnet restore GatewayApi2/GatewayApi2.csproj
COPY . .
WORKDIR /src/GatewayApi2
RUN dotnet build GatewayApi2.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish GatewayApi2.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GatewayApi2.dll"]
