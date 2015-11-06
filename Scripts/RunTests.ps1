$parentDirectory = split-path $PSScriptRoot -parent
$packagesDirectory = join-path $parentDirectory "packages"
$xunitRunnerDirectory = Get-ChildItem -Path $packagesDirectory | Where-Object { $_.Name -match "xunit.runner.console" } | Select-Object -last 1 | % {$_.FullName }
$xunitRunner = join-path $xunitRunnerDirectory "tools\xunit.console.exe"
$testAssemblyPath = join-path $parentDirectory "LightBlue.Tests\bin\Release\LightBlue.Tests.dll"

& $xunitRunner $testAssemblyPath
exit $LastExitCode