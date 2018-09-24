using DBManager.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace DBManager.Commands
{
	public class CCommand : ICommand
	{
		/// <summary>
		/// Действие(или параметризованное действие) которое вызывается при активации команды.
		/// </summary>
		protected Action m_action = null;
		protected Action<object> m_parameterizedAction = null;

		protected Func<bool> m_CanExecuteFunc = null;

		#region Свойство CanExecute
		/// <summary>
		/// Будевое значение, отвечающие за возможность выполнения команды.
		/// </summary>
		private bool m_canExecute = false;

		/// <summary>
		/// Установка /  получение значения, отвечающего за возможность выполнения команды
		/// </summary>
		/// <value>
		///     <c>true</c> если выполнение разрешено; если запрещено - <c>false</c>.
		/// </value>
		public bool CanExecute
		{
			get { return m_canExecute; }
			set
			{
				if (m_canExecute != value)
				{
					m_canExecute = value;
					ThreadManager.Instance.InvokeUI((arg) =>
						{
							CanExecuteChanged?.Invoke(this, EventArgs.Empty);
							CommandManager.InvalidateRequerySuggested();
						},
						EventArgs.Empty);
				}
			}
		}
		#endregion


		// <summary>
		///  Вызывается, когда меняется возможность выполнения команды
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Вызывается во время выполнения команды
		/// </summary>
		public event CancelCommandEventHandler Executing;

		/// <summary>
		/// Вызывается, когда команды выполнена
		/// </summary>
		public event CommandEventHandler Executed;


		#region Конструкторы
		/// <summary>
		/// Инициализация нового экземпляра класса без параметров <see cref="Command"/>.
		/// </summary>
		/// <param name="action">Действие.</param>
		/// <param name="canExecute">Если установлено в<c>true</c> [can execute] (выполнение разрешено).</param>
		public CCommand(Action action, bool canExecute = true)
		{
			//  Set the action.
			m_action = action;
			m_canExecute = canExecute;
		}

		/// <summary>
		/// Инициализация нового экземпляра класса с параметрами <see cref="Command"/> class.
		/// </summary>
		/// <param name="parameterizedAction">Параметризированное действие.</param>
		/// <param name="canExecute"> Если установлено в <c>true</c> [can execute](выполнение разрешено).</param>
		public CCommand(Action<object> parameterizedAction, bool canExecute = true)
		{
			//  Set the action.
			m_parameterizedAction = parameterizedAction;
			m_canExecute = canExecute;
		}


		/// <summary>
		/// Инициализация нового экземпляра класса без параметров <see cref="Command"/>.
		/// </summary>
		/// <param name="action">Действие.</param>
		/// <param name="canExecute">Если установлено в<c>true</c> [can execute] (выполнение разрешено).</param>
		public CCommand(Action action, Func<bool> canExecute)
		{
			//  Set the action.
			m_action = action;
			m_CanExecuteFunc = canExecute;
			m_canExecute = m_CanExecuteFunc();
		}

		/// <summary>
		/// Инициализация нового экземпляра класса с параметрами <see cref="Command"/> class.
		/// </summary>
		/// <param name="parameterizedAction">Параметризированное действие.</param>
		/// <param name="canExecute"> Если установлено в <c>true</c> [can execute](выполнение разрешено).</param>
		public CCommand(Action<object> parameterizedAction, Func<bool> canExecute)
		{
			//  Set the action.
			m_parameterizedAction = parameterizedAction;
			m_CanExecuteFunc = canExecute;
		}
		#endregion


		#region Интерфейс ICommand
		/// <summary>
		/// Определяем метод, определющий, что выполнение команды допускается в текущем состоянии
		/// </summary>
		/// <param name="parameter">Этот параметр используется командой.
		///  Если команда вызывается без использования параметра,
		///  то этот объект может быть установлен в null.</param>
		/// <returns>
		/// <c>true</c> если выполнение команды разрешено; если запрещено - false.
		/// </returns>
		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute;
		}

		/// <summary>
		/// Задание метода, который будет вызван при активации команды.
		/// </summary>
		/// <param name="parameter"> Этот параметр используется командой.
		///  Если команда вызывается без использования параметра,
		///  то этот объект может быть установлен в  null.</param>
		void ICommand.Execute(object parameter)
		{
			DoExecute(parameter);
		}
		#endregion


		#region Выполнение команды
		/// <summary>
		/// Выполнение команды
		/// </summary>
		/// <param name="param">The param.</param>
		public virtual void DoExecute(object param = null)
		{
			//  Вызывает выполнении команды с возможностью отмены
			CancelCommandEventArgs args =
			   new CancelCommandEventArgs() { Parameter = param, Cancel = false };
			InvokeExecuting(args);

			//  Если событие было отменено -  останавливаем.
			if (args.Cancel)
				return;

			//  Вызываем действие с / без параметров, в зависимости от того. Какое было устанвленно.
			InvokeAction(param);

			//  Call the executed function.
			InvokeExecuted(new CommandEventArgs() { Parameter = param });
		}
		

		protected void InvokeAction(object param)
		{
			ThreadManager.Instance.InvokeUI((arg) =>
				{
					if (m_action != null)
						m_action();
					else
						m_parameterizedAction?.Invoke(arg);
				},
				param);
		}

		protected void InvokeExecuted(CommandEventArgs args)
		{
			//  Вызвать все события
			ThreadManager.Instance.InvokeUI((arg) =>
				{
					Executed?.Invoke(this, arg);
				},
				args);
		}

		protected void InvokeExecuting(CancelCommandEventArgs args)
		{
			//  Call the executed event.
			ThreadManager.Instance.InvokeUI((arg) =>
				{
					Executing?.Invoke(this, arg);
				},
				args);
		}
		#endregion


		public void RefreshCanExecute()
		{
			ThreadManager.Instance.InvokeUI(() =>
				{
					CanExecute = m_CanExecuteFunc();
				});
		}
	}


	/// <summary>
	/// The CommandEventHandler delegate.
	/// </summary>
	public delegate void CommandEventHandler(object sender, CommandEventArgs args);


	/// <summary>
	/// The CancelCommandEvent delegate.
	/// </summary>
	public delegate void CancelCommandEventHandler(object sender, CancelCommandEventArgs args);


	/// <summary>
	/// CommandEventArgs - simply holds the command parameter.
	/// </summary>
	public class CommandEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the parameter.
		/// </summary>
		/// <value>The parameter.</value>
		public object Parameter { get; set; }
	}

	/// <summary>
	/// CancelCommandEventArgs - just like above but allows the event to 
	/// be cancelled.
	/// </summary>
	public class CancelCommandEventArgs : CommandEventArgs
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CancelCommandEventArgs"/> command should be cancelled.
		/// </summary>
		/// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
		public bool Cancel { get; set; }
	}
}
