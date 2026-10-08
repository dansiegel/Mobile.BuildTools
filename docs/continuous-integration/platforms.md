# Build platforms

Mobile.BuildTools integrates with MSBuild. Use an agent with the .NET SDK and platform workloads required by your MAUI, Uno Platform, or other .NET project, and an MBT package compatible with those tools.

Common hosts include GitHub Actions, Azure Pipelines, AppVeyor, and Jenkins. Configuration values must be available to the build process; see [CI setup](setup.md) for environment-variable mapping.

Legacy build-host detection remains in the source for compatibility. Its presence does not indicate that a retired service or platform is supported for new applications.
