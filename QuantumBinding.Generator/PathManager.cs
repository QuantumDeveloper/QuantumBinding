using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace QuantumBinding.Generator
{
    public class PathManager
    {
        private static readonly OSPlatform currentOS;
        
        private Dictionary<OSPlatform, List<String>> files;
        private Dictionary<OSPlatform, List<String>> includeFolders;

        public PathManager()
        {
            files = new Dictionary<OSPlatform, List<String>>();
            includeFolders = new Dictionary<OSPlatform, List<String>>();
        }

        static PathManager()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                currentOS = OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                currentOS = OSPlatform.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                currentOS = OSPlatform.Linux;
            }
        }

        public String[] Files
        {
            get
            {
                if (!files.TryGetValue(currentOS, out var paths))
                {
                    return new String[0];
                }

                return paths.ToArray();
            }
        }
        
        public String[] IncludeDirectories
        {
            get
            {
                if (!includeFolders.TryGetValue(currentOS, out var paths))
                {
                    return new String[0];
                }

                return paths.ToArray();
            }
        }
        
        public void AddFilePath(OSPlatform platform, String path)
        {
            if (files.ContainsKey(platform))
            {
               files[platform].Add(path); 
            }
            else
            {
                files[platform] = new List<String>() {path};
            }
        }

        public void AddIncludeFolderPath(OSPlatform platform, String path)
        {
            if (includeFolders.ContainsKey(platform))
            {
                files[platform].Add(path);
            }
            else
            {
                includeFolders[platform] = new List<String>() {path};
            }
        }
    }
}