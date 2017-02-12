$out_name = "hdt-plugin-victoryshot_$env:APPVEYOR_REPO_TAG_NAME.zip"
mkdir C:\Release\VictoryShot
Copy-Item -Path "$env:APPVEYOR_BUILD_FOLDER\VictoryShot\bin\x86\Release\*dll*" -Destination C:\Release\VictoryShot
& 7z a C:\Release\victoryshot.zip C:\Release\VictoryShot
Rename-Item -NewName "C:\Release\$out_name" -Path 'C:\Release\victoryshot.zip'
Push-AppveyorArtifact "C:\Release\$out_name" -FileName $out_name -DeploymentName release
