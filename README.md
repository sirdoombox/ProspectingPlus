# ProspectingPlus

A Vintage Story mod which replaces the largely useless client-side only text printouts you get from prospecting into a toggleable and filterable map overlay that is synced with the server, this helps prevent the need for mutliple people to prospect the same chunks multiple times and make notes to be efficient.

## Building Locally
The project is largely setup to be pulled, opened and built, the only requirement is for you to create a `paths.csproj` file. It should be included right alongside the `ProspectingPlus.sln` file in the solution root.
The format for this is as follows...
```xml
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <VINTAGE_STORY>C:\Path\To\Your\Vintagestory\Install</VINTAGE_STORY>
        <VINTAGE_STORY_DATA>C:\Path\To\Your\Vintagestory\Data</VINTAGE_STORY_DATA>
    </PropertyGroup>
</Project>
```
As a side note I recommend a separate install & data path for development purposes.
