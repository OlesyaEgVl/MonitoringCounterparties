using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ContragentAnalyse.ViewModel.Commands
{
    public class MyCommand<T> : ICommand
    {
#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
        private Action<T> Action { get; set; }
        public MyCommand(Action<T> action)
        {
            Action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is T t && parameter != null)
            {
                Action?.Invoke(t);
            }
        }
    }
}
