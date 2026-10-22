@echo off
set "DOTNET_ROOT=%USERPROFILE%\.dotnet"
set "PATH=%USERPROFILE%\.dotnet;%PATH%"

cd /d "%~dp0"
echo Running automated verification tests...
dotnet test FlowchartExplainer.sln
pause
