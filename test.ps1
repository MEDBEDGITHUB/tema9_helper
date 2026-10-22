$env:DOTNET_ROOT = "$env:USERPROFILE\.dotnet"
$env:PATH = "$env:USERPROFILE\.dotnet;$env:PATH"

Set-Location $PSScriptRoot
Write-Host "Running 5 mandatory verification tests..." -ForegroundColor Green
dotnet test FlowchartExplainer.sln
