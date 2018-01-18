using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebTest.Entities;

namespace WebTest.Modules.WebTestModule
{
    /// <summary>
    /// Replace text in downloaded file using replacements found in project settings
    /// </summary>
    public class TextReplacer : IWebTestModule
    {
        public Task Process(Project project, TestResult result)
        {
            var task = Task.FromResult(0);

            if (string.IsNullOrEmpty(result.Content))
                return task;

            foreach (var textReplacement in project.TextReplacements)
                result.Content = result.Content.Replace(textReplacement.Input, textReplacement.Output);

            foreach (var regexReplacement in project.RegexReplacements)
                result.Content = Regex.Replace(result.Content, regexReplacement.Input, regexReplacement.Output);

            return task;
        }
    }
}
