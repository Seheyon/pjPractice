using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using pjPractice.buttons;

namespace pjPractice.items
{
    class AllItems
    {
        protected ChromeDriverService _driverService = null;
        protected ChromeOptions _opt = null;
        protected ChromeDriver _driver = null;

        public System.Windows.Controls.Button[] buts = new System.Windows.Controls.Button[9];
        System.Windows.Controls.Button[] menuBut = new System.Windows.Controls.Button[4];
        System.Windows.Controls.Button LastMenu;

        public System.Windows.Controls.Button LeftBut, RightBut;
        private System.Windows.Controls.Button Search, NewSetting, home, backPage, Scrap;
        
        System.Windows.Controls.ComboBox Division;
        System.Windows.Controls.TextBox SerachText;
        public string[] AllString;

        string number, category, title, date, seeing, webAddress, url_notice;

        public ButControl btc;
        public SerachBut searchBut;
        public ScrapButton scrapbut;
        System.Windows.Controls.Label noticeTitle;
        TextBlock noticeView;
        string row;
        string[] value = new string[7];
        public string[] urls = new string[9];
        public bool PageClicked = false;
        public bool notice_menu = true, notice_div = false, notice_search = false, IsScrapClicked = false;

        private int stringIndex, lastnoticeNumber, tempPage = 0;
        StackPanel noticeStack, ScrapButtons, noticeText;
        public int StringIndex
        {
            get { return stringIndex; }
            set { stringIndex = value; }
        }

        public int LastnoticeNumber
        {
            get { return lastnoticeNumber; }
            set { lastnoticeNumber = value; }
        }
        public AllItems(System.Windows.Controls.Button[] buts, System.Windows.Controls.Button LeftBut, System.Windows.Controls.Button RightBut, StackPanel noticeStack, StackPanel ScrapButtons, StackPanel noticeText)
        {
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _opt = new ChromeOptions();
            _opt.AddArgument("disable-gpu");

            this.buts = buts;
            this.LeftBut = LeftBut;
            this.RightBut = RightBut;
            this.noticeStack = noticeStack;
            this.ScrapButtons = ScrapButtons;
            this.noticeText = noticeText;

            for (int i = 0; i < 9; i++)
                this.buts[i].Click += notice_Click;

            this.LeftBut.Click += Left_Click;
            this.RightBut.Click += Right_Click;

            SetAllStrings();
        }

        public void SetMenu(System.Windows.Controls.Button[] menuBut)
        {
            this.menuBut = menuBut;
            for (int i = 0; i < 4; i++)
                this.menuBut[i].Click += menu_Click;
        }

        public void SetClasses(ButControl btc, SerachBut searchBut, ScrapButton scrapbut)
        {
            this.btc = btc;
            this.searchBut = searchBut;
            this.scrapbut = scrapbut;
        }

        public void SetOtherButtons(System.Windows.Controls.ComboBox Division, System.Windows.Controls.TextBox SerachText, System.Windows.Controls.Button Search , System.Windows.Controls.Label noticeTitle, TextBlock noticeView, System.Windows.Controls.Button NewSetting, System.Windows.Controls.Button home, System.Windows.Controls.Button backPage, System.Windows.Controls.Button Scrap)
        {
            this.Division = Division;
            this.SerachText = SerachText;
            this.Search = Search;
            this.noticeTitle = noticeTitle;
            this.noticeView = noticeView;
            this.NewSetting = NewSetting;
            this.home = home;
            this.backPage = backPage;
            this.Scrap = Scrap;

            this.Scrap.Click += ScrapClick;
            this.backPage.Click += BackPage;
            this.Division.SelectionChanged += Division_SelectionChanged;
            this.NewSetting.Click += SearchWeb;
            this.home.Click += Home_Click;
            this.Search.Click += MainSerach_Click;
        }

        public void SetAllStrings()
        {
            StreamReader sr = new StreamReader(new FileStream("test.txt", FileMode.Open));
            string temp = sr.ReadToEnd();
            StringIndex = temp.Split('\n').Length - 1;

            AllString = null;
            AllString = new string[StringIndex];
            AllString = temp.Split('\n');
            
            LastnoticeNumber = StringIndex - 1;
            sr.Close();
        }

        private void ScrapClick(object sender, RoutedEventArgs e)
        {
            if (!IsScrapClicked)
            {
                IsScrapClicked = true;
                LeftBut.Click -= Left_Click;
                RightBut.Click -= Right_Click;

                for (int i = 0; i < 9; i++)
                    buts[i].Click -= notice_Click;

                scrapbut.DoScrapButton(LeftBut, RightBut);

            } else
            {
                IsScrapClicked = false;
                scrapbut.UndoScrapButton(LeftBut, RightBut);

                for (int i = 0; i < 9; i++)
                    buts[i].Click += notice_Click;

                LeftBut.Click += Left_Click;
                RightBut.Click += Right_Click;
            }
        }

        public void SetEmptyPage(int n)
        {
            while (n < 9)
            {
                urls[n] = "";
                buts[n].Content = "";
                n++;
            }
        }

        public void MainSerach_Click(object sender, RoutedEventArgs e)
        {
            if (IsScrapClicked)
                ScrapClick(sender, e);

            string temp = SerachText.Text;
            if (PageClicked)
                BackPage(sender, e);

            if (notice_div)
                Division.SelectedIndex = 0;

            SerachText.Text = temp;

            if (SerachText.Text.Equals(""))
            {
                if (!notice_menu) { 
                    menu_Click(LastMenu, e);
                    return;
                }

                if (!notice_search)
                    return;

                Home_Click(sender, e);

                LeftBut.Click -= searchBut.Left_Search_Click;
                RightBut.Click -= searchBut.Right_Search_Click;

                LeftBut.Click += Left_Click;
                RightBut.Click += Right_Click;

                notice_search = false;

                return;
            }

            searchBut.SetSearchText(SerachText.Text);
            searchBut.Serach_Click();

            if (notice_search)
                return;

            LeftBut.Click -= Left_Click;
            RightBut.Click -= Right_Click;

            LeftBut.Click += searchBut.Left_Search_Click;
            RightBut.Click += searchBut.Right_Search_Click;

            notice_search = true;
        }

        private void SearchWeb(object sender, RoutedEventArgs e)
        {
            if (IsScrapClicked)
                ScrapClick(sender, e);

            StreamWriter sw = File.AppendText("test.txt");
            int index = 10; //n페이지 내에서만 검색, 숫자 바꾸면 됨

            _opt.AddArgument("headless");


            _driver = new ChromeDriver(_driverService, _opt);
            _driver.Navigate().GoToUrl($"https://www.jbnu.ac.kr/kor/?menuID=139&pno={index}");
            var element = _driver.FindElement(By.XPath("//*[@id='print_area']/div[2]/table/tbody/tr[1]/th"));

            row = AllString[StringIndex - 1];
            value = row.Split('\t');

            int valueTemp, numberTemp;
            int.TryParse(value[0], out valueTemp);

            while (index > 0)
            {
                _driver.Navigate().GoToUrl($"https://www.jbnu.ac.kr/kor/?menuID=139&pno={index}");
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                for (int i = 9; i > 0; i--)
                {
                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/th"));
                    number = element.Text;

                    if (int.TryParse(number, out numberTemp))
                    {
                        if (valueTemp >= numberTemp)
                            continue;
                    }

                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/td[1]/span"));
                    category = element.Text;

                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/td[2]/span/a"));
                    title = element.Text;

                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/td[5]"));
                    date = element.Text;

                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/td[6]"));
                    seeing = element.Text;

                    element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/td[2]/span/a"));
                    webAddress = element.GetAttribute("href");

                    sw.WriteLine($"{number}\t{category}\t{title}\t{date}\t{seeing}\t{webAddress}\t0");
                }
                index--;
            }
            sw.Close();
            SetAllStrings();
            setBut(LastnoticeNumber);
        }

        public void setBut(int n)
        {
            scrapbut.ScrapInitialize(n);
            for (int i = 0; i < 9; i++)
            {
                if (n < 0 || n > StringIndex - 1)
                {
                    return;
                }

                row = AllString[n];
                value = row.Split('\t');
                urls[i] = value[5];
                buts[i].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";
                n--;
            }
            tempPage = n;
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            if (!notice_menu)
            {
                btc.MovePage(-1);
                return;
            }

            if (LastnoticeNumber >= AllString.Length - 1)
                return;

            LastnoticeNumber += 9;
            setBut(LastnoticeNumber);
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            if (!notice_menu)
            {
                btc.MovePage(1);
                return;
            }

            if (LastnoticeNumber <= 0)
                return;
            LastnoticeNumber = tempPage;
            setBut(LastnoticeNumber);
        }

        private void Division_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsScrapClicked)
                ScrapClick(sender, e);

            if (PageClicked)
                BackPage(sender, e);

            if (notice_search)
            {
                LeftBut.Click -= searchBut.Left_Search_Click;
                RightBut.Click -= searchBut.Right_Search_Click;

                LeftBut.Click += Left_Click;
                RightBut.Click += Right_Click;
                notice_search = false;

                SerachText.Text = "";
            }

            string txt = Division.SelectedItem.ToString();
            txt = txt.Substring(38);

            if (txt != "전체보기")
            {
                btc.Division_Choice(txt);

                if (notice_div)
                    return;

                notice_div = true;

                LeftBut.Click -= Left_Click;
                RightBut.Click -= Right_Click;

                LeftBut.Click += btc.Left_div_Click;
                RightBut.Click += btc.Right_div_Click;
            }
            else
            {
                Home_Click(sender, e);
                if (!notice_div)
                    return;

                notice_div = false;

                LeftBut.Click -= btc.Left_div_Click;
                RightBut.Click -= btc.Right_div_Click;

                LeftBut.Click += Left_Click;
                RightBut.Click += Right_Click;
            }
        }

        private void notice_Click(object sender, RoutedEventArgs e) //공지 사항 창 띄우기
        {
            System.Windows.Controls.Button but = (System.Windows.Controls.Button)sender;
            string num = but.Tag.ToString();
            int tempnum = 1;
            if (int.TryParse(num, out tempnum)) { }

            if (urls[tempnum].Equals(""))
                return;

            _opt.AddArgument("headless");

            _driver = new ChromeDriver(_driverService, _opt);
            _driver.Navigate().GoToUrl($"{urls[tempnum]}");
            var element = _driver.FindElement(By.XPath("//*[@id='form_view']/div/div[3]/div"));
            var titleElement = _driver.FindElement(By.XPath("//*[@id='form_view']/div/div[1]/p"));
            string tempTitle = titleElement.Text;

            noticeStack.Visibility = Visibility.Collapsed;
            ScrapButtons.Visibility = Visibility.Collapsed;
            noticeText.Visibility = Visibility.Visible;

            noticeTitle.Content = tempTitle;
            noticeView.Text = element.Text;
            PageClicked = true;
        }

        public void ScrapNoticeClick(string str, string title)
        {
            _opt.AddArgument("headless");

            _driver = new ChromeDriver(_driverService, _opt);
            _driver.Navigate().GoToUrl($"{str}");
            var element = _driver.FindElement(By.XPath("//*[@id='form_view']/div/div[3]/div"));

            noticeStack.Visibility = Visibility.Collapsed;
            ScrapButtons.Visibility = Visibility.Collapsed;
            noticeText.Visibility = Visibility.Visible;

            noticeTitle.Content = title;
            noticeView.Text = element.Text;
            PageClicked = true;
        }

        public void MainSettingAllstring(int index, bool IsScrap)
        {
            row = AllString[index];
            value = row.Split('\t');

            if(IsScrap)
                AllString[index] = $"{value[0]}\t{value[1]}\t{value[2]}\t{value[3]}\t{value[4]}\t{value[5]}\t1";
            else
                AllString[index] = $"{value[0]}\t{value[1]}\t{value[2]}\t{value[3]}\t{value[4]}\t{value[5]}\t0";

        }

        public void SettingAllstring(int lastnotice, bool IsScrap)
        {
            row = AllString[lastnotice];
            value = row.Split('\t');

            if (IsScrap)
                AllString[lastnotice] = $"{value[0]}\t{value[1]}\t{value[2]}\t{value[3]}\t{value[4]}\t{value[5]}\t1";
            else
                AllString[lastnotice] = $"{value[0]}\t{value[1]}\t{value[2]}\t{value[3]}\t{value[4]}\t{value[5]}\t0";
        }

        public void BackPage(object sender, RoutedEventArgs e)
        {
            noticeText.Visibility = Visibility.Collapsed;
            noticeStack.Visibility = Visibility.Visible;
            ScrapButtons.Visibility = Visibility.Visible;
            PageClicked = false;
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            if (IsScrapClicked)
                ScrapClick(sender, e);

            if (PageClicked)
                BackPage(sender, e);

            if (notice_div)
                Division.SelectedIndex = 0;

            if (SerachText.Text != "")
            {
                SerachText.Text = "";
                MainSerach_Click(sender, e);
            }

            if (!notice_menu)
            {
                btc.SetPage(1);
                return;
            }

            LastnoticeNumber = StringIndex - 1;
            setBut(LastnoticeNumber);
        }

        private void menu_Click(object sender, RoutedEventArgs e)
        {
            SerachText.Text = "";
            if (IsScrapClicked)
                ScrapClick(sender, e);

            if (PageClicked)
                BackPage(sender, e);

            if (notice_div)
                Division.SelectedIndex = 0;

            System.Windows.Controls.Button but = (System.Windows.Controls.Button)sender;
            LastMenu = but;
            string num = but.Tag.ToString();
            switch (num)
            {
                case "1":
                    notice_menu = true;
                    LastnoticeNumber = StringIndex - 1;
                    setBut(LastnoticeNumber);
                    break;

                case "2":
                    notice_menu = false;
                    url_notice = "https://www.jbnu.ac.kr/kor/?menuID=452&pno=";
                    btc.url_load(url_notice, 1, 2);
                    break;

                case "3":
                    notice_menu = false;
                    url_notice = "https://www.jbnu.ac.kr/kor/?menuID=140&pno=";
                    btc.url_load(url_notice, 1, 3);
                    break;

                case "4":
                    notice_menu = false;
                    url_notice = "https://www.jbnu.ac.kr/kor/?menuID=141&pno=";
                    btc.url_load(url_notice, 1, 4);
                    break;
            }
            searchBut.Notice_menu = notice_menu;
            butsetting();
        }

        public void butsetting()
        {
            if (!notice_menu)
            {
                NewSetting.Visibility = Visibility.Hidden;
                Division.Visibility = Visibility.Hidden;
                ScrapButtons.Visibility = Visibility.Hidden;
            }
            else
            {
                NewSetting.Visibility = Visibility.Visible;
                Division.Visibility = Visibility.Visible;
                ScrapButtons.Visibility = Visibility.Visible;
            }
        }

        public void WPF_Closing(object sender, ExitEventArgs e)
        { 
            StreamWriter sw = File.CreateText("test.txt");
            string[] temp;
            for(int i = 0; i < StringIndex; i++)
            {
                temp = AllString[i].Split('\t');
                row = $"{temp[0]}\t{temp[1]}\t{temp[2]}\t{temp[3]}\t{temp[4]}\t{temp[5]}\t{temp[6]}";
                row = row.Replace('\r', ' ');
                sw.WriteLine(row);
            }
               
            sw.Close();
        }
    }
}
