@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

REM ******************
REM CODED ON SHIFT-JIS
REM ******************

SET OBJ=todomgr.exe

SET EXE="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
SET REF="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF"

SET TYP=/t:winexe
SET ICO=-win32icon:app.ico

SET DLL=/r:%REF%\PresentationCore.dll
SET DLL=%DLL% /r:%REF%\PresentationFramework.dll
SET DLL=%DLL% /r:%REF%\WindowsBase.dll

SET OUT=/out:%OBJ%

REM --- プロセスが動いていれば終了 ---
TASKLIST /FI "IMAGENAME eq %OBJ%" | FINDSTR /I %OBJ% >NUL
IF %ERRORLEVEL%==0 (
  TASKKILL /F /IM %OBJ%
)

REM --- 埋め込みリソース ---
SET RES=
FOR %%F IN (resources/*.png) DO (
    SET RES=!RES! /res:resources\%%F
    ECHO /res: %%F added...
)

REM --- コンパイルオプション ---
SET OPT=/nologo
REM SET OPT=/nologo /optimize+ /debug- /define:RELEASE 

REM --- コンパイル ---
%EXE% %OPT% %ICO% %TYP% %OUT% %DLL% %RES% src\*.cs

IF ERRORLEVEL 1 (
  ECHO.
  ECHO *******************
  ECHO   コンパイルエラー
  ECHO *******************
  ECHO.
) ELSE (
  ECHO.
  ECHO ***************************
  ECHO コンパイルは正常終了しました。
  ECHO ***************************
  ECHO.
)