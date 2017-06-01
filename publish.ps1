param([switch]$CleanRebuild, [switch]$Publish) 

$Msbuild = "c:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\msbuild.exe"

$FuncAppName = "khaale-bot"
$Configuration = "Debug"
$Staging = ".staging"

if ($CleanRebuild) {
    Write-Host "Clean and rebuild.."
    Get-ChildItem .\ -include bin,obj -Recurse | Remove-Item -Force -Recurse
    &$Msbuild /t:build /p:Configuration=$Configuration /p:Platform="Any CPU"
}

$Projects = "CheckTorrents","CheckWeather","Messages"

Remove-Item $Staging -Force -Recurse
md $Staging | Out-Null
md $Staging\bin | Out-Null
Copy-Item .\host.json -Destination $Staging\host.json
Copy-Item .\appsettings.json -Destination $Staging\appsettings.json

foreach($proj in $Projects) 
{
    Write-Host "Copying $proj.."
    Copy-Item "$proj\bin\$Configuration\*" -Destination "$Staging\bin\" -Recurse -Force

    md $Staging\$proj | Out-Null 
    Copy-Item $proj\function.prod.json -Destination $Staging\$proj\function.json
}

if ($Publish)
{
    Write-Host "Publishing.."
    Start-Process func -ArgumentList "azure","functionapp","publish",$FuncAppName -WorkingDirectory $Staging -Wait
}
else
{
    Write-Host "Running.."
    Start-Process func -ArgumentList "host","start",$FuncAppName -WorkingDirectory $Staging -Wait
}