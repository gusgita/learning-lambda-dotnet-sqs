using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Model;
using ServerlessConfig;
using SQSHelper.Abstraction;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace SQSWatcher
{
    public class Function
    {
        private ISQSHelper _sqsHelper;
        private ILambdaLogger? _logger;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            ServicesConfigurator.ConfigureServices();
            
            _sqsHelper = ServicesConfigurator.Services.GetRequiredService<ISQSHelper>();
        }


        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            _logger = context.Logger;
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            context.Logger.Log($"Processed message {message.Body}");

            Message? msg = null;
            try {
                msg = JsonSerializer.Deserialize<Message>(message.Body);
            }
            catch(Exception e) {
                _logger?.Log($"Exception when trying to deserialize message body: {e.ToString()}");
                //TODO: Move to dead-letter
            }

            if (msg != null) {
                msg.ProcessingStatus = Status.Processed;

                var msgJson = JsonSerializer.Serialize(msg);
                _logger?.Log($"Sending message: {msgJson}");
                await _sqsHelper.SendMessage(msgJson);

                _logger?.Log("End processing received request.");
            }
        }
    }
}

