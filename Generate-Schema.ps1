$currentDir = Get-Location
dotnet publish ./tools/Mobile.BuildTools.SchemaGenerator/Mobile.BuildTools.SchemaGenerator.csproj -o Artifacts/SchemaTool
./Artifacts/SchemaTool/Mobile.BuildTools.SchemaGenerator $currentDir.Path