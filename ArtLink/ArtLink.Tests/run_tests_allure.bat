@echo off
echo Starting test execution...
echo.

echo Cleaning previous results...
if exist "allure-results" rmdir /s /q "allure-results"
if exist "allure-report" rmdir /s /q "allure-report"
if exist "TestResults" rmdir /s /q "TestResults"

echo Restoring packages...
dotnet restore

echo Building project...
dotnet build --no-restore

echo Running tests...
dotnet test --no-build --verbosity normal --logger trx --results-directory TestResults

if %ERRORLEVEL% NEQ 0 (
    echo Some tests failed!
) else (
    echo All tests passed!
)

echo Generating Allure report...
allure generate TestResults -o allure-report --clean

echo Opening report...
allure open allure-report

echo Script completed!