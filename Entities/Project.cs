using System;
using System.Collections.Generic;
using System.IO;

namespace WebTest.Entities
{
    public class Project
    {
        public string OutputDir { get; set; }
        public List<string> Servers { get; set; }
        
        public List<string> URLs { get; set; }
        public List<string> URLSources { get; set; }
        public List<TextReplacement> TextReplacements { get; set; }
        public List<TextReplacement> RegexReplacements { get; set; }

        public char[] InvalidFileNameChars { get; set; } 
        public Dictionary<string, string> ServerDirMap { get; set; }
        public Dictionary<string, string> MimeTypeExtensionMap { get; set; }

        /// <summary>
        /// How many requests to process in one go
        /// </summary>
        public int BatchSize { get; set; }
        /// <summary>
        /// Number of milliseconds to wait inbetween batches in order to not overload server
        /// </summary>
        public int DelayBetweenBatches { get; set; }
        /// <summary>
        /// Passed through to HttpClient.Timeout
        /// Number of milliseconds to wait before the request times out
        /// </summary>
        public int RequestTimeout { get; set; }

        public Project()
        {
            BatchSize = 200;
            RequestTimeout = 30000;
            DelayBetweenBatches = 1500;
            InvalidFileNameChars = Path.GetInvalidFileNameChars();
            MimeTypeExtensionMap = new Dictionary<string, string>
            {
                { "application/json", ".json"},
                { "text/html",".htm"},
                { "text/csv", ".csv" },
                { "text/xml", ".xml" }
            };
        }

        public void Validate()
        {
            if (!Directory.Exists(OutputDir))
                throw new Exception($"OutputDir '{OutputDir}' doesn't exist.");

            if (URLs?.Count == 0 && URLSources?.Count == 0)
                throw new Exception("No URLs or URLSources specified.");
        }
    }
}
