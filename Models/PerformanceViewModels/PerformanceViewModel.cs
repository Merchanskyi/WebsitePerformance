using System;

namespace website_performance.Models.PerformanceViewModels
{
    public class PerformanceViewModel
    {
        public string WebSite { get; set; }
        public TimeSpan Responce { get; set; }
    }
}
