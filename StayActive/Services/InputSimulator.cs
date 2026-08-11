using System.Runtime.InteropServices;

namespace StayActive.Services
{
    public enum ActivityType
    {
        MoveMouse,
        ClickMouse,
        PressKey
    }

    public static class InputSimulator
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx, dy;
            public uint mouseData, dwFlags, time;
            public IntPtr dwExtraInfo;
        }

        private const uint INPUT_MOUSE = 0;
        private const uint MOUSEEVENTF_MOVE = 0x0001;

        public static void JiggleMouse()
        {
            var input = new INPUT
            {
                type = INPUT_MOUSE,
                mi = new MOUSEINPUT { dx = 1, dy = 0, dwFlags = MOUSEEVENTF_MOVE }
            };
            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));

            // mover de vuelta para que no "camine" el cursor
            input.mi.dx = -1;
            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }


        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        public static void ClickAtCurrentPosition()
        {
            var down = new INPUT { type = INPUT_MOUSE, mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_LEFTDOWN } };
            var up = new INPUT { type = INPUT_MOUSE, mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_LEFTUP } };
            SendInput(1, new[] { down }, Marshal.SizeOf(typeof(INPUT)));
            SendInput(1, new[] { up }, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void PressHarmlessKey()
        {
            // F15 no existe físicamente en teclados normales, así que no interfiere con nada
            System.Windows.Forms.SendKeys.SendWait("{F15}");
        }

        public static void PerformActivity(ActivityType type)
        {
            switch (type)
            {
                case ActivityType.MoveMouse: JiggleMouse(); break;
                case ActivityType.ClickMouse: ClickAtCurrentPosition(); break;
                case ActivityType.PressKey: PressHarmlessKey(); break;
            }
        }


    }
}
