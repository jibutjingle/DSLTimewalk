@echo off
setlocal enabledelayedexpansion

REM Check if the script is provided with the Unity project path and pattern parameter
if "%~1"=="" (
    echo Usage: %0 ^<UnityProjectPath^> ^<Pattern^>
    exit /b 1
)

REM Set the source path
set "sourcePath=%~dp0"

REM Set the Unity project path
set "unityProjectPath=%~1"

REM Set the pattern list with all arguments except the first
set "patternList="
:loop
shift
if "%~1"=="" goto :patternListDone
set "patternList=!patternList! %1"
goto :loop
:patternListDone

REM Check if the specified directory exists
if not exist "%unityProjectPath%" (
    echo Error: Specified Unity project path does not exist.
    exit /b 1
)

REM Set the destination path
set "destinationPath=%unityProjectPath%\Packages\dependencies"

REM Check if the source directory exists
if not exist "%sourcePath%" (
    echo Error: Source directory "dependencies" does not exist.
    exit /b 1
)

REM Create the destination directory if it doesn't exist
if not exist "%destinationPath%" (
    mkdir "%destinationPath%"
)

REM Use findstr to filter files based on the pattern list
for /f "delims=" %%I in ('dir /b /a-d "%sourcePath%" ^| findstr /v /i /r "%patternList:\"=\"%"') do (
    copy "%sourcePath%\%%I" "%destinationPath%"
    echo Copied: %%I
)

echo Dependencies copied successfully to the Packages\dependencies folder.

REM Check if ds-unity-package-importer.exe exists
set "importerExe=%destinationPath%\ds-unity-package-importer.exe"
if exist "%importerExe%" (
    echo Running ds-unity-package-importer.exe...
    pushd "%destinationPath%"
    start "" "%importerExe%"
    popd
) else (
    echo Error: ds-unity-package-importer.exe not found in the dependencies folder.
    exit /b 1
)

exit /b 0
