using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Bot.Builder.Adapters;
using Akka.Actor;
using SupportBot.Actors;
using Microsoft.Bot.Builder.Ai;

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

                // NOTICE: The keys and app IDs are stored as user secrets. Please follow the guide
                // at https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio
                // to set them up.

                adapter.Use(new LuisRecognizerMiddleware(Configuration["Luis:AppId"], Configuration["Luis:Key"], Configuration["Luis:Url"]));
                adapter.Use(new QnAMakerMiddleware(Configuration.GetSection("QnaMaker").Get<QnAMakerMiddlewareOptions>()));

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
