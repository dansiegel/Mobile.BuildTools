The Mobile.BuildTools has an astonishing 858 built in watermark templates that you can choose from. These templates give you 6 different position choices, with 13 different color options, and 11 different keywords like `Dev`, `QA`, `Free`, `Pro` and more!

To use a built in watermark you will need to specify the built in template name as follows `{key-word}-{position}-{color}`. For example you might use `dev-bot-blue`.

```json
{
  "$schema": "http://mobilebuildtools.com/schemas/v2/resourceDefinition.schema.json",
  "watermarkFile": "{key-word}-{position}-{color}"
}
```

## Locations

To keep it simple each of the locations use an abbreviation as shown below.

| Locations | Name |
|-----------|------|
| Top | top |
| Bottom | bot |
| Bottom Left | bl |
| Bottom Right | br |
| Top Left | tl |
| Top Right | tr |

## Colors

| Color | Name |
|----------|------|
| Black | black |
| Blue | blue |
| Green | green |
| Red | red |
| Yellow | yellow |
| Flat Black | flatblack |
| Flat Blue | flatblue |
| Flat Green | flatgreen |
| Flat Orange | flatorange |
| Flat Purple | flatpurple |
| Flat Red | flatred |
| Flat White | flatwhite |
| Flat Yellow | flatyellow |

!!! note
    Flat colors are solid bars of the given color. All other color options have a gradient.

## Key Words

| Key Word | Name | Sample |
|:--------:|:----:|:------:|
| Alpha | alpha | [sample](samples/alpha-samples.md) |
| Beta | beta | [sample](samples/beta-samples.md) |
| Debug | debug | [sample](samples/debug-samples.md) |
| Dev | dev | [sample](samples/dev-samples.md) |
| Free | free | [sample](samples/free-samples.md) |
| Lite | lite | [sample](samples/lite-samples.md) |
| Preview | preview | [sample](samples/preview-samples.md) |
| Pro | pro | [sample](samples/pro-samples.md) |
| QA | qa | [sample](samples/qa-samples.md) |
| Stage | stage | [sample](samples/stage-samples.md) |
| UAT | uat | [sample](samples/uat-samples.md) |
