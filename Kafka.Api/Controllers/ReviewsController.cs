using System;
using System.Linq;
using System.Threading.Tasks;
using Kafka.Api.Models;
using Kafka.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Udemy;

namespace Kafka.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewProducer _reviewProducer;
        
        public ReviewsController(IReviewProducer reviewProducer)
        {
            _reviewProducer = reviewProducer;
        }

        [HttpPost("{courseId:long}")]
        public async Task<IActionResult> Post(long courseId, [FromBody] ReviewData review)
        {
            Console.WriteLine($"CourseId is: {courseId}");

            await _reviewProducer.ProduceAsync(new Review
            {
                Id = courseId,
                Title = review.Title,
                Content = review.Content,
                Rating = review.Rating,
                Created = DateTime.UtcNow.Ticks,
                Modified = DateTime.UtcNow.Ticks,
                Course = new Course {Id = courseId, Title = "", Url = ""},
                User = new User {DisplayName = review.UserName, Name = review.UserName, Title = "n/a"}
            });

            return Ok(new { message = "Thank you for your review"});
        }

        [HttpPost("{courseId:long}/bulk")]
        public async Task<IActionResult> PostMany(long courseId, [FromBody] ReviewData review)
        {
            Console.WriteLine($"CourseId is: {courseId}");
            var nMessages = 1000000;
            
            var reviewCounts = Enumerable.Range(0, nMessages).Select(x => new Review
            {
                Id = courseId,
                Title = $"{review.Title} #{x}",
                Content = review.Content,
                Rating = review.Rating,
                Created = DateTime.UtcNow.Ticks,
                Modified = DateTime.UtcNow.Ticks,
                Course = new Course {Id = courseId, Title = "", Url = ""},
                User = new User {DisplayName = review.UserName, Name = review.UserName, Title = "n/a"}
            });

            var startTime = DateTime.UtcNow.Ticks;
            await _reviewProducer.ProduceManyAsync(reviewCounts);
            var duration = DateTime.UtcNow.Ticks - startTime;
            Console.WriteLine($"Consumed {nMessages} messages in {duration/10000.0:F0}ms");
            Console.WriteLine($"{(nMessages) / (duration/10000.0):F0}k msg/s");

            return Ok(new { message = "Thank you for your review"});
        }
    }
}