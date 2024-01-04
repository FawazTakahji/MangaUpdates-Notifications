using System;
using System.Runtime.InteropServices;

namespace MangaUpdates_Notifications.Native.Structs
{
    /// <summary> The RECT structure defines a rectangle by the coordinates of its upper-left and lower-right corners. </summary>
    [Serializable] [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary> Specifies the x-coordinate of the upper-left corner of the rectangle. </summary>
        public int Left;
        /// <summary> Specifies the y-coordinate of the upper-left corner of the rectangle. </summary>
        public int Top;
        /// <summary> Specifies the x-coordinate of the lower-right corner of the rectangle. </summary>
        public int Right;
        /// <summary> Specifies the y-coordinate of the lower-right corner of the rectangle. </summary>
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}