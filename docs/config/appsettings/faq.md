# Troubleshooting App Settings

## No generated class

Check that the package is installed in the project that should contain the class, that `buildtools.json` is beside the solution, and that the `appSettings` key exactly matches the compiling `.csproj` filename without `.csproj`. The key is independent of `RootNamespace`. Define at least one property and rebuild the solution.

With forthcoming v3 packages, make sure both the Roslyn generator and matching MSBuild core assets are present. Installing `Mobile.BuildTools.Configuration` alone selects the XML runtime API and does not generate AppSettings. There is no separate command required to generate the class.

## Released v2 reports missing ConfigurationPath

If `SecretsJsonTask` reports that its required `ConfigurationPath` parameter was not supplied, the v2 targets may have run generation before configuration discovery. Pass `BuildToolsConfigFilePath` as the directory containing `buildtools.json`, as shown in [the verified walkthrough](index.md#4-build-and-use-the-generated-class). Do not pass the JSON filename itself. This workaround does not resolve unrelated SDK or task-assembly compatibility errors.

## The class exists but my code cannot use it

The default generated name is `{RootNamespace}.Helpers.AppSettings`. Use that namespace, or the `rootNamespace`, `namespace`, and `className` you configured. `namespace: "."` places it in the root namespace. The class is `Internal` by default; choose `Public` and add a project reference when consuming it from another assembly.

Do not expect it to be a normal checked-in source file. V2 adds generated files under `obj` to compilation. V3 adds generated source through Roslyn. Build errors are evidence of an actual compilation problem; editor completion alone is not.

For **released v2**, issue #272 documents an IDE completion problem sometimes resolved by closing and reopening the project after a successful build. This historical advice does not replace package, configuration, and generator checks for v3.

## A property is missing or has the wrong value

Check the exact property definition, type, prefix, active build configuration, and [file lookup rules](index.md#file-lookup-and-precedence). Use `BuildTools_PropertyName` for the default prefix. `Secret_` and platform-specific legacy prefixes are not the AppSettings default. Remove duplicate prefixed/unprefixed keys while diagnosing precedence.

V3 reports **MBT404** if a configured property has neither a matching input nor a non-empty `defaultValue`; it skips that class when values are missing. An invalid URI, number, date, or other typed value can also prevent usable generated code. Use safe defaults only when the missing value is optional.

For v3, a solution-level applicable file stops discovery before project-level files. Put base and configuration/platform overrides together. V2 instead merges files through the solution directory, whose duplicate values may win.

Neither API searches the `dotnet user-secrets` store automatically. Supply the flat JSON inputs or environment variables explicitly.

## Inspect generated source

For v2, look for `*.g.cs` beneath the project's intermediate output directory (`obj` by default).

For the **v3 Roslyn generator**, temporarily add this to the project:

```xml
<PropertyGroup>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)generated</CompilerGeneratedFilesOutputPath>
</PropertyGroup>
```

Rebuild and inspect `obj/generated`. Keep output under `obj` so it is ignored and is not picked up as a second source file on subsequent builds. Do not manually compile or commit these files. The v3 preparation step also writes `obj/.../Mobile.BuildTools/buildtools.env`; this may contain environment values, so do not post it unredacted.

## Do I need dependency injection?

No. The basic AppSettings output is a static generated class; use `AppSettings.PropertyName`. If you want your own interface or options wrapper, register that application abstraction yourself. XML `ConfigurationManager` has its own initialization and registration described in [Using app.config in code](../app.config/in-code.md).

## Can I use lists or shared projects?

`isArray: true` generates an array; convert it to a list in your application if needed. `delimiter` separates string input into array items; use `;` unless you need another single character.

A shared source project is compiled in its host assembly. Configure the host project keys and use `rootNamespace` when you need a consistent generated namespace across hosts. Do not assume a generated class in one compiled assembly automatically appears in another.
