using System.ComponentModel;

namespace Kafka.Common.Configuration
{
    public enum TopicNames
    {
        [Description("new-reviews")]
        NewReviews,
        [Description("verified-reviews")]
        VerifiedReviews,
        [Description("review-counts")]
        ReviewCounts
    }
}