﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Snaelro.Utils.Mvc.Configuration;
using Snaelro.Utils.Mvc.Extensions;
using Snaelro.Utils.Mvc.Middlewares;

namespace Snaelro.Silo.Dashboard
{
    public class Startup
    {
        private readonly FromEnvironment _fromEnvironment;
        private static IClusterClient _clusterClient;

        public Startup(IConfiguration configuration)
        {
            _fromEnvironment = FromEnvironment.Build(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(CreateClient())
                .AddSingleton(AppStopper.New)
                .AddSingleton(_fromEnvironment);

            services.AddServicesForSelfHostedDashboard(null, opt =>
            {
                opt.HideTrace = true;
                opt.Port = 7002;
                opt.CounterUpdateIntervalMs = 5000;
            });
        }

        private IClusterClient CreateClient()
        {
            _clusterClient = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = _fromEnvironment.ClusterId;
                    options.ServiceId = _fromEnvironment.ServiceId;
                })
                .UseAdoNetClustering(opt =>
                {
                    opt.Invariant = _fromEnvironment.PostgresInvariant;
                    opt.ConnectionString = _fromEnvironment.PostgresConnection;
                })
                .ConfigureApplicationParts(parts => parts.AddFromDependencyContext().WithReferences())
                .UseDashboard()
                .Build();

            return _clusterClient;
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseLeave();
            appBuilder.UseVersionCheck();
            appBuilder.UseOrleansDashboard();
        }
    }
}