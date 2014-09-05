$nugetRelativePath = "src\.nuget\nuget.exe"
$parentDirectory = split-path $PSScriptRoot -parent

$coreNuspecPath = join-path $parentDirectory "LightBlue.nuspec"
$autofacNuspecPath = join-path $parentDirectory "LightBlue.Autofac.nuspec"
$hostsNuspecPath = join-path $parentDirectory "LightBlue.Hosts.nuspec"

$noticePath = join-path $parentDirectory "NOTICE"

$lightBlueDllPath = join-path $parentDirectory "LightBlue\bin\Release\LightBlue.dll"
$lightBlueAutofacDllPath = join-path $parentDirectory "LightBlue.Autofac\bin\Release\LightBlue.Autofac.dll"
$workerHostExePath = join-path $parentDirectory "LightBlue.Host\bin\Release\LightBlue.Host.exe"
$workerHostStubDllPath = join-path $parentDirectory "LightBlue.Host\bin\Release\LightBlue.Host.Stub.dll"
$ndeskDllPath = join-path $parentDirectory "LightBlue.Host\bin\Release\NDesk.Options.dll"
$webHostExePath = join-path $parentDirectory "LightBlue.WebHost\bin\Release\LightBlue.WebHost.exe"

$linqpadEnvironmentTemplatePath = join-path $parentDirectory "LINQPad\LightBlue Environment Template.linq"

$packagePath = join-path $parentDirectory -childpath "createpackage"
$outputDirectory = join-path $packagePath "output"

$corePackagePath = join-path $packagePath -childpath "core"
$coreNet45LibPath = join-path $corePackagePath -childpath "lib\net45"
$linqpadSamplesPath = join-path $corePackagePath -childpath "linqpad-samples\LightBlue"

$autofacPackagePath = join-path $packagePath -childpath "autofac"
$autofacNet45LibPath = join-path $autofacPackagePath -childpath "lib\net45"

$hostsPackagePath = join-path $packagePath -childpath "host"
$toolsPath = join-path $hostsPackagePath "tools"

if (Test-Path -Path $packagePath){
    Remove-Item -Recurse -Force $packagePath
}

New-Item -ItemType directory -Path $packagePath | Out-Null
New-Item -ItemType directory -Path $outputDirectory | Out-Null

New-Item -ItemType directory -Path $coreNet45LibPath | Out-Null
New-Item -ItemType directory -Path $linqpadSamplesPath | Out-Null
Copy-Item $coreNuspecPath $corePackagePath
Copy-Item  $lightBlueDllPath $coreNet45LibPath
Copy-Item $linqpadEnvironmentTemplatePath  $linqpadSamplesPath

New-Item -ItemType directory -Path $autofacNet45LibPath | Out-Null
Copy-Item $autofacNuspecPath $autofacPackagePath
Copy-Item  $lightBlueAutofacDllPath $autofacNet45LibPath

New-Item -ItemType directory -Path $toolsPath | Out-Null
Copy-Item $hostsNuspecPath $hostsPackagePath
Copy-Item $workerHostExePath $toolsPath
Copy-Item $workerHostStubDllPath $toolsPath
Copy-Item $ndeskDllPath $toolsPath
Copy-Item $webHostExePath $toolsPath
Copy-Item $noticePath $toolsPath

Push-Location -Path $corePackagePath

& '..\..\.nuget\nuget.exe' Pack "LightBlue.nuspec"
Copy-Item "*.nupkg" $outputDirectory

Pop-Location

Push-Location -Path $autofacPackagePath

& '..\..\.nuget\nuget.exe' Pack "LightBlue.Autofac.nuspec"
Copy-Item "*.nupkg" $outputDirectory

Pop-Location

Push-Location -Path $hostsPackagePath

& '..\..\.nuget\nuget.exe' Pack "LightBlue.Hosts.nuspec"
Copy-Item "*.nupkg" $outputDirectory

Pop-Location