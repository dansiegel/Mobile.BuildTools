using System.Collections.Generic;

namespace Mobile.BuildTools.LibSass
{
    /// <summary>
    /// Options used by <see cref="Scss.ConvertToCss"/> and <see cref="Scss.ConvertFileToCss"/>.
    /// </summary>
    public class ScssOptions
    {
        /// <summary>
        /// Delegates that tries to import the specified file.
        /// </summary>
        /// <param name="file">The file to import.</param>
        /// <param name="parentPath">The path of the parent file that is trying to import <paramref name="file"/>.</param>
        /// <param name="scss">The output scss if import was found.</param>
        /// <param name="map">The output map if import was found. May be null</param>
        /// <returns><c>true</c> if import was found; <c>false</c> otherwise</returns>
        public delegate bool TryImportDelegate(string file, string parentPath, out string scss, out string map);

        /// <summary>
        /// Initializes a new instance of the <see cref="ScssOptions"/> class.
        /// </summary>
        public ScssOptions()
        {
            //Precision = 5;
            OutputStyle = ScssOutputStyle.Nested;
            IncludePaths = new List<string>();
            Indent = "  ";
            Linefeed = "\n";
        }

        ///// <summary>
        ///// Gets or sets the maximum number of digits after the decimal. Default is 5. 
        ///// TODO: the C function is not working
        ///// </summary>
        //public int Precision { get; set; }

        /// <summary>
        /// Gets or sets the output style. Default is <see cref="ScssOutputStyle.Nested"/>
        /// </summary>
        public ScssOutputStyle OutputStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to generate source map (result in <see cref="ScssResult.SourceMap"/>
        /// </summary>
        /// <remarks>
        /// Note that <see cref="OutputFile"/> should be setup. <see cref="SourceMapFile"/> will then automatically
        /// map to <see cref="OutputFile"/> + ".map" unless specified.
        /// </remarks>
        public bool GenerateSourceMap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable additional debugging information in the output file as CSS comments. Default is <c>false</c>
        /// </summary>
        public bool SourceComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to embed the source map as a data URI. Default is <c>false</c>
        /// </summary>
        public bool SourceMapEmbed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include the contents in the source map information. Default is <c>false</c>
        /// </summary>
        public bool SourceMapContents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable the inclusion of source map information in the output file. Default is <c>false</c>
        /// </summary>
        /// <remarks>
        /// If this is set to <c>true</c>, the <see cref="OutputFile"/> must be setup to avoid unexpected behavior.
        /// </remarks>
        public bool OmitSourceMapUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the scss content to transform is using indented syntax.
        /// </summary>
        public bool IsIndentedSyntaxSource { get; set; }

        /// <summary>
        /// Gets or sets the indent string. Default is 2 spaces.
        /// </summary>
        public string Indent { get; set; }

        /// <summary>
        /// Gets or sets the linefeed. Default is LF (\n)
        /// </summary>
        public string Linefeed { get; set; }

        /// <summary>
        /// Gets or sets the name of the input file. See remarks for more information.
        /// </summary>
        /// <remarks>
        /// This is recommended when <see cref="GenerateSourceMap"/> so that they can properly refer back to their intended files.
        /// Note also that this is not used to load the data from a file. Use <see cref="Scss.ConvertFileToCss"/> instead.
        /// </remarks>
        public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets the location of the output file. This is recommended when <see cref="GenerateSourceMap"/> so that they can properly refer back to their intended files.
        /// </summary>
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the intended location of the source map file. Note that is used when <see cref="GenerateSourceMap"/> is set. By default, if this property is not set,
        /// the <see cref="OutputFile"/> + ".map" extension will be used. Default is <c>null</c>
        /// </summary>
        public string SourceMapFile { get; set; }

        /// <summary>
        /// Gets or sets the value that will be emitted as sourceRoot in the source map information. Default is null.
        /// </summary>
        public string SourceMapRoot { get; set; }

        /// <summary>
        /// Gets the include paths that will be used to search for @import directives in scss content.
        /// </summary>
        public List<string> IncludePaths { get; }

        /// <summary>
        /// Gets or sets a dynamic delegate used to resolve imports dynamically.
        /// </summary>
        public TryImportDelegate TryImport { get; set; }
    }
}
