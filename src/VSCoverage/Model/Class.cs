using System.Diagnostics;

namespace VSCoverage.Model
{
    [DebuggerDisplay("Class: {Name}")]
    public class Class : Item
    {
        public string FullPath { get; set; }
    }
}
