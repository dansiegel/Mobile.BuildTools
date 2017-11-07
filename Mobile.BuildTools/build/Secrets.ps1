Param(
    [string]$RootNamespace,
    [string]$ProjectPath,
    [string]$JsonSecretsFileName,
    [string]$OutputFileName
  )
  
  $tabSpace = "    "
  $placeholder = "%%REPLACEME%%"
  $secretsClass = "namespace $RootNamespace.Helpers`n{`n$($tabSpace)internal static class Secrets`n$($tabSpace){`n$($placeholder)`n$($tabSpace)}`n}`n"
  
  $replacement = ""
  
  $secretsJsonPath = "$ProjectPath/$JsonSecretsFileName"
  if(Test-Path $secretsJsonPath)
  {
      $secrets = Get-Content $secretsJsonPath | Out-String | ConvertFrom-Json
      foreach($key in ($secrets | Get-Member -MemberType NoteProperty).Name)
      {
          $replacement += "$($tabSpace)$($tabSpace)internal const string $key = ""$($secrets.$key)"";`n`n"
      }

      $replacement = $replacement -replace "`n`n$",""
      
      $secretsClass = $secretsClass -replace $placeholder,$replacement
      
      Write-Host $secretsClass
      
      $outputPath = Split-Path -Path $ProjectPath/$OutputFileName
      if(!(Test-Path $outputPath))
      {
          New-Item -ItemType Directory -Force -Path $outputPath
      }

      Out-File -FilePath $ProjectPath/$OutputFileName -Force -InputObject $secretsClass -Encoding ASCII
  }
  else 
  {
      Write-Host "No secrets json found at $secretsJsonPath"
  }
  