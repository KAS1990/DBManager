using DBManager.Global;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace DBManager
{
    /// <summary>
    ////Класс для создания дампов памяти, если программа неожиданно падает. 
    /// </summary>
    public class DumpMaker
    {
        private static bool ShowMessageBeforeCrashed = true;
        private static bool ShowMsgWithStackTraceFile = false;
        private static bool StackTraceFileHasBeenGenerated = false;
        private static string StackTraceFileName = "";

        [DllImport("kernel32.dll")]
        private static uint GetCurrentThreadId();

        [DllImport("Dbghelp.dll")]
        private static bool MiniDumpWriteDump(IntPtr hProcess, uint ProcessId, IntPtr hFile, int DumpType, ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct MINIDUMP_EXCEPTION_INFORMATION
        {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            public int ClientPointers;
        }

        [Flags]
        public enum MINIDUMP_TYPE : uint
        {
            // From dbghelp.h:
            Normal = 0x00000000,
            WithDataSegs = 0x00000001,
            WithFullMemory = 0x00000002,
            WithHandleData = 0x00000004,
            FilterMemory = 0x00000008,
            ScanMemory = 0x00000010,
            WithUnloadedModules = 0x00000020,
            WithIndirectlyReferencedMemory = 0x00000040,
            FilterModulePaths = 0x00000080,
            WithProcessThreadData = 0x00000100,
            WithPrivateReadWriteMemory = 0x00000200,
            WithoutOptionalData = 0x00000400,
            WithFullMemoryInfo = 0x00000800,
            WithThreadInfo = 0x00001000,
            WithCodeSegs = 0x00002000,
            WithoutAuxiliaryState = 0x00004000,
            WithFullAuxiliaryState = 0x00008000,
            WithPrivateWriteCopyMemory = 0x00010000,
            IgnoreInaccessibleMemory = 0x00020000,
            ValidTypeFlags = 0x001fffff,
        };

        [DllImport("user32.dll")]
        private static IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public extern static IntPtr GetDesktopWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern UInt64 BitBlt(IntPtr hDestDC,
                                            int x,
                                            int y,
                                            int nWidth,
                                            int nHeight,
                                            IntPtr hSrcDC,
                                            int xSrc,
                                            int ySrc,
                                            System.Int32 dwRop);


        public static string TodayInStringForFileName()
        {
            return DateTime.Today.ToString(CultureInfo.GetCultureInfo(GlobalDefines.RUSSIAN_CULTURE_NAME).DateTimeFormat.ShortDatePattern);
        }


        public static string GetDefaultDumpFileName()
        {
            return string.Format(@"CRASH_DUMP_{0}_{1}_{2}.dmp", AppAttributes.Title, TodayInStringForFileName(), DateTime.Now.Ticks);
        }


        public static string GetDefaultStackTraceFileName()
        {
            return string.Format(@"STACK_TRACE_{0}_{1}_{2}.stk", AppAttributes.Title, TodayInStringForFileName(), DateTime.Now.Ticks);
        }


        public static string GetDefaultScreenFileName()
        {
            return string.Format(@"SCREEN_{0}_{1}_{2}.png", AppAttributes.Title, TodayInStringForFileName(), DateTime.Now.Ticks);
        }

        private static void AddStackToFile(Exception ex, StreamWriter sw)
        {
            if (sw == null)
                return;

            if (ex != null)
            {
                const int STACK_TRACE_WRITE_PORTION = 1000;

                sw.WriteLine("StackTrace: ");

                /* Записываем кадр стека в файл порциями, потому что StreamWriter может писать данные в файл кусками ограниченного размера.
				 * Размер ограничения узнать не удалось, поэтому выбрал STACK_TRACE_WRITE_PORTION, чтобы точно было меньше */
                char[] arrStackTrace = ex.StackTrace.ToCharArray();
                int BytesWritten = 0;
                while (BytesWritten < arrStackTrace.Length)
                {
                    if (BytesWritten + STACK_TRACE_WRITE_PORTION < arrStackTrace.Length)
                        sw.Write(arrStackTrace, BytesWritten, STACK_TRACE_WRITE_PORTION);
                    else
                        sw.Write(arrStackTrace, BytesWritten, arrStackTrace.Length - BytesWritten);
                    BytesWritten += STACK_TRACE_WRITE_PORTION;
                    sw.Flush();
                }
                sw.WriteLine("");
            }
            else
            {
                sw.Write("ex == null\n");
                sw.Flush();
            }
        }

        private static void WriteExceptionToFile(Exception ex, StreamWriter sw, int ExceptionNum)
        {
            if (ex != null)
            {
                sw.WriteLine("\nException №" + ExceptionNum.ToString());
                sw.WriteLine("Message: " + ex.Message);
                sw.WriteLine("Method: " + ex.TargetSite.ToString());
                sw.WriteLine("Source: " + ex.Source);
                if (ex.Data != null)
                {
                    sw.WriteLine("Data: ");
                    foreach (object val in ex.Data)
                        if (val is System.Collections.DictionaryEntry)
                        {
                            System.Collections.DictionaryEntry DictEntry = (System.Collections.DictionaryEntry)val;
                            sw.WriteLine(string.Format("\tSystem.Collections.DictionaryEntry: Key = {0}, value = {1}",
                                                        DictEntry.Key.ToString(),
                                                        DictEntry.Value == null ? "null" : DictEntry.Value.ToString()));
                        }
                        else
                            sw.WriteLine("\t" + val.ToString());
                }
                else
                    sw.WriteLine("Data = null");

                AddStackToFile(ex, sw);
            }
            else
            {
                sw.Write("ex == null");
                sw.Flush();
            }
        }

        private static void WriteExceptionToFile(Exception ex, int ExceptionNum)
        {
            StackTraceFileName = GetDefaultStackTraceFileName();
            using (FileStream fs = new FileStream(StackTraceFileName, FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);

                Exception IntEx = ex;
                while (IntEx != null)
                {
                    WriteExceptionToFile(IntEx, sw, ExceptionNum++);
                    IntEx = IntEx.InnerException;
                }
            }
        }

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string FileName = GetDefaultDumpFileName();

            if (!StackTraceFileHasBeenGenerated)
                WriteExceptionToFile(e.ExceptionObject as Exception, 1);
            StackTraceFileHasBeenGenerated = false;

            if (ShowMessageBeforeCrashed)
            {
                string Msg = "";
                ShowMsgWithStackTraceFile = true;   // Т.к. сейчас стек формируется всегда (это очень удобно для отладки), то значение ShowMsgWithStackTraceFile всегда true */
                if (ShowMsgWithStackTraceFile)
                {
                    Msg = string.Format(DBManager.Properties.Resources.resfmtAppCrashedWithStackTrace,
                                        AppAttributes.Title,
                                        ((Exception)e.ExceptionObject).Message,
                                        Path.GetFullPath(FileName),
                                        Path.GetFullPath(StackTraceFileName));
                }
                else
                {
                    Msg = string.Format(DBManager.Properties.Resources.resmsgfmtAppCrashed,
                                        AppAttributes.Title,
                                        ((Exception)e.ExceptionObject).Message,
                                        Path.GetFullPath(FileName));
                }

                if (DBManagerApp.MainWnd != null)
                {
                    DBManagerApp.MainWnd.Activate();
                    DBManagerApp.MainWnd.Topmost = true; // Чтобы пользователь увидел окно
                    DBManagerApp.MainWnd.Topmost = false;
                }

                Window ActiveWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(wnd => wnd.IsActive);
                if (DBManagerApp.MainWnd != null && (DBManagerApp.MainWnd.WindowState == WindowState.Minimized || ActiveWindow == null))
                    MessageBox.Show(DBManagerApp.MainWnd, Msg, AppAttributes.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show(Msg, AppAttributes.Title, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            }

            ShowMessageBeforeCrashed = true;

            CreateMiniDump(FileName);

            if (!e.IsTerminating)
                Environment.Exit(0);
        }


        public static void ShowErrMessageAndClose(Window OwnerWindow, string msg, string Title)
        {
            if (OwnerWindow == null)
                /* Т.к. нельзя передать null вместо Owner в MessageBox.Show, то вызываем MessageBox.Show без этого параметра */
                MessageBox.Show(msg, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(OwnerWindow, msg, Title, MessageBoxButton.OK, MessageBoxImage.Error);

            ShowMessageBeforeCrashed = false;
            throw new NotImplementedException(msg);
        }


        /// <summary>
        /// Выводит строку сообщения об ошибке и сохраняет ex.StackTrace в файл.
        /// В сообщении об ошибке будет написано о создании этого файла.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="Title"></param>
        public static void HandleExceptionAndClose(Exception ex, string Title)
        {
            StackTraceFileHasBeenGenerated = true;
            WriteExceptionToFile(ex, 1);

            ShowMessageBeforeCrashed = true;
            ShowMsgWithStackTraceFile = true;
            throw ex;
        }


        public static void CreateMiniDump(string FileName)
        {
            using (System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess())
            {
                MINIDUMP_EXCEPTION_INFORMATION Mdinfo = new MINIDUMP_EXCEPTION_INFORMATION();

                Mdinfo.ThreadId = GetCurrentThreadId();
                Mdinfo.ExceptionPointers = Marshal.GetExceptionPointers();
                Mdinfo.ClientPointers = 1;

                using (FileStream fs = new FileStream(FileName, FileMode.Create))
                {
                    MiniDumpWriteDump(process.Handle,
                                        (uint)process.Id,
                                        fs.SafeFileHandle.DangerousGetHandle(),
                                        (int)(MINIDUMP_TYPE.Normal),
                                        ref Mdinfo,
                                        IntPtr.Zero,
                                        IntPtr.Zero);
                }
            }
        }
    }
}
