using System;
using System.Windows.Input;

namespace Uno.Disasm
{
    public class Command : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _optionalCanExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Action<object> handler, Predicate<object> optionalCanExecute = null)
        {
            _execute = handler;
            _optionalCanExecute = optionalCanExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_optionalCanExecute != null)
                return _optionalCanExecute(parameter);

            return true;
        }

        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
