using System.ComponentModel.DataAnnotations;

namespace Kafka.Api.Models
{
    public class ReviewData
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Rating { get; set; }

        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Content { get; set; }
    }
}