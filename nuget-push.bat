@echo off

SET proj=Exomia.Database

cd %proj%

del /q /f *.nupkg

msbuild /t:restore /t:build /t:pack /p:Configuration=Release /p:Platform=AnyCPU
msbuild /t:restore /t:build /t:pack /p:Configuration=Release /p:Platform=x86
msbuild /t:restore /t:build /t:pack /p:Configuration=Release /p:Platform=x64

set /p version=version: 

IF "%version%" NEQ "" (
	nuget push "%proj%.%version%.nupkg" -Source https://api.nuget.org/v3/index.json
	nuget push "%proj%.x86.%version%.nupkg" -Source https://api.nuget.org/v3/index.json
	nuget push "%proj%.x64.%version%.nupkg" -Source https://api.nuget.org/v3/index.json	
) 
pause