@echo off
setlocal enabledelayedexpansion

:: Configuration
set SOLUTION=ArtLink.sln
set CONFIGURATION=Release
set OUTPUT_DIR=..\deploy
set MAIN_PROJECT=ArtLink.Server

:: Cleaning previous build
echo Cleaning previous build...
dotnet clean %SOLUTION% -c %CONFIGURATION%

:: Restoring packages
echo Restoring packages...
dotnet restore %SOLUTION%

:: Building components separately
echo Building components...
for %%p in (
    "ArtLink.Dto",
    "ArtLink.Domain",
    "ArtLink.DataAccess",
    "Services\ArtLink.Services.Artist",
    "Services\ArtLink.Services.Artwork",
    "Services\ArtLink.Services.Contract",
    "Services\ArtLink.Services.Employer",
    "Services\ArtLink.Services.Portfolio",
    "Services\ArtLink.Services.Search",
    "Services\ArtLink.Services.Technique",
    "%MAIN_PROJECT%"
) do (
    echo Building: %%~p
    dotnet build "%%~p" -c %CONFIGURATION% --no-restore
)

:: Creating deployment directory
echo Creating deployment directory...
if exist "%OUTPUT_DIR%" rmdir /s /q "%OUTPUT_DIR%"
mkdir "%OUTPUT_DIR%"
mkdir "%OUTPUT_DIR%\components"

:: Copying components
echo Copying components...
for %%p in (
    "ArtLink.Dto\bin\%CONFIGURATION%\net8.0\*.dll",
    "ArtLink.Domain\bin\%CONFIGURATION%\net8.0\*.dll",
    "ArtLink.DataAccess\bin\%CONFIGURATION%\net8.0\*.dll",
    "Services\ArtLink.Services.*\bin\%CONFIGURATION%\net8.0\*.dll"
) do (
    copy /y "%%~p" "%OUTPUT_DIR%\components\"
)

copy "ArtLink.Server\appsettings.json" "%OUTPUT_DIR%\"

:: Publishing the main application
echo Publishing the main application...
dotnet publish "%MAIN_PROJECT%" -c %CONFIGURATION% -o "%OUTPUT_DIR%" --no-restore

:: Copying configuration files
echo Copying configuration files...
if exist "config.json" copy "config.json" "%OUTPUT_DIR%\"
if not exist "%OUTPUT_DIR%\logs" mkdir "%OUTPUT_DIR%\logs"

:: Creating run script
echo Creating run script...
echo @echo off > "%OUTPUT_DIR%\run.bat"
echo set COMPONENTS_PATH=%~dp0components >> "%OUTPUT_DIR%\run.bat"
echo ArtLink.Server.exe >> "%OUTPUT_DIR%\run.bat"

endlocal
