using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace MangaUpdates_Notifications.Native.Structs
{
    /// <summary> Contains information about the placement of a window on the screen. </summary>
    [Serializable] [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        /// <summary> The length of the structure, in bytes. </summary>
        [JsonIgnore] public int length;
        /// <summary> The flags that control the position of the minimized window and the method by which the window is restored. </summary>
        [JsonIgnore] public int flags;
        /// <summary> The current show state of the window. </summary>
        public int showCmd;
        /// <summary> The coordinates of the window's upper-left corner when the window is minimized. </summary>
        public POINT ptMinPosition;
        /// <summary> The coordinates of the window's upper-left corner when the window is maximized. </summary>
        public POINT ptMaxPosition;
        /// <summary> The window's coordinates when the window is in the restored position. </summary>
        public RECT rcNormalPosition;

        public WINDOWPLACEMENT()
        {
            length = Marshal.SizeOf(this);
        }
    }
}