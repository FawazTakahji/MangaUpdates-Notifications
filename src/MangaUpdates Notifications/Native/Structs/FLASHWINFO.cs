using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace MangaUpdates_Notifications.Native.Structs
{
    /// <summary> Contains the flash status for a window and the number of times the system should flash the window. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO
    {
        /// <summary> The size of the structure, in bytes. </summary>
        [JsonIgnore] public uint cbSize;
        /// <summary> A handle to the window to be flashed. The window can be either opened or minimized. </summary>
        public IntPtr hwnd;
        /// <summary> The flash status. This parameter can be one or more of the <see cref="Enums.FLASHW"/> enum </summary>
        public uint dwFlags;
        /// <summary> The number of times to flash the window. </summary>
        public uint uCount;
        /// <summary> The rate at which the window is to be flashed, in milliseconds. If dwTimeout is zero, the function uses the default cursor blink rate. </summary>
        public uint dwTimeout;

        public FLASHWINFO()
        {
            cbSize = (uint)Marshal.SizeOf(typeof(FLASHWINFO));
        }
    }
}