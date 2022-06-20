using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace ServerlessConfig.SQS
{
    public class SQSHelper : ISQSHelper
    {
        private IAmazonSQS _sqsClient;
        private string qUrl;

        public SQSHelper(IAmazonSQS sqsCient, string qUrl) {

        }

        protected async Task SendMessage(string messageBody)
        {
            SendMessageResponse response = await _sqsClient.SendMessageAsync(qUrl, messageBody);
        }
    }
}