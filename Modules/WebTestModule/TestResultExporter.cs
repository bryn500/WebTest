using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using WebTest.Entities;
using WebTest.Util;

namespace WebTest.Modules.WebTestModule
{
    public class TestResultExporter : IWebTestModule
    {
        public async Task Process(Project project, TestResult result)
        {
            string fileName;
            string extension;

            var splitUrl = result.URL.Split('?');

            var url = splitUrl[0];
            var queryStringHash = "";

            if (splitUrl.Length > 1)
                queryStringHash = "_" + Hashing.CreateMD5(splitUrl[1]);

            // if has extension then keep
            if (Path.HasExtension(url))
            {
                extension = Path.GetExtension(url);
                fileName = Path.GetFileNameWithoutExtension(url);
            }
            // else use content type
            else
            {
                extension = GetExtensionFromMimeType(project, result.ContentType);
                fileName = url;
            }

            // if we failed to get an extension use .txt
            if (string.IsNullOrEmpty(extension))
                extension = ".txt";

            // replace all invalid filename characters with an underscore
            foreach (var c in project.InvalidFileNameChars)
                fileName = fileName.Replace(c, '_');

            // remove _ at beginning of filename unless we are on root url
            if (fileName != "_")
                fileName = fileName.TrimStart('_');


            string path = $@"{project.OutputDir}\{project.ServerDirMap[result.Server]}\";
            const int maxPathLength = 259; // .net max path length is 260
            var remainingCharacters = maxPathLength - extension.Length - path.Length;

            fileName = fileName.Truncate(remainingCharacters);

            // write file to disk
            await WriteAsync($"{path}{fileName}{queryStringHash}{extension}", result.Content);

            result.Content = null; // release memory
        }

        private async Task WriteAsync(string path,string content)
        {
            using (var sw = new StreamWriter(path))
            {
                await sw.WriteAsync(content);
            }
        }

        private static string GetExtensionFromMimeType(Project project, string mimeType)
        {
            if (string.IsNullOrEmpty(mimeType))
                return null;

            // Attempt to look up extension in local dictionary
            if (project.MimeTypeExtensionMap.ContainsKey(mimeType))
                return project.MimeTypeExtensionMap[mimeType];

            // Otherwise attempt to retrieve from Registry
            var key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            var value = key?.GetValue("Extension", null);
            var result = value?.ToString() ?? string.Empty;

            return result;
        }
    }
}
