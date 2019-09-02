$nupkg = Get-ChildItem -Path . -Filter *.nupkg -Recurse | Select-Object -First 1
$nupkg.Name -match '^(.*?)\.((?:\.?[0-9]+){3,}(?:[-a-z]+)?)\.nupkg$'

$VersionName = $Matches[2]
$IsPreview = $VersionName -match '-pre$'
$DeployToNuGet = !($VersionName -match '-ci$')
$ReleaseDisplayName = $VersionName

if($IsPreview -eq $true)
{
    $ReleaseDisplayName = "$VersionName - Preview"
}

Write-Host "Version Name" $VersionName
Write-Host "IsPreview $IsPreview"
Write-Host "Deploy to NuGet: $DeployToNuGet"

Write-Output ("##vso[task.setvariable variable=DeployToNuGet;]$DeployToNuGet")
Write-Output ("##vso[task.setvariable variable=VersionName;]$VersionName")
Write-Output ("##vso[task.setvariable variable=IsPreview;]$IsPreview")
Write-Output ("##vso[task.setvariable variable=ReleaseDisplayName;]$ReleaseDisplayName")