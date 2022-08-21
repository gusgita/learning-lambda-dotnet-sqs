using SQSHelper.Abstraction;

namespace SQSHelper
{
    public class SQSReceiveConfigurator : SQSConfigurator
    {
        protected override string _queueUrlEnvVariableName => "SqsQueueReceiveUrl";
    }
}
