"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" "%~dp0..\LightBlue.sln" /target:Clean,Build /p:Configuration=Release /maxcpucount
powershell.exe %~dp0PackageNuget.ps1