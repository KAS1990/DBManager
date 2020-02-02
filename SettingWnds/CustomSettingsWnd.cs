using DBManager.Global;
using DBManager.Global.Converters;
using DBManager.InterfaceElements;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFLocalization;

namespace DBManager.SettingWnds
{
    /// <summary>
    /// Базовый класс для всех окон настроек
    /// </summary>
    public abstract class СCustomSettingsWnd : CNotifyPropertyChangedWnd
    {
        public bool m_InInit = true;
        protected bool m_Modified = false;
        private bool m_ModifiedFromOpen = false;
        private bool m_IsWndReadonly = false;


        public virtual bool Modified
        {
            get { return m_Modified; }
            set
            {
                m_Modified = value;
                if (m_Modified)
                    ModifiedFromOpen = true;
                OnPropertyChanged("Modified");
            }
        }


        public bool IsWndReadonly
        {
            get { return m_IsWndReadonly; }
            set
            {
                m_IsWndReadonly = value;
                OnPropertyChanged("IsWndReadonly");
            }
        }

        /// <summary>
        /// Были ли изменения на форме с момента её открытия.
        /// Это свойство нужно, чтобы вызывающая форма не делала никаких действий, если пользователь просто открыл окно, просмотрел его и нажал ОК.
        /// Чтобы это заработало, нужно на кнопке ОК делать DialogResult = ModifiedFromOpen
        /// </summary>
        protected bool ModifiedFromOpen
        {
            get { return m_ModifiedFromOpen; }
            set { m_ModifiedFromOpen = value; }
        }

        #region Команды
        /// <summary>
        /// Команда "Применить".
        /// </summary>
        public static RoutedCommand cmdApply = new RoutedCommand();


        /// <summary>
        /// cmdApply.Execute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ApplyCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }


        /// <summary>
        /// cmdApply.CanExecute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ApplyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
        }
        /*----------------------------------------------------------*/

        /// <summary>
        /// Создаём привязки для описанных выше команд
        /// </summary>
        protected void SetCmdBindings()
        {
            CommandBinding cmdb = new CommandBinding()
            {
                Command = cmdApply
            };
            cmdb.Executed += ApplyCmdExecuted;
            cmdb.CanExecute += ApplyCmdCanExecute;
            CommandBindings.Add(cmdb);
        }
        #endregion

        public СCustomSettingsWnd()
        {
            SetCmdBindings();
        }


        /// <summary>
        /// Этот метод можно привязать к событию TextBox.TextChanged,
        /// чтобы при изменении введённого текста автоматически регистрировался факт изменения формы.
        /// метод просто устанавливает запоминает факт изменения данных формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Modified = true;
        }

        /// <summary>
        /// Этот метод можно привязать к событию ComboBox.SelectionChanged,
        /// чтобы при изменении введённого текста автоматически регистрировался факт изменения формы.
        /// метод просто устанавливает запоминает факт изменения данных формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Modified = true;
        }


        /// <summary>
        /// Этот метод можно привязать к событи CheckBox.Checked и CheckBox.Unchecked,
        /// чтобы при изменении флажка автоматически регистрировался факт изменения формы.
        /// метод просто устанавливает запоминает факт изменения данных формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chk_CheckedOrUnchecked(object sender, RoutedEventArgs e)
        {
            Modified = true;
        }


        /// <summary>
        /// Эту процедуру нужно привязать к событию GotFocus для всех TextBox, в которых используются Binding и StrToIntConverter, StrToFloatConverter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void txt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxEx txt = sender as TextBoxEx;
            if (txt != null && txt.Modified && txt.IsRightInput)
            {
                BindingBase bindBase = BindingOperations.GetBindingBase(txt, TextBox.TextProperty);

                if (bindBase != null)
                {
                    Binding bind = bindBase as Binding;
                    if (bind != null)
                    {
                        if (bind.Converter is StrToIntConverter)
                            ((StrToIntConverter)bind.Converter).m_PrevVal = int.Parse(txt.Text);
                        else
                            if (bind.Converter is StrToFloatConverter)
                            ((StrToFloatConverter)bind.Converter).m_PrevVal = float.Parse(txt.Text);
                    }
                }
            }
        }


        /// <summary>
        /// Метод, который производит сохранение настроек
        /// </summary>
        /// <returns></returns>
        protected virtual bool SaveSettings()
        {
            return Modified;
        }


        #region Всё, что связано с закрытием окна
        private string m_ResMsgOnCloseIfModified = "";
        /// <summary>
        /// Строка из ресурса, в которой содержится сообщение, отображаемое при нажатии на красный крест, если были изменения данных на форме.
        /// В этой строке не должно быть символов форматирования
        /// </summary>
        public string ResMsgOnCloseIfModified
        {
            get { return m_ResMsgOnCloseIfModified; }
            set { m_ResMsgOnCloseIfModified = value; }
        }

        private Func<bool> m_HasUnsavedChanges = null;
        /// <summary>
        /// Метод, который вызывается, чтобы узнать, есть ли несохранённые изменения на форме
        /// </summary>
        public Func<bool> HasUnsavedChanges
        {
            get { return m_HasUnsavedChanges; }
            set { m_HasUnsavedChanges = value; }
        }


        private bool m_AlwaysAccountChanges = false;
        /// <summary>
        /// true - даже при закрытии окна кнопкой отмена или красным крестом в DialogResult будет равен значению ModifiedFromOpen
        /// </summary>
        public bool AlwaysAccountChanges
        {
            get { return m_AlwaysAccountChanges; }
            set { m_AlwaysAccountChanges = value; }
        }


        /// <summary>
        /// Базовая функция для обработки события Closing.
        /// Она нужна, чтобы во всех дочерних окнах, если были изменения на форме, появлялось сообщение типа "Были изменения. Сохранить?"
        /// Текст сообщения задаётся с помощью свойства ResMsgOnCloseIfModified.
        /// Условие, которое будет говорить, были ли изменения или нет - HasUnsavedChanges. По умолчанию этот метод будет возвращать true.
        /// Этот метод будет вызван только при нажатии на красный крест 
        /// Сохранение настроек происходит вызовом виртуального метода SaveSettings().
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                if (!DialogResult.HasValue && (HasUnsavedChanges == null || HasUnsavedChanges()))
                {
                    string msg = LocalizationManager.ResourceManager.GetString(ResMsgOnCloseIfModified);
                    switch (MessageBox.Show(this,
                                            string.IsNullOrEmpty(msg) ? "You forgot to initialize property ResMsgOnCloseIfModified in this form" : msg,
                                            Title,
                                            MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                    {
                        case MessageBoxResult.Yes:
                            if (SaveSettings())
                                DialogResult = true;
                            else
                            {
                                e.Cancel = true;
                                return;
                            }
                            break;

                        case MessageBoxResult.No:
                            if (AlwaysAccountChanges)
                                DialogResult = ModifiedFromOpen;
                            break;

                        case MessageBoxResult.Cancel:
                            if (AlwaysAccountChanges)
                                DialogResult = ModifiedFromOpen;
                            e.Cancel = true;
                            break;
                    }
                }
                else
                    if (AlwaysAccountChanges)
                    DialogResult = ModifiedFromOpen;
            }

            base.OnClosing(e);
        }
        #endregion
    }
}
