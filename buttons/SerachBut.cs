using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using pjPractice.items;

namespace pjPractice.buttons
{
    class SerachBut
    {
        protected static ChromeDriverService _driverService = null;
        protected static ChromeOptions _opt = null;
        protected static ChromeDriver _driver = null;

        string word, row;
        public int[] NoticeNumber = new int[9];
        string[] value = new string[7];
        int  SearchIndexFirst, SearchIndexLast;

        private bool notice_menu;
        public bool Notice_menu
        {
            get { return notice_menu; }
            set { notice_menu = value; }
        }


        ButControl btc;
        AllItems items;

        public SerachBut() {
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _opt = new ChromeOptions();
            _opt.AddArgument("disable-gpu");
        }

        public SerachBut(AllItems items, bool notice_menu, ButControl btc)
        {
            this.items = items;
            Notice_menu = notice_menu;
            this.btc = btc;
        }

        public void SetSearchText(string word)
        {
            this.word = word;
        }

        public void Serach_Click()
        {
            if (Notice_menu)
                MainNotice_Search(items.StringIndex);
            else
                OtherNotice_Serach(1);
        }

        private void MainNotice_Search(int n)
        {
            int count = 0;
            SearchIndexFirst = n + 1;

            while (count < 9)
            {
                n--;
                if (n < 0 || n > items.StringIndex) {
                    items.SetEmptyPage(count);
                    return;
                }

                row = items.AllString[n]; 
                value = row.Split('\t');
                
                if(value[2].IndexOf(word) != -1)
                {
                    NoticeNumber[count] = n;

                    items.urls[count] = value[5];
                    items.buts[count].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";
                    
                    if (count == 0)
                        SearchIndexFirst = n;
                    count++;
                }
            }
            SearchIndexLast = n;
            items.scrapbut.ScrapInitialize_Other(NoticeNumber);
        }

        public void Left_Search_Click(object sender, RoutedEventArgs e)
        {
            int count = 8;
            int n = SearchIndexFirst;

            if (!Notice_menu) { 
                btc.MovePage(-1);
                return;
            }

            while (count > -1)
            {
                n++;
                if (n <= 0 || n > items.StringIndex)
                {
                    SearchIndexFirst = items.StringIndex;
                    return;
                }

                row = items.AllString[n];
                value = row.Split('\t');

                if (value[2].IndexOf(word) != -1)
                {
                    NoticeNumber[count] = n;

                    items.urls[count] = value[5];
                    items.buts[count].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";

                    if (count == 8)
                        SearchIndexLast = n;
                    count--;
                }
            }
            SearchIndexFirst = n;
            items.scrapbut.ScrapInitialize_Other(NoticeNumber);
        }

        public void Right_Search_Click(object sender, RoutedEventArgs e)
        {
            if (!Notice_menu) {
                btc.MovePage(1);
                return;
            }
            MainNotice_Search(SearchIndexLast - 1);
        }

        private void OtherNotice_Serach(int n)
        {
            string url = btc.saveURL;
            int index = n;
            int type = btc.SaveType;

            switch (type)
            {
                case 2:
                    url = $"https://www.jbnu.ac.kr/kor/?menuID=452&subject={word}&sfv=subject&pno=";
                    break;

                case 3:
                    url = $"https://www.jbnu.ac.kr/kor/?menuID=140&subject={word}&sfv=subject&pno=";
                    break;

                case 4:
                    url = $"https://www.jbnu.ac.kr/kor/?menuID=141&subject={word}&sfv=subject&pno=";
                    break;
            }

            btc.url_load(url, index, type);
            
        }

    }
}
