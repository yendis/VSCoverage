using System.Collections.Generic;
using System.Linq;

namespace VSCoverage.Model
{
    public abstract class Item
    {
        public string Name { get; set; }

        public IList<Item> Items { get; set; } = new List<Item>();

        public Coverage Coverage { get; set; }

        public double Percent => Coverage.CoveragePercentage;

        public int Level { get; set; }
    }
}
