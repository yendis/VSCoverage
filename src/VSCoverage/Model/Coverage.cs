namespace VSCoverage.Model
{
    public struct Coverage
    {
        public int TotalLines { get; set; }

        public int CoveredLines { get; set; }

        public int TotalBranches { get; set; }

        public int CoveredBranches { get; set; }

        public Coverage(int totalLines, int coveredLines, int totalBranches, int coveredBranches)
        {
            TotalLines = totalLines;
            CoveredLines = coveredLines;
            TotalBranches = totalBranches;
            CoveredBranches = coveredBranches;
        }

        public int CoveragePercentage
        {
            get
            {
                if (TotalLines == 0 && TotalBranches == 0)
                    return 0;

                return (int)(100M / (TotalLines + TotalBranches) * (CoveredLines + CoveredBranches));
            }
        }

        public static Coverage operator +(Coverage a, Coverage b)
        {
            return new Coverage(
                a.TotalLines + b.TotalLines,
                a.CoveredLines + b.CoveredLines,
                a.TotalBranches + b.TotalBranches,
                a.CoveredBranches + b.CoveredBranches
            );
        }
    }
}
