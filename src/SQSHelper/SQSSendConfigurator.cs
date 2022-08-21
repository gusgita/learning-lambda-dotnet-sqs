using SQSHelper.Abstraction;

namespace SQSHelper
{
    public class SQSSendConfigurator : SQSConfigurator {
        protected override string _queueUrlEnvVariableName => "SqsQueueSendUrl";
    }
}
