using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Autofac.Features.AttributeFilters;
using SQSHelper.Abstraction;

namespace SQSHelper
{
    public class SQSHelper : ISQSHelper
    {
        IAmazonSQS _sqsSendClient;
        IAmazonSQS _sqsReceiveClient;
        string _sendQUrl;
        string _receiveQUrl;

        public SQSHelper(
            [KeyFilter("SQS_SEND")] ISQSConfigurator sqsSendConfigurator,
            [KeyFilter("SQS_RECEIVE")] ISQSConfigurator sqsReceiveConfigurator
        )
        {
            _sqsSendClient = sqsSendConfigurator.Client;
            _sendQUrl = sqsSendConfigurator.QueueUrl;
            _sqsReceiveClient = sqsReceiveConfigurator.Client;
            _receiveQUrl = sqsReceiveConfigurator.QueueUrl;
        }

        public async Task SendMessage(string messageBody)
        {
            SendMessageResponse response = await _sqsSendClient.SendMessageAsync(_sendQUrl, messageBody);
        }

        public async ValueTask<IEnumerable<string>> ReceiveMessages(int maxNumOfMessages)
        {
            ReceiveMessageResponse messageResp = await _sqsReceiveClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = _receiveQUrl,
                MaxNumberOfMessages = maxNumOfMessages
            });

            List<string> ret = null;
            List<DeleteMessageBatchRequestEntry> deletionEntries;

            if (messageResp.Messages.Count > 0)
            {
                ret = new List<string>();
                deletionEntries = new List<DeleteMessageBatchRequestEntry>();

                messageResp.Messages.ForEach(m =>
                {
                    ret.Add(m.Body);
                    deletionEntries.Add(new DeleteMessageBatchRequestEntry
                    {
                        ReceiptHandle = m.ReceiptHandle
                    });
                });

                await _sqsReceiveClient.DeleteMessageBatchAsync(new DeleteMessageBatchRequest
                {
                    QueueUrl = _receiveQUrl,
                    Entries = deletionEntries
                });
            }

            return ret;
        }
    }
}
