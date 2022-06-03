using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFProject.ViewModel.Commands
{
    class SearchCommand : ICommand
    {
        public NoticeVM VM { get; set; }
        public SearchCommand(NoticeVM vm)
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
            VM.MainSerach_Click();
        }
    }
}
