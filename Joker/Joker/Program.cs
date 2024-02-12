using System.Runtime.InteropServices;

namespace Joker
{
    /// <summary>
    /// Author: GAR
    /// Console version of mouse jiggler.
    /// </summary>
    internal class Program
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hCursor;
            public POINT ptScreenPos;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press 'Z' to toggle Zen mode. Press any other key to exit.");

            var random = new Random();
            bool zenMode = false;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Z)
                    {
                        zenMode = !zenMode;
                        Console.WriteLine($"Zen mode {(zenMode ? "enabled" : "disabled")}");
                    }
                    else
                    {
                        break;
                    }
                }

                if (!zenMode)
                {
                    MoveMouseRandomly(random);
                }
                else
                {
                    KeepScreenActive();
                }

                Thread.Sleep(1000); // Adjust the delay as needed
            }
        }

        static void MoveMouseRandomly(Random random)
        {
            POINT currentPos;
            GetCursorPos(out currentPos);

            int newX = currentPos.X + random.Next(-5, 5);
            int newY = currentPos.Y + random.Next(-5, 5);

            SetCursorPos(newX, newY);
        }

        static void KeepScreenActive()
        {
            CURSORINFO pci;
            pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));

            if (GetCursorInfo(out pci))
            {
                if (pci.flags == 0)
                {
                    // Cursor is hidden, move it slightly to keep the screen active
                    SetCursorPos(pci.ptScreenPos.X + 1, pci.ptScreenPos.Y + 1);
                }
            }
        }
    }
}