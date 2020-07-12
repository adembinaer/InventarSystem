using System;
using System.Diagnostics;
using System.Windows.Input;

namespace LogSystem
{
    public class RelayCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        [DebuggerStepThrough]
        public bool CanExecute(object param)
        {
            if (_canExecute != null)
                return _canExecute();
            return _canExecute == null;
        }
        public void Execute(object param)
        {
            _execute?.Invoke();
        }
    }
}

