@echo off
echo Starting test execution...
echo.

echo Cleaning previous results...
if exist "Common/TestResults" rmdir /s /q "Common/TestResults"

echo Restoring packages...
dotnet restore

echo Building project...
dotnet build --no-restore

echo Running tests...
set ALLURE_RESULTS_DIRECTORY=Common\TestResults\allure-results
dotnet test --no-build --verbosity n --settings:.runsettings --results-directory Common/TestResults

if %ERRORLEVEL% NEQ 0 (
    echo Some tests failed!
) else (
    echo All tests passed!
)

allure serve Common/TestResults
