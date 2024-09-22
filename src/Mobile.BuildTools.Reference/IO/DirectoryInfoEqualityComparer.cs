using System;
using System.Collections.Generic;
using System.IO;

namespace Mobile.BuildTools.Reference.IO;

internal class DirectoryInfoEqualityComparer : IEqualityComparer<DirectoryInfo>
{
    public static DirectoryInfoEqualityComparer Instance { get; } = new ();

    public bool Equals(DirectoryInfo x, DirectoryInfo y) =>
        x.FullName.Equals(y.FullName, StringComparison.Ordinal);

    public int GetHashCode(DirectoryInfo obj) =>
        obj.FullName.GetHashCode();
}
