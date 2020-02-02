using DBManager.Global;
using DBManager.SettingsWriter;
using DBManager.TrayNotification;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace DBManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class DBManagerApp : Application
    {
        /// <summary>
        /// Mutex, который запрещает повторный запуск приложения.
        /// Он должен быть объявлен глобально, иначе будет закрыт после выхода из процедуры, где был объявлен
        /// </summary>
        private Mutex m_SyncMutex = null;

        public static DBManagerApp m_App = null;

        public static MainWindow MainWnd = null;

        public static XMLSettingsWriter m_AppSettings = null;

        public static compdbEntities m_Entities = null;

        /// <summary>
        /// Коэффиенты пересчета стандартных dpi (96) в текущие 
        /// </summary>
        public static System.Windows.Point m_DPIScale = new System.Windows.Point(1, 1);

        public static TaskbarIcon m_NotifyIcon = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            m_App = this;

            bool createdNew;
            AppDomain.CurrentDomain.UnhandledException += DumpMaker.CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += (source, ev) =>
            {
                ev.ToString();
            };

            m_AppSettings = new XMLSettingsWriter();

            m_SyncMutex = new Mutex(true, "DBManager mutex", out createdNew);

            if (!createdNew)
            {
                MessageBox.Show(DBManager.Properties.Resources.resmsgAppAlreadyOpened, AppAttributes.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
                return;
            }

            // Вычисляем коэффициенты пересчёта размера формы-хоста в зависимости от установленного DPI
            PropertyInfo dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            PropertyInfo dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
            int dpiX = (int)dpiXProperty.GetValue(null, null);
            int dpiY = (int)dpiYProperty.GetValue(null, null);
            m_DPIScale = new System.Windows.Point((float)dpiX / 96.0, (float)dpiY / 96.0);

            m_Entities = new compdbEntities();

            try
            {
                if (!m_Entities.Database.Exists())
                {
                    throw new InvalidOperationException();
                }
            }
            catch
            {   // Невозможно подключится к БД => пробуем запустить bat-ник, запускающий MySQL 
                try
                {
                    ProcessStartInfo procInfo = new ProcessStartInfo()
                    {
                        FileName = m_AppSettings.m_Settings.MySQLBatFullPath,
                        WorkingDirectory = Path.GetDirectoryName(m_AppSettings.m_Settings.MySQLBatFullPath),
                        Verb = "runas",
                        CreateNoWindow = true,
                    };
                    Process.Start(procInfo);  //Start that process.
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(DBManager.Properties.Resources.resfmtCantStartMySQL, ex.Message),
                                    AppAttributes.Title,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    Environment.Exit(0);
                    return;
                }

                // Делаем ещё несколько попыток подключится к БД
                int i = 5;
                while (--i > 0)
                {
                    Thread.Sleep(2 * 1000); // Ожидаем запуска MySQL
                    try
                    {
                        if (m_Entities.Database.Exists())
                            break;
                    }
                    catch
                    { }
                }

                if (i == 0)
                {
                    MessageBox.Show(string.Format(DBManager.Properties.Resources.resrmtCantConnectToDB, m_Entities.Database.Connection.ConnectionString),
                                    AppAttributes.Title,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    Environment.Exit(0);
                    return;
                }
            }

            //m_NotifyIcon = (TaskbarIcon)FindResource("ctrlNotifyIcon");

            GlobalDefines.RefreshVariables();

            base.OnStartup(e);
        }

        private void CloseWndOwnedWnds(Window wnd)
        {
            if (wnd == null)
                return;

            while (wnd.OwnedWindows.Count > 0)
            {
                CloseWndOwnedWnds(wnd.OwnedWindows[0]);
                wnd.OwnedWindows[0].Close();
            }
        }


        private void ctrlNotifyIcon_TrayBalloonTipClicked(object sender, RoutedEventArgs e)
        {
            CActivateMainWndCommand ActivateMainWndCommand = new CActivateMainWndCommand();
            if (ActivateMainWndCommand.CanExecute(null))
                ActivateMainWndCommand.Execute(null);
        }
    }

    internal static class AppAttributes
    {
        private static readonly Assembly m_Assembly = null;
        private static readonly AssemblyTitleAttribute m_Title = null;
        private static readonly AssemblyCompanyAttribute m_Company = null;
        private static readonly AssemblyCopyrightAttribute m_Copyright = null;
        private static readonly AssemblyProductAttribute m_Product = null;
        private static readonly Icon m_Icon = null;

        public static string Title { get; private set; }
        public static string CompanyName { get; private set; }
        public static string Copyright { get; private set; }
        public static string ProductName { get; private set; }
        public static Icon AppIcon { get; private set; }

        private static readonly Version m_Version = null;
        public static string Version
        {
            get { return m_Version == null ? "" : m_Version.ToString(); }
        }

        static AppAttributes()
        {
            try
            {
                Title = "";
                CompanyName = "";
                Copyright = "";
                ProductName = "";
                m_Version = null;
                AppIcon = null;

                m_Assembly = Assembly.GetEntryAssembly();

                if (m_Assembly != null)
                {
                    object[] attributes = m_Assembly.GetCustomAttributes(false);

                    foreach (object attribute in attributes)
                    {
                        Type type = attribute.GetType();

                        if (type == typeof(AssemblyTitleAttribute)) m_Title = (AssemblyTitleAttribute)attribute;
                        if (type == typeof(AssemblyCompanyAttribute)) m_Company = (AssemblyCompanyAttribute)attribute;
                        if (type == typeof(AssemblyCopyrightAttribute)) m_Copyright = (AssemblyCopyrightAttribute)attribute;
                        if (type == typeof(AssemblyProductAttribute)) m_Product = (AssemblyProductAttribute)attribute;
                    }

                    m_Version = m_Assembly.GetName().Version;

                    m_Icon = System.Drawing.Icon.ExtractAssociatedIcon(m_Assembly.ManifestModule.FullyQualifiedName);
                }

                if (m_Title != null)
                    Title = m_Title.Title;
                if (m_Company != null)
                    CompanyName = m_Company.Company;
                if (m_Copyright != null)
                    Copyright = m_Copyright.Copyright;
                if (m_Product != null)
                    ProductName = m_Product.Product;

                if (m_Icon != null)
                    AppIcon = m_Icon;
            }
            catch
            {
            }
        }
    }
}
