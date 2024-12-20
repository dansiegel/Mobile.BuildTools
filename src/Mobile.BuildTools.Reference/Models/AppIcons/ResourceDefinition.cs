﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class ResourceDefinition : PlatformConfiguration
    {
        [JsonPropertyName("$schema")]
        public string Schema
        {
            get => "http://mobilebuildtools.com/schemas/v2/resourceDefinition.schema.json";
            set { }
        }

        [Description("Specifies the Android specific configuration for the source image.")]
        [JsonPropertyName("android")]
        public PlatformConfiguration Android { get; set; }

        [Description("Specifies the configuration for all Apple targets if building for iOS, macOS, TVOS, WatchOS.")]
        [JsonPropertyName("apple")]
        public PlatformConfiguration Apple { get; set; }

        [Description("Specifies the iOS specific configuration for the source image.")]
        [JsonPropertyName("ios")]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Properly named iOS")]
        public PlatformConfiguration iOS { get; set; }

        [Description("Specifies the TVOS specific configuration for the source image.")]
        [JsonPropertyName("tvos")]
        public PlatformConfiguration TVOS { get; set; }

        [Description("Specifies the macOS specific configuration for the source image.")]
        [JsonPropertyName("macos")]
        public PlatformConfiguration MacOS { get; set; }

        [Description("Specifies the Tizen specific configuration for the source image.")]
        [JsonPropertyName("tizen")]
        public PlatformConfiguration Tizen { get; set; }

        [Description("Specifies the UWP specific configuration for the source image.")]
        [JsonPropertyName("uwp")]
        public PlatformConfiguration UWP { get; set; }

        [Description("Specifies the Windows specific configuration for the source image.")]
        [JsonPropertyName("windows")]
        public PlatformConfiguration Windows { get; set; }

        public IEnumerable<IImageResource> GetConfigurations(Platform platform)
        {
            var plats = platform switch
            {
                Platform.Android => [Android],
                Platform.iOS => [iOS, Apple],
                Platform.macOS => [MacOS, Apple],
                Platform.TVOS => [TVOS, Apple],
                Platform.Tizen => [Tizen],
                Platform.Windows => [Windows],
                Platform.UWP => [UWP],
                _ => Array.Empty<IImageResource>(),
            };
            plats = plats.Where(x => x != null).ToArray();

            var configs = new List<IImageResource> { GetConfiguration(this, plats) };

            var additional = platform switch
            {
                Platform.Android => Android?.AdditionalOutputs,
                Platform.iOS => iOS?.AdditionalOutputs ?? Apple?.AdditionalOutputs,
                Platform.macOS => MacOS?.AdditionalOutputs ?? Apple.AdditionalOutputs,
                Platform.TVOS => TVOS?.AdditionalOutputs ?? Apple.AdditionalOutputs,
                Platform.Tizen => Tizen?.AdditionalOutputs,
                Platform.Windows => Windows?.AdditionalOutputs,
                Platform.UWP => UWP?.AdditionalOutputs,
                _ => null
            } ?? AdditionalOutputs;

            if(additional != null && additional.Count > 0)
            {
                foreach(var additionalOutput in additional)
                {
                    if (additionalOutput is null)
                        continue;

                    configs.Add(GetConfiguration(additionalOutput));
                }
            }

            if (platform == Platform.Android)
            {
                foreach (var config in configs.Where(x => x.ResourceType != PlatformResourceType.Mipmap && x.ResourceType != PlatformResourceType.Drawable).Cast<IUpdatableImageResource>())
                {
                    config.ResourceType = PlatformResourceType.Drawable;
                }
            }

            return configs;
        }

        private IImageResource GetConfiguration(IImageResource xPlat, params IImageResource[] platforms)
        {
            var resourceType = GetValue(xPlat, c => c.ResourceType, x => true, platforms);
            var config = new ImageResource
            {
                BackgroundColor = GetValue(xPlat, c => c.BackgroundColor, x => !string.IsNullOrEmpty(x), platforms),
                Height = GetValue(xPlat, c => c.Height, x => x.HasValue && x > 0, platforms),
                Ignore = GetValue(xPlat, c => c.Ignore, x => x, platforms),
                Name = GetValue(xPlat, c => c.Name, x => !string.IsNullOrEmpty(x), platforms),
                PaddingColor = GetValue(xPlat, c => c.PaddingColor, x => !string.IsNullOrEmpty(x), platforms),
                PaddingFactor = GetValue(xPlat, c => c.PaddingFactor, x => x.HasValue && x > 0, platforms),
                ResourceType = GetValue(xPlat, c => c.ResourceType, x => true, platforms),
                Scale = GetValue(xPlat, c => c.Scale, x => x > 0, platforms),
                Watermark = GetValue(xPlat, c => c.Watermark, x => !string.IsNullOrEmpty(x?.SourceFile) || !string.IsNullOrEmpty(x?.Text), platforms),
                Width = GetValue(xPlat, c => c.Width, x => x.HasValue && x > 0, platforms),
                SourceFile = SourceFile
            };

            if(string.IsNullOrEmpty(config.Name))
            {
                config.Name = Path.GetFileNameWithoutExtension(SourceFile);
            }

            return config;
        }

        private T GetValue<T>(IImageResource xPlat, Func<IImageResource, T> locator, Func<T, bool> predicate, params IImageResource[] platforms)
        {
            var defaultValue = locator(xPlat);
            if (platforms is null || platforms.Length == 0)
                return defaultValue;

            foreach(var plat in platforms)
            {
                if (plat is null)
                    continue;

                var value = locator(plat);
                if (predicate(value))
                    return value;
            }

            return defaultValue;
        }
    }
}
