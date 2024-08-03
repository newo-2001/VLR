using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VLR;
using VLR.DisconnectHandlers;
using VLR.Input;

var services = new ServiceCollection();
services.AddSingleton<NetworkMonitor>();
services.AddTransient<ChaosHeadNoahDriver>();
services.AddTransient<VirtualKeyboard>();
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
var networkMonitor = provider.GetRequiredService<NetworkMonitor>();
var chaosHeadNoahDriver = provider.GetRequiredService<ChaosHeadNoahDriver>();

networkMonitor.OnDisconnect += chaosHeadNoahDriver.DisableAutoMode;

while (true)
{
    Thread.Sleep(10000);
}
