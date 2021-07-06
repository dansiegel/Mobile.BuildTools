# App Configuration for Mobile Projects

Mobile Apps unlike traditional Desktop and Web projects are limiting for developers because you must define configuration values at build and not on deployment. The Mobile.BuildTools is here to help you solve this problem and meet you where you are.

Developers have different needs at different times. For v1.X, the Mobile.BuildTools took the very opinionated idea that configuration values and app secrets should be treaded as something that should be strongly typed. This has a few advantages that come from errors surfacing at build time rather than runtime.

Sometimes though it may be more desireable to perform quick swaps from one environment to another where you are certain that you are running the same exact tested binary build as you have previously. For this reason starting with v2.0 you will have support for using an app.config to provide you the same sort of configurations support that you may be used to from Desktop development or from the web.config variant with ASP.NET development.

## See Also

- Using [app.config](app.config/index.md)
- Using [App Settings](appsettings/index.md)
