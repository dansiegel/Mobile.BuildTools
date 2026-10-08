# Frequently asked questions

## Are generated settings confidential?

No. AppSettings helps keep environment values out of handwritten source and selects values at build time. The result is compiled into your client application and can be extracted. Keep confidential service credentials on your backend. See [CI input handling](continuous-integration/setup.md).

## Which configuration API should I use?

[AppSettings and XML ConfigurationManager](config/index.md) have different inputs and consumption APIs. AppSettings generates strongly typed members; ConfigurationManager reads XML string values and can perform runtime transforms of bundled files. Installing one does not configure the other.

## Why is no AppSettings class visible?

Follow the [complete walkthrough](config/appsettings/index.md) and [troubleshooting guide](config/appsettings/faq.md). The project key, package placement, generated namespace, accessibility, and successful build all matter. Editor completion and Solution Explorer do not necessarily show generated source.

## Do all tasks run on every build?

Tasks evaluate configuration, target platform, and available inputs before running. A task can also be disabled in `buildtools.json`. Compatibility code and retained target names do not guarantee support for every current SDK. Test the configured feature with your installed MBT version.

## Where are v1 instructions?

V1 is no longer maintained. Historical documentation is in the [GitHub wiki](https://github.com/dansiegel/Mobile.BuildTools/wiki). For migration to AppSettings, see [Upgrading from v1](appendix/upgrade.md). Released v2.0.245 and forthcoming v3 source are distinguished in the current guides.
