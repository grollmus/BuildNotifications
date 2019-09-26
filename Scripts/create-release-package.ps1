$applicationName = "BuildNotifications"
$repo = "grollmus/$applicationName"
$targetFolder = "Releases"
$versionToBuild = "$($env:APPVEYOR_REPO_TAG_NAME)"

$squirrelUrl = "https://github.com/Squirrel/Squirrel.Windows/releases/download/1.9.1/Squirrel.Windows-1.9.1.zip"
$nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

###############################################################################
New-Item -ItemType Directory -Force -Path $targetFolder

Write-Host Determining latest release
$latestReleaseUrl = "https://api.github.com/repos/$repo/releases"
$tag = (Invoke-WebRequest $latestReleaseUrl | ConvertFrom-Json)[0].tag_name
$version = $tag.Substring(1)
Write-Host Latest release is $tag => $version

Write-Host Downloading latest RELEASE file
$releasesFileUrl = "https://github.com/$repo/releases/download/$tag/RELEASES";
$releasesFilePath = "$targetFolder/RELEASES"
Invoke-WebRequest $releasesFileUrl -Out $releasesFilePath

Write-Host Downloading latest full package
$fullPackageFileName = "$applicationName-$version-full.nupkg"
$fullPackageUrl = "https://github.com/$repo/releases/download/$tag/$fullPackageFileName"
$fullPackageFilePath = "$targetFolder/$fullPackageFileName"
Invoke-WebRequest $fullPackageUrl -Out $fullPackageFilePath

Write-Host Downloading squirrel
$squirrelZipFile = "Squirrel.zip"
Invoke-WebRequest $squirrelUrl -Out $squirrelZipFile
Expand-Archive $squirrelZipFile -Force -DestinationPath .

Write-Host Downloading nuget.exe
Invoke-WebRequest $nugetUrl -Out "nuget.exe"

Write-Host Creating nuget package
$nuspecFileName = "Scripts/$applicationName.nuspec" 
$nupkgFileName = "$applicationName-$versionToBuild.nupkg"
.\nuget.exe pack $nuspecFileName -Version $versionToBuild

Write-Host Creating squirrel release
.\squirrel.exe --releasify $nupkgFileName

Write-Host Done