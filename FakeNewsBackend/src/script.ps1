$directorypath = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
[System.Reflection.Assembly]::LoadFrom("$directorypath\libs\TestTool.dll");
[System.Reflection.Assembly]::LoadFrom("$directorypath\libs\OtherTool.dll");
$obj = New-Object Runner
$obj.Setup()
$obj.Run()