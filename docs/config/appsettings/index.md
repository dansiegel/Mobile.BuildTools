Application Settings is a completely rewritten API from the legacy Secrets API. This is the evolution of the Secrets API and has an updated name to reflect the real intent as there are many time you are not actually protecting your codebase from actual application secrets as much as simply keeping configuration something that gets injected at build rather than being hard coded.

!!! note
    If you were using a 2.0 preview, the BuildTools will attempt to migrate your configuration over to using the Application Settings

## What are AppSettings?

We all have settings that help make our app run whether in development, staging, or production. Some examples might be Client ID's, Backend Uri's, OAuth Scopes we need. When you have any of these sorts of things you can have the Mobile.BuildTools automatically generate one or more classes in your project.

## Using the appsettings.json

The Mobile.BuildTools is smart, we look for the appsettings.json in the project and recurse up to the solution directory. This allows you to provide a single file for your entire solution while providing values that can be used through the solution.

```json
{
  "AppCenterAppId": "{your app id}",
  "BackendUri": "https://someapp.azurewebsites.net"
}
```

In addition to the core appsettings.json you can additionally provide an `appsettings.Debug.json` to override values for Debug and similarly with any other Build configuration that you may have following the format `appsettings.{Configuration}.json`.

!!! note
    If upgrading to 2.0 and you are using a secrets.json, the secrets.json will continue to work with a build warning. You should convert the file over to `appsettings.json` as support for secrets.json will be deprecated in a future release.

## Variable priority

The Mobile.BuildTools attempts to locate the values for your generated class through several sources. In the event that a variable key is duplicated, the Mobile.BuildTools has a precedence that the last one in wins. Variables are loaded from the following sources

1. buildtools.json Environment Defaults
1. buildtools.json Environment Configuration (i.e. Debug, Release)
1. System Environment
1. Recursively load legacy `secrets.json` from the Project directory to the Solution directory
1. Recursively load `appsettings.json` from the Project directory to the Solution directory

!!! note
    This variable gathering is additionally used for replacing [Manifest tokens](../../manifests/index.md)
