using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace StayActive.Services;

public enum ActivityType
{
    MoveMouse,
    ClickMouse,
    PressKey,
    PressSpace
}

public static class InputSimulator
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    // INPUT es una "unión" en Win32: mi y ki comparten el mismo espacio de memoria,
    // por eso usamos Explicit + FieldOffset en vez de Sequential.
    [StructLayout(LayoutKind.Explicit)]
    private struct INPUT
    {
        [FieldOffset(0)] public uint type;
        [FieldOffset(8)] public MOUSEINPUT mi;
        [FieldOffset(8)] public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx, dy;
        public uint mouseData, dwFlags, time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private const uint INPUT_MOUSE = 0;
    private const uint INPUT_KEYBOARD = 1;

    private const uint MOUSEEVENTF_MOVE = 0x0001;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint KEYEVENTF_SCANCODE = 0x0008;

    private const ushort VK_SPACE = 0x20;

    private const ushort SCANCODE_SPACE = 0x39;
    private const ushort SCANCODE_CONTROL = 0x1D;

    private const int KeyHoldMs = 60; // duración real entre down y up, para que juegos como Fortnite lo detecten

    public static void JiggleMouse()
    {
        var input = new INPUT
        {
            type = INPUT_MOUSE,
            mi = new MOUSEINPUT { dx = 1, dy = 0, dwFlags = MOUSEEVENTF_MOVE }
        };
        SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));

        input.mi.dx = -1;
        SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
    }

    public static void ClickAtCurrentPosition()
    {
        var down = new INPUT { type = INPUT_MOUSE, mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_LEFTDOWN } };
        var up = new INPUT { type = INPUT_MOUSE, mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_LEFTUP } };
        SendInput(1, new[] { down }, Marshal.SizeOf(typeof(INPUT)));
        SendInput(1, new[] { up }, Marshal.SizeOf(typeof(INPUT)));
    }

    public static void PressHarmlessKey()
    {
        // F15 no existe físicamente en teclados normales, así que no interfiere con nada.
        System.Windows.Forms.SendKeys.SendWait("{F15}");
    }

    private static void SendScanKey(ushort scanCode, bool keyUp)
    {
        var input = new INPUT
        {
            type = INPUT_KEYBOARD,
            ki = new KEYBDINPUT
            {
                wScan = scanCode,
                dwFlags = KEYEVENTF_SCANCODE | (keyUp ? KEYEVENTF_KEYUP : 0)
            }
        };
        SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
    }

    public static async Task PressSpaceBarAsync()
    {
        SendScanKey(SCANCODE_SPACE, keyUp: false);
        await Task.Delay(KeyHoldMs);
        SendScanKey(SCANCODE_SPACE, keyUp: true);
    }

    public static async Task PressCtrlOnlyAsync()
    {
        SendScanKey(SCANCODE_CONTROL, keyUp: false);
        await Task.Delay(KeyHoldMs);
        SendScanKey(SCANCODE_CONTROL, keyUp: true);
    }

    public static void PerformActivity(ActivityType type)
    {
        switch (type)
        {
            case ActivityType.MoveMouse: JiggleMouse(); break;
            case ActivityType.ClickMouse: ClickAtCurrentPosition(); break;
            case ActivityType.PressKey: PressHarmlessKey(); break;
                // PressSpace ya no pasa por aquí: se maneja como async desde ActivityService
        }
    }
}