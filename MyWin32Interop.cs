using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DBManager
{
    internal class MyWin32Interop
    {
        public enum PeekMessageOption
        {
            PM_NOREMOVE = 0,
            PM_REMOVE
        }

        public static class WindowStyles
        {
            public static readonly int WS_BORDER = 0x00800000,
                                        WS_CAPTION = 0x00C00000,
                                        WS_CHILD = 0x40000000,
                                        WS_CHILDWINDOW = 0x40000000,
                                        WS_CLIPCHILDREN = 0x02000000,
                                        WS_CLIPSIBLINGS = 0x04000000,
                                        WS_DISABLED = 0x08000000,
                                        WS_DLGFRAME = 0x00400000,
                                        WS_GROUP = 0x00020000,
                                        WS_HSCROLL = 0x00100000,
                                        WS_ICONIC = 0x20000000,
                                        WS_MAXIMIZE = 0x01000000,
                                        WS_MAXIMIZEBOX = 0x00010000,
                                        WS_MINIMIZE = 0x20000000,
                                        WS_MINIMIZEBOX = 0x00020000,
                                        WS_OVERLAPPED = 0x00000000,
                                        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
                                        WS_POPUP = unchecked((int)0x80000000),
                                        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
                                        WS_SIZEBOX = 0x00040000,
                                        WS_SYSMENU = 0x00080000,
                                        WS_TABSTOP = 0x00010000,
                                        WS_THICKFRAME = 0x00040000,
                                        WS_TILED = 0x00000000,
                                        WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
                                        WS_VISIBLE = 0x10000000,
                                        WS_VSCROLL = 0x00200000,
                                        CS_SAVEBITS = 0x0800;
        };

        public static class ExtendedWindowStyles
        {
            public static readonly int WS_EX_ACCEPTFILES = 0x00000010,
                                        WS_EX_APPWINDOW = 0x00040000,
                                        WS_EX_CLIENTEDGE = 0x00000200,
                                        WS_EX_COMPOSITED = 0x02000000,
                                        WS_EX_CONTEXTHELP = 0x00000400,
                                        WS_EX_CONTROLPARENT = 0x00010000,
                                        WS_EX_DLGMODALFRAME = 0x00000001,
                                        WS_EX_LAYERED = 0x00080000,
                                        WS_EX_LAYOUTRTL = 0x00400000,
                                        WS_EX_LEFT = 0x00000000,
                                        WS_EX_LEFTSCROLLBAR = 0x00004000,
                                        WS_EX_LTRREADING = 0x00000000,
                                        WS_EX_MDICHILD = 0x00000040,
                                        WS_EX_NOACTIVATE = 0x08000000,
                                        WS_EX_NOINHERITLAYOUT = 0x00100000,
                                        WS_EX_NOPARENTNOTIFY = 0x00000004,
                                        WS_EX_NOREDIRECTIONBITMAP = 0x00200000,
                                        WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
                                        WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
                                        WS_EX_RIGHT = 0x00001000,
                                        WS_EX_RIGHTSCROLLBAR = 0x00000000,
                                        WS_EX_RTLREADING = 0x00002000,
                                        WS_EX_STATICEDGE = 0x00020000,
                                        WS_EX_TOOLWINDOW = 0x00000080,
                                        WS_EX_TOPMOST = 0x00000008,
                                        WS_EX_TRANSPARENT = 0x00000020,
                                        WS_EX_WINDOWEDGE = 0x00000100;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public Int32 hwmd;
            public Int32 message;
            public Int32 wParam;
            public Int32 lParam;
            public Int32 time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }

            public static implicit operator POINT(System.Windows.Point p)
            {
                return new POINT((int)p.X, (int)p.Y);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;


            public RECT(int _left, int _top, int _right, int _bottom)
            {
                this.left = _left;
                this.top = _top;
                this.right = _right;
                this.bottom = _bottom;
            }


            public override string ToString()
            {
                return string.Format("{0}x = {1}, y = {2}, width = {3}, height = {4}{5}", "{", left, top, right - left, bottom - top, "}");
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static readonly IntPtr HWND_TOP = IntPtr.Zero;
        public static readonly IntPtr HWND_MESSAGE = (IntPtr)(-3);

        public const int GW_CHILD = 5;

        public const int SWP_DRAWFRAME = 0x0020;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOCOPYBITS = 0x0100;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_ASYNCWINDOWPOS = 0x4000;
        public const int SWP_NOOWNERZORDER = 0x0200;
        public const int SWP_NOREDRAW = 0x0008;
        public const int SWP_SHOWWINDOW = 0x0040;

        public const int WM_WINDOWPOSCHANGING = 0x0046;
        public const int WM_WINDOWPOSCHANGED = 0x0047;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_PAINT = 0x000F;
        public const int WM_ERASEBKGND = 0x0014;
        public const int WM_SETCURSOR = 0x0020;
        public const int WM_USER = 0x0400;
        public const int WM_KEYUP = 0x0101;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_SYSCHAR = 0x0106;
        public const int WM_SYSKEYUP = 0x0105;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SIZE = 0x0005;
        public const int WM_QUIT = 0x0012;
        public const int WM_CHILDACTIVATE = 0x0022;
        public const int WM_CLOSE = 0x0010;
        public const int WM_MOUSEACTIVATE = 0x0021;
        public const int WM_DEVICECHANGE = 0x0219;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int WM_DESTROY = 0x0002;
        public const int GCL_STYLE = -26;
        public const int WM_CANCELMODE = 0x001F;
        public const int SS_OWNERDRAW = 0x000D;

        public const int GCLP_HBRBACKGROUND = -10;

        public const uint MF_BYCOMMAND = 0x00000000;
        public const uint MF_GRAYED = 0x00000001;

        public const uint SC_CLOSE = 0xF060;


        public const int MK_MBUTTON = 0x0010;
        public const int MK_RBUTTON = 0x0002;
        public const int MK_XBUTTON1 = 0x0020;
        public const int MK_XBUTTON2 = 0x0040;


        /// <summary>
        /// Эта структура используется при обработке сообщения WM_WINDOWPOSCHANGING
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOSPARAMS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }


        public static ushort LOWORD(long l)
        {
            return ((ushort)(((long)(l)) & 0xffff));
        }


        public static ushort HIWORD(long l)
        {
            return ((ushort)((((long)(l)) >> 16) & 0xffff));
        }


        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }


        // This static method is required because Win32 does not support 
        // GetWindowLongPtr directly
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return GetWindowLongPtr32(hWnd, nIndex);
        }


        public static IntPtr SetClassLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetClassLong64(hWnd, nIndex, dwNewLong);
            else
                return SetClassLong32(hWnd, nIndex, dwNewLong);
        }

        public static IntPtr GetClassLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetClassLong64(hWnd, nIndex);
            else
                return GetClassLong32(hWnd, nIndex);
        }

        [DllImport("User32")]
        public extern static IntPtr GetWindow(IntPtr hWnd, uint wCmd);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Calling code is expected to handle the different size of IntPtr")]
        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32", EntryPoint = "SetClassLong")]
        private static extern IntPtr SetClassLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32", EntryPoint = "SetClassLongPtr")]
        private static extern IntPtr SetClassLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32", EntryPoint = "GetClassLong")]
        private static extern IntPtr GetClassLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32", EntryPoint = "GetClassLongPtr")]
        private static extern IntPtr GetClassLong64(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr BeginDeferWindowPos(int nNumWindows);

        [DllImport("user32", SetLastError = true)]
        public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        [DllImport("user32", EntryPoint = "DeferWindowPos", SetLastError = true)]
        public static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd,
           IntPtr hWndInsertAfter, int x, int y, int cx, int cy, UInt32 uFlags);

        [DllImport("user32", SetLastError = true)]
        internal static extern IntPtr SetFocus(IntPtr hwnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool InvalidateRect(IntPtr hwnd, ref RECT lpRect, int bErase);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int GetWindowRgn(IntPtr hwnd, IntPtr hRgn);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SetWindowRgn(IntPtr hwnd, IntPtr hRgn, int bRedraw);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PeekMessage(
                           ref MSG lpMsg,
                           Int32 hwnd,
                           Int32 wMsgFilterMin,
                           Int32 wMsgFilterMax,
                           PeekMessageOption wRemoveMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("gdi32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("gdi32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool DeleteObject(IntPtr hObject);


        public static void SetTopMost(IntPtr wnd)
        {
            SetForegroundWindow(wnd);
        }


        public static void InsertAfter(Window That, IntPtr statusbarwnd)
        {
            SetWindowPos(((HwndSource)HwndSource.FromVisual(That)).Handle, statusbarwnd, 0, 0, 0, 0, 0x0100 | 0x0002 | 0x0001 | 0x0040);
        }


        public static void SetOverlapped(Window wnd)
        {
            SetFocus(((HwndSource)HwndSource.FromVisual(wnd)).Handle);
            IntPtr style = GetWindowLongPtr(((HwndSource)HwndSource.FromVisual(wnd)).Handle, GWL_STYLE);
            style = (IntPtr)((int)style | WindowStyles.WS_OVERLAPPEDWINDOW);
            SetWindowLongPtr(((HwndSource)HwndSource.FromVisual(wnd)).Handle, GWL_STYLE, style);
        }


        public static void SetChildren(IntPtr wnd, Window Parent)
        {
            SetParent(wnd, ((HwndSource)HwndSource.FromVisual(Parent)).Handle);
            SetFocus(wnd);
            IntPtr style = GetWindowLongPtr(wnd, GWL_STYLE);
            style = (IntPtr)((int)style | WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILDWINDOW | WindowStyles.WS_CLIPSIBLINGS);
            SetWindowLongPtr(wnd, GWL_STYLE, style);
        }


        public static void SetChildren(IntPtr Parent, Window wnd, bool IsChildMdi)
        {
            SetParent(((HwndSource)HwndSource.FromVisual(wnd)).Handle, Parent);
            SetFocus(((HwndSource)HwndSource.FromVisual(wnd)).Handle);
            IntPtr style = GetWindowLongPtr(((HwndSource)HwndSource.FromVisual(wnd)).Handle, GWL_STYLE);
            style = (IntPtr)((int)style | WindowStyles.WS_VISIBLE | WindowStyles.WS_CLIPSIBLINGS | 0xC000);
            SetWindowLongPtr(((HwndSource)HwndSource.FromVisual(wnd)).Handle, GWL_STYLE, style);

            if (IsChildMdi)
            {
                style = GetWindowLongPtr(((HwndSource)HwndSource.FromVisual(wnd)).Handle, GWL_EXSTYLE);
                style = (IntPtr)((int)style | 0x40 | 0x00000100 | 0x00000200);
                SetWindowLongPtr(((HwndSource)HwndSource.FromVisual(wnd)).Handle, GWL_EXSTYLE, style);
            }
        }


        public static void SetChildren(IntPtr wnd, IntPtr Parent)
        {
            SetParent(wnd, Parent);
            SetFocus(wnd);
            IntPtr style = GetWindowLongPtr(wnd, GWL_STYLE);
            style = (IntPtr)((int)style | WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILDWINDOW | WindowStyles.WS_CLIPSIBLINGS);
            SetWindowLongPtr(wnd, GWL_STYLE, style);
        }


        public static void SetChildren(Window Children, Window Parent)
        {
            SetParent(((HwndSource)HwndSource.FromVisual(Children)).Handle, ((HwndSource)HwndSource.FromVisual(Parent)).Handle);

            // remove control box
            IntPtr style = GetWindowLongPtr(((HwndSource)HwndSource.FromVisual(Children)).Handle, GWL_STYLE);
            int iStyle = (int)style;
            iStyle |= WindowStyles.WS_VISIBLE | WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS;
            style = (IntPtr)iStyle;
            SetWindowLongPtr(((HwndSource)HwndSource.FromVisual(Children)).Handle, GWL_STYLE, style);
        }
    }
}
