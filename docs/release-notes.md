# Release Notes

Generating Release notes can be painful. The Mobile.BuildTools will help solve this problem in v2.X with the Release Notes Task. For this we will generate a ReleaseNotes.txt automatically for you. The configuration options are shown here with their default values.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "releaseNotes": {
    "maxDays": 7,
    "maxCommit": 10,
    "characterLimit": 250,
    "filename": "ReleaseNotes.txt",
    "createInRoot": true,
    "disable": false
  }
}
```