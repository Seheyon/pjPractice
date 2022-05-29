using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;

using pjPractice.buttons;
using pjPractice.items;

namespace pjPractice
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        ButControl btc;
        SerachBut searchBut;
        AllItems items;
        ScrapButton scarpbut;

        public MainWindow()
        {
            InitializeComponent();

            items = new AllItems(new Button[] { notice1, notice2, notice3, notice4, notice5, notice6, notice7, notice8, notice9 }, leftButton, rightButton, noticeStack, ScrapButtons, noticeText);
            btc = new ButControl(items);
            
            items.SetOtherButtons(Division, SerachText, Serach, noticeTitle, noticeView, NewSetting, home, backPage, Scrap);
            
            scarpbut = new ScrapButton(new Button[] { scrap0, scrap1, scrap2, scrap3, scrap4, scrap5, scrap6, scrap7, scrap8 }, new Image[] { scrap0Img, scrap1Img, scrap2Img, scrap3Img, scrap4Img, scrap5Img, scrap6Img, scrap7Img, scrap8Img }, items);

            searchBut = new SerachBut(items, true, btc);
            items.SetClasses(btc, searchBut, scarpbut);
            items.SetMenu(new Button[] { menu1, menu2, menu3, menu4 });

            items.setBut(items.LastnoticeNumber);

            Division.SelectedIndex = 0;

            System.Windows.Application.Current.Exit += items.WPF_Closing;
            
        }

        private void SerachText_key(object sender, KeyEventArgs e) //누르면 문자열 초기화
        {
            if (e.Key == Key.Enter)
            {
                items.MainSerach_Click(sender, e);
            }
        }
    }
}
