@echo off
setlocal
echo Running tests with XPlat Code Coverage...

:: Run tests and collect coverage
dotnet test --collect:"XPlat Code Coverage"

if errorlevel 1 (
    echo Test run failed. Aborting report generation.
    pause
    exit /b 1
)

:: Find the latest TestResults folder with a coverage file
echo Looking for latest coverage file...
set "resultsDir=%cd%\TestResults"
set "coverageFile="

for /f "delims=" %%F in ('dir "%resultsDir%" /ad /b /o:-d') do (
    if exist "%resultsDir%\%%F\coverage.cobertura.xml" (
        set "coverageFile=%resultsDir%\%%F\coverage.cobertura.xml"
        goto :found
    )
)

echo No coverage.cobertura.xml found.
pause
exit /b 1

:found
echo Found coverage file: %coverageFile%

:: Generate HTML report
reportgenerator -reports:"%coverageFile%" -targetdir:CoverageReport -reporttypes:Html

:: Open the report
if exist CoverageReport\index.html (
    echo Opening coverage report...
    start CoverageReport\index.html
) else (
    echo Failed to generate report. No index.html found.
)

endlocal
pause