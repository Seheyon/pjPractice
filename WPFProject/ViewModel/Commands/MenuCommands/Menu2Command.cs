﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFProject.ViewModel.Commands.MenuCommands
{
    class Menu2Command : ICommand
    {
        public NoticeVM VM { get; set; }

        public Menu2Command(NoticeVM vm)
        {
            VM = vm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            VM.Menu2Com();
        }
    }
}
