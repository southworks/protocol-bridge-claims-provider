@ECHO OFF
ECHO Enabling Load User Profile for DefaultAppPool...

%windir%\system32\inetsrv\appcmd set config -section:applicationPools /[name='DefaultAppPool'].processModel.loadUserProfile:true

iisreset