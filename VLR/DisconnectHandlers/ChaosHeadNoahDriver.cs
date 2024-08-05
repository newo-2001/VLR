namespace VLR.DisconnectHandlers;

using Microsoft.Extensions.Logging;
using VLR.Input;

public class ChaosHeadNoahDriver
{
    private const ushort KEY_F1 = 0x53;

    private readonly ILogger<ChaosHeadNoahDriver> _logger;
    private readonly VirtualKeyboard _keyboard;

    public ChaosHeadNoahDriver(ILogger<ChaosHeadNoahDriver> logger, VirtualKeyboard keyboard)
    {
        _logger = logger;
        _keyboard = keyboard;
    }

    public void DisableAutoMode()
    {
        _logger.LogInformation("Pausing Chaos;Head Noah");
        _keyboard.SendKey(KEY_F1);
    }
}
