function SearchInstall($SearchVersion, $PathKey)
{
	$installObjects = ls -path $PathKey;
	$found = $FALSE;

	foreach($installEntry in $installObjects)
	{
		$entryProperty = Get-ItemProperty -path registry::$installEntry
	   
		if($entryProperty.psobject.Properties -ne $null -and $entryProperty.psobject.Properties["(default)"].value -eq $searchVersion)
		{
			$found = $TRUE;
			break;
		}
	}

	$found;
}

$res1 = SearchInstall -SearchVersion '6.1.7600.0' -PathKey 'HKLM:SOFTWARE\Wow6432Node\Microsoft\Windows Identity Foundation\Setup\';
$res2 = SearchInstall -SearchVersion '6.1.7600.0' -PathKey 'HKLM:SOFTWARE\Microsoft\Windows Identity Foundation\Setup\';
(($res1 -eq $TRUE) -or ($res2 -eq $TRUE))