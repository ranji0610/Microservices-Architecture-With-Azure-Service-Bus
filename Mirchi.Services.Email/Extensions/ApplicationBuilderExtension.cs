using Mirchi.Services.Email.Messaging;

namespace Mirchi.Services.Email.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IAzureServiceBusConsumer AzureServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            AzureServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationlife = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostApplicationlife.ApplicationStarted.Register(OnStart);
            hostApplicationlife.ApplicationStarted.Register(OnStop);
            return app;
        }

        private static void OnStart()
        {
            AzureServiceBusConsumer.Start();
        }

        private static void OnStop()
        {
            AzureServiceBusConsumer.Stop();
        }        
    }
}
