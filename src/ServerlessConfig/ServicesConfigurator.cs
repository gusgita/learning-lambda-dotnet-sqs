using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.DependencyInjection;
using SQSHelper;
using SQSHelper.Abstraction;

namespace ServerlessConfig
{
    public static class ServicesConfigurator
    {
        public static IServiceProvider Services { get; private set; }

        public static void ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            //serviceCollection.AddSingleton<ISQSConfigurator, SQSSendConfigurator>();
            //serviceCollection.AddSingleton<ISQSConfigurator, SQSRetriveConfigurator>();

            //Services = serviceCollection.BuildServiceProvider();
            
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);

            containerBuilder.RegisterType<SQSSendConfigurator>().Keyed<ISQSConfigurator>("SQS_SEND");
            containerBuilder.RegisterType<SQSReceiveConfigurator>().Keyed<ISQSConfigurator>("SQS_RECEIVE");
            containerBuilder.RegisterType<SQSHelper.SQSHelper>().As<ISQSHelper>().WithAttributeFiltering();
            
            Services = new AutofacServiceProvider(containerBuilder.Build());
        }
    }
}
