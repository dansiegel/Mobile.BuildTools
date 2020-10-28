using System;
using System.Text;

namespace Mobile.BuildTools.Utils
{
    public sealed class CodeWriter : IDisposable
    {
        private const string Indent = "    ";

        private int _indentLevel = 0;
        private readonly StringBuilder _outputCode = new StringBuilder();
        private readonly StringBuilder _safeCode = new StringBuilder();

        public IDisposable Block(string value, string safeValue = null)
        {
            AppendLine(value, safeValue);
            AppendLine("{");
            _indentLevel++;
            return this;
        }

        public void AppendAttribute(string value, string safeValue = null)
        {
            _indentLevel++;
            AppendLine(value, safeValue);
            _indentLevel--;
        }

        public void Append(string value, string safeValue = null)
        {
            _outputCode.Append(GetIndentedValue(value));
            _safeCode.Append(GetIndentedValue(safeValue ?? value));
        }

        public void AppendLine()
        {
            _outputCode.AppendLine();
            _safeCode.AppendLine();
        }

        public void AppendLine(string value, string safeValue = null)
        {
            _outputCode.AppendLine(GetIndentedValue(value));
            _safeCode.AppendLine(GetIndentedValue(safeValue ?? value));
        }

        private string GetIndentedValue(string value)
        {
            var indent = string.Empty;
            for (var i = 0; i < _indentLevel; i++)
                indent += Indent;

            return indent + value;
        }

        public void Dispose()
        {
            if (_indentLevel > 0)
            {
                _indentLevel--;
                _outputCode.AppendLine(GetIndentedValue("}"));
                _safeCode.AppendLine(GetIndentedValue("}"));
            }
        }

        public string SafeOutput => _safeCode.ToString();

        public override string ToString()
        {
            while (_indentLevel > 0)
                Dispose();

            return _outputCode.ToString();
        }
    }
}
