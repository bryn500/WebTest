using System;
using System.Collections.Generic;
using WebTest.Modules;
using WebTest.Modules.FileDownloader;
using WebTest.Modules.ReportWriter;
using WebTest.Modules.WebTestModule;
using WebTest.Util;

namespace WebTest
{
    /// <summary>
    /// Program to download html from urls so they can be compared for changes
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Setup:
            var projectLoader = new ProjectLoader(new Sitemaps());
            var project = projectLoader.FromFile(args[0]);

            var fileDownloader = new UrlDownloader();
            var reportWriter = new TestReportWriter();
            var testRunner = new TestRunner(fileDownloader, reportWriter);
            var modules = new List<IWebTestModule>
            {
                new TextReplacer(),
                new TestResultExporter()
            };

            // Run
            Console.WriteLine("Starting");
            testRunner.Run(project, modules);
            Console.ReadLine();
        }
    }
}
