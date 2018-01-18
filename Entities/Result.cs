using System;

namespace WebTest.Entities
{
    public class TestResult
    {
        public string Server { get; set; }
        public string URL { get; set; }
        public int StatusCode { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
    }
}
