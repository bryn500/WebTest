using System;
using System.Threading.Tasks;
using WebTest.Entities;

namespace WebTest.Modules.FileDownloader
{
    public interface IUrlDownloader
    {
        Task<TestResult> DownloadFile(Project project, string server, string url);
    }
}
