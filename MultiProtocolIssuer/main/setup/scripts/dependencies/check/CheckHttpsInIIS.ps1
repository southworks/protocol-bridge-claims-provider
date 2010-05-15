# Get https binding from default web site
$websiteName = 'Default Web Site'

$website = gwmi -namespace "root/WebAdministration" -query "select * from Site where Name=""$websiteName"""
$httpsBinding = $website.Bindings | where { $_.Protocol -eq "https" }

if ($httpsBinding -eq $null)
{
    return $false;
}

return $true