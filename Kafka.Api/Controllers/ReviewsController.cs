using System;
using System.Linq;
using System.Threading.Tasks;
using Kafka.Api.Models;
using Kafka.Api.Services;
using Kafka.Common.Json;
using Microsoft.AspNetCore.Mvc;

namespace Kafka.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewProducer _reviewProducer;
        private readonly IManyEntityProducer _manyEntityProducer;

        public ReviewsController(IReviewProducer reviewProducer, IManyEntityProducer manyEntityProducer)
        {
            _reviewProducer = reviewProducer;
            _manyEntityProducer = manyEntityProducer;
        }

        [HttpPost("{courseId:long}")]
        public async Task<IActionResult> Post(long courseId, [FromBody] ReviewData review)
        {
            Console.WriteLine($"CourseId is: {courseId}");

            await _manyEntityProducer.ProduceAsync(courseId, new ReviewEntity
            {
                Id = courseId,
                Title = review.Title,
                Content = review.Content,
                Rating = review.Rating
            });

            return Ok(new { message = "Thank you for your review"});
        }
        
        [HttpPost("company/{id:long}")]
        public async Task<IActionResult> PostCompany(long id)
        {
            Console.WriteLine($"CompanyId is: {id}");

            await _manyEntityProducer.ProduceAsync(id, new CreateCompanyEvent
            {
                Id = id,
                Name = Guid.NewGuid().ToString()
            });

            return Ok(new { message = "Thank you for creating company"});
        }

        [HttpPost("{courseId:long}/bulk")]
        public async Task<IActionResult> PostMany(long courseId, [FromBody] ReviewData review)
        {
            Console.WriteLine($"CourseId is: {courseId}");
            var nMessages = 1000000;
            
            var reviewCounts = Enumerable.Range(0, nMessages).Select(x => new ReviewEntity
            {
                Id = courseId,
                Title = $"{review.Title} #{x}",
                Content = review.Content,
                Rating = review.Rating
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