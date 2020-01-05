using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverage.UnitTests.LCOV
{
    [DebuggerDisplay("{FilePath}")]
    public class FileCoverage
    {
        public string FilePath { get; set; }

        public string Title { get; internal set; }

        public IList<Function> Functions { get; set; } = new List<Function>();

        public IList<Line> Lines { get; set; } = new List<Line>();

        public IList<Branch> Branches { get; set; } = new List<Branch>();
    }
}
