using System.Text.Json;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Model;
using ServerlessConfig;
using SQSHelper.Abstraction;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace SQSPusher
{
    public class Function
    {
        private ISQSHelper _sqsHelper;
        private ILambdaLogger? _logger;
        public Function()
        {
            ServicesConfigurator.ConfigureServices();
            
            _sqsHelper = ServicesConfigurator.Services.GetRequiredService<ISQSHelper>();
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ApplicationLoadBalancerResponse> FunctionHandler(ApplicationLoadBalancerRequest request, ILambdaContext context)
        {
            _logger = context.Logger;

            _logger.Log("Start processing received request.");

            if (string.IsNullOrWhiteSpace(request.Body)) {
                return new ApplicationLoadBalancerResponse {
                    Body = "Request body is empty.",
                    StatusCode = 400
                };
            }

            _logger.Log($"Processing message : {request.Body}");
            Message message;
            try
            {
                message = JsonSerializer.Deserialize<Message>(request.Body);
            }
            catch (Exception e)
            {
                _logger.Log($"Exception when deserializing message body: {e.ToString()}");

                return new ApplicationLoadBalancerResponse {
                    Body = "Request body is invalid.",
                    StatusCode = 400
                };
            }
            message.ProcessingStatus = Status.Received;
            var messageJson = JsonSerializer.Serialize(message);

            _logger.Log($"Sending message : {messageJson}");
            await _sqsHelper.SendMessage(messageJson);

            _logger.Log("End processing received request.");

            return new ApplicationLoadBalancerResponse {
                Body = "Succesfully added message",
                StatusCode = 200
            };
        }
    }
}
