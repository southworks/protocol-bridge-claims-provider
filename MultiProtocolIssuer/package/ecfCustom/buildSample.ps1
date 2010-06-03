#------------------------------------------------------------
# Evangelism Content Framework. PowerShell implementation
#------------------------------------------------------------

# Arguments

$env:ECF = [Environment]::GetEnvironmentVariable("ECF","Machine")

[string] $sampleManifestFile = [System.IO.Path]::GetFullPath($args[0])
[string] $outputDirectory = [System.IO.Path]::GetFullPath($args[1])

[bool] $createSeftExtracting = $false
[string] $arg2 = $args[2]
if ([System.String]::Compare($arg2, "SelfExtracting" , $false) -eq 0)
{	
	$createSeftExtracting = $true
}

[string] $assetsDirectory = ""
if ($args[3] -ne "")
{	
	$assetsDirectory = [System.IO.Path]::GetFullPath($args[3])	
}

# Directories
[string] $resourcesDirectory = "$env:ECF\resources"

[string] $sampleDirectory =[System.IO.Path]::GetDirectoryName($sampleManifestFile)

#temps
[string] $tempDirectory = [System.IO.Path]::Combine($outputDirectory, "temp")

$reader = [xml] (get-content $sampleManifestFile)
[string] $sampleId = $reader.Sample.Id;

[string] $relativeSample = [System.IO.Path]::Combine($sampleId, [System.IO.Path]::GetFileName($sampleManifestFile))

[string] $tempSampleFile = [System.IO.Path]::Combine($tempDirectory, $relativeSample) 

#Remove the Log.xml file if it exists
[string] $logFile = [System.IO.Path]::Combine($packageDirectory, "Log.xml") 
if(test-path $logFile)
{
    Remove-item $logFile
}

#This script registers the SnapIn
add-pssnapin ContentFrameworkSnapIn

#---------------------------------------------------------------------------
#-| CopySample Step -------------------------------------------------------
#---------------------------------------------------------------------------

[string] $beginExcludes = ".svn obj bin"

Copy-Sample $sampleManifestFile $tempDirectory $beginExcludes $assetsDirectory

write "Copy sample done."
#---------------------------------------------------------------------------

#---------------------------------------------------------------------------
#-| RemoveSoucreCodeBindings Step ------------------------------------------
#---------------------------------------------------------------------------

Remove-SourceControlBindings $tempDirectory

write "Source Control Bindings Removal done."
#---------------------------------------------------------------------------

#---------------------------------------------------------------------------
#-| AddHeaders Step --------------------------------------------------------
#---------------------------------------------------------------------------

[string] $copyrightFile = [System.IO.Path]::Combine($resourcesDirectory, "Copyright.txt")

Add-Headers $copyrightFile $tempDirectory

write "Adding headers done."
#---------------------------------------------------------------------------

#---------------------------------------------------------------------------
#-| Convert Documents Step --------------------------------------------------
#---------------------------------------------------------------------------

[string] $xsltPath= [System.IO.Path]::Combine($resourcesDirectory, "HtmlConversion\xsl")

Convert-Document -Sample $tempSampleFile $xsltPath

write "Converting documents done."

#---------------------------------------------------------------------------
#-| Create Zip or Self Extracting ------------------------------------------
#---------------------------------------------------------------------------
if($createSeftExtracting)
{
	write "Creating self extracting ..."

	[string] $name = $sampleId + ".Setup.exe"
	[string] $licenseFile = [System.IO.Path]::GetFullPath("LicenseAgreement.txt")
	[string] $imageFile = [System.IO.Path]::Combine($resourcesDirectory, "vertical.bmp")
	[string] $excludeFile = [System.IO.Path]::Combine($resourcesDirectory, "Exclude.txt")

	Create-SelfExtract $name $tempSampleFile $outputDirectory $licenseFile $imageFile $excludeFile
}
else
{
	write "Creating Zip file ..."
	[string] $name = $sampleId + ".zip"
	[string] $zipName = [System.IO.Path]::Combine($outputDirectory, $name)
	[string] $excludeFile = [System.IO.Path]::Combine($resourcesDirectory, "Exclude.txt")

	Create-Zip $tempDirectory $zipName $excludeFile
}

write "Packaging Sample Finished."
#---------------------------------------------------------------------------