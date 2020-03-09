namespace Mobile.BuildTools.LibSass
{
    /// <summary>
    /// Determines the output format of the final CSS style used by <see cref="Scss.ConvertToCss"/> and <see cref="Scss.ConvertFileToCss"/>.
    /// </summary>
    public enum ScssOutputStyle
    {
        /// <summary>
        /// Nested format.
        /// </summary>
        Nested = 0,

        /// <summary>
        /// Expanded format.
        /// </summary>
        Expanded = 1,

        /// <summary>
        /// Compact format.
        /// </summary>
        Compact = 2,

        /// <summary>
        /// Compressed format.
        /// </summary>
        Compressed = 3,

        /// <summary>
        /// TODO: No documentation.
        /// </summary>
        Inspect = 4,

        /// <summary>
        /// TODO: No documentation.
        /// </summary>
        Sass = 5
    }
}
