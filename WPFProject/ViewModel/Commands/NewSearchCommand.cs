using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFProject.ViewModel.Commands
{
    class NewSearchCommand : ICommand
    {
        public NoticeVM VM { get; set; }
        public event EventHandler CanExecuteChanged;

        public NewSearchCommand(NoticeVM vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            VM.NewSearchWeb();
        }
    }
}
