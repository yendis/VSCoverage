using System;
using System.Collections.Generic;

namespace VSCoverage.UnitTests.LCOV
{
    internal static class Parser
    {
        public static IList<FileCoverage> Parse(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
                return new List<FileCoverage>();

            var data = System.IO.File.ReadAllLines(fileName);

            var result = new List<FileCoverage>();
            FileCoverage file = null;
            Function function = null;
            for (var i = 0; i < data.Length; i++)
            {
                var split = data[i].Split(new[] { ':' }, 2);
                switch (split[0])
                {
                    case "TN":
                        //file.Title = split[1];
                        break;
                    case "SF":
                        file = new FileCoverage
                        {
                            FilePath = split[1]
                        };
                        result.Add(file);
                        break;
                    case "FNF":
                        // Functions found
                        break;
                    case "FNH":
                        // Functions hit
                        break;
                    case "LF":
                        // Lines found
                        break;
                    case "LH":
                        // Lines hit
                        break;
                    case "DA":
                        var lineInfo = split[1].Split(',');
                        file.Lines.Add(new Line
                        {
                            LineNumber = Convert.ToInt32(lineInfo[0]),
                            Hit = lineInfo[1] != "0"
                        });
                        break;
                    case "FN":
                        // Function details
                        var functionInfo = split[1].Split(',');
                        function = new Function
                        {
                            LineNumber = Convert.ToInt32(functionInfo[0]),
                            Name = functionInfo[1]
                        };
                        file.Functions.Add(function);
                        break;
                    case "FNDA":
                        function.Hit = split[1][0] == '1';
                        break;
                    case "BRDA":
                        var branchInfo = split[1].Split(',');
                        var branch = new Branch
                        {
                            LineNumber = Convert.ToInt32(branchInfo[0]),
                            Block = Convert.ToInt32(branchInfo[1]),
                            Branch_ = Convert.ToInt32(branchInfo[2]),
                            Taken = Convert.ToInt32(branchInfo[3])
                        };
                        file.Branches.Add(branch);
                        break;
                    case "BRF":
                        break;
                    case "BRH":
                        break;
                    case "end_of_record":
                        break;
                    default:
                        break;
                }

            }

            return result;
        }
    }
}
