set WORKSPACE=..
set GEN_CLIENT=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %GEN_CLIENT% ^
    -t client ^
	-d bin ^
	-c cs-bin ^
    --conf %CONF_ROOT%\luban.conf ^
	-x cs-bin.outputCodeDir=output_Gen ^
    -x json.outputDataDir=output_Json ^
    -x bin.outputDataDir=output_Bin 