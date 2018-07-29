$artifactDir = Join-Path -Path $env:SYSTEM_DEFAULTWORKINGDIRECTORY -ChildPath "Artifacts"
$downloadDir = Join-Path -Path $env:SYSTEM_DEFAULTWORKINGDIRECTORY -ChildPath "Download"
New-Item -ItemType Directory -Force -Path $artifactDir

Get-ChildItem -Path $downloadDir | Where-Object { $_.Extension -match ".nupkg" } | Copy-Item -Destination $artifactDir

