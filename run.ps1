$env:DOTNET_ROOT = "$env:USERPROFILE\.dotnet"
$env:PATH = "$env:USERPROFILE\.dotnet;$env:PATH"

Set-Location "$PSScriptRoot\FlowchartExplainer"
Write-Host "Starting FlowchartExplainer (.NET 9 Blazor WASM)..." -ForegroundColor Cyan
dotnet run
