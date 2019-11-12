using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Mobile.BuildTools
{
    [JsonConverter(typeof(VersionInfoConverter))]
    public class VersionInfo : IComparable<VersionInfo>, IEquatable<VersionInfo>
    {

        //--- Class Methods ---
        public static VersionInfo Parse(string text)
        {
            var index = text.IndexOf('-');
            if (index < 0)
            {
                return new VersionInfo(Version.Parse(text), "");
            }
            else
            {
                return new VersionInfo(Version.Parse(text.Substring(0, index)), text.Substring(index).TrimEnd('*'));
            }
        }

        public static bool TryParse(string text, out VersionInfo version)
        {
            try
            {
                version = Parse(text);
                return true;
            }
            catch
            {
                version = null;
                return false;
            }
        }

        public static bool operator <(VersionInfo left, VersionInfo right)
            => left.CompareTo(right) < 0;

        public static bool operator >(VersionInfo left, VersionInfo right)
            => left.CompareTo(right) > 0;

        public static bool operator ==(VersionInfo left, VersionInfo right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(left, null))
            {
                return false;
            }
            if (ReferenceEquals(right, null))
            {
                return false;
            }
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(VersionInfo left, VersionInfo right)
        {
            if (ReferenceEquals(left, right))
            {
                return false;
            }
            if (ReferenceEquals(left, null))
            {
                return true;
            }
            if (ReferenceEquals(right, null))
            {
                return true;
            }
            return left.CompareTo(right) != 0;
        }

        //--- Fields ---
        public readonly Version Version;
        public readonly string Suffix;

        //--- Constructors ---
        private VersionInfo(Version version, string suffix)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Suffix = suffix ?? throw new ArgumentNullException(nameof(suffix));
        }

        //--- Properties ---
        public int Major => Version.Major;
        public int Minor => Version.Minor;
        public bool IsPreRelease => Suffix.Length > 0;

        //--- Methods ---
        public override string ToString() => Version.ToString() + Suffix;

        public bool Equals(VersionInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals(obj as VersionInfo);
        }

        public override int GetHashCode()
            => Version.GetHashCode() ^ Suffix.GetHashCode();

        public int CompareTo(VersionInfo other)
        {
            if (ReferenceEquals(other, null))
            {
                throw new ArgumentNullException(nameof(other));
            }
            var result = Version.CompareTo(other.Version);
            if (result != 0)
            {
                return result;
            }

            // a suffix indicates a pre-release version, but cannot be compared otherwise
            if ((Suffix == null) && (other.Suffix != null))
            {
                return 1;
            }
            if ((Suffix != null) && (other.Suffix == null))
            {
                return -1;
            }
            return 0;
        }

        public bool IsCompatibleWith(VersionInfo other)
        {
            if (Suffix != other.Suffix)
            {
                return false;
            }
            if (Major != other.Major)
            {
                return false;
            }
            if (Version.Major != 0)
            {
                return Minor == other.Minor;
            }

            // when Major version is 0, we rely on Minor and Build to match
            return ((Minor == other.Minor) && (Math.Max(0, Version.Build) == Math.Max(0, other.Version.Build)));
        }

        public string GetWildcardVersion()
        {
            if (IsPreRelease)
            {
                // NOTE (2018-12-16, bjorg): for pre-release version, there is no wildcard; the version must match everything
                return ToString();
            }
            if (Major == 0)
            {
                // when Major version is 0, the build number is relevant
                return $"{Major}.{Minor}.{Math.Max(0, Version.Build)}.*";
            }
            return $"{Major}.{Minor}.*";
        }

        public VersionInfo GetCompatibleBaseVersion()
        {
            if (IsPreRelease)
            {
                // NOTE (2019-02-19, bjorg): for pre-release version, the base version is this version
                return this;
            }
            if ((Major == 0) && (Version.Build > 0))
            {
                // when Major version is 0, the build number is relevant
                return new VersionInfo(new Version(Major, Minor, Version.Build), suffix: "");
            }
            return new VersionInfo(new Version(Major, Minor), suffix: "");
        }
    }

    public class VersionInfoConverter : JsonConverter
    {

        //--- Methods ---
        public override bool CanConvert(Type objectType)
            => objectType == typeof(VersionInfo);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => (reader.Value != null)
                ? VersionInfo.Parse((string)reader.Value)
                : null;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString());
    }
}
