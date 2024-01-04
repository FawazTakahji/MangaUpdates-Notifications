namespace MangaUpdates_Notifications.Native.Enums
{
    public enum FLASHW : uint
    {
        /// <summary> Flash both the window caption and taskbar button. This is equivalent to setting the <see cref="CAPTION"/> | <see cref="TRAY"/> flags. </summary>
        ALL = 0x00000003,
        /// <summary> Flash the window caption. </summary>
        CAPTION = 0x00000001,
        /// <summary> Stop flashing. The system restores the window to its original state. </summary>
        STOP = 0,
        /// <summary> Flash continuously, until the <see cref="STOP"/> flag is set. </summary>
        TIMER = 0x00000004,
        /// <summary> Flash continuously until the window comes to the foreground. </summary>
        TIMERNOFG = 0x0000000C,
        /// <summary> Flash the taskbar button. </summary>
        TRAY = 0x00000002
    }
}