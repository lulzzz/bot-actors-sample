using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Bot.Builder.Adapters;
using Akka.Actor;
using SupportBot.Actors;

namespace SupportBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            services.AddSingleton(_ =>
            {
                var adapter = new BotFrameworkAdapter(Configuration);

                //TODO: Include additional middleware here.

                return adapter;
            });

            // Add the actor system instance and the actor provider to the services.
            // You'll use the ConversationDispatcherProvider to get access to the dispatcher actor.
            services.AddSingleton( _ => ActorSystem.Create("supportbot"));
            services.AddSingleton<ConversationDispatcherProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
