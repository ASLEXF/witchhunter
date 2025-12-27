set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.
set GameData=%WORKSPACE%\Assets\GameData

dotnet %LUBAN_DLL% ^
	-t all ^
	-d json ^
	--conf %CONF_ROOT%\luban.conf ^
	-x outputDataDir=%GameData%

powershell -Command "Get-ChildItem '..\Assets\GameData\*.json' | ForEach-Object { $newName = $_.Name.Substring(0,1).ToUpper() + $_.Name.Substring(1); if ($_.Name -cne $newName) { Rename-Item $_.FullName -NewName $newName } }"

pause