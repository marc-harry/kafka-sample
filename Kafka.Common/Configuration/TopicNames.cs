using System.ComponentModel;

namespace Kafka.Common.Configuration
{
    public enum TopicNames
    {
        [Description("newreviews")]
        NewReviews,
        [Description("verifiedreviews")]
        VerifiedReviews,
        [Description("reviewcounts")]
        ReviewCounts
    }
}