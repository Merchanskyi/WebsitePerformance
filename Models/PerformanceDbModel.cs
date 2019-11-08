using System;
using System.ComponentModel.DataAnnotations;

namespace website_performance.Models
{
    public class WebsiteDbModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Website { get; set; }
    }
}
