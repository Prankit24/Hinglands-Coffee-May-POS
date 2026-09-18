@echo off
setlocal
cd /d "%~dp0"

echo ==========================================
echo MayPOS - Build Release Portable
 echo ==========================================

set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if not exist "%VSWHERE%" (
  echo Khong tim thay Visual Studio Installer / vswhere.exe.
  echo Hay build Release bang Visual Studio: Build ^> Rebuild Solution.
  pause
  exit /b 1
)

for /f "usebackq tokens=*" %%i in (`"%VSWHERE%" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "MSBUILD=%%i"
if not defined MSBUILD (
  echo Khong tim thay MSBuild.
  pause
  exit /b 1
)

echo MSBuild: %MSBUILD%
"%MSBUILD%" MayPOS.sln /restore /t:Rebuild /p:Configuration=Release /p:Platform="Any CPU"
if errorlevel 1 (
  echo.
  echo BUILD THAT BAI. Mo Visual Studio xem Error List.
  pause
  exit /b 1
)

set "OUT=%~dp0MayPOS-Portable"
if exist "%OUT%" rmdir /s /q "%OUT%"
mkdir "%OUT%"
xcopy /e /i /y "%~dp0PhanMemBanHang\bin\Release\*" "%OUT%\" >nul

del /q "%OUT%\*.pdb" 2>nul

echo.
echo Da tao: %OUT%
echo Test MayPOS.exe trong folder nay truoc khi zip gui HR.
pause
