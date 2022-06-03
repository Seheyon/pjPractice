using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFProject.ViewModel
{
    class TextboxVM : INotifyPropertyChanged
    {
        private string txt;
        public string TXT
        {
            get { return txt; }
            set {
                txt = value;
                OnPropertyChanged("txt");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
