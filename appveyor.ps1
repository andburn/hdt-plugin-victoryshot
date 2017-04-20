#Requires -Version 3.0

Param(
	[switch]$PreBuild,
	[switch]$PostBuild
)

$Root = "C:\projects\build"

If (-not (test-path $Root)) {
	mkdir $Root | out-null
}

If ($PreBuild) {	
	. "$Root\Powershell\Scripts\Utils-GitHub.ps1"
	Write-Host -Foreground Cyan "Cloning dependencies from GitHub"	
	GetLatestRelease $Root "HearthSim" "Hearthstone-Deck-Tracker"
	$ExtractPath = Join-Path -Path $Root -ChildPath "Hearthstone Deck Tracker"
	Rename-Item -NewName "$ExtractPath\HearthstoneDeckTracker.exe" "$ExtractPath\Hearthstone Deck Tracker.exe"
	GetLatestRelease $Root "andburn" "hdt-plugin-common"
	Write-Host -Foreground Cyan "Restoring nuget packages"	
	nuget restore
} ElseIf ($PostBuild) {
	Write-Host -Foreground Cyan "Creating deployment artifact"
	$OutDir = "$Root\VictoryShot"
	$OutName = "hdt-plugin-victoryshot_$env:APPVEYOR_REPO_TAG_NAME.zip"
	mkdir $OutDir | out-null
	Copy-Item -Path "$env:APPVEYOR_BUILD_FOLDER\VictoryShot\bin\x86\Release\VictoryShot*.dll" -Destination $OutDir
	& 7z a "$Root\victoryshot.zip" $OutDir | out-null
	Rename-Item -NewName "$Root\$OutName" -Path "$Root\victoryshot.zip"
	Push-AppveyorArtifact "$Root\$OutName" -FileName $OutName -DeploymentName release
}
