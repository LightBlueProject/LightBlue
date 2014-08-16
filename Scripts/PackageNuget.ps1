$nugetRelativePath = "src\.nuget\nuget.exe"
$parentDirectory = split-path $PSScriptRoot -parent

$nuspecPath = join-path $parentDirectory "LightBlue.nuspec"
$lightBlueDllPath = join-path $parentDirectory "LightBlue\bin\Release\LightBlue.dll"
$nugetPath = join-path $parentDirectory -childpath $nugetRelativePath
$packagePath = join-path $parentDirectory -childpath "createpackage"
$corePackagePath = join-path $packagePath -childpath "core"
$net45LibPath = join-path $corePackagePath -childpath "lib\net45"

if (Test-Path -Path $packagePath){
    Remove-Item -Recurse -Force $packagePath
}

New-Item -ItemType directory -Path $packagePath | Out-Null
New-Item -ItemType directory -Path $net45LibPath | Out-Null
Copy-Item $nuspecPath $corePackagePath
Copy-Item  $lightBlueDllPath $net45LibPath

Push-Location -Path $corePackagePath

& '..\..\.nuget\nuget.exe' Pack "LightBlue.nuspec"

Pop-Location