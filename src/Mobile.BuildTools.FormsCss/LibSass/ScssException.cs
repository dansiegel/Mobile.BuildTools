using System;

namespace Mobile.BuildTools.LibSass
{
    /// <summary>
    /// Exception used by <see cref="Scss"/> to report errors from libsass.
    /// </summary>
    [Serializable]
    public sealed class ScssException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScssException" /> class.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        /// <param name="file">The file.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errorText">The error text.</param>
        public ScssException(int line, int column, string file, string message, string errorText) : base(message)
        {
            Line = line;
            Column = column;
            File = file;
            ErrorText = errorText;
        }

        /// <summary>
        /// Gets the line the exception occured.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the column the exception occured.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets the file the exception occured.
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Gets a short error message describing the error. For a full message use the <see cref="Exception.Message"/> property.
        /// </summary>
        public string ErrorText { get; set; }

        private ScssException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
