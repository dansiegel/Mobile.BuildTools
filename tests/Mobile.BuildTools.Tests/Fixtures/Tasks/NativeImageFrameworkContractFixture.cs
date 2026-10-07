using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Tasks;
using Moq;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Tasks;

/// <summary>Runs the real framework NuGet tasks, not a substitute density/catalog generator.</summary>
public sealed class NativeImageFrameworkContractFixture : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), nameof(NativeImageFrameworkContractFixture), Guid.NewGuid().ToString("N"));

    public NativeImageFrameworkContractFixture()
    {
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "logo.svg"), "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"32\" height=\"32\"><rect width=\"32\" height=\"32\" fill=\"red\"/></svg>");
        File.WriteAllText(Path.Combine(directory, "inputs"), "framework-contract");
    }

    [Theory]
    [InlineData("Maui", false)]
    [InlineData("Maui", true)]
    [InlineData("Uno", false)]
    [InlineData("Uno", true)]
    public void AndroidManifestReferencesBothRealGeneratedAdaptiveResources(string framework, bool useAlias)
    {
        var icon = Icon(framework, useAlias);
        RunResizetizer(framework, "android", icon);
        var manifest = Path.Combine(directory, "AndroidManifest.xml");
        File.WriteAllText(manifest, "<manifest><application /></manifest>");
        var task = ManifestTask("android", manifest, icon);
        Assert.True(task.Execute());
        XNamespace android = "http://schemas.android.com/apk/res/android";
        var application = XDocument.Load(task.GeneratedManifest.ItemSpec).Root.Element("application");
        foreach (var attribute in new[] { "icon", "roundIcon" })
        {
            var reference = (string)application.Attribute(android + attribute);
            Assert.StartsWith("@mipmap/", reference);
            var resourceName = reference.Split('/').Last();
            var output = Assert.Single(Directory.GetFiles(directory, resourceName + ".xml", SearchOption.AllDirectories),
                path => Path.GetFileName(Path.GetDirectoryName(path)) == "mipmap-anydpi-v26");
            var adaptive = XDocument.Load(output);
            Assert.Equal("adaptive-icon", adaptive.Root.Name.LocalName);
            foreach (var layer in adaptive.Root.Elements().Where(x => x.Name.LocalName is "background" or "foreground"))
            {
                var layerName = ((string)layer.Attribute(android + "drawable")).Split('/').Last();
                Assert.NotEmpty(Directory.GetFiles(directory, layerName + ".png", SearchOption.AllDirectories));
            }
        }
    }

    [Theory]
    [InlineData("Maui", false)]
    [InlineData("Maui", true)]
    [InlineData("Uno", false)]
    [InlineData("Uno", true)]
    public void AppleManifestSelectsRealGeneratedCatalogAndCatalogImagesExist(string framework, bool useAlias)
    {
        var icon = Icon(framework, useAlias);
        RunResizetizer(framework, "ios", icon);
        var manifest = Path.Combine(directory, "Info.plist");
        File.WriteAllText(manifest, "<plist><dict /></plist>");
        var task = ManifestTask("apple", manifest, icon);
        Assert.True(task.Execute());
        var selector = XDocument.Load(task.GeneratedManifest.ItemSpec).Descendants("string").Single().Value;
        var catalog = Path.Combine(directory, "output", "images", selector.Replace('/', Path.DirectorySeparatorChar));
        using var contents = JsonDocument.Parse(File.ReadAllText(Path.Combine(catalog, "Contents.json")));
        var images = contents.RootElement.GetProperty("images").EnumerateArray().ToArray();
        Assert.NotEmpty(images);
        Assert.All(images.Where(image => image.TryGetProperty("filename", out _)), image =>
            Assert.True(File.Exists(Path.Combine(catalog, image.GetProperty("filename").GetString())), image.ToString()));
    }

    [Theory]
    [InlineData("Maui")]
    [InlineData("Uno")]
    public void WindowsFrameworkManifestReferencesRealIconAndSplashOutputs(string framework)
    {
        var manifest = Path.Combine(directory, "Package.appxmanifest");
        const string original = """
            <Package xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
                     xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10">
              <Identity Name="com.example.contract" Publisher="CN=Contract" Version="1.0.0.0" />
              <Properties><DisplayName>Contract</DisplayName><PublisherDisplayName>Contract</PublisherDisplayName><Logo>$placeholder$.png</Logo></Properties>
              <Applications><Application Id="App" Executable="$targetnametoken$.exe" EntryPoint="$targetentrypoint$">
                <uap:VisualElements DisplayName="Contract" Description="Contract" Square150x150Logo="$placeholder$.png" Square44x44Logo="$placeholder$.png" BackgroundColor="transparent">
                  <uap:DefaultTile Square71x71Logo="$placeholder$.png" Wide310x150Logo="$placeholder$.png" Square310x310Logo="$placeholder$.png" />
                  <uap:SplashScreen Image="$placeholder$.png" />
                </uap:VisualElements>
              </Application></Applications>
            </Package>
            """;
        File.WriteAllText(manifest, original);
        RunResizetizer(framework, "uwp", Icon(framework, false), appx: manifest);
        var generated = XDocument.Load(Path.Combine(directory, "output", "manifest", "Package.appxmanifest"));
        var references = generated.Descendants().SelectMany(element => element.Attributes())
            .Where(attribute => attribute.Name.LocalName.EndsWith("Logo") || attribute.Name.LocalName == "Image")
            .Select(attribute => attribute.Value)
            .Concat(generated.Descendants().Where(element => element.Name.LocalName == "Logo").Select(element => element.Value)).ToArray();
        Assert.True(references.Length >= 7);
        foreach (var reference in references)
        {
            Assert.DoesNotContain("$placeholder$", reference);
            // Appx references omit scale qualifiers; Windows selects the packaged density.
            var name = Path.GetFileNameWithoutExtension(reference.Replace('\\', '/'));
            var outputs = Directory.GetFiles(Path.Combine(directory, "output"), "*.png", SearchOption.AllDirectories);
            Assert.True(outputs.Any(output => Path.GetFileName(output).StartsWith(name, StringComparison.Ordinal)),
                $"No generated image matches '{reference}'. Generated images: {string.Join(", ", outputs.Select(Path.GetFileName))}");
        }
        Assert.Equal(original, File.ReadAllText(manifest));
    }

    [Fact]
    public void UnoPwaIconsReferenceRealOutputsAndBootManifestIsUntouched()
    {
        var pwa = Path.Combine(directory, "manifest.webmanifest");
        const string original = "{\"name\":\"Contract app\",\"icons\":[],\"display\":\"standalone\"}";
        File.WriteAllText(pwa, original);
        var boot = Path.Combine(directory, "AppManifest.js");
        const string bootSource = "var UnoAppManifest = { splashScreenImage: 'custom-splash.png' };";
        File.WriteAllText(boot, bootSource);
        RunResizetizer("Uno", "wasm", Icon("Uno", false), pwa);
        var generated = File.ReadAllText(Path.Combine(directory, "pwa-output.txt")).Trim();
        Assert.True(File.Exists(generated));
        using var manifest = JsonDocument.Parse(File.ReadAllText(generated));
        var icons = manifest.RootElement.GetProperty("icons").EnumerateArray().ToArray();
        Assert.NotEmpty(icons);
        foreach (var icon in icons)
        {
            var source = icon.GetProperty("src").GetString();
            Assert.DoesNotContain("/", source);
            Assert.NotEmpty(Directory.GetFiles(Path.Combine(directory, "output"), source, SearchOption.AllDirectories));
            Assert.Equal("image/png", icon.GetProperty("type").GetString());
        }
        Assert.Equal(original, File.ReadAllText(pwa));
        Assert.Equal(bootSource, File.ReadAllText(boot));
    }

    private TaskItem Icon(string framework, bool alias)
    {
        var icon = new TaskItem(Path.Combine(directory, "logo.svg"));
        icon.SetMetadata("MobileBuildToolsItemType", framework + "Icon");
        icon.SetMetadata("IsAppIcon", "true");
        if (alias)
            icon.SetMetadata("Link", "Assets/branded.svg");
        return icon;
    }

    private GenerateNativeImageManifestTask ManifestTask(string platform, string manifest, TaskItem icon) =>
        new GenerateNativeImageManifestTask { Images = new[] { icon }, Manifest = new TaskItem(manifest), Platform = platform,
            ProjectDirectory = directory, OutputDirectory = "obj/manifests", BuildEngine = new Mock<IBuildEngine>().Object };

    private void RunResizetizer(string framework, string platform, TaskItem icon, string pwa = null, string appx = null)
    {
        var metadata = typeof(NativeImageFrameworkContractFixture).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(item => item.Key == framework + "ResizetizerPackagePath");
        Assert.False(string.IsNullOrWhiteSpace(metadata.Value), "Restore the framework-contract test PackageReferences before running these tests.");
        var package = metadata.Value;
        var targets = Path.Combine(package, framework == "Maui" ? "buildTransitive/Microsoft.Maui.Resizetizer.After.targets" : "build/Uno.Resizetizer.targets");
        var taskNames = XDocument.Load(targets).Descendants().Where(element => element.Name.LocalName == "UsingTask")
            .Select(element => (string)element.Attribute("TaskName"))
            .ToArray();
        var taskName = taskNames.Single(name => name.Contains(".ResizetizeImages"));
        var assembly = framework == "Maui" ? Path.Combine(package, "buildTransitive/Microsoft.Maui.Resizetizer.dll") :
            Directory.GetFiles(Path.Combine(package, "build"), "Uno.Resizetizer_*.dll", SearchOption.AllDirectories).Single();
        var extraParameters = framework == "Uno" ? $"IntermediateOutputIconPath=\"{Escape(Path.Combine(directory, "output", "icons") + Path.DirectorySeparatorChar)}\" PWAManifestPath=\"{Escape(pwa ?? string.Empty)}\"" : string.Empty;
        var extraOutput = framework == "Uno" ? "<Output TaskParameter=\"PwaGeneratedManifestPath\" PropertyName=\"GeneratedPwa\" />" : string.Empty;
        var windowsTasks = string.Empty;
        var windowsImports = string.Empty;
        if (appx != null)
        {
            var splashTask = taskNames.Single(name => name.Contains(".GenerateSplashAssets"));
            var manifestTask = taskNames.Single(name => name.Contains(".GeneratePackageAppxManifest"));
            windowsImports = $"<UsingTask TaskName=\"{splashTask}\" AssemblyFile=\"{Escape(assembly)}\" /><UsingTask TaskName=\"{manifestTask}\" AssemblyFile=\"{Escape(assembly)}\" />";
            File.Copy(icon.ItemSpec, Path.Combine(directory, "splash.svg"));
            var unoParameters = framework == "Uno" ? "TargetFramework=\"windows\" TargetPlatformMinVersion=\"10.0.17763.0\" TargetPlatformVersion=\"10.0.19041.0\" AssemblyName=\"Contract\"" : string.Empty;
            windowsTasks = $"""
                <ItemGroup><Splash Include="{Escape(Path.Combine(directory, "splash.svg"))}" Color="White" /></ItemGroup>
                <{splashTask} IntermediateOutputPath="{Escape(Path.Combine(directory, "output", "splash") + Path.DirectorySeparatorChar)}" {framework}SplashScreen="@(Splash)" />
                <{manifestTask} IntermediateOutputPath="{Escape(Path.Combine(directory, "output", "manifest") + Path.DirectorySeparatorChar)}" AppxManifest="{Escape(appx)}" GeneratedFilename="Package.appxmanifest" AppIcon="@(Icon)" SplashScreen="@(Splash)" ApplicationId="com.example.contract" ApplicationTitle="Contract" ApplicationDisplayVersion="1.0" ApplicationVersion="1" {unoParameters} />
                """;
        }
        var project = Path.Combine(directory, "Contract.proj");
        File.WriteAllText(project, $"""
            <Project>
              <UsingTask TaskName="{taskName}" AssemblyFile="{Escape(assembly)}" />
              {windowsImports}
              <ItemGroup><Icon Include="{Escape(icon.ItemSpec)}" IsAppIcon="true" Link="{Escape(icon.GetMetadata("Link"))}" Color="White" /></ItemGroup>
              <Target Name="Generate">
                <{taskName} Images="@(Icon)" PlatformType="{platform}" IntermediateOutputPath="{Escape(Path.Combine(directory, "output", "images") + Path.DirectorySeparatorChar)}" InputsFile="{Escape(Path.Combine(directory, "inputs"))}" {extraParameters}>
                  {extraOutput}
                </{taskName}>
                {windowsTasks}
                <WriteLinesToFile File="{Escape(Path.Combine(directory, "pwa-output.txt"))}" Lines="$(GeneratedPwa)" Overwrite="true" />
              </Target>
            </Project>
            """);
        // A fresh MSBuild process isolates each framework's own bundled drawing runtime from
        // the test host's Skia version. No dependency/runtime files are replaced or injected.
        var start = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ??
            Path.GetFullPath(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "..", "..", "..",
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "dotnet.exe" : "dotnet")))
        {
            WorkingDirectory = directory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        foreach (var argument in new[] { "msbuild", project, "-t:Generate", "-nologo", "-m:1", "-nodeReuse:false", "-p:UseSharedCompilation=false", "-v:minimal" })
            start.ArgumentList.Add(argument);
        using var process = Process.Start(start);
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(120000))
        {
            process.Kill(true);
            Assert.Fail("Native resizetizer contract build did not finish within two minutes.");
        }
        Assert.True(process.ExitCode == 0, output.GetAwaiter().GetResult() + Environment.NewLine + error.GetAwaiter().GetResult());
    }

    private static string Escape(string value) => SecurityElement.Escape(value);
    public void Dispose() => Directory.Delete(directory, true);
}
