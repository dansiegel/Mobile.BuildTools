# Apple push notification entitlements

Set the push-notification entitlement appropriate for the signing and distribution profile of your iOS app. Current v3 source includes an opt-in `APSProductionEnvironment` task that supplies `aps-environment: production` through the .NET Apple SDK's `CustomEntitlements` input before entitlement compilation. This describes source behavior; it does not announce a v3 release or guarantee that released v2.0.245 contains it.

To opt in for a designated distribution configuration, add this to the app project or its `Directory.Build.props`:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <APSProductionEnvironment>true</APSProductionEnvironment>
</PropertyGroup>
```

Only enable it when production push entitlements match your signing setup. Inspect the final signed app's entitlements as part of your distribution validation. Configuration alone does not replace the SDK's signing and provisioning requirements.
