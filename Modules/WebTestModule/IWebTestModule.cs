using System;
using System.Threading.Tasks;
using WebTest.Entities;

namespace WebTest.Modules.WebTestModule
{
    public interface IWebTestModule
    {
        Task Process(Project project, TestResult result);
    }
}
