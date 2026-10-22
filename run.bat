@echo off
title FlowchartExplainer - Blazor WebAssembly
set "DOTNET_ROOT=%USERPROFILE%\.dotnet"
set "PATH=%USERPROFILE%\.dotnet;%PATH%"

cd /d "%~dp0FlowchartExplainer"
echo Starting FlowchartExplainer (.NET 9 Blazor WASM)...
dotnet run
pause
