using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VSCoverage.Helpers;
using VSCoverage.UnitTests.LCOV;

namespace VSCoverage.UnitTests
{
    internal static class Coverage
    {

        public static IList<FileCoverage> GetTestCoverage()
        {
            var result = new List<FileCoverage>();
            var testProjects = VsSolutionHelper.GetTestProjects();
            foreach (var testProject in testProjects)
            {
                var projectResult = RunTestCoverage(testProject);
                foreach (var fileCoverage in projectResult)
                {
                    var existing = result.FirstOrDefault(x => x.FilePath == fileCoverage.FilePath);
                    if (existing is null)
                        result.Add(fileCoverage);
                    else
                        MergeFileCoverage(existing, fileCoverage);
                }
            }

            return result;
        }

        private static IList<FileCoverage> RunTestCoverage(EnvDTE.Project project)
        {
            var directory = Path.GetDirectoryName(project.FileName);
            return RunTestCoverage(directory);
        }

        private static IList<FileCoverage> RunTestCoverage(string path)
        {
            var tmpPath = Path.GetTempFileName();
            
            var startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = path;
            startInfo.FileName = "dotnet";
            startInfo.Arguments = $"test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=\"{tmpPath}\"";

            var p = new Process();
            p.StartInfo = startInfo;

            p.Start();
            p.WaitForExit();

            var result = Parser.Parse(tmpPath);
            File.Delete(tmpPath);
            return result;
        }

        private static void MergeFileCoverage(FileCoverage a, FileCoverage b)
        {
            
        }
    }
}
