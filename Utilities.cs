using System;
using System.IO;

namespace WrightCover
{
    public static class Utilities
    {
        public static string GetAssemblyDirectory => Path.GetDirectoryName(new Uri(typeof(MainWindowPackage).Assembly.CodeBase, UriKind.Absolute).LocalPath);
    }
}
