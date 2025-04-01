@echo off
echo Running tests and collecting coverage...

:: Run the tests with coverage output to a known location
dotnet test ^
  /p:CollectCoverage=true ^
  /p:CoverletOutput=TestResults/ ^
  /p:CoverletOutputFormat=cobertura

if errorlevel 1 (
    echo Test run failed. Aborting report generation.
    exit /b 1
)

:: Generate HTML report
reportgenerator -reports:TestResults/coverage.cobertura.xml -targetdir:CoverageReport -reporttypes:Html

if exist CoverageReport\index.html (
    echo Opening coverage report...
    start CoverageReport\index.html
) else (
    echo Report generation failed. No HTML file found.
)