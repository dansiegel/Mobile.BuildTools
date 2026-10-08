# Upgrading from v1

V1 is no longer maintained. Its Secrets API is the historical predecessor of AppSettings. Use [the walkthrough](../config/appsettings/index.md) when moving to released v2 or evaluating forthcoming v3 source.

1. Install the correct package in the compiling project that should receive the class.
2. Put `buildtools.json` beside the solution and add an `appSettings` entry keyed by the exact project filename without `.csproj`.
3. Describe each class and property explicitly. Use `isArray` and `defaultValue`.
4. Rename local `secrets.json` inputs to `appsettings.json`, and check CI mappings against the configured prefix (`BuildTools_` by default).
5. Update application code to the generated namespace and class. ConfigurationManager is a separate XML API and does not replace generated settings.
6. Rebuild and verify the actual values in a safe test environment.

V2 and v3 have different settings-file discovery rules. V2 merges project-to-solution files; forthcoming v3 selects the first applicable directory, starting at the solution. Review [precedence](../config/appsettings/index.md#file-lookup-and-precedence) before migration so a previously working override does not silently stop applying.

Older code may migrate `projectSecrets` automatically and may still load `secrets.json` with a warning. Explicit migration makes your configuration reviewable and avoids relying on compatibility paths. Do not assume that every retained legacy platform target is actively supported.
