using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFProject.ViewModel;

namespace WPFProject.View
{
    /// <summary>
    /// NoticeJBNU.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NoticeJBNU : Window
    {
        public NoticeJBNU()
        {
            InitializeComponent();

            NoticeVM vm = new NoticeVM();

            vm.ScrapNotice.setScrapButton(new Button[] { scrap0, scrap1, scrap2, scrap3, scrap4, scrap5, scrap6, scrap7, scrap8 }, new Image[] { scrap0Img, scrap1Img, scrap2Img, scrap3Img, scrap4Img, scrap5Img, scrap6Img, scrap7Img, scrap8Img });
            vm.SetBut(notice1, notice2, notice3, notice4, notice5, notice6, notice7, notice8, notice9);
            vm.SetOthers(noticeStack, ScrapButtons, noticeText, noticeTitle, noticeView, Division);
            
            System.Windows.Application.Current.Exit += vm.WPF_Closing;

            this.DataContext = vm;
        }
    }
}
