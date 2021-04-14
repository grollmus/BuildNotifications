param( 
    [String]$versionToBuild="0.0.0",
    [String]$workingDirectory="."
)

$applicationName = "BuildNotifications"
$repo = "grollmus/$applicationName"
$targetFolder = "$workingDirectory/Releases"

$squirrelUrl = "https://github.com/Squirrel/Squirrel.Windows/releases/download/1.9.1/Squirrel.Windows-1.9.1.zip"
$nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

###############################################################################
Write-Output "Determining latest release"
$latestReleaseUrl = "https://api.github.com/repos/$repo/releases"
$tag = (Invoke-WebRequest $latestReleaseUrl | ConvertFrom-Json)[0].tag_name
$version = $tag
if($tag)
{
	New-Item -ItemType Directory -Force -Path $targetFolder

	if( $version.StartsWith("v") )
	{
		$version = $tag.Substring(1)
	}
    Write-Output "Latest release is $tag => $version"

    Write-Output "Downloading latest RELEASE file"
    $releasesFileUrl = "https://github.com/$repo/releases/download/$tag/RELEASES";
    $releasesFilePath = "$targetFolder/RELEASES"
    Invoke-WebRequest $releasesFileUrl -Out $releasesFilePath

    Write-Output "Downloading latest full package"    
    $fullPackageFileName = "$applicationName-$version-full.nupkg"
    $fullPackageUrl = "https://github.com/$repo/releases/download/$tag/$fullPackageFileName"
    $fullPackageFilePath = "$targetFolder/$fullPackageFileName"
    Invoke-WebRequest $fullPackageUrl -Out $fullPackageFilePath
}

Write-Output "Downloading squirrel"
$squirrelZipFile = "Squirrel.zip"
Invoke-WebRequest $squirrelUrl -Out $squirrelZipFile
Expand-Archive $squirrelZipFile -Force -DestinationPath $workingDirectory

Write-Output "Downloading nuget.exe"
Invoke-WebRequest $nugetUrl -Out "nuget.exe"

Write-Output "Preparing files for nuget package"
$legacyTargetFolder = "$workingDirectory\BuildNotifications\bin\Release\net5.0-windows\win-x64\publish"
if( -Not (Test-Path -Path $legacyTargetFolder) )
{
    New-Item -ItemType Directory -Force -Path $legacyTargetFolder
}
$legacyTarget = "$legacyTargetFolder\BuildNotifications.PluginInterfacesLegacy.dll"
$legacyDllPath = Get-ChildItem -Name "BuildNotifications.PluginInterfacesLegacy.dll" -Recurse -Path $workingDirectory | Select-Object -First 1
Move-Item -Path $legacyDllPath -Destination $legacyTarget -Force

Write-Output "Creating nuget package"
$nuspecFileName = "$workingDirectory/Scripts/$applicationName.nuspec" 
$nugetArgs = "pack",$nuspecFileName,"-Version",$versionToBuild
Start-Process -FilePath .\nuget.exe -ArgumentList $nugetArgs | Wait-Process

Write-Output "Creating squirrel release"
$searchPattern = "*$versionToBuild*.nupkg"
$nupkgFilePath = Get-ChildItem -Name $searchPattern -Recurse -Path $workingDirectory | Select-Object -First 1
Write-Output $nupkgFilePath
$arguments = "--releasify",$nupkgFilePath,"--no-msi"
$squirrelExe = "$workingDirectory\squirrel.exe"
Start-Process -FilePath $squirrelExe -ArgumentList $arguments -PassThru | Wait-Process

Get-Content -Path SquirrelSetup.log

Write-Output "Cleaning up"
$latestFullPackageFileName = "$applicationName-$version-full.nupkg"
$latestFullPackageFilePath = "$targetFolder/$latestFullPackageFileName"
Remove-Item -Path $latestFullPackageFilePath

Write-Output Done