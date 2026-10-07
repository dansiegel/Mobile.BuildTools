#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using global::Svg;

namespace Mobile.BuildTools.Drawing.Svg;

// Collects font declarations through public parser/DOM APIs. The parser remains
// responsible for SVG parsing, CSS application, media matching, and resource policy.
// This does not depend on Svg.Custom's private compatibility-style state.
internal static class SvgCssFontSources
{
    internal readonly struct Source
    {
        internal Source(string content, Uri? baseUri)
        {
            Content = content;
            BaseUri = baseUri;
        }

        internal string Content { get; }
        internal Uri? BaseUri { get; }
    }

    internal static IEnumerable<Source> Enumerate(SvgDocument document)
    {
        // Serialize the already parsed DOM, so custom XML entities are not parsed
        // again and the source stylesheet text has the parser's existing semantics.
        var text = new StringBuilder();
        using (var writer = XmlWriter.Create(text, new XmlWriterSettings { OmitXmlDeclaration = true }))
            document.Write(writer);
        var dom = XDocument.Parse(text.ToString());
        var sources = new List<Source>();
        var media = new Dictionary<string, bool>(StringComparer.Ordinal);

        foreach (var instruction in ReadStylesheetInstructions(document.BaseUri))
            AddLinkedSource(instruction, document, dom, sources, media);

        foreach (var element in dom.Descendants())
        {
            if (element.Name.LocalName == "style" && IsStyleCss(element))
            {
                sources.Add(new Source(element.Value, document.BaseUri));
            }
            else if (element.Name.LocalName == "link")
            {
                var rel = (Attribute(element, "rel") ?? string.Empty)
                    .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                if (rel.Any(value => value.Equals("stylesheet", StringComparison.OrdinalIgnoreCase)))
                    AddLinkedSource(element, document, dom, sources, media);
            }
        }

        foreach (var source in sources)
        {
            var chain = new HashSet<string>(StringComparer.Ordinal);
            foreach (var expanded in Expand(source, source.BaseUri, document, dom, media, chain, allowImports: true))
                yield return expanded;
        }
    }

    private static string? Attribute(XElement element, string name) =>
        element.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value;

    private static bool IsStyleCss(XElement element)
    {
        var type = Attribute(element, "type");
        return type is null || type.Split(';')[0].Trim().Equals("text/css", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCss(XElement element)
    {
        var type = Attribute(element, "type");
        return string.IsNullOrWhiteSpace(type) || type.Trim().Equals("text/css", StringComparison.OrdinalIgnoreCase);
    }

    private static void AddLinkedSource(XElement element, SvgDocument document, XDocument dom,
        List<Source> sources, Dictionary<string, bool> media)
    {
        if (!IsCss(element) || !MatchesMedia(Attribute(element, "media"), dom, media))
            return;
        var href = Attribute(element, "href");
        if (TryLoad(href, document.BaseUri, document.BaseUri, document.LoadOptions, out var source))
            sources.Add(source);
    }

    private static List<XElement> ReadStylesheetInstructions(Uri? sourceUri)
    {
        var instructions = new List<XElement>();
        if (sourceUri is not { IsAbsoluteUri: true, IsFile: true })
            return instructions;

        try
        {
            using var file = File.OpenRead(sourceUri.LocalPath);
            using var gzip = sourceUri.LocalPath.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase)
                ? new GZipStream(file, CompressionMode.Decompress, leaveOpen: true)
                : null;
            // Read prolog metadata only. Never resolve a DTD, an external entity,
            // or a network resource during this secondary metadata pass.
            using var reader = XmlReader.Create((Stream?)gzip ?? file, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            });
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                    break;
                if (reader.NodeType == XmlNodeType.ProcessingInstruction && reader.Name.Equals("xml-stylesheet", StringComparison.OrdinalIgnoreCase))
                {
                    // PI pseudo-attributes are literal text; unlike XML attributes,
                    // the upstream loader does not entity-decode their values.
                    var attributes = new XElement("link");
                    foreach (Match match in Regex.Matches(reader.Value,
                                 "(?<name>[^\\s=]+)\\s*=\\s*(?<quote>['\"])(?<value>.*?)\\k<quote>",
                                 RegexOptions.Singleline))
                    {
                        var name = match.Groups["name"].Value;
                        if (name.Equals("href", StringComparison.OrdinalIgnoreCase) ||
                            name.Equals("type", StringComparison.OrdinalIgnoreCase) ||
                            name.Equals("media", StringComparison.OrdinalIgnoreCase))
                            attributes.SetAttributeValue(name.ToLowerInvariant(), match.Groups["value"].Value);
                    }
                    instructions.Add(attributes);
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or XmlException or NotSupportedException)
        {
            // The primary parser has already succeeded; retain any valid prolog
            // instructions read so far and use the parsed DOM for inline styles.
        }
        return instructions;
    }

    private static IEnumerable<Source> Expand(Source source, Uri? policyBaseUri, SvgDocument document,
        XDocument dom, Dictionary<string, bool> media, HashSet<string> chain, bool allowImports)
    {
        foreach (var statement in Statements(source.Content))
        {
            var header = statement.Header.Trim();
            if (IsAtRule(header, "@charset"))
                continue;

            if (IsAtRule(header, "@import"))
            {
                if (allowImports && TryReadImport(header.Substring(7), out var href, out var condition) &&
                    MatchesMedia(condition, dom, media) &&
                    TryLoad(href, source.BaseUri, policyBaseUri, document.LoadOptions, out var imported) &&
                    imported.BaseUri is { } uri && chain.Add(uri.AbsoluteUri))
                {
                    try
                    {
                        foreach (var nested in Expand(imported, policyBaseUri, document, dom, media, chain, true))
                            yield return nested;
                    }
                    finally { chain.Remove(uri.AbsoluteUri); }
                }
                continue;
            }

            allowImports = false;
            if (IsAtRule(header, "@media") && statement.Body is { } body)
            {
                if (MatchesMedia(header.Substring(6), dom, media))
                    foreach (var nested in Expand(new Source(body, source.BaseUri), policyBaseUri, document, dom, media, chain, false))
                        yield return nested;
            }
            else
            {
                yield return new Source(statement.Text, source.BaseUri);
            }
        }
    }

    private static bool MatchesMedia(string? condition, XDocument document, Dictionary<string, bool> cache)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return true;
        condition = condition!.Trim();
        if (cache.TryGetValue(condition, out var matches))
            return matches;
        // A media query cannot contain a rule or import. Keep the probe's CSS
        // self-contained even for malformed source input.
        if (condition.IndexOfAny(new[] { '{', '}', ';', '@' }) >= 0)
            return cache[condition] = false;

        XNamespace ns = "http://www.w3.org/2000/svg";
        var svg = new XElement(ns + "svg");
        if (document.Root is { } root)
            foreach (var attribute in root.Attributes().Where(a => a.Name.LocalName is "width" or "height" or "viewBox"))
                svg.Add(new XAttribute(attribute));
        svg.Add(new XElement(ns + "style", "@media " + condition + " { #mbt_font_media_probe { opacity: 0.314159; } }"));
        svg.Add(new XElement(ns + "rect", new XAttribute("id", "mbt_font_media_probe"),
            new XAttribute("width", "1"), new XAttribute("height", "1")));
        var probe = SvgDocumentCompatibilityLoader.FromSvg<SvgDocument>(svg.ToString(SaveOptions.DisableFormatting));
        matches = probe.Descendants().OfType<SvgRectangle>().Any(rect => Math.Abs(rect.Opacity - 0.314159f) < 0.00001f);
        cache[condition] = matches;
        return matches;
    }

    private static bool TryLoad(string? href, Uri? baseUri, Uri? policyBaseUri,
        SvgDocumentLoadOptions options, out Source source)
    {
        source = default;
        if (string.IsNullOrWhiteSpace(href))
            return false;
        Uri? uri;
        var valid = baseUri is { IsAbsoluteUri: true }
            ? Uri.TryCreate(baseUri, href, out uri)
            : Uri.TryCreate(href, UriKind.Absolute, out uri);
        if (!valid || uri is null || !uri.IsAbsoluteUri)
            return false;

        var data = uri.Scheme.Equals("data", StringComparison.OrdinalIgnoreCase);
        var policy = SvgExternalResourceResolver.GetEffectiveExternalResourcePolicy(options);
        var allowed = policyBaseUri is not null
            ? SvgExternalResourceResolver.AllowsStylesheetResource(uri, policyBaseUri, options, SvgExternalResourcePolicy.SameDocumentAndDataOnly)
            : policy == SvgExternalResourcePolicy.Enabled || (data && policy != SvgExternalResourcePolicy.Disabled);
        if (!allowed || (!uri.IsFile && !data))
            return false;

        try
        {
            if (uri.IsFile)
            {
                source = new Source(File.ReadAllText(uri.LocalPath), uri);
                return true;
            }
            var value = uri.OriginalString;
            var comma = value.IndexOf(',');
            if (comma < 0)
                return false;
            var headers = value.Substring(5, comma - 5).Split(';');
            var mime = headers[0].Contains("/") ? headers[0].Trim() : "text/plain";
            if (!mime.Equals("text/css", StringComparison.OrdinalIgnoreCase) && !mime.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
                return false;
            var charset = headers.FirstOrDefault(h => h.TrimStart().StartsWith("charset=", StringComparison.OrdinalIgnoreCase));
            var encoding = charset is null
                ? (headers[0].Contains("/") ? Encoding.UTF8 : Encoding.ASCII)
                : Encoding.GetEncoding(charset.Substring(charset.IndexOf('=') + 1).Trim());
            var payload = value.Substring(comma + 1);
            var css = headers.Any(h => h.Trim().Equals("base64", StringComparison.OrdinalIgnoreCase))
                ? encoding.GetString(Convert.FromBase64String(payload))
                : Uri.UnescapeDataString(payload);
            source = new Source(css, uri);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException or ArgumentException or FormatException or System.Security.SecurityException)
        {
            return false;
        }
    }

    private static bool IsAtRule(string header, string keyword) =>
        header.StartsWith(keyword, StringComparison.OrdinalIgnoreCase) &&
        (header.Length == keyword.Length ||
         !(char.IsLetterOrDigit(header[keyword.Length]) || header[keyword.Length] is '_' or '-'));

    private static bool TryReadImport(string input, out string href, out string condition)
    {
        href = condition = string.Empty;
        var value = input.Trim();
        var url = value.StartsWith("url", StringComparison.OrdinalIgnoreCase) &&
                  (value.Length == 3 || !(char.IsLetterOrDigit(value[3]) || value[3] is '_' or '-'));
        var index = url ? 3 : 0;
        SkipSpaceAndComments(value, ref index);
        if (url)
        {
            if (index >= value.Length || value[index++] != '(') return false;
            SkipSpaceAndComments(value, ref index);
        }
        if (index >= value.Length)
            return false;
        if (value[index] is '\'' or '"')
        {
            var quote = value[index++];
            var decoded = new StringBuilder();
            while (index < value.Length && value[index] != quote)
            {
                var current = value[index++];
                if (current == '\\')
                {
                    if (index >= value.Length) return false;
                    current = value[index++];
                }
                decoded.Append(current);
            }
            if (index == value.Length) return false;
            index++;
            href = decoded.ToString();
        }
        else if (url)
        {
            var start = index;
            while (index < value.Length && value[index] != ')') index++;
            href = value.Substring(start, index - start).Trim();
        }
        else return false;
        if (url)
        {
            SkipSpaceAndComments(value, ref index);
            if (index >= value.Length || value[index++] != ')') return false;
        }
        condition = value.Substring(index).Trim();
        return href.Length > 0;
    }

    private readonly struct Statement
    {
        internal Statement(string header, string? body, string text) { Header = header; Body = body; Text = text; }
        internal string Header { get; }
        internal string? Body { get; }
        internal string Text { get; }
    }

    private static IEnumerable<Statement> Statements(string css)
    {
        var index = 0;
        while (index < css.Length)
        {
            SkipSpaceAndComments(css, ref index);
            var start = index;
            var parentheses = 0;
            while (index < css.Length)
            {
                var character = css[index];
                if (SkipQuotedOrComment(css, ref index)) continue;
                if (character == '(') parentheses++;
                else if (character == ')') parentheses = Math.Max(0, parentheses - 1);
                else if (parentheses == 0 && (character == ';' || character == '{')) break;
                index++;
            }
            var header = css.Substring(start, index - start);
            if (index == css.Length || css[index] == ';')
            {
                if (index < css.Length) index++;
                if (header.Length > 0) yield return new Statement(header, null, css.Substring(start, index - start));
                continue;
            }
            var bodyStart = ++index;
            var depth = 1;
            while (index < css.Length && depth > 0)
            {
                if (SkipQuotedOrComment(css, ref index)) continue;
                if (css[index] == '{') depth++;
                else if (css[index] == '}') depth--;
                index++;
            }
            var bodyEnd = depth == 0 ? index - 1 : index;
            yield return new Statement(header, css.Substring(bodyStart, bodyEnd - bodyStart), css.Substring(start, index - start));
        }
    }

    private static void SkipSpaceAndComments(string value, ref int index)
    {
        while (index < value.Length)
        {
            if (char.IsWhiteSpace(value[index])) { index++; continue; }
            if (index + 1 < value.Length && value[index] == '/' && value[index + 1] == '*')
            {
                var end = value.IndexOf("*/", index + 2, StringComparison.Ordinal);
                index = end < 0 ? value.Length : end + 2;
                continue;
            }
            break;
        }
    }

    private static bool SkipQuotedOrComment(string value, ref int index)
    {
        if (index + 1 < value.Length && value[index] == '/' && value[index + 1] == '*')
        {
            var end = value.IndexOf("*/", index + 2, StringComparison.Ordinal);
            index = end < 0 ? value.Length : end + 2;
            return true;
        }
        if (value[index] is not ('\'' or '"')) return false;
        var quote = value[index++];
        while (index < value.Length)
        {
            if (value[index++] == quote) break;
            if (value[index - 1] == '\\' && index < value.Length) index++;
        }
        return true;
    }
}
