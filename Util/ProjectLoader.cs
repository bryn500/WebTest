using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WebTest.Entities;

namespace WebTest.Util
{
    public class ProjectLoader
    {
        private Sitemaps _sitemaps;

        public ProjectLoader(Sitemaps sitemaps)
        {
            _sitemaps = sitemaps;
        }

        /// <summary>
        /// Create Project object from project project json file
        /// </summary>
        public Project FromFile(string projectFilename)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (String.IsNullOrEmpty(projectFilename) || !File.Exists(projectFilename))
                throw new Exception($"Couldn't find project file '{projectFilename}'.");

            var projectFile = File.ReadAllText(projectFilename);

            var project = JsonConvert.DeserializeObject<Project>(projectFile);

            project.Validate();

            ExpandUrlSources(project);

            // handle variables in text replacements
            foreach (var textReplacement in project.TextReplacements)
            {
                textReplacement.Input = SwapVariablesForValues(textReplacement.Input, project);
                textReplacement.Output = SwapVariablesForValues(textReplacement.Output, project);
            }

            // Build map from server to directory
            project.ServerDirMap = new Dictionary<string, string>();

            for (var i = 0; i < project.Servers.Count; i++)
                project.ServerDirMap.Add(project.Servers[i], i.ToString());

            // create output directorys if they don't exist already
            foreach (var dir in project.ServerDirMap)
            {
                var directory = $@"{project.OutputDir}\{dir.Value}";

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
            }

            stopwatch.Stop();
            Console.WriteLine($"Project.Load took {stopwatch.ElapsedMilliseconds}ms");

            return project;
        }

        private void ExpandUrlSources(Project project)
        {
            foreach (var url in project.URLSources)
            {
                var sitemap = _sitemaps.FromUrl(project.Servers[0] + url);

                var urls = sitemap.Urls.Select(x => x.Loc)
                    .Select(MakeURLRelative)
                    .ToList();

                project.URLs.AddRange(urls);
            }
        }

        private string MakeURLRelative(string url)
        {
            if (url != null && (url.StartsWith("http://") || url.StartsWith("https://")))
                return "/" + string.Join("/", url.Split('/').Skip(3));

            return url;
        }

        private string SwapVariablesForValues(string input, Project project)
        {
            for (var i = 0; i < project.Servers.Count; i++)
                input = input.Replace($"#Server{i}#", project.Servers[i].Replace("https://", "").Replace("http://", ""));

            return input;
        }
    }
}
