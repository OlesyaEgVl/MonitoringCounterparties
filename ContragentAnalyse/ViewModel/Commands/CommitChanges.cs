using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ContragentAnalyse.ViewModel.Commands
{
    public class CommitChanges : ICommand
    {
        public event EventHandler CanExecuteChanged;
        Action _action;
        public CommitChanges(Action action)
        {
            _action = action;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }
    }
}
