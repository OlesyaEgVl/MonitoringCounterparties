using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ContragentAnalyse.ViewModel.Commands
{
    public class MyCommand : ICommand
    {
#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        private Action Action { get; set; }
        public MyCommand(Action action)
        {
            Action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action?.Invoke();
        }

        public static implicit operator MyCommand(MyCommand<string> v)
        {
            throw new NotImplementedException();
        }
    }
}
