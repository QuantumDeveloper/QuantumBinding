using System.IO;

namespace QuantumBinding.Generator
{
    public class FileLocation
    {
        public string FileName { get; set; }

        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);

        public uint LineNumber { get; set; }

        public uint Column { get; set; }

        public override string ToString()
        {
            return $"File: {FileName} Line: {LineNumber} Column: {Column}";
        }
    }
}