@echo off
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" "%~dp0..\LightBlue.sln" /target:Clean,Build /p:Configuration=Release;VisualStudioVersion=12.0 /maxcpucount
if %ERRORLEVEL% NEQ 0 (
   exit /b %errorlevel%
)
powershell.exe %~dp0RunTests.ps1
if %ERRORLEVEL% NEQ 0 (
   exit /b %errorlevel%
)
powershell.exe %~dp0PackageNuget.ps1