using System.Runtime.InteropServices;

namespace Joker
{
    /// <summary>
    /// Author: GAR
    /// Console version of mouse jiggler.
    /// </summary>
    internal class Program
    {
        const int MOUSEEVENTF_MOVE = 0x0001;

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint GetTickCount();

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        static void Main(string[] args)
        {
            const int MOUSE_MOVE_AMOUNT = 10; // Adjust this value as needed
            const int CHECK_INTERVAL = 10000; // Check every 10 seconds

            bool isActive = true;

            while (isActive)
            {
                MoveMouse(MOUSE_MOVE_AMOUNT);
                ResetIdleTimer();
                Thread.Sleep(CHECK_INTERVAL);
            }
        }

        static void MoveMouse(int distance)
        {
            mouse_event(MOUSEEVENTF_MOVE, distance, distance, 0, 0);
        }

        static void ResetIdleTimer()
        {
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                // Resetting the idle timer by simulating a keyboard event
                uint currentTickCount = GetTickCount();
                uint elapsedTime = currentTickCount - lastInputInfo.dwTime;

                if (elapsedTime >= 0 && elapsedTime < uint.MaxValue)
                {
                    uint simulatedElapsedTime = elapsedTime + 100; // Simulate some activity
                    INPUT[] inputs = new INPUT[1];
                    inputs[0].type = INPUT_KEYBOARD;
                    inputs[0].U.ki.wVk = 0;
                    inputs[0].U.ki.wScan = 0;
                    inputs[0].U.ki.dwFlags = KEYEVENTF_SCANCODE;
                    inputs[0].U.ki.time = 0;
                    inputs[0].U.ki.dwExtraInfo = IntPtr.Zero;

                    SendInput(1, inputs, INPUT.Size);
                }
            }
        }

        const uint INPUT_KEYBOARD = 1;
        const uint KEYEVENTF_SCANCODE = 8;

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public InputUnion U;

            public static int Size => Marshal.SizeOf(typeof(INPUT));
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public MOUSEINPUT mi;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }
}