using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FamilyTree.Utils
{
    public class ActionCommand : ICommand
    {
        private readonly Action<Object> _executeAction;
        private readonly Func<Object, bool> _canExecuteFunc;

        public ActionCommand(INotifyPropertyChanged vm, Action<Object> executeAction, Func<Object, bool> canExecuteFunc)
        {
            _executeAction = executeAction;
            _canExecuteFunc = canExecuteFunc;

            vm.PropertyChanged += (s, e) => CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteFunc == null || _canExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            if (_executeAction != null)
                _executeAction(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
