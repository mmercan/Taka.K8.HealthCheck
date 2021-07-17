using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrystalQuartz.Application;
using CrystalQuartz.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Quartz;
using Serilog;
using Serilog.Events;
using Taka.Common;
using Taka.Common.CustomFeatureFilter;
using Taka.K8s;
using Taka.Worker.Sync.Middlewares;
using Taka.Worker.Sync.Services;
using Turquoise.HealthChecks.Common;
using Turquoise.HealthChecks.Common.CheckCaller;

namespace Taka.Worker.Sync
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            if (Configuration["RunOnCluster"] == "true") { services.AddSingleton<IKubernetesClient, KubernetesClientInClusterConfig>(); }
            else { services.AddSingleton<IKubernetesClient, KubernetesClientFromConfigFile>(); }
            // services.AddSingleton<K8sGeneralService>();


            // Feature Namespace Scheheduled Sync (sync every 5 min), Namespace Live Sync
            // Feature Service Scheheduled Sync (sync every 5 min), Service Live Sync
            // Feature Deployment Scheheduled Sync (sync every 5 min), Deployment Live Sync

            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

            services.AddSingleton<IServiceCollection>(services);
            services.AddSingleton<IConfiguration>(Configuration);


            services.AddHttpClient<HealthCheckReportDownloaderService>("HealthCheckReportDownloader", options =>
            {
                // options.BaseAddress = new Uri(Configuration["CrmConnection:ServiceUrl"] + "api/data/v8.2/");
                options.Timeout = new TimeSpan(0, 2, 0);
                options.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                options.DefaultRequestHeaders.Add("OData-Version", "4.0");
                options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            })
            // .ConfigurePrimaryHttpMessageHandler((ch) =>
            // {
            //     var handler = new HttpClientHandler();
            //     handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            //     handler.ClientCertificates.Add(HttpClientHelpers.GetCert());
            //     return handler;
            // })
            //.AddHttpMessageHandler()
            // .AddHttpMessageHandler<OAuthTokenHandler>()
            .AddPolicyHandler(HttpClientHelpers.GetRetryPolicy())
            .AddPolicyHandler(HttpClientHelpers.GetCircuitBreakerPolicy());

            services.AddHttpContextAccessor();

            services.AddFeatureManagement()
            .AddFeatureFilter<PercentageFilter>()
            .AddFeatureFilter<HeadersFeatureFilter>();

            services.AddHealthChecks();

            services.Configure<QuartzOptions>(Configuration.GetSection("Quartz"));
            services.Configure<QuartzOptions>(options =>
            {
                options.Scheduling.IgnoreDuplicates = true; // default: false
                options.Scheduling.OverWriteExistingData = true; // default: true
            });

            services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler-Core";

                // we take this from appsettings.json, just show it's possible
                // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

                q.UseMicrosoftDependencyInjectionJobFactory();

                // these are the defaults
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                // q.ScheduleJob<SyncNamespacesJob>(trigger => trigger
                // .WithIdentity("SyncNamespacesJob")
                // .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10)))
                // .WithCronSchedule(Configuration["Schedules:SyncNamespaces"])
                // .WithDescription("Namespaces sync trigger configured run in Cron"));

                // q.ScheduleJob<SyncDeploymentsJob>(trigger => trigger
                // .WithIdentity("SyncDeploymentsJob")
                // .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(20)))
                // .WithCronSchedule(Configuration["Schedules:SyncDeployments"])
                // .WithDescription("Deployments sync trigger configured run in Cron"));

                // q.ScheduleJob<SyncServicesJob>(trigger => trigger
                // .WithIdentity("SyncServicesJob")
                // .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(30)))
                // .WithCronSchedule(Configuration["Schedules:SyncDeployments"])
                // .WithDescription("Services sync trigger configured run in Cron"));
            });

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
                //  options.StartDelay = TimeSpan.FromSeconds(10);
            });



            services.AddHostedService<NamespaceWatcher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, ISchedulerFactory schedulerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Enviroment", env.EnvironmentName)
                .Enrich.WithProperty("ApplicationName", "Turquoise.Worker.Sync")
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Console()
                .WriteTo.File("Logs/logs.txt");
            //.WriteTo.Elasticsearch()
            logger.WriteTo.Console();
            loggerFactory.AddSerilog();
            Log.Logger = logger.CreateLogger();
            app.UseExceptionLogger();


            var options = new CrystalQuartzOptions
            {
                ErrorDetectionOptions = new CrystalQuartz.Application.ErrorDetectionOptions
                {
                    VerbosityLevel = ErrorVerbosityLevel.Detailed
                }
                //JobDataMapDisplayOptions 
            };

            var scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            app.UseCrystalQuartz(() => scheduler, options);

            app.UseRouting();

            app.UseHealthChecks("/Health/IsAliveAndWell", new HealthCheckOptions()
            {
                ResponseWriter = WriteResponses.WriteListResponse,
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/Health/IsAlive", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

        }
    }
}
