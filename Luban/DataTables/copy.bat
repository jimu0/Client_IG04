set Target_PATH_Gen=..\..\..\Solution_IG04\RPGCore_IG04\GameConfig\Gen
set Target_PATH_Bin=..\..\Client\Assets\StreamingAssets\GameConfig\Bin

if not exist %Target_PATH_Gen% (
	md %Target_PATH_Gen%
)

if not exist %Target_PATH_Bin% (
	md %Target_PATH_Bin%
)

del %Target_PATH_Gen%\*.* /f /s /q
del %Target_PATH_Bin%\*.* /f /s /q

for  %%i in (.\output_Gen\*) do (
	copy /y %%~fi %Target_PATH_Gen%
)

for  %%i in (.\output_Bin\*) do (
	copy /y %%~fi %Target_PATH_Bin%
)