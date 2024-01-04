using System;
using System.Runtime.InteropServices;

namespace MangaUpdates_Notifications.Native.Structs
{
    /// <summary> The POINT structure defines the x- and y-coordinates of a point. </summary>
    [Serializable] [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary> Specifies the x-coordinate of the point. </summary>
        public int X;
        /// <summary> Specifies the y-coordinate of the point. </summary>
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}