@echo off

echo Installing CAPICOM...
echo.

msiexec /qn /i "%~dp0capicom_dc_sdk.msi"

call "%~dp0CleanupCertificate.cmd"

echo Installing certificate...
echo.

certutil -f -addstore TrustedPeople "%~dp0certs\www.multiprotocolissuer.net.cer"

IF EXIST "%PROGRAMFILES%\Microsoft CAPICOM 2.1.0.2 SDK" (
    SET capicompath="%PROGRAMFILES%\Microsoft CAPICOM 2.1.0.2 SDK\Samples\vbs\cstore.vbs"
    SET cscript=%windir%\system32\cscript.exe
)

IF EXIST "%PROGRAMFILES(x86)%\Microsoft CAPICOM 2.1.0.2 SDK" (
    SET capicompath="%PROGRAMFILES(x86)%\Microsoft CAPICOM 2.1.0.2 SDK\Samples\vbs\cstore.vbs"
    SET cscript=%windir%\syswow64\cscript.exe

    ECHO Setting up CAPICOM for 64 bits environment...
    copy /y "%PROGRAMFILES(x86)%\Microsoft CAPICOM 2.1.0.2 SDK\Lib\X86\capicom.dll" %windir%\syswow64
    %windir%\syswow64\regsvr32.exe /s %windir%\syswow64\capicom.dll
)

%cscript% /nologo %capicompath% import -l LM "%~dp0certs\www.multiprotocolissuer.net.pfx" ""

set IsWinClient=false
set IsW7=
(ver | findstr /C:"6.1") && set IsW7=true

@if "%IsW7%" == "true" (
	"%~dp0winhttpcertcfg.exe" -g -c LOCAL_MACHINE\My -s www.multiprotocolissuer.net -a "IIS_IUSRS"
	set IsWinClient=true
) else (
	"%~dp0winhttpcertcfg.exe" -g -c LOCAL_MACHINE\My -s www.multiprotocolissuer.net -a "NETWORK SERVICE"
)

echo.
echo ============================
echo Certificates Setup finished!
echo ============================

pause
:end