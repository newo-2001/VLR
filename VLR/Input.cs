﻿using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace VLR.Input;

[StructLayout(LayoutKind.Sequential)]
public struct MouseInput
{
    public int dx;
    public int dy;
    public uint mouseData;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
public struct KeyboardInput
{
    public ushort wVk;
    public ushort wScan;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
public struct HardwareInput
{
    public uint uMsg;
    public ushort wParamL;
    public ushort wParamH;
}

[StructLayout(LayoutKind.Explicit)]
public struct InputUnion
{
    [FieldOffset(0)] public MouseInput mi;
    [FieldOffset(0)] public KeyboardInput ki;
    [FieldOffset(0)] public HardwareInput hi;
}

public struct Input
{
    public int type;
    public InputUnion u;
}

[Flags]
public enum InputType
{
    Mouse = 0,
    Keyboard = 1,
    Hardware = 2
}

[Flags]
public enum KeyEventF
{
    KeyDown = 0x0000,
    ExtendedKey = 0x0001,
    KeyUp = 0x0002,
    Unicode = 0x0004,
    Scancode = 0x0008
}

public class VirtualKeyboard
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    private static extern IntPtr GetMessageExtraInfo();

    private readonly ILogger<VirtualKeyboard> _logger;

    public VirtualKeyboard(ILogger<VirtualKeyboard> logger)
    {
        _logger = logger;
    }

    public void SendKey(ushort scanCode)
    {
        _logger.LogDebug("Sending key with scancode {scancode}", scanCode);

        SendAction(scanCode, KeyEventF.KeyDown);
        Thread.Sleep(500);
        SendAction(scanCode, KeyEventF.KeyUp);
    }

    private void SendAction(ushort scanCode, KeyEventF @event)
    {
        var input = new Input
        {
            type = (int) InputType.Keyboard,
            u = new InputUnion
            {
                ki = new KeyboardInput
                {
                    wVk = 0,
                    wScan = scanCode,
                    dwFlags = (uint) (KeyEventF.Scancode | @event),
                    dwExtraInfo = GetMessageExtraInfo()
                }
            }
        };

        var result = SendInput(1, [input], Marshal.SizeOf(typeof(Input)));
        if (result != 1)
        {
            string error = new Win32Exception(Marshal.GetLastWin32Error()).Message;
            _logger.LogError("Failed to send key press: {error}", error);
        }
    }
}
