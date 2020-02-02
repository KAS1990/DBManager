using System;
using System.Runtime.InteropServices;

namespace DBManager.TrayNotification
{
    public enum enTaskbarStates
    {
        NoProgress = 0,
        Indeterminate = 0x1,
        Normal = 0x2,
        Error = 0x4,
        Paused = 0x8
    }


    public static class CTaskBarIconTuning
    {
        [ComImportAttribute()]
        [GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList3
        {
            // ITaskbarList

            /// <summary>Initializes the taskbar list object. This method must be
            /// called before any other ITaskbarList methods can be called.</summary>
            [PreserveSig]
            void HrInit();

            /// <summary>Adds an item to the taskbar.</summary>
            /// <param name=”hWnd”>A handle to the window to be
            /// added to the taskbar.</param>
            [PreserveSig]
            void AddTab(IntPtr hWnd);

            /// <summary>Deletes an item from the taskbar.</summary>
            /// <param name=”hWnd”>A handle to the window to be deleted
            /// from the taskbar.</param>
            [PreserveSig]
            void DeleteTab(IntPtr hWnd);

            /// <summary>Activates an item on the taskbar. The window is not actually activated;
            /// the window’s item on the taskbar is merely displayed as active.</summary>
            /// <param name=”hWnd”>A handle to the window on the taskbar to be displayed as active.</param>
            [PreserveSig]
            void ActivateTab(IntPtr hWnd);

            /// <summary>Marks a taskbar item as active but does not visually activate it.</summary>
            /// <param name=”hWnd”>A handle to the window to be marked as active.</param>
            [PreserveSig]
            void SetActiveAlt(IntPtr hWnd);


            // ITaskbarList2

            /// <summary>Marks a window as full-screen</summary>
            /// <param name=”hWnd”></param>
            /// <param name=”fFullscreen”></param>
            [PreserveSig]
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);


            // ITaskbarList3

            /// <summary>Displays or updates a progress bar hosted in a taskbar button to show
            /// the specific percentage completed of the full operation.</summary>
            /// <param name=”hWnd”>The handle of the window whose associated taskbar button is being used as
            /// a progress indicator.</param>
            /// <param name=”ullCompleted”>An application-defined value that indicates the proportion of the
            /// operation that has been completed at the time the method is called.</param>
            /// <param name=”ullTotal”>An application-defined value that specifies the value ullCompleted will
            /// have when the operation is complete.</param>
            [PreserveSig]
            void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
            /// <summary>Sets the type and state of the progress indicator displayed on a taskbar button.</summary>
            /// <param name=”hWnd”>The handle of the window in which the progress of an operation is being
            /// shown. This window’s associated taskbar button will display the progress bar.</param>
            /// <param name=”tbpFlags”>Flags that control the current state of the progress button. Specify
            /// only one of the following flags; all states are mutually exclusive of all others.</param>
            [PreserveSig]
            void SetProgressState(IntPtr hwnd, enTaskbarStates state);

            /// <summary>Informs the taskbar that a tab or document window has been made the active window.</summary>
            /// <param name=”hWndTab”>Handle of the active tab window. This handle must already be registered
            /// through ITaskbarList3::RegisterTab. This value can be NULL if no tab is active.</param>
            /// <param name=”hWndMDI”>Handle of the application’s main window. This value tells the taskbar
            /// which group the thumbnail is a member of. This value is required and cannot be NULL.</param>
            /// <param name=”tbatFlags”>None, one, or both of the following values that specify a thumbnail
            /// and peek view to use in place of a representation of the specific tab or document.</param>
            [PreserveSig]
            void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, uint dwReserved);
        }


        [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
        [ClassInterfaceAttribute(ClassInterfaceType.None)]
        [ComImportAttribute()]
        private class TaskbarInstance
        {

        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            /// <summary>
            /// The size of the structure in bytes.
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// The Flash Status.
            /// </summary>
            public uint dwFlags;
            /// <summary>
            /// The number of times to Flash the window.
            /// </summary>
            public uint uCount;
            /// <summary>
            /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
            /// </summary>
            public uint dwTimeout;
        }


        public static IntPtr hWnd = IntPtr.Zero;
        private static readonly ITaskbarList3 taskbarInstance = (ITaskbarList3)new TaskbarInstance();
        private static readonly bool taskbarSupported = Environment.OSVersion.Version >= new Version(6, 1);
        private static readonly bool flashWindowSupported = System.Environment.OSVersion.Version.Major >= 5;


        static CTaskBarIconTuning()
        {
            taskbarInstance.HrInit();
        }


        #region ITaskbarList
        public static void ActivateTab()
        {
            if (taskbarSupported && hWnd != IntPtr.Zero)
                taskbarInstance.ActivateTab(hWnd);
        }


        public static void SetActiveAlt()
        {
            if (taskbarSupported && hWnd != IntPtr.Zero)
                taskbarInstance.SetActiveAlt(hWnd);
        }
        #endregion


        #region ITaskbarList2
        public static void MarkFullscreenWindow(bool Fullscreen = true)
        {
            if (taskbarSupported && hWnd != IntPtr.Zero)
                taskbarInstance.MarkFullscreenWindow(hWnd, Fullscreen);
        }
        #endregion


        #region ITaskbarList3
        public static void SetProgressState(enTaskbarStates taskbarState)
        {
            if (taskbarSupported && hWnd != IntPtr.Zero)
                taskbarInstance.SetProgressState(hWnd, taskbarState);
        }


        public static void SetProgressValue(double progressValue, double progressMax)
        {
            if (taskbarSupported && hWnd != IntPtr.Zero)
                taskbarInstance.SetProgressValue(hWnd, (ulong)progressValue, (ulong)progressMax);
        }


        public static void ResetProgressValue()
        {
            if (taskbarSupported && hWnd != IntPtr.Zero)
            {
                taskbarInstance.SetProgressValue(hWnd, (ulong)0, (ulong)0);
                taskbarInstance.SetProgressState(hWnd, enTaskbarStates.NoProgress);
            }
        }
        #endregion


        #region FlashWindow
        /// <summary>
        /// Stop flashing. The system restores the window to its original stae.
        /// </summary>
        private const uint FLASHW_STOP = 0;

        /// <summary>
        /// Flash the window caption.
        /// </summary>
        private const uint FLASHW_CAPTION = 1;

        /// <summary>
        /// Flash the taskbar button.
        /// </summary>
        private const uint FLASHW_TRAY = 2;

        /// <summary>
        /// Flash both the window caption and taskbar button.
        /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
        /// </summary>
        private const uint FLASHW_ALL = 3;

        /// <summary>
        /// Flash continuously, until the FLASHW_STOP flag is set.
        /// </summary>
        private const uint FLASHW_TIMER = 4;

        /// <summary>
        /// Flash continuously until the window comes to the foreground.
        /// </summary>
        private const uint FLASHW_TIMERNOFG = 12;


        private static FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
        {
            FLASHWINFO fi = new FLASHWINFO();
            fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
            fi.hwnd = handle;
            fi.dwFlags = flags;
            fi.uCount = count;
            fi.dwTimeout = timeout;
            return fi;
        }


        /// <summary>
        /// Flash the spacified Window (Form) until it recieves focus.
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        /// <returns></returns>
        public static bool Flash()
        {
            // Make sure we're running under Windows 2000 or later
            if (flashWindowSupported && hWnd != IntPtr.Zero)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hWnd, FLASHW_ALL | FLASHW_TIMERNOFG, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }


        /// <summary>
        /// Flash the specified Window (form) for the specified number of times
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        /// <param name="count">The number of times to Flash.</param>
        /// <returns></returns>
        public static bool Flash(uint count)
        {
            if (flashWindowSupported && hWnd != IntPtr.Zero)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hWnd, FLASHW_ALL, count, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Start Flashing the specified Window (form)
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        /// <returns></returns>
        public static bool Start(System.Windows.Forms.Form form)
        {
            if (flashWindowSupported && hWnd != IntPtr.Zero)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hWnd, FLASHW_ALL, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Stop Flashing the specified Window (form)
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool Stop(System.Windows.Forms.Form form)
        {
            if (flashWindowSupported && hWnd != IntPtr.Zero)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hWnd, FLASHW_STOP, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }
        #endregion
    }
}
