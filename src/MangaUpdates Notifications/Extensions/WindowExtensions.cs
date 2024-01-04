using System;
using System.Windows;
using System.Windows.Interop;
using MangaUpdates_Notifications.Native;
using MangaUpdates_Notifications.Native.Structs;

namespace MangaUpdates_Notifications.Extensions
{
    public static class WindowExtensions
    {
        public static WINDOWPLACEMENT? GetPlacement(this Window window)
        {
            WindowInteropHelper helper = new(window);
            if (helper.Handle == IntPtr.Zero)
                return null;

            WINDOWPLACEMENT wp = new();

            return Methods.GetWindowPlacement(helper.Handle, out wp) ? wp : null;
        }
    }
}