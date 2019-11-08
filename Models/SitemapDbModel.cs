using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace website_performance.Models
{
    public class SitemapDbModel
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Id")]
        public Guid WebsiteId { get; set; }
        public string Sitemap { get; set; }
        public TimeSpan Responce { get; set; }
    }
}
