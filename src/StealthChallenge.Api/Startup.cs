using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using StealthChallenge.Abstractions.Domain.Services;
using StealthChallenge.Abstractions.Infrastructure.Commands;
using StealthChallenge.Domain.Services;
using StealthChallenge.Logging.Factories;
using StealthChallenge.Logging.Loggers;
using StealthChallenge.MessagePack;
using StealthChallenge.Abstractions.Logging;
using StealthChallenge.Abstractions.Infrastructure.Services;
using StealthChallenge.Infrastructure;
using StealthChallenge.Abstractions.Infrastructure.Configuration;
//using ILogger = global::StealthChallenge.Abstractions.Logging.ILogger;
//using ILoggerFactory = global::StealthChallenge.Abstractions.Logging.ILoggerFactory;

namespace StealthChallenge.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <example> services.AddControllers() </example>
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton(sp => Serilog.Log.Logger);
            services.TryAddSingleton(typeof(ILogger), typeof(Serilogr));
            services.TryAddSingleton(typeof(ILogger<>), typeof(Serilogr<>));
            services.TryAddSingleton<ILoggerFactory, StealthLoggerFactory>();

            services.TryAddSingleton<ISerializeClientCommands, MessagePackClientCommandsSerializer>();
            services.TryAddSingleton<IUserService, InMemoryStubUserService>();

            var configurationSettings = Configuration
                   .GetSection(nameof(ConfigurationSettings))
                   .Get<ConfigurationSettings>();
            services.TryAddSingleton<IMakeMatches>(sp =>
                new RankingRangeMatchmaker(
                    sp.GetRequiredService<IUserService>(),
                    configurationSettings.MatchmakeRankingRange));
            services.TryAddSingleton<IClientCommandStrategy, ClientCommandStrategy>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapControllers();
            });
        }
    }
}
