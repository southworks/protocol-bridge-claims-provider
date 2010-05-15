@echo off

echo Removing certificate...
echo.

certutil -delstore My "www.multiprotocolissuer.net"

certutil -delstore TrustedPeople "www.multiprotocolissuer.net"

echo.
echo Clean up finished!
echo.