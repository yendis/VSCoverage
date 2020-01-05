using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverage.UnitTests.LCOV
{
    [DebuggerDisplay("{Name} (ln={LineNumber} hit={Hit})")]
    public class Function
    {
        public string Name { get; set; }

        public int LineNumber { get; set; }

        public bool Hit { get; set; }
    }
}
