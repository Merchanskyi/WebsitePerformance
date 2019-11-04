using System;
using System.ComponentModel.DataAnnotations;

namespace website_performance.Models
{
    public class PerformanceDbModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Website { get; set; }
        public TimeSpan Responce { get; set; }
    }
}
