using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SDR_Relay;

public static class Program
{
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder =
            Host.CreateApplicationBuilder(args);

        builder.Services.AddHostedService<SdrRelayService>();

        IHost host = builder.Build();

        await host.RunAsync();
    }
}