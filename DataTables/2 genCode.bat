set WORKSPACE=..
set GEN_CLIENT=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables\luban.conf
set CODE_DIR=%WORKSPACE%\Assets\Scripts\Gen

dotnet %GEN_CLIENT% ^
    -t client ^
    -c cs-simple-json ^
    --conf %CONF_ROOT% ^
    -x outputCodeDir=%CODE_DIR%

pause