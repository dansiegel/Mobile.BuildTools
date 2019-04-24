using System.Collections;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable IDE0040 // Add accessibility modifiers
namespace Mobile.BuildTools.Configuration
{
    public class NameValueCollection : IReadOnlyList<KeyValuePair<string, string>>
    {
        private IList<KeyValuePair<string, string>> _source { get; }

        internal NameValueCollection(IList<KeyValuePair<string, string>> source)
        {
            _source = source;
        }

        public IEnumerable<string> AllKeys => this.Select(pair => pair.Key);

        public string this[string name] => _source.SingleOrDefault(kv => kv.Key.Equals(name)).Value;

        public int Count => _source.Count;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _source.GetEnumerator();

        public KeyValuePair<string, string> this[int index] => _source[index];
    }
}
#pragma warning restore IDE0040 // Add accessibility modifiers
