$searchPath = Get-Location
$e2eDirectory = Join-Path -Path $searchPath -ChildPath "E2E"

if ($null -ne $env:PIPELINE_WORKSPACE)
{
    $e2eDirectory = Join-Path -Path $env:BUILD_SOURCESDIRECTORY -ChildPath 'E2E'
}

Write-Host "Search Path: $searchPath"
Write-Host "Files in Path"
ls

$nupkg = Get-ChildItem -Path $searchPath -Filter *.nupkg -Recurse | Select-Object -First 1

Write-Host "Found Packge: $($nupkg.Name)"

$nupkg.Name -match '^(.*)((?:\.\d+){3,}(?:[-a-z\d]+))\.nupkg$'

$VersionName = $Matches[2]

if($VersionName.StartsWith('.'))
{
    $VersionName = $VersionName.Substring(1)
}

Write-Host "Vesion Name: $VersionName"

$props = (Get-Content "$e2eDirectory\CI.props") -replace 'GeneratedPackageVersion', $VersionName

Write-Host "Updated Props: "
Write-Host $props

Out-File -FilePath "$e2eDirectory\Directory.Build.props" -InputObject $props -Force
