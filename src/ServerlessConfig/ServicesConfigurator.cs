using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServerlessConfig
{
    public static class ServicesConfigurator
    {
        public static IServiceProvider Services { get; private set; }

        public static void ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            Services = serviceCollection.BuildServiceProvider();
        }
    }
}
