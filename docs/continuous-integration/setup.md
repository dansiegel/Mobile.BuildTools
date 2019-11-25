# Continuous Integration Setup

Obviously if we checked in a json file with our secrets it would negate the entire point of trying to secure our code base. The Build Tools include a Build Host Secrets task that executes prior to the Secrets Generation. This task will execute when the secrets json file does not exist in an attempt to generate the missing json file. This is designed to handle secrets across multiple projects. By default we assume you have a single shared project such as a .NET Standard library, and one or more platform projects like iOS, Android, UWP, macOS, & Tizen. To override the secrets prefix for any project you simply need to provide a value for `BuildHostSecretPrefix`

| Platform | Secrets Prefix |
| -------- | -------------- |
| Android | DroidSecret_ |
| iOS | iOSSecret_ |
| UWP | UWPSecret_ |
| macOS | MacSecret_ |
| Tizen | TizenSecret_ |
| Default | Secret_ |