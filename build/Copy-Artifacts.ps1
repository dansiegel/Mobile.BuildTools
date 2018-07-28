$artifactDir = Join-Path -Path $env:SYSTEM_DEFAULTWORKINGDIRECTORY -ChildPath "Artifacts"
New-Item -ItemType Directory -Force -Path $artifactDir

Get-ChildItem -Path $env:BUILD_ARTIFACTSTAGINGDIRECTORY | Where-Object { $_.Extension -match ".nupkg" } | Copy-Item -Destination $artifactDir

