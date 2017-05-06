#Requires -Version 3.0

Param(
	[switch]$PreBuild,
	[switch]$PostBuild
)

$Root = "C:\projects\build"
If (-not (test-path $Root)) {
	mkdir $Root | out-null
}

# check that the powershell scripts exist (should be cloned before this script runs)
$ScriptDir = "$Root\Powershell\Scripts"
If (-not (test-path $ScriptDir)) {
	Write-Host -Foreground Red "Powershell scripts not found. Exiting."
	Return
}

# source external scripts
. "$ScriptDir\Utils-GitHub.ps1"
. "$ScriptDir\Utils-Appveyor.ps1"

If ($PreBuild) {
	# Restore nuget packages and get dependent libraries
	Write-Host -Foreground Cyan "Cloning dependencies from GitHub"	
	GetLatestRelease $Root "HearthSim" "Hearthstone-Deck-Tracker" -Scrape
	$ExtractPath = Join-Path -Path $Root -ChildPath "Hearthstone Deck Tracker"
	Rename-Item -NewName "$ExtractPath\HearthstoneDeckTracker.exe" "$ExtractPath\Hearthstone Deck Tracker.exe"
	GetLatestRelease $Root "andburn" "hdt-plugin-common" -Scrape
	Write-Host -Foreground Cyan "Restoring nuget packages"
	nuget restore
} ElseIf ($PostBuild) {
	# Create a release package
	Write-Host -Foreground Cyan "Creating deployment artifacts"
	BuildArtifacts "VictoryShot.Plugin" "hdt-plugin-victoryshot" "$Root\VictoryShot" "bin\x86\Release"
}
