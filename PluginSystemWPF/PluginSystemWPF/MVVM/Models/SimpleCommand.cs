using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PluginSystemWPF.MVVM.Models
{
    class SimpleCommand : ICommand
    {
        private Action _execute;
        private Predicate<object> _canExecute;

        private event EventHandler _canExecuteChangedInternal;

        private static Predicate<object> _defaultCanExecute = (_) => true;

        public SimpleCommand(Action execute) : this(execute, _defaultCanExecute) { }

        public SimpleCommand(Action execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this._canExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                this._canExecuteChangedInternal -= value;
            }
        }

        public void OnCanExecuteChanged()
        {
            _canExecuteChangedInternal?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }

    public class SimpleCommand<T> : ICommand
    {
        private Action<T> _execute;
        private Predicate<object> _canExecute;

        private event EventHandler _canExecuteChangedInternal;

        private static Predicate<object> _defaultCanExecute = (_) => true;

        public SimpleCommand(Action<T> execute) : this(execute, _defaultCanExecute) { }

        public SimpleCommand(Action<T> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this._canExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                this._canExecuteChangedInternal -= value;
            }
        }

        public void OnCanExecuteChanged()
        {
            _canExecuteChangedInternal?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            var res = _canExecute(parameter);
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}
