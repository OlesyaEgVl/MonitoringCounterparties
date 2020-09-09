using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ContragentAnalyse.ViewModel.Commands
{
    public class UnloadHistory : ICommand
    {
        public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        MessageBox.Show($"Сохранить историю");
    }
}
}
