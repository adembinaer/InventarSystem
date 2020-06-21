using System;
using System.Diagnostics;
using System.Windows.Input;

namespace LogSystem
{
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
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
            return _canExecute == null || _canExecute(param);
        }
        public void Execute(object param)
        {
            _execute(param);
        }
    }
}

