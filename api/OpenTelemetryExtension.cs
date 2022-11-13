using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace O11y.Social;

public static class OpenTelemetryExtension
{
    public static IServiceCollection SetupOpenTelemetry(this IServiceCollection services)
    {
        var honeycombApiKey = Environment.GetEnvironmentVariable("HONEYCOMB_API_KEY");
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
            services.AddSingleton(tracerProvider);
        }
        return services;
    }
}