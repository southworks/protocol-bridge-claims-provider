@echo off

echo.
echo ================================================================
echo WARNING: This setup script will replace the existing "localhost" 
echo certificate. If you have applications that uses the current
echo localhost certificate, remember to create a backup copy of it 
echo before continue with this setup.
echo ================================================================
echo.

echo Installing CAPICOM...
echo.

msiexec /qn /i "%~dp0capicom_dc_sdk.msi"

echo Removing certificate...
echo.
certutil -delstore My "localhost"
certutil -delstore TrustedPeople "localhost"

echo Installing certificate...
echo.

certutil -f -addstore TrustedPeople "%~dp0certs\localhost.cer"

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

%cscript% /nologo %capicompath% import -l LM "%~dp0certs\localhost.pfx" "xyz"

set IsWinClient=false
set IsW7=
(ver | findstr /C:"6.1") && set IsW7=true

@if "%IsW7%" == "true" (
	"%~dp0winhttpcertcfg.exe" -g -c LOCAL_MACHINE\My -s localhost -a "IIS_IUSRS"
	set IsWinClient=true
) else (
	"%~dp0winhttpcertcfg.exe" -g -c LOCAL_MACHINE\My -s localhost -a "NETWORK SERVICE"
)

for /f "tokens=2,* usebackq" %%a in ( `certutil -store my localhost ^| findstr /c:"Cert Hash(sha1)"` ) do (
    set CERTHASH=%%b
)

@if NOT "%CERTHASH%" == "" (
 set CERTHASH=%CERTHASH: =%
)



set IsVista=
(ver | findstr /C:"6.0") && set IsVista=true

if "%IsWinClient%" == "false" (
	set IsWinClient=%IsVista%
)

if "%IsWinClient%" == "true" (
   ECHO Setting up SSL at port 443 using localhost certificate...
   netsh http delete sslcert ipport=0.0.0.0:443 > nul
   netsh http add sslcert ipport=0.0.0.0:443 appid={00000000-0000-0000-0000-000000000000} certhash=%CERTHASH% clientcertnegotiation=enable
) else ( 
   ECHO Importing server certificate and point HTTP.SYS at it...
   httpcfg.exe delete ssl -i 0.0.0.0:443 > nul
   httpcfg.exe set ssl -i 0.0.0.0:443 -f 2 -h %CERTHASH%
)

"%~dp0IIS7Util.exe" SetSslCert %CERTHASH%

IISReset

echo.
echo ===========================
echo Certificate Setup finished!
echo ===========================

pause
:end