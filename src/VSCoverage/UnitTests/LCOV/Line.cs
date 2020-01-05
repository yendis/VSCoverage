using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverage.UnitTests.LCOV
{
    [DebuggerDisplay("{LineNumber} ({Hit})")]
    public class Line
    {
        public int LineNumber { get; set; }

        public bool Hit { get; set; }
    }
}
