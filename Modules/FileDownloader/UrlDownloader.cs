using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebTest.Entities;

namespace WebTest.Modules.FileDownloader
{
    /// <summary>
    /// Download file from url async
    /// </summary>
    public class UrlDownloader : IUrlDownloader
    {
        public async Task<TestResult> DownloadFile(Project project, string server, string url)
        {
            TestResult result;
           
            using (var client = new HttpClient() { BaseAddress = new Uri(server), Timeout = TimeSpan.FromMilliseconds(project.RequestTimeout) })
            using (var async = client.GetAsync(url))
            using (var response = await async)
            using (var content = response.Content)
            {
                result = new TestResult
                {
                    URL = url,
                    Server = server,
                    StatusCode = (int)response.StatusCode,
                    Content = content?.ReadAsStringAsync().Result,
                    ContentType = content?.Headers?.ContentType?.MediaType
                };
            }

            Console.WriteLine("Downloaded:" + url);
            return result;
        }
    }
}
