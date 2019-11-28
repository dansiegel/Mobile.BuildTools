### XAML (preferred)

```xml
<ContentPage x:Class="...">
  <ContentPage.Resources>
    <StyleSheet Source="appresources/style.css" />
  </ContentPage.Resources>
</ContentPage>
```

the `Source` argument takes an Uri relative to the current xaml control, or relative to the application root if it starts with a `/`. The `style.css` has to be an EmbeddedResource.

alternatively, you can inline your style in a `CDATA` Section

```xml
<ContentPage x:Class="...">
  <ContentPage.Resources>
    <StyleSheet>
<![CDATA[
^contentpage {
    background-color: orange;
    padding: 20;
}

stacklayout > * {
    margin: 3;
}
]]>
    </StyleSheet>
  </ContentPage.Resources>
</ContentPage>
```

do not abuse of that second syntax.

### in C#

From an embedded resource:

```c#
myPage.Resources.Add(StyleSheet.FromAssemblyResource(this.GetType().Assembly, "resource.id.of.the.css"));
```

or from a TextReader:

```c#
using (var reader = new StringReader(my_css_string))
    myPage.Resources.Add(StyleSheet.FromReader(reader));
```

## StyleSheet, XamlC and other potential optimizations

At this time, CSS StyleSheets are parsed and evaluated at runtime. That aren't compiled. Every time a StyleSheet is used, it's reparsed again. If parsing time is an issue, enabling caching is trivial, but comes at memory cost.