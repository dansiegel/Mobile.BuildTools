using System.Linq;

namespace Mobile.BuildTools.Handlers
{
    internal abstract class NumericValueHandler : IValueHandler
    {
        private char _requiredSuffix { get; }

        protected NumericValueHandler(char requiredSuffix)
        {
            _requiredSuffix = requiredSuffix;
        }

        public string Format(string rawValue)
        {
            var c = rawValue.Last();
            if (char.IsLetter(c) && char.ToLowerInvariant(c).Equals(char.ToLowerInvariant(_requiredSuffix)))
                return rawValue;

            return rawValue + char.ToUpperInvariant(_requiredSuffix);
        }
    }
}
