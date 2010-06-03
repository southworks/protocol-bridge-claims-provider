@echo off

IF EXIST %WINDIR%\SysWow64 (
	set powerShellDir=%WINDIR%\SysWow64\windowspowershell\v1.0
) ELSE (
	set powerShellDir=%WINDIR%\system32\windowspowershell\v1.0
)

call %powerShellDir%\powershell.exe -Command "&'%~dp0\ecfCustom\buildSample.ps1' '..\main\Sample.xml' '.\Output' 'SelfExtracting' ''"