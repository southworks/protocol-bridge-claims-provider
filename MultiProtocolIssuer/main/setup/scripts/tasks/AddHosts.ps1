function Add-Host
{
	param([string]$ip, [string]$hostName)

	$f = $env:windir + "\System32\drivers\etc\hosts"

	$hostEntry = select-string "$ip	$hostName" $f
	if ($hostEntry -isnot [object]) {
        "$ip	$hostName" | Add-Content $f
    }
}

"`n" | Add-Content ($env:windir + "\System32\drivers\etc\hosts")

Add-Host "127.0.0.1" "www.multiprotocolissuer.net"