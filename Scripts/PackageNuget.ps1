$nugetRelativePath = "src\.nuget\nuget.exe"
$parentDirectory = split-path $PSScriptRoot -parent

$coreNuspecPath = join-path $parentDirectory "LightBlue.nuspec"
$autofacNuspecPath = join-path $parentDirectory "LightBlue.Autofac.nuspec"
$hostsNuspecPath = join-path $parentDirectory "LightBlue.Hosts.nuspec"
$windsorNuspecPath = join-path $parentDirectory "LightBlue.Windsor.nuspec"

$noticePath = join-path $parentDirectory "NOTICE"

$lightBlueDllPath = join-path $parentDirectory "LightBlue\bin\Release\LightBlue.dll"
$lightBluePdbPath = join-path $parentDirectory "LightBlue\bin\Release\LightBlue.pdb"
$lightBlueAutofacDllPath = join-path $parentDirectory "LightBlue.Autofac\bin\Release\LightBlue.Autofac.dll"
$lightBlueAutofacPdbPath = join-path $parentDirectory "LightBlue.Autofac\bin\Release\LightBlue.Autofac.pdb"
$lightBlueWindsorDllPath = join-path $parentDirectory "LightBlue.Windsor\bin\Release\LightBlue.Windsor.dll"
$lightBlueWindsorPdbPath = join-path $parentDirectory "LightBlue.Windsor\bin\Release\LightBlue.Windsor.pdb"
$workerHostExePath = join-path $parentDirectory "LightBlue.Host\bin\Release\LightBlue.Host.exe"
$workerHostPdbPath = join-path $parentDirectory "LightBlue.Host\bin\Release\LightBlue.Host.pdb"
$workerHostStubDllPath = join-path $parentDirectory "LightBlue.Host\bin\Release\LightBlue.Host.Stub.dll"
$workerHostStubPdbPath = join-path $parentDirectory "LightBlue.Host\bin\Release\LightBlue.Host.Stub.pdb"
$ndeskDllPath = join-path $parentDirectory "LightBlue.Host\bin\Release\NDesk.Options.dll"
$webHostExePath = join-path $parentDirectory "LightBlue.WebHost\bin\Release\LightBlue.WebHost.exe"
$webHostPdbPath = join-path $parentDirectory "LightBlue.WebHost\bin\Release\LightBlue.WebHost.pdb"

$linqpadEnvironmentTemplatePath = join-path $parentDirectory "LINQPad\Environment Template.linq"
$linqpadIssueTemplatePath = join-path $parentDirectory "LINQPad\Issue Report Template.linq"

$packagePath = join-path $parentDirectory -childpath "createpackage"
$outputDirectory = join-path $packagePath "output"

$corePackagePath = join-path $packagePath -childpath "core"
$coreNet45LibPath = join-path $corePackagePath -childpath "lib\net45"
$linqpadSamplesPath = join-path $corePackagePath -childpath "linqpad-samples\LightBlue"

$autofacPackagePath = join-path $packagePath -childpath "autofac"
$autofacNet45LibPath = join-path $autofacPackagePath -childpath "lib\net45"

$windsorPackagePath = join-path $packagePath -childpath "windsor"
$windsorNet45LibPath = join-path $windsorPackagePath -childpath "lib\net45"

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
Copy-Item  $lightBluePdbPath $coreNet45LibPath
Copy-Item $linqpadEnvironmentTemplatePath $linqpadSamplesPath
Copy-Item $linqpadIssueTemplatePath $linqpadSamplesPath

New-Item -ItemType directory -Path $autofacNet45LibPath | Out-Null
Copy-Item $autofacNuspecPath $autofacPackagePath
Copy-Item  $lightBlueAutofacDllPath $autofacNet45LibPath
Copy-Item  $lightBlueAutofacPdbPath $autofacNet45LibPath

New-Item -ItemType directory -Path $windsorNet45LibPath | Out-Null
Copy-Item $windsorNuspecPath $windsorPackagePath
Copy-Item  $lightBlueWindsorDllPath $windsorNet45LibPath
Copy-Item  $lightBlueWindsorPdbPath $windsorNet45LibPath

New-Item -ItemType directory -Path $toolsPath | Out-Null
Copy-Item $hostsNuspecPath $hostsPackagePath
Copy-Item $workerHostExePath $toolsPath
Copy-Item $workerHostPdbPath $toolsPath
Copy-Item $workerHostStubDllPath $toolsPath
Copy-Item $workerHostStubPdbPath $toolsPath
Copy-Item $ndeskDllPath $toolsPath
Copy-Item $webHostExePath $toolsPath
Copy-Item $webHostPdbPath $toolsPath
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

Push-Location -Path $windsorPackagePath

& '..\..\.nuget\nuget.exe' Pack "LightBlue.Windsor.nuspec"
Copy-Item "*.nupkg" $outputDirectory

Pop-Location
