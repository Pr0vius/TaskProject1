using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ms.rabbitmq.Consumer;

namespace ms.rabbitmq.Middlewares
{
    public static class ConsumerMiddleware
    {
        public static IApplicationBuilder UseRabbitConsumer(this IApplicationBuilder app)
        {
            var consumer = app.ApplicationServices.GetService<IConsumer>();

            var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            lifetime.ApplicationStarted.Register(() => consumer?.Subscribe());
            lifetime.ApplicationStopping.Register(() => consumer?.Unsubscribe());

            return app;
        }

    }
}
