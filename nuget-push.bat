@echo off
cd Exomia.Database

msbuild /t:restore /t:build /t:pack /p:Configuration=Release /p:Platform=AnyCPU
msbuild /t:restore /t:build /t:pack /p:Configuration=Release /p:Platform=x86
msbuild /t:restore /t:build /t:pack /p:Configuration=Release /p:Platform=x64

set /p version=version: 

IF "%version%" NEQ "" (
	nuget push "Exomia.Database.%version%.nupkg" -Source https://api.nuget.org/v3/index.json
	nuget push "Exomia.Database.x86.%version%.nupkg" -Source https://api.nuget.org/v3/index.json
	nuget push "Exomia.Database.x64.%version%.nupkg" -Source https://api.nuget.org/v3/index.json	
) 
pause