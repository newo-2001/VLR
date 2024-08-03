using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VLR;

var services = new ServiceCollection();
services.AddSingleton<NetworkMonitor>();
services.AddLogging(options =>
{
    options.SetMinimumLevel(LogLevel.Trace);
    options.AddSimpleConsole(console =>
    {
        console.SingleLine = true;
        console.TimestampFormat = "[HH:mm:ss] ";
    });
});

var provider = services.BuildServiceProvider();
var monitor = provider.GetRequiredService<NetworkMonitor>();

while (true)
{
    Thread.Sleep(10000);
}
