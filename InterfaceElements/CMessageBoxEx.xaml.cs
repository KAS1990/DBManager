using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Interop;
using System.ComponentModel;
using DBManager.Global;

namespace DBManager.Stuff
{
	/// <summary>
	/// Логика взаимодействия для CMessageBoxEx.xaml
	/// </summary>
	public partial class CMessageBoxEx : Window
	{
		public enum MessageBoxResult
		{
			/// <summary>
			/// Окно сообщения не возвращает никаких результатов.
			/// </summary>
			None = 0,
			/// <summary>
			/// Полученное значение окна сообщения — ОК.
			/// </summary>
			OK = 1,
			/// <summary>
			/// Полученное значение окна сообщения — Отмена.
			/// </summary>
			Cancel = 2,
			/// <summary>
			/// Полученное значение окна сообщения — Да.
			/// </summary>
			Yes = 6,
			/// <summary>
			/// Полученное значение окна сообщения — Нет.
			/// </summary>
			No = 7,
			/// <summary>
			/// Полученное значение окна сообщения — Нет.
			/// Номер в свойстве <paramref name="AdditionalButtonNum"/>.
			/// </summary>
			AdditionalButton = 8,
		}


		public enum MessageBoxButton
		{
			/// <summary>
			/// В окне не будет никаких стандартных кнопок
			/// </summary>
			None = 0,
			/// <summary>
			/// В окне сообщения отображается кнопка ОК.
			/// </summary>
			OK = 1,
			/// <summary>
			/// В окне сообщения отображаются кнопки ОК и Отмена.
			/// </summary>
			OKCancel = 2,
			/// <summary>
			/// В окне сообщения отображаются кнопки Да, Нет, and Отмена.
			/// </summary>
			YesNoCancel = 3,
			/// <summary>
			/// В окне сообщения отображаются кнопки Да и Нет.
			/// </summary>
			YesNo = 4,
		}

		/// <summary>
		/// Нужно ли показывать кнопку закрытия окна
		/// </summary>
		bool m_DisableCloseButton = false;

		
		Button[] AddedStdButtons = null;
		
		public MessageBoxResult Result { get; private set; }
		public int AdditionalButtonNum { get; private set; }

		
		#region Конструкторы
		public CMessageBoxEx(string messageBoxText,
							string [] AdditionalButonsTexts = null,
							int DefaultAdditionalButton = -1,
							bool DisableCloseButton = false)
		{
			InitializeComponent();

			Result = MessageBoxResult.None;
			AdditionalButtonNum = -1;
			m_DisableCloseButton = DisableCloseButton;
			Title = AppAttributes.Title;
			txtblkText.Text = messageBoxText;

			img.Source = GlobalDefines.GetMsgBoxSystemIcon(MessageBoxImage.Information);
			AddStdButtons(MessageBoxButton.OK, MessageBoxResult.None);
			AddAdditionalButons(AdditionalButonsTexts, DefaultAdditionalButton);
		}


		public CMessageBoxEx(string messageBoxText,
							string caption,
							string [] AdditionalButonsTexts = null,
							int DefaultAdditionalButton = -1,
							bool DisableCloseButton = false)
		{
			InitializeComponent();
													
			Result = MessageBoxResult.None;
			AdditionalButtonNum = -1;
			m_DisableCloseButton = DisableCloseButton;
			Title = caption;
			txtblkText.Text = messageBoxText;

			img.Source = GlobalDefines.GetMsgBoxSystemIcon(MessageBoxImage.Information);
			AddedStdButtons = AddStdButtons(MessageBoxButton.OK, MessageBoxResult.None);
			AddAdditionalButons(AdditionalButonsTexts, DefaultAdditionalButton);
		}

		
		public CMessageBoxEx(string messageBoxText,
							string caption,
							MessageBoxButton button,
							string [] AdditionalButonsTexts = null,
							int DefaultAdditionalButton = -1,
							bool DisableCloseButton = false)
		{
			InitializeComponent();

			Result = MessageBoxResult.None;
			AdditionalButtonNum = -1;
			m_DisableCloseButton = DisableCloseButton;
			Title = caption;
			txtblkText.Text = messageBoxText;

			img.Source = GlobalDefines.GetMsgBoxSystemIcon(MessageBoxImage.Information);
			AddedStdButtons = AddStdButtons(button, MessageBoxResult.None);
			AddAdditionalButons(AdditionalButonsTexts, DefaultAdditionalButton);
		}


		public CMessageBoxEx(string messageBoxText,
								string caption,
								MessageBoxButton button,
								MessageBoxImage icon,
								string[] AdditionalButonsTexts = null,
								int DefaultAdditionalButton = -1,
								bool DisableCloseButton = false)
		{
			InitializeComponent();
			
			Result = MessageBoxResult.None;
			AdditionalButtonNum = -1;
			m_DisableCloseButton = DisableCloseButton;
			Title = caption;
			txtblkText.Text = messageBoxText;

			img.Source = GlobalDefines.GetMsgBoxSystemIcon(icon);
			AddedStdButtons = AddStdButtons(button, MessageBoxResult.None);
			AddAdditionalButons(AdditionalButonsTexts, DefaultAdditionalButton);
		}


		public CMessageBoxEx(string messageBoxText,
								string caption,
								MessageBoxButton button,
								MessageBoxImage icon,
								MessageBoxResult defaultResult,
								string[] AdditionalButonsTexts = null,
								int DefaultAdditionalButton = -1,
								bool DisableCloseButton = false)
		{
			InitializeComponent();

			Result = MessageBoxResult.None;
			AdditionalButtonNum = -1;
			m_DisableCloseButton = DisableCloseButton;
			Title = caption;
			txtblkText.Text = messageBoxText;

			img.Source = GlobalDefines.GetMsgBoxSystemIcon(icon);
			AddedStdButtons = AddStdButtons(button, defaultResult);
			AddAdditionalButons(AdditionalButonsTexts, DefaultAdditionalButton);
		}
		#endregion


		protected override void OnSourceInitialized(EventArgs e)
		{
			IntPtr hWnd = (new WindowInteropHelper(this)).Handle;

			IntPtr style = MyWin32Interop.GetWindowLongPtr(hWnd, MyWin32Interop.GWL_EXSTYLE);
			style = (IntPtr)((int)style | MyWin32Interop.ExtendedWindowStyles.WS_EX_DLGMODALFRAME);
			MyWin32Interop.SetWindowLongPtr(hWnd, MyWin32Interop.GWL_EXSTYLE, style);

			if (m_DisableCloseButton)
			{
				style = MyWin32Interop.GetWindowLongPtr(hWnd, MyWin32Interop.GWL_STYLE);
				style = (IntPtr)((int)style & ~MyWin32Interop.WindowStyles.WS_SYSMENU);
				MyWin32Interop.SetWindowLongPtr(hWnd, MyWin32Interop.GWL_STYLE, style);
			}			
		}


		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = m_DisableCloseButton;
		}


		private void CMessageBoxEx_Loaded(object sender, RoutedEventArgs e)
		{
			if (AddedStdButtons != null)
				GlobalDefines.MakeButtonsSameWidth(AddedStdButtons);
		}


		/// <summary>
		/// Закрывает окно, даже если m_DisableCloseButton == true
		/// </summary>
		public void PrivilegeClose()
		{
			m_DisableCloseButton = false;
			Close();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="AdditionalButonsTexts"></param>
		/// <param name="DefaultAdditionalButton">
		/// Номер кнопки, которая должна быть выбрана по умолчанию.
		/// </param>
		void AddAdditionalButons(string[] AdditionalButonsTexts, int DefaultAdditionalButton)
		{
			if (AdditionalButonsTexts != null)
			{
				bool HasStdButtons = dbButtons.Children.Count > 0;

				for (int i = 0; i < AdditionalButonsTexts.Length; i++)
				{
					Button btn = new Button()
					{
						Content = AdditionalButonsTexts[i],
						Margin = new Thickness(0, 0, 7, 0),
						IsDefault = DefaultAdditionalButton == i,
						MinWidth = 75,
						Tag = i
					};
					btn.Click += (s, e) =>
					{
						Result = MessageBoxResult.AdditionalButton;
						AdditionalButtonNum = (int)((s as Button).Tag);
						m_DisableCloseButton = false;
						DialogResult = true;
					};
					DockPanel.SetDock(btn, HasStdButtons ? Dock.Left : Dock.Right);
					dbButtons.Children.Add(btn);
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="MsgKind"></param>
		/// <param name="DefaultButton">
		/// Если None, то выбирается подходящая клавиша в зависимости от <paramref name="MsgKind"/>
		/// </param>
		Button[] AddStdButtons(MessageBoxButton MsgKind, MessageBoxResult DefaultButton)
		{
			Button [] result = null;

			switch (MsgKind)
			{
				case MessageBoxButton.None:
					break;

				case MessageBoxButton.OKCancel:
					if (DefaultButton == MessageBoxResult.None)
						DefaultButton = MessageBoxResult.OK;

					result = new Button[2];
					result[0] = AddStdButton(MessageBoxResult.OK, DefaultButton);
					result[1] = AddStdButton(MessageBoxResult.Cancel, DefaultButton);
					break;

				case MessageBoxButton.YesNo:
					if (DefaultButton == MessageBoxResult.None)
						DefaultButton = MessageBoxResult.Yes;

					result = new Button[2];
					result[0] = AddStdButton(MessageBoxResult.Yes, DefaultButton);
					result[1] = AddStdButton(MessageBoxResult.No, DefaultButton);
					break;

				case MessageBoxButton.YesNoCancel:
					if (DefaultButton == MessageBoxResult.None)
						DefaultButton = MessageBoxResult.Yes;

					result = new Button[3];
					result[0] = AddStdButton(MessageBoxResult.Yes, DefaultButton);
					result[1] = AddStdButton(MessageBoxResult.No, DefaultButton);
					result[2] = AddStdButton(MessageBoxResult.Cancel, DefaultButton);
					break;

				default:
					if (DefaultButton == MessageBoxResult.None)
						DefaultButton = MessageBoxResult.OK;
					AddStdButton(MessageBoxResult.OK, DefaultButton);
					break;
			}

			return result;
		}


		Button AddStdButton(MessageBoxResult ButtonKind, MessageBoxResult DefaultButton)
		{
			Button btn = new Button()
			{
				Margin = new Thickness(7, 0, 0, 0),
				IsDefault = DefaultButton == ButtonKind,
				MinWidth = 75,
				Tag = ButtonKind
			};
			btn.Click += (s, e) =>
			{
				Result = (MessageBoxResult)((s as Button).Tag);
				m_DisableCloseButton = false;
				DialogResult = true;
			};
			
			switch (ButtonKind)
			{
				case MessageBoxResult.OK:
					btn.Content = Properties.Resources.resOK;
					break;

				case MessageBoxResult.Cancel:
					btn.Content = Properties.Resources.resCancel;
					btn.Style = new Style(typeof(Button));
					break;

				case MessageBoxResult.Yes:
					btn.Content = Properties.Resources.resYes;
					break;

				case MessageBoxResult.No:
					btn.Content = Properties.Resources.resNo;
					break;
			}
			
			DockPanel.SetDock(btn, Dock.Right);
			dbButtons.Children.Add(btn);

			return btn;
		}
	}
}
