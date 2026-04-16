set GEN_CLIENT={Luban.dll的路径}
set CONF_ROOT={DataTables目录的路径}

dotnet %GEN_CLIENT% ^
    -t server ^
    -c cs-dotnet-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir={生成的代码的路径} ^
    -x outputDataDir={生成的数据的路径}