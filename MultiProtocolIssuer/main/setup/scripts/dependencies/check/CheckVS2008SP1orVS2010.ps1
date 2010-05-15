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

function Check-RegistryValueInChilds($PropertyPath, $PropertyName, $PropertyValue)
{
	if ((Test-Path -path $PropertyPath) -ne $True) 
	{
		return $FALSE;
	}

	$installObjects = ls -path $PropertyPath;
	$found = $FALSE;
    
    foreach($installEntry in $installObjects)
	{
        if (Check-RegistryValue -PropertyPath registry::$installEntry -PropertyName $PropertyName -PropertyValue $PropertyValue)
		{
            $found = $TRUE;
            break;
        }
        
	}

	$found;
}

function SearchUninstall($SearchFor, $UninstallKey)
{
	$uninstallObjects = ls -path $UninstallKey;
	$found = $FALSE;

	foreach($uninstallEntry in  $uninstallObjects)
	{
		$entryProperty = Get-ItemProperty -path registry::$uninstallEntry
		if($entryProperty.DisplayName -like $searchFor)
		{       
			$found = $TRUE;
			break;
		}
	}

	$found;
}

function Check-VisualStudio2008SP1($ProductFamily, $ProductEdition)
{
	Check-RegistryValueInChilds -PropertyPath "HKLM:\SOFTWARE\Microsoft\DevDiv\$ProductFamily\Servicing\9.0\$ProductEdition" -PropertyName 'SP' -PropertyValue '1';
}

function Check-VisualStudio2010($SearchFor)
{
	$os = Get-WMIObject win32_operatingsystem
	[bool] $found = $False;
	$SearchFor64bits = "$SearchFor (64-bit)"    
   
	# Seach in uninstall folder
	$found =  SearchUninstall -SearchFor $SearchFor -UninstallKey 'HKLM:SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\';    
	if($found) {return $found;}

	#Search in 64 bit keys folders
	if ($os.OSArchitecture -eq "64-bit") {
	   #Wow folder
	   $found =  SearchUninstall -SearchFor $SearchFor -UninstallKey 'HKLM:SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\';    
	   if($found)  {return $found;}
	   
	   #Wow folder, 64 bits
	   $found =  SearchUninstall -SearchFor  $SearchFor64bits -UninstallKey 'HKLM:SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\';    
	   if($found)  {return $found;}
	   
	   #32bit folder, 64 bits
	   $found =  SearchUninstall -SearchFor  $SearchFor64bits -UninstallKey 'HKLM:SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\';    
	   if($found)  {return $found;}
	}
	   
	return $found;
}

$hasVS2008SP1 = $FALSE;

# VCS EXP Microsoft Visual C# 2008 Express Edition - ENU
# $hasVS2008SP1 = $hasVS2008SP1 -or (Check-VisualStudio2008SP1 -ProductFamily 'VCS' -ProductEdition 'EXP');

# VNS EXP Microsoft Visual Web Developer 2008 Express Edition - ENU
$hasVS2008SP1 = $hasVS2008SP1 -or (Check-VisualStudio2008SP1 -ProductFamily 'VNS' -ProductEdition 'EXP');

# VS PRO Microsoft Visual Studio 2008 Professional Edition - ENU
$hasVS2008SP1 = $hasVS2008SP1 -or (Check-VisualStudio2008SP1 -ProductFamily 'VS' -ProductEdition 'PRO');

# VS STD Microsoft Visual Studio 2008 Standard Edition - ENU
$hasVS2008SP1 = $hasVS2008SP1 -or (Check-VisualStudio2008SP1 -ProductFamily 'VS' -ProductEdition 'STD');

# VS VSDB Visual Studio Team System 2008 Database Edition - ENU
$hasVS2008SP1 = $hasVS2008SP1 -or (Check-VisualStudio2008SP1 -ProductFamily 'VS' -ProductEdition 'VSDB');

# VS VSTA Microsoft Visual Studio Team System 2008 Architecture Edition - ENU
$hasVS2008SP1 = $hasVS2008SP1 -or (Check-VisualStudio2008SP1 -ProductFamily 'VS' -ProductEdition 'VSTA');

# VS VSTD Microsoft Visual Studio Team System 2008 Development Edition - ENU
$hasVS2008SP1 = $hasVS2008SP1 -or (Check-VisualStudio2008SP1 -ProductFamily 'VS' -ProductEdition 'VSTD');

# VS VSTS Microsoft Visual Studio Team System 2008 Team Suite - ENU
$hasVS2008SP1 = $hasVS2008SP1 -or (Check-VisualStudio2008SP1 -ProductFamily 'VS' -ProductEdition 'VSTS');

# VS VSTT Microsoft Visual Studio Team System 2008 Test Edition – ENU
$hasVS2008SP1 = $hasVS2008SP1 -or (Check-VisualStudio2008SP1 -ProductFamily 'VS' -ProductEdition 'VSTT');

# VS BSLN edition
$hasVS2008SP1 = $hasVS2008SP1 -or (Check-VisualStudio2008SP1 -ProductFamily 'VS' -ProductEdition 'BSLN');

if ($hasVS2008SP1) { return $hasVS2008SP1; }

$hasVS2010 = $FALSE;
$hasVS2010 = Check-VisualStudio2010 -SearchFor 'Microsoft Visual Studio 2010 *RC*';

return $hasVS2010;