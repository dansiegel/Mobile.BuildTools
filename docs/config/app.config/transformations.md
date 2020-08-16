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
    <add key="foo" value="transformed" xdt:Transform="Replace"/>
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

While the XDT namespace allows you to radically change your app.config. In most cases however you will only need to focus on the xdt:Transform attribute.

- Replace
- Insert
- InsertBefore(XPath expression)
- InsertAfter(XPath expression)
- Remove
- Remove All