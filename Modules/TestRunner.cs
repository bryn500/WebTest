using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebTest.Entities;
using WebTest.Modules.FileDownloader;
using WebTest.Modules.ReportWriter;
using WebTest.Modules.WebTestModule;
using WebTest.Util;

namespace WebTest.Modules
{
    public class TestRunner
    {
        private IUrlDownloader _urlDownloader;
        private IReportWriter _reportWriter;

        public TestRunner(IUrlDownloader urlDownloader, IReportWriter reportWriter)
        {
            _urlDownloader = urlDownloader;
            _reportWriter = reportWriter;
        }

        public async void Run(Project project, List<IWebTestModule> modules)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Task<TestResult>> tasks = new List<Task<TestResult>>();

            List<TestResult> results = null;

            var urls = project.URLs.Distinct();

            // loop over urls to download and process in batches, with delay between them so they can finish downloading
            // removing this could overload server
            foreach (var batch in urls.Batch(project.BatchSize))
            {
                foreach (var url in batch)
                    foreach (var server in project.Servers)
                        tasks.Add(CreateTaskForUrl(project, modules, server, url));

                results = (await Task.WhenAll(tasks)).ToList();

                // batch Delay
                Console.WriteLine($"Sleeping for {project.DelayBetweenBatches}ms");
                Console.WriteLine($"Current time taken: {stopwatch.ElapsedMilliseconds}ms");
                Thread.Sleep(project.DelayBetweenBatches);
            }

            _reportWriter.Process(project, results);

            stopwatch.Stop();
            Console.WriteLine($"TestRunner.Process took {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine("Done");
        }

        private async Task<TestResult> CreateTaskForUrl(Project project, List<IWebTestModule> modules, string server, string url)
        {
            TestResult result;

            try
            {
                // run Network operations as async
                result = await _urlDownloader.DownloadFile(project, server, url);

                // Run other operations as threads in pool
                var task = Task.Run(async () => {
                    foreach (var module in modules)
                        await module.Process(project, result);
                });

                await task;

                Console.WriteLine("Saved File:" + url);
            }
            catch (TaskCanceledException canceled)
            {
                return new TestResult() { Server = server, URL = url, Content = canceled.ToString() };
            }
            
            return result;
        }
    }
}
