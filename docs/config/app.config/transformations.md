# Transformations

A basic app config may look like:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="foo" value="my foo" />
    <add key="bar" value="my bar" />
  </appSettings>
  <connectionStrings>
    <add name="test" providerName="my provider" connectionString="my connection string"/>
  </connectionStrings>
</configuration>
```

A Transformation config may look like:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
  <appSettings>
    <add key="foo" value="transformed" xdt:Transform="Replace" xdt:Locator="Match(key)"/>
    <add key="Environment" value="Dev" xdt:Transform="Insert "/>
  </appSettings>
</configuration>
```

After running the transform from either the automatic build task, at runtime or with the .NET CLI Tool the resulting app.config will look like:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="foo" value="transformed" />
    <add key="bar" value="my bar" />
    <add key="Environment" value="Dev" />
  </appSettings>
  <connectionStrings>
    <add name="test" providerName="my provider" connectionString="my connection string"/>
  </connectionStrings>
</configuration>
```

## XDT Transformations

While the XDT namespace allows you to radically change your app.config. In most cases however you will only need to focus on the xdt:Transform attribute.

- Insert
- InsertBefore(XPath expression)
- InsertAfter(XPath expression)
- Remove
- Remove All

To find out more about the available transformations, see the [microsoft documentation](https://learn.microsoft.com/en-us/previous-versions/aspnet/dd465326(v=vs.110)).

### Replace

This transformation requires the `xdt:Locator` attribute to function correctly. In most cases this will be a simple match on the key attribute:

```xml
<add key="foo" value="transformed" xdt:Transform="Replace" xdt:Locator="Match(key)"/>
```

See the [documentation](https://learn.microsoft.com/en-us/previous-versions/aspnet/dd465326(v=vs.110)#locator-attribute-syntax) for all the ways in which the `xdt:Locator` attribute can be used.