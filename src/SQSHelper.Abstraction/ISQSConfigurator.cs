using Amazon.SQS;

namespace SQSHelper.Abstraction
{
    public interface ISQSConfigurator
    {
        string QueueUrl { get; }
        IAmazonSQS Client { get; }
    }
}
