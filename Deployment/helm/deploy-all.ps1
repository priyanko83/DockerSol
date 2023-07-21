$registry='https://index.docker.io/v1';
$dockerUser='priyanko83';
$dockerPassword='Ab289405';
$chart='gatewayapi'

#Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
helm delete $chart
helm install --values inf.yaml --set app.name=$appName --set inf.registry.server=$registry --set inf.registry.login=$dockerUser --set inf.registry.pwd=$dockerPassword --set inf.registry.secretName=priyankodockerhubregistrykey  $chart $chart
#helm install --dry-run --debug --values inf.yaml --set app.name=$appName --set inf.registry.server=$registry --set inf.registry.login=$dockerUser --set inf.registry.pwd=$dockerPassword --set inf.registry.secretName=priyankodockerhubregistrykey ./gatewayapi
