using System;
using System.Collections.Generic;
using WebTest.Entities;

namespace WebTest.Modules.ReportWriter
{
    public interface IReportWriter
    {
        void Process(Project project, List<TestResult> testResults);
    }
}
