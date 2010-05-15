function Check-RegistryValue($PropertyPath, $PropertyName, $PropertyValue)
{
	if ((Test-Path -path $PropertyPath) -ne $True) 
	{
		return $FALSE;
	}

	$found = $FALSE;
    
    $entryProperty = Get-ItemProperty -path $PropertyPath

    if($entryProperty.psobject.Properties -ne $null)
    {
        $registryValue = $entryProperty.psobject.Properties[$PropertyName].value;

        if($registryValue -eq $PropertyValue)
        {
            $found = $TRUE;
        }
    }

    $found;
}

$query = Get-WmiObject Win32_Service | where {$_.Name -eq "w3svc" }

$hasIIS = ($query -ne $null);
$hasAspNet = Check-RegistryValue -PropertyPath "HKLM:\SOFTWARE\Microsoft\INETSTP\Components" -PropertyName "ASPNET" -PropertyValue "1";

$hasIIS -and $hasAspNet;