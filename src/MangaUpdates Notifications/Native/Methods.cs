using System;
using System.Runtime.InteropServices;
using MangaUpdates_Notifications.Native.Structs;

namespace MangaUpdates_Notifications.Native
{
    public static class Methods
    {
        /// <summary> Flashes the specified window. It does not change the active state of the window. </summary>
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlashWindowEx(ref FLASHWINFO pfwi);

        /// <summary> Sets the show state and the restored, minimized, and maximized positions of the specified window. </summary>
        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        /// <summary> Retrieves the show state and the restored, minimized, and maximized positions of the specified window. </summary>
        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
    }
}