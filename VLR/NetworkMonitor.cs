using Microsoft.Extensions.Logging;
using ROOT.CIMV2.Win32;
using System.Management;
using System.Net.NetworkInformation;

namespace VLR;
public class NetworkMonitor
{
    public delegate void DisconnectEventHandler();
    public event DisconnectEventHandler OnDisconnect;

    const string ADAPTER_NAME = "Ethernet";

    private readonly object _eventLock = new object();
    private readonly ILogger<NetworkMonitor> _logger;
    private DateTime? _rebooting;

    public NetworkMonitor(ILogger<NetworkMonitor> logger)
    {
        _logger = logger;
        _logger.LogInformation("Listening for changes to network availability...");

        NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(OnNetworkEvent);
    }

    private void OnNetworkEvent(object? sender, EventArgs e)
    {
        lock (_eventLock)
        {
            var available = NetworkInterface.GetIsNetworkAvailable();
            
            if (available && _rebooting is not null)
            {
                var duration = DateTime.UtcNow - _rebooting.Value;
                _rebooting = null;
                _logger.LogInformation("Network connectivity restored after {duration:ss\\.ff}s", duration);
            }
            else if (!available && _rebooting is null)
            {
                _logger.LogInformation("Network connectivity lost, resetting network adapter");
                Task.Run(OnDisconnect.Invoke);

                ResetNetworkAdapter();
            }
        }
    }

    private void ResetNetworkAdapter()
    {
        _rebooting = DateTime.UtcNow;

        var query = new SelectQuery("Win32_NetworkAdapter", $"NetConnectionID = \"{ADAPTER_NAME}\"");
        var searcher = new ManagementObjectSearcher(query);

        var adapters = searcher.Get().Cast<ManagementObject>();
        var adapter = new NetworkAdapter(adapters.Single());

        if (adapter.NetEnabled)
        {
            _logger.LogTrace("Network adapter still enabled, disabling");
            if (adapter.Disable() != 0)
            {
                _logger.LogError("Failed to disable network adapter");
            }
        }

        _logger.LogTrace("Enabling network adapter...");
        if (adapter.Enable() != 0)
        {
            _logger.LogError("Failed to enable network adapter");
        }
    }
}
