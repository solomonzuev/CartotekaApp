using System;
using System.Windows.Input;

namespace CartotekaApp.Domain
{
    /// <summary>
    /// Реализация интерфейса ICommand для создания команды с возможностью выполнения и проверки состояния выполнения.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Событие, которое срабатывает при изменении возможности выполнения команды.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Создает новый экземпляр класса RelayCommand.
        /// </summary>
        /// <param name="execute">Метод, который будет выполнен при вызове команды.</param>
        /// <param name="canExecute">Метод, определяющий, может ли команда быть выполнена.</param>

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Определяет, может ли команда быть выполнена.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        /// <returns>true, если команда может быть выполнена, в противном случае — false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute.Invoke(parameter);
        }

        /// <summary>
        /// Выполняет команду.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Вызывает событие CanExecuteChanged для обновления состояния выполнения команды.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
