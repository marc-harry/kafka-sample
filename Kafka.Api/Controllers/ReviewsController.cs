using System;
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
        
        public ReviewsController()
        {
            _reviewProducer = new ReviewProducer();
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
    }
}