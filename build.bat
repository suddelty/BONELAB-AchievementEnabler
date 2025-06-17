@echo off
setlocal enabledelayedexpansion
echo Building BONELAB Achievement Enabler...
echo.

REM Check if dotnet is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo Error: .NET SDK not found. Please install .NET 6.0 SDK or later.
    echo Download from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

REM Set default game path if not already set
if not defined GamePath (
    echo Please enter the path to your BONELAB installation folder.
    echo.
    echo Common locations:
    echo   C:\Program Files ^(x86^)\Steam\steamapps\common\BONELAB
    echo   C:\Program Files\Steam\steamapps\common\BONELAB
    echo   D:\Steam\steamapps\common\BONELAB
    echo.
    echo NOTE: You can copy and paste the path from File Explorer
    echo.
    set /p "GamePath=Enter BONELAB directory path: "
    
    REM Remove surrounding quotes if present
    if "!GamePath:~0,1!"=="\"" if "!GamePath:~-1!"=="\"" (
        set "GamePath=!GamePath:~1,-1!"
    )
)

REM Check if game path exists
if not exist "%GamePath%" (
    echo Error: Game path does not exist: "%GamePath%"
    echo Please verify your BONELAB installation and set the correct GamePath.
    pause
    exit /b 1
)

REM Debug: Show what's in the game directory
echo Checking contents of: "%GamePath%"
echo.
echo Contents found:
dir "%GamePath%" /b
echo.

REM Check if MelonLoader is installed
if not exist "%GamePath%\MelonLoader" (
    echo Error: MelonLoader folder not found at "%GamePath%\MelonLoader"
    echo.
    echo Looking for MelonLoader files...
    if exist "%GamePath%\version.dll" (
        echo Found: version.dll ^(MelonLoader proxy^)
    )
    if exist "%GamePath%\MelonLoader.dll" (
        echo Found: MelonLoader.dll ^(direct installation^)
    )
    if exist "%GamePath%\dobby.dll" (
        echo Found: dobby.dll ^(MelonLoader dependency^)
    )
    echo.
    echo Please install MelonLoader before building this mod.
    echo Download from: https://github.com/LavaGang/MelonLoader/releases
    pause
    exit /b 1
)

REM Check what's inside the MelonLoader folder
echo Checking MelonLoader folder contents:
echo.
dir "%GamePath%\MelonLoader" /b
echo.

REM Check if required MelonLoader files exist
if not exist "%GamePath%\MelonLoader\MelonLoader.dll" (
    echo MelonLoader.dll not found in MelonLoader folder.
    echo.
    echo Looking for MelonLoader.dll in subdirectories...
    if exist "%GamePath%\MelonLoader\net6.0\MelonLoader.dll" (
        echo Found: MelonLoader.dll in net6.0 subfolder
        set "MELON_FOUND=1"
    )
    if exist "%GamePath%\MelonLoader\net35\MelonLoader.dll" (
        echo Found: MelonLoader.dll in net35 subfolder  
        set "MELON_FOUND=1"
    )
    if exist "%GamePath%\MelonLoader\Dependencies\MelonLoader.dll" (
        echo Found: MelonLoader.dll in Dependencies subfolder
        set "MELON_FOUND=1"
    )
    
    if not defined MELON_FOUND (
        echo Error: MelonLoader.dll not found anywhere. Your MelonLoader installation may be incomplete.
        echo Please reinstall MelonLoader.
        pause
        exit /b 1
    )
)

if not exist "%GamePath%\MelonLoader\Managed" (
    if not exist "%GamePath%\MelonLoader\Il2CppAssemblies" (
        echo Error: Neither Managed nor Il2CppAssemblies folder found.
        echo This usually means the game hasn't been run with MelonLoader yet.
        echo Please launch BONELAB once with MelonLoader installed, then try building again.
        pause
        exit /b 1
    ) else (
        echo Found Il2CppAssemblies folder ^(newer MelonLoader version^)
    )
) else (
    echo Found Managed folder ^(older MelonLoader version^)
)

echo MelonLoader installation verified successfully.
echo.

REM Debug: Check for Harmony library
echo Looking for 0Harmony.dll...
if exist "%GamePath%\MelonLoader\Dependencies\0Harmony.dll" (
    echo Found: 0Harmony.dll in Dependencies folder
) else if exist "%GamePath%\MelonLoader\0Harmony.dll" (
    echo Found: 0Harmony.dll in MelonLoader root
) else if exist "%GamePath%\MelonLoader\net6\0Harmony.dll" (
    echo Found: 0Harmony.dll in net6 folder
) else if exist "%GamePath%\MelonLoader\net35\0Harmony.dll" (
    echo Found: 0Harmony.dll in net35 folder
) else (
    echo 0Harmony.dll not found in expected locations.
    echo.
    echo Checking all MelonLoader subfolders:
    echo Dependencies folder contents:
    if exist "%GamePath%\MelonLoader\Dependencies" (
        dir "%GamePath%\MelonLoader\Dependencies" /b
    ) else (
        echo Dependencies folder does not exist
    )
    echo.
    echo net6 folder contents:
    if exist "%GamePath%\MelonLoader\net6" (
        dir "%GamePath%\MelonLoader\net6" /b
    ) else (
        echo net6 folder does not exist
    )
    echo.
    echo net35 folder contents:
    if exist "%GamePath%\MelonLoader\net35" (
        dir "%GamePath%\MelonLoader\net35" /b
    ) else (
        echo net35 folder does not exist
    )
    echo.
)
echo.

REM Build the project
echo Building with GamePath: "%GamePath%"
echo.
dotnet build --configuration Release

if errorlevel 1 (
    echo.
    echo Build failed! Please check the error messages above.
    pause
    exit /b 1
)

echo.
echo Build completed successfully!
echo.
echo Build output can be found in the "builds" folder
echo.
set /p "COPY_TO_GAME=Would you like to copy the mod to your BONELAB installation? (Y/N): "
if /i "!COPY_TO_GAME!"=="Y" (
    echo Copying mod to: "%GamePath%\Mods\"
    if not exist "%GamePath%\Mods" mkdir "%GamePath%\Mods"
    copy /Y "builds\net6.0\AchievementEnabler.dll" "%GamePath%\Mods\"
    if errorlevel 1 (
        echo Failed to copy mod to BONELAB folder.
    ) else (
        echo Successfully copied mod to BONELAB folder.
        echo You can now launch BONELAB to use the Achievement Enabler mod.
    )
) else (
    echo Skipped copying to BONELAB folder.
)
echo.
pause 