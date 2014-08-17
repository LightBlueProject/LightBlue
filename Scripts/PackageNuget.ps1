$nugetRelativePath = "src\.nuget\nuget.exe"
$parentDirectory = split-path $PSScriptRoot -parent

$coreNuspecPath = join-path $parentDirectory "LightBlue.nuspec"
$hostsNuspecPath = join-path $parentDirectory "LightBlue.Hosts.nuspec"

$thirdPartyLicencePath = join-path $parentDirectory "THIRDPARTY"

$lightBlueDllPath = join-path $parentDirectory "LightBlue\bin\Release\LightBlue.dll"
$workerHostExePath = join-path $parentDirectory "LightBlue.Host\bin\Release\LightBlue.Host.exe"
$workerHostStubDllPath = join-path $parentDirectory "LightBlue.Host\bin\Release\LightBlue.Host.Stub.dll"
$ndeskDllPath = join-path $parentDirectory "LightBlue.Host\bin\Release\NDesk.Options.dll"
$webHostExePath = join-path $parentDirectory "LightBlue.WebHost\bin\Release\LightBlue.WebHost.exe"

$packagePath = join-path $parentDirectory -childpath "createpackage"

$corePackagePath = join-path $packagePath -childpath "core"
$net45LibPath = join-path $corePackagePath -childpath "lib\net45"

$hostsPackagePath = join-path $packagePath -childpath "host"

$toolsPath = join-path $hostsPackagePath "tools"

if (Test-Path -Path $packagePath){
    Remove-Item -Recurse -Force $packagePath
}

New-Item -ItemType directory -Path $packagePath | Out-Null

New-Item -ItemType directory -Path $net45LibPath | Out-Null
Copy-Item $coreNuspecPath $corePackagePath
Copy-Item  $lightBlueDllPath $net45LibPath

New-Item -ItemType directory -Path $toolsPath | Out-Null
Copy-Item $hostsNuspecPath $hostsPackagePath
Copy-Item $workerHostExePath $toolsPath
Copy-Item $workerHostStubDllPath $toolsPath
Copy-Item $ndeskDllPath $toolsPath
Copy-Item $webHostExePath $toolsPath
Copy-Item $thirdPartyLicencePath $toolsPath

Push-Location -Path $corePackagePath

& '..\..\.nuget\nuget.exe' Pack "LightBlue.nuspec"

Pop-Location

Push-Location -Path $hostsPackagePath

& '..\..\.nuget\nuget.exe' Pack "LightBlue.Hosts.nuspec"

Pop-Location