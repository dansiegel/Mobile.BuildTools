Google Firebase is an incredibly popular solution for app analytics and cross platform push notifications. Unfortunately we see a lot of developers following poor programming practices including the `google-services.json` and `GoogleService-Info.plist` in source control. The Mobile.BuildTools is all about empowering you to follow best practices while developing your mobile apps. 

To start be sure to add these files to your `.gitignore` that way the file isn't accidentally checked into source control while you use it for local development. The Mobile.BuildTools is able to bring these resources in at build time using your favorite CI Build service. To get started you'll want to come up with a variable name and set this in your `buildtools.json` like the following:

```json
{
  "google": {
    "servicesJson": "GoogleServicesJson",
    "infoPlist": "GoogleServicesInfoPlist"
  }
}
```

If the variable exists locally or on your CI Server, the Mobile.BuildTools will automatically look it up and determine if it is a file path. If it is it will add the appropriate include for iOS/Android. Otherwise it will take the contents of the variable and add the file to the Intermediate Output Directory (the obj folder) during the build, and it will again add the appropriate platform include.

!!! note
    Due to some build agents, such as all Windows agents on Azure DevOps, performing a ToUpper on all environment variable names, the Mobile.BuildTools will do a Case insensitive lookup to match the variable name.
