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

namespace SQSPuller
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
        /// Lambda function handler to respond to events coming from an Application Load Balancer.
        /// 
        /// Note: If "Multi value headers" is disabled on the ELB Target Group then use the Headers and QueryStringParameters properties 
        /// on the ApplicationLoadBalancerRequest and ApplicationLoadBalancerResponse objects. If "Multi value headers" is enabled then
        /// use MultiValueHeaders and MultiValueQueryStringParameters properties.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ApplicationLoadBalancerResponse> FunctionHandler(ApplicationLoadBalancerRequest request, ILambdaContext context)
        {
            _logger = context.Logger;

            _logger.Log("Start retrieving data");

            var response = new ApplicationLoadBalancerResponse
            {
                StatusCode = 200,
                StatusDescription = "200 OK",
                IsBase64Encoded = false
            };

            var messages = await _sqsHelper.ReceiveMessages(1);
            string msgJson  = string.Empty;
            if (messages.Count() > 0)
            {
                try
                {
                    var message = JsonSerializer.Deserialize<Message>(messages.First());
                    msgJson = JsonSerializer.Serialize(message);
                }
                catch (Exception e)
                {
                    _logger.Log($"Exception when deserializing message body: {e}");
                    response.StatusCode = 500;
                    response.StatusDescription = "500 Internal Server Error";
                    return response;
                }
            }

            if (!string.IsNullOrWhiteSpace(msgJson)) {
                // If "Multi value headers" is enabled for the ELB Target Group then use the "response.MultiValueHeaders" property instead of "response.Headers".
                response.Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json" }
                };

                response.Body = msgJson;
            }

            _logger.Log("End of retrieving data.");

            return response;
        }
    }
}
