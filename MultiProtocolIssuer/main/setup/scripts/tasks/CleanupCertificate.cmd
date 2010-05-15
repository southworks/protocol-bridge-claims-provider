@echo off

echo Removing certificates...
echo.

certutil -delstore My "www.multiprotocolissuer.net"

certutil -delstore TrustedPeople "www.multiprotocolissuer.net"

echo.
echo Clean up finished!
echo.