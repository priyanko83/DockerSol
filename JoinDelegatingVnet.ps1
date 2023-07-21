#Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

#az login
az account set --subscription a0671ab8-7fe9-4f47-aed6-faf804035ba9

$resourceGroup = "VNET-TEST"
$vnetName = "My-VNET"
$subnetName = "WEBAPP-SNET"
$frontendApiAppName = "public-facing-web"
$middleApiAppName = "private-facing-app"

# Below code joins a particular App Service to a delegating subnet ($subnetName  parameter)
# A  secured resource (e.g. a function app)  can be configured to accept incoming calls to only from above delegating subnet
# This below code enables seamless access to the secured resource to the App Service
function Join-Vnet ($resourceGroup, $webAppName, $vnetName, $subnetName)
{
    $subscriptionId = az account show --query id -o tsv
    $location = az group show -n $resourceGroup --query location -o tsv
    $subnetId = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.Network/virtualNetworks/$vnetName/subnets/$subnetName"

    $resourceId = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.Web/sites/$webAppName/config/virtualNetwork"
    $url = "https://management.azure.com$resourceId" + "?api-version=2018-02-01"

    $payload = @{ id=$resourceId; location=$location;  properties=@{subnetResourceId=$subnetId; swiftSupported="true"} } | ConvertTo-Json
    $accessToken = az account get-access-token --query accessToken -o tsv
    $response = Invoke-RestMethod -Method Put -Uri $url -Headers @{ Authorization="Bearer $accessToken"; "Content-Type"="application/json" } -Body $payload

	$response
}

#Join-Vnet  $resourceGroup $frontendAppName $vnetName $subnetName
Join-Vnet  $resourceGroup $middleApiAppName $vnetName $subnetName