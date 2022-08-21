using System;
using Amazon.SQS;

namespace SQSHelper.Abstraction
{
    public abstract class SQSConfigurator : ISQSConfigurator, IDisposable
    {
        private bool disposedValue;

        protected virtual string _queueUrlEnvVariableName { get; }
        public string QueueUrl { get; private set; }
        public IAmazonSQS Client { get; private set; }

        public SQSConfigurator()
        {
            Client = new AmazonSQSClient();
            QueueUrl = Environment.GetEnvironmentVariable(_queueUrlEnvVariableName);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Client.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SQSConfigurator()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
