using System;
using System.Windows;
using System.Windows.Interop;
using MangaUpdates_Notifications.Native;
using MangaUpdates_Notifications.Native.Enums;
using MangaUpdates_Notifications.Native.Structs;

namespace MangaUpdates_Notifications.Utilities
{
    public static class WindowFlasher
    {
        public static void FlashUntilActivated(Window window)
        {
            WindowInteropHelper helper = new(window);
            if (helper.Handle == IntPtr.Zero)
                return;

            FLASHWINFO flashInfo = new()
            {
                hwnd = helper.Handle,
                dwFlags = (uint)FLASHW.ALL | (uint)FLASHW.TIMERNOFG,
                uCount = uint.MaxValue,
                dwTimeout = 0
            };
            Methods.FlashWindowEx(ref flashInfo);
        }

        public static void Start(Window window)
        {
            WindowInteropHelper helper = new(window);
            if (helper.Handle == IntPtr.Zero)
                return;

            FLASHWINFO flashInfo = new()
            {
                hwnd = helper.Handle,
                dwFlags = (uint)FLASHW.ALL,
                uCount = uint.MaxValue,
                dwTimeout = 0
            };
            Methods.FlashWindowEx(ref flashInfo);
        }

        public static void Stop(Window window)
        {
            WindowInteropHelper helper = new(window);
            if (helper.Handle == IntPtr.Zero)
                return;

            FLASHWINFO flashInfo = new()
            {
                hwnd = helper.Handle,
                dwFlags = (uint)FLASHW.STOP,
                uCount = 0,
                dwTimeout = 0
            };
            Methods.FlashWindowEx(ref flashInfo);
        }
    }
}
