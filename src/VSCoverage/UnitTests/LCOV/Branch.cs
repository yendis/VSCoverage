using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverage.UnitTests.LCOV
{
    public class Branch
    {
        public int LineNumber { get; set; }

        public int Block { get; set; }

        public int Branch_ { get; set; }

        public int Taken { get; set; }
    }
}
