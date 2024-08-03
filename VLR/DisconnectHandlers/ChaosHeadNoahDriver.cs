namespace VLR.DisconnectHandlers;

using Microsoft.Extensions.Logging;
using System.Drawing;
using VLR.Input;

public class ChaosHeadNoahDriver
{
    private const decimal AUTO_RED_THRESHOLD = 0.15M;
    private const ushort KEY_F1 = 0x53;

    private static readonly Point AUTO_SPRITE_LOCATION = new Point(1804, 989);
    private static readonly Size AUTO_SPRITE_SIZE = new Size(40, 40);

    private readonly ILogger<ChaosHeadNoahDriver> _logger;
    private readonly VirtualKeyboard _keyboard;

    public ChaosHeadNoahDriver(ILogger<ChaosHeadNoahDriver> logger, VirtualKeyboard keyboard)
    {
        _logger = logger;
        _keyboard = keyboard;
    }

    public bool AutoModeEnabled()
    {
        using var bitmap = new Bitmap(AUTO_SPRITE_SIZE.Width, AUTO_SPRITE_SIZE.Height);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.CopyFromScreen(AUTO_SPRITE_LOCATION, Point.Empty, bitmap.Size, CopyPixelOperation.SourceCopy);

        var red = Enumerable.Range(0, bitmap.Height).SelectMany(y =>
        {
            return Enumerable.Range(0, bitmap.Width).Select(x =>
            {
                return (decimal) bitmap.GetPixel(x, y).R / 255;
            });
        }).Average();

        _logger.LogDebug("Detected {:0.0}% red at Chaos;Head Noah auto-icon location", red * 100);

        return red > AUTO_RED_THRESHOLD;
    }

    public void DisableAutoMode()
    {
        if (!AutoModeEnabled())
        {
            _logger.LogInformation("Pausing Chaos;Head Noah");
            _keyboard.SendKey(KEY_F1);
        }
        else
        {
            _logger.LogTrace("Attempted to disable auto mode, but it was already disabled");
        }
    }
}
