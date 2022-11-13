using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace O11y.Social;
public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(s =>
            {
                s.SetupOpenTelemetry();
                s.Configure<JsonSerializerOptions>(options =>
                {
                    options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                    options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.WriteIndented = true;
                });
            })
            .Build();

        host.Run();
    }
}