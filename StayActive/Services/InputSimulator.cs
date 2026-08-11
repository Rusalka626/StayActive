using System.Runtime.InteropServices;

namespace StayActive.Services
{
    public class InputSimulator
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
        }
    }
