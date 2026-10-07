# Package signing

CI and Start NuGet Release use the existing Azure Artifact Signing account `avantipoint-signing`, certificate profile `AvantiPoint`, at `https://eus.codesigning.azure.net/`.

The shared workflow selects Artifact Signing because `codeSignEndpoint` is set. It signs NuGet packages with the dotnet `sign` CLI on Windows. Authentication uses GitHub OIDC through Azure Login; no client secret or Key Vault certificate is passed.

## Access prerequisites

Before enabling this workflow on master, verify the existing `CodeSignClientId` and `CodeSignTenantId` secrets identify the intended signing identity. Secret names alone do not establish that identity or its access.

That identity must already have:
- A federated credential with issuer `https://token.actions.githubusercontent.com`, audience `api://AzureADTokenExchange`, and subject `repo:dansiegel/Mobile.BuildTools:ref:refs/heads/master`.
- The [Artifact Signing Certificate Profile Signer role](https://learn.microsoft.com/azure/artifact-signing/tutorial-assign-roles) for the `AvantiPoint` profile.

The manual release workflow should be run from master. A different branch or tag requires separately reviewed matching federation.

Only the signing build jobs request `id-token: write`. PR validation stays unsigned. Signing failure prevents artifact upload and downstream feed publication. This source change does not create credentials, federated trust, role assignments, or change repository secrets.

## Verification

Unsigned PR checks validate the workflow and package build. They cannot prove Azure authentication or certificate-profile access. The first separately authorized trusted build must pass Azure Login and Artifact Signing before this migration is considered operationally verified.

