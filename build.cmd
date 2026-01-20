@echo off
echo 🚀 Starting Build Process...

:: Define paths
set "PROJ_PATH=%~dp0BeamMPLauncher.csproj"
set "OUT_DIR=%~dp0bin\Dist"

:: Clean previous builds
if exist "%OUT_DIR%" (
    echo 🧹 Cleaning old build files...
    rmdir /s /q "%OUT_DIR%"
)

echo 🔨 Building single-file executable...
dotnet publish "%PROJ_PATH%" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=embedded -o "%OUT_DIR%"

if %ERRORLEVEL% EQU 0 (
    echo ✅ Build success!
    echo 📂 Output: %OUT_DIR%\BeamMPLauncher.exe
    explorer "%OUT_DIR%"
) else (
    echo ❌ Build failed.
    pause
    exit /b 1
)

pause
