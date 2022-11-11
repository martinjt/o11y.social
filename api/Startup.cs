using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using O11y.Social;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

[assembly: FunctionsStartup(typeof(Startup))]

namespace O11y.Social;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var honeycombApiKey = Environment.GetEnvironmentVariable("HONEYCOMB_API_KEY");
        Console.WriteLine($"Honeycomb API Key: {honeycombApiKey}");
        if (!string.IsNullOrEmpty(honeycombApiKey))
        {
            var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("o11y-social-api"))
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri($"https://api.honeycomb.io:443");
                    otlpOptions.Headers = string.Join(",", new List<string>
                    {
                        "x-otlp-version=0.16.0",
                        $"x-honeycomb-team={honeycombApiKey}"
                    });
                })
                .AddAspNetCoreInstrumentation()
                .SetSampler(new AlwaysOnSampler())
                .Build();
            builder.Services.AddSingleton(tracerProvider);
        }
    }
}