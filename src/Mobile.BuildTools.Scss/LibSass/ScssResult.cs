using System.Collections.Generic;

namespace Mobile.BuildTools.LibSass
{
    /// <summary>
    /// The result of CSS rendering by <see cref="Scss.ConvertToCss"/> and <see cref="Scss.ConvertFileToCss"/>.
    /// </summary>
    public struct ScssResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScssResult" /> struct.
        /// </summary>
        /// <param name="css">The Css.</param>
        /// <param name="sourceMap">The source SourceMap.</param>
        /// <param name="includedFiles">The included files.</param>
        public ScssResult(string css, string sourceMap, List<string> includedFiles)
        {
            Css = css;
            SourceMap = sourceMap;
            IncludedFiles = includedFiles;
        }

        /// <summary>
        /// Gets the generated CSS.
        /// </summary>
        public string Css { get; }

        /// <summary>
        /// Gets the source map (may be null if <see cref="ScssOptions.GenerateSourceMap"/> was <c>false</c>.
        /// </summary>
        public string SourceMap { get; }

        /// <summary>
        /// Gets the included files used to generate this result when converting the input scss content.
        /// </summary>
        public List<string> IncludedFiles { get; }
    }
}
