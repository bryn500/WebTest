using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WebTest.Entities;

namespace WebTest.Modules.ReportWriter
{
    /// <summary>
    /// Create CSV of download results, this provides a quick means to see if any url is throwing an error
    /// </summary>
    public class TestReportWriter : IReportWriter
    {
        public void Process(Project project, List<TestResult> testResults)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            StringBuilder sb = new StringBuilder();

            // create headers
            var fieldNames = new List<string> {"Url", "Error"};

            for (var i = 0; i < project.Servers.Count; i++)
                fieldNames.Add("Server" + i);

            foreach (var s in fieldNames)
                sb.Append(s + ",");

            sb.AppendLine();

            foreach (var results in testResults.GroupBy(x => x.URL))
            {
                var fields = new List<string>();

                var codes = results.Select(x => x.StatusCode.ToString()).ToList();

                var isOk = true;

                foreach (var code in codes)
                    if (code != "200")
                        isOk = false;

                if (codes.Distinct().Skip(1).Any())
                    isOk = false;

                fields.Add(results.Key);
                fields.Add((!isOk).ToString());
                fields.AddRange(codes);

                foreach (var s in fields)
                    sb.Append(s + ",");

                sb.AppendLine();
            }

            File.WriteAllText($@"{project.OutputDir}\results.csv", sb.ToString());

            stopwatch.Stop();
            Console.WriteLine($"TestReportWriter.Process took {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
