& "$env:APPVEYOR_BUILD_FOLDER\.paket\paket.bootstrapper.exe"
& "$env:APPVEYOR_BUILD_FOLDER\.paket\paket.exe" install -v
git checkout -- VictoryShot\FodyWeavers.xml
