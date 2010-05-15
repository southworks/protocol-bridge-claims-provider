function isLoadUserProfileEnabled ($applicationPool) {
    $actualDir = Get-Location
    $dir = $env:windir + "\system32\inetsrv"

    Set-Location $dir
    $IsLoadUserProfileEnabled = .\appcmd.exe list apppools /name:$applicationPool /processModel.loadUserProfile:true

    Set-Location $actualDir

    ($IsLoadUserProfileEnabled -ne $NULL);
}

isLoadUserProfileEnabled -applicationPool "DefaultAppPool";