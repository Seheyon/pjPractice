using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.IO;
using WPFProject.ViewModel.Commands;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using WPFProject.ViewModel.Commands.MenuCommands;

namespace WPFProject.ViewModel
{
    class NoticeVM
    {
        public Button[] buts;
        StackPanel noticeStack, ScrapButtons, noticeText;
        Label noticeTitle;
        TextBlock noticeView;
        ComboBox Division;
        public LeftCommand LeftCommand { get; set; }
        public RightCommand RightCommand { get; set; }
        public NewSearchCommand NewSearchCommand { get; set; }
        public BackPageCommand BackPageCommand { get; set; }
        public HomeCommand HomeCommand { get; set; }
        public Menu1Command Menu1Command { get; set; }
        public Menu2Command Menu2Command { get; set; }
        public Menu3Command Menu3Command { get; set; }
        public Menu4Command Menu4Command { get; set; }
        public ScrapCommand ScrapCommand { get; set; }
        public TextboxVM TextboxVM { get; set; }
        public SearchCommand SearchCommand { get; set; }
        public ScrapNotice ScrapNotice { get; set; }

        protected ChromeDriverService _driverService = null;
        protected ChromeOptions _opt = null;
        protected ChromeDriver _driver = null;

        public string[] AllString;
        public int stringIndex, lastnoticeNumber, tempPage = 0, SaveIndex, SaveType;
        string number, category, title, date, seeing, webAddress, url_notice, row, Savetxt;
        string[] value = new string[7];
        string[] urls = new string[9];
        public int[] NoticeNumber = new int[9];
        int SearchIndexFirst, SearchIndexLast, first_index, last_index;
        public bool PageClicked = false, notice_menu = true, IsDivided = false, IsSearched = false, IsScrapClicked = false;

        public NoticeVM() //실행됨
        {
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _opt = new ChromeOptions();
            _opt.AddArgument("disable-gpu");

            buts = new Button[9];

            LeftCommand = new LeftCommand(this);
            RightCommand = new RightCommand(this);
            NewSearchCommand = new NewSearchCommand(this);
            BackPageCommand = new BackPageCommand(this);
            HomeCommand = new HomeCommand(this);
            Menu1Command = new Menu1Command(this);
            Menu2Command = new Menu2Command(this);
            Menu3Command = new Menu3Command(this);
            Menu4Command = new Menu4Command(this);
            ScrapCommand = new ScrapCommand(this);
            TextboxVM = new TextboxVM();
            SearchCommand = new SearchCommand(this);

            ScrapNotice = new ScrapNotice(this);

            InitializeAllString();
        }

        public void SetBut(Button but1, Button but2, Button but3, Button but4, Button but5, Button but6, Button but7, Button but8, Button but9)
        {
            buts[0] = but1;
            buts[1] = but2;
            buts[2] = but3;
            buts[3] = but4;
            buts[4] = but5;
            buts[5] = but6;
            buts[6] = but7;
            buts[7] = but8;
            buts[8] = but9;

            for (int i = 0; i < 9; i++)
                buts[i].Click += notice_Click;

            SetText(lastnoticeNumber);
        }

        public void SetOthers(StackPanel noticeStack, StackPanel ScrapButtons, StackPanel noticeText, Label noticeTitle, TextBlock noticeView, ComboBox Division)
        {
            this.noticeStack = noticeStack;
            this.ScrapButtons = ScrapButtons;
            this.noticeText = noticeText;
            this.noticeTitle = noticeTitle;
            this.noticeView = noticeView;
            this.Division = Division;
            this.Division.SelectionChanged += Division_SelectionChanged;
        }

        private void InitializeAllString()
        {
            StreamReader sr = new StreamReader(new FileStream("test.txt", FileMode.Open));
            string temp = sr.ReadToEnd();
            stringIndex = temp.Split('\n').Length - 1;

            AllString = null;
            AllString = new string[stringIndex];
            AllString = temp.Split('\n');

            lastnoticeNumber = stringIndex - 1;
            sr.Close();
        }

        private void SetText(int n)
        {
            ScrapNotice.ScrapInitialize(n);
            for (int i = 0; i < 9; i++)
            {
                if (n < 0 || n > stringIndex - 1)
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

        public void BackPage()
        {
            noticeText.Visibility = Visibility.Collapsed;
            noticeStack.Visibility = Visibility.Visible;

            if(notice_menu)
                ScrapButtons.Visibility = Visibility.Visible;
            PageClicked = false;
        }

        public void NewSearchWeb() //새로고침
        {
            if (IsScrapClicked)
                return;

            if (!notice_menu)
                return;

            if (IsDivided)
                Home_Click();

            StreamWriter sw = File.CreateText("test.txt"); //현재까지 스크랩 기록 저장
            string[] temp;
            for (int i = 0; i < stringIndex; i++)
            {
                temp = AllString[i].Split('\t');
                row = $"{temp[0]}\t{temp[1]}\t{temp[2]}\t{temp[3]}\t{temp[4]}\t{temp[5]}\t{temp[6]}";
                row = row.Replace('\r', ' ');
                sw.WriteLine(row);
            }

            sw.Close();

            sw = File.AppendText("test.txt");
            int index = 10; //n페이지 내에서만 검색, 숫자 바꾸면 됨

            _opt.AddArgument("headless");

            _driver = new ChromeDriver(_driverService, _opt);
            _driver.Navigate().GoToUrl($"https://www.jbnu.ac.kr/kor/?menuID=139&pno={index}");
            var element = _driver.FindElement(By.XPath("//*[@id='print_area']/div[2]/table/tbody/tr[1]/th"));

            row = AllString[stringIndex - 1];
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
            InitializeAllString();
            SetText(lastnoticeNumber);
        }

        public void Left_Click()
        {
            if (!notice_menu)
            {
                MovePage(-1);
                return;
            }

            if(IsScrapClicked)
            {
                ScrapNotice.ScrapLeftClick();
                return;
            }

            if(IsDivided)
            {
                Left_div_Click();
                return;
            }

            if(IsSearched)
            {
                int count = 8;
                int n = SearchIndexFirst;
                string word = TextboxVM.TXT;

                while (count > -1)
                {
                    n++;
                    if (n <= 0 || n > stringIndex)
                    {
                        SearchIndexFirst = stringIndex;
                        return;
                    }

                    row = AllString[n];
                    value = row.Split('\t');

                    if (value[2].IndexOf(word) != -1)
                    {
                        NoticeNumber[count] = n;

                        urls[count] = value[5];
                        buts[count].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";

                        if (count == 8)
                            SearchIndexLast = n;
                        count--;
                    }
                }
                SearchIndexFirst = n;
                ScrapNotice.ScrapInitialize_Other(NoticeNumber);
                return;
            }

            if (lastnoticeNumber >= AllString.Length - 1)
                return;

            lastnoticeNumber += 9;
            SetText(lastnoticeNumber);
        }

        public void Right_Click()
        {
            if (!notice_menu)
            {
                MovePage(1);
                return;
            }

            if (IsScrapClicked)
            {
                ScrapNotice.ScrapRightClick();
                return;
            }

            if(IsDivided)
            {
                Right_div_Click();
                return;
            }

            if(IsSearched)
            {
                MainNotice_Search(SearchIndexLast - 1);
                return;
            }

            if (lastnoticeNumber <= 0)
                return;
            lastnoticeNumber = tempPage;
            SetText(lastnoticeNumber);
        }

        public void Home_Click()
        {
            if (IsScrapClicked)
                ScrapClick();

            if (PageClicked)
                BackPage();

            if (IsDivided)
                Division.SelectedIndex = 0;

            if (TextboxVM.TXT != "")
            {
                TextboxVM.TXT = "";
                IsSearched = false;
                switch(SaveType)
                {
                    case 2:
                        Menu2Com();
                        break;

                    case 3:
                        Menu3Com();
                        break;
                    case 4:

                        Menu4Com();
                        break;
                }
            }

            if (!notice_menu)
            {
                SetPage(1);
                return;
            }

            lastnoticeNumber = stringIndex - 1;
            SetText(lastnoticeNumber);
        }

        //스크랩
        public void ScrapClick() //스크랩 버튼 클릭 -> 스크랩 창 이동
        {
            if (!IsScrapClicked)
            {
                IsScrapClicked = true;

                for (int i = 0; i < 9; i++)
                    buts[i].Click -= notice_Click;

                ScrapNotice.DoScrapButton();
            }
            else
            {
                IsScrapClicked = false;
                ScrapNotice.UndoScrapButton();

                for (int i = 0; i < 9; i++)
                    buts[i].Click += notice_Click;
            }
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

            if (IsScrap)
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

        //검색
        public void Serach_Click()
        {
            if (notice_menu)
                MainNotice_Search(stringIndex);
            else
                OtherNotice_Serach(1);
        }

        public void MainSerach_Click()
        {
            if (IsScrapClicked)
                ScrapClick();

            string temp = TextboxVM.TXT;
            if (PageClicked)
                BackPage();

            if (IsDivided)
                Division.SelectedIndex = 0;

            TextboxVM.TXT = temp;

            if (TextboxVM.TXT.Equals(""))
            {
                if (!notice_menu)
                {
                    MenuSetting(1, SaveType);
                    return;
                }

                if (!IsSearched)
                    return;

                Home_Click();
                IsSearched = false;
                return;
            }

            Serach_Click();

            if (IsSearched)
                return;

            IsSearched = true;
        }

        private void MainNotice_Search(int n)
        {
            int count = 0;
            string word = TextboxVM.TXT;
            SearchIndexFirst = n + 1;

            while (count < 9)
            {
                n--;
                if (n < 0 || n > stringIndex)
                {
                    SetEmptyPage(count);
                    return;
                }

                row = AllString[n];
                value = row.Split('\t');

                if (value[2].IndexOf(word) != -1)
                {
                    NoticeNumber[count] = n;

                    urls[count] = value[5];
                    buts[count].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";

                    if (count == 0)
                        SearchIndexFirst = n;
                    count++;
                }
            }
            SearchIndexLast = n;
            ScrapNotice.ScrapInitialize_Other(NoticeNumber);
        }

        private void OtherNotice_Serach(int n)
        {
            int index = n;
            string word = TextboxVM.TXT;

            switch (SaveType)
            {
                case 2:
                    url_notice = $"https://www.jbnu.ac.kr/kor/?menuID=452&subject={word}&sfv=subject&pno=";
                    break;

                case 3:
                    url_notice = $"https://www.jbnu.ac.kr/kor/?menuID=140&subject={word}&sfv=subject&pno=";
                    break;

                case 4:
                    url_notice = $"https://www.jbnu.ac.kr/kor/?menuID=141&subject={word}&sfv=subject&pno=";
                    break;
            }

            SetPage(index);

        }

        //분류
        private void Division_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsScrapClicked)
                ScrapClick();

            if (PageClicked)
                BackPage();

            if (IsSearched)
            {
                IsSearched = false;
                TextboxVM.TXT = "";
            }

            Savetxt = Division.SelectedItem.ToString();
            Savetxt = Savetxt.Substring(38);

            if (Savetxt != "전체보기")
            {
                Division_Choice();
                IsDivided = true;
            }
            else
            {
                Home_Click();
                IsDivided = false;
            }
        }

        public void Division_Choice()
        {
            last_index = stringIndex;
            Setbut_Div(last_index);
        }

        public void Left_div_Click() 
        {
            int count = 8;
            int n = first_index;

            while (count > -1)
            {
                n++;
                if (n < 0 || n > stringIndex)
                {
                    first_index = stringIndex;
                    return;
                }

                row = AllString[n];
                value = row.Split('\t');

                if (value[1] == Savetxt)
                {
                    NoticeNumber[count] = n;

                    urls[count] = value[5];
                    buts[count].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";

                    if (count == 8)
                        last_index = n;
                    count--;
                }
            }
            first_index = n;
            ScrapNotice.ScrapInitialize_Other(NoticeNumber);
        }

        public void Right_div_Click()
        {
            Setbut_Div(last_index - 1);
        }

        private void Setbut_Div(int n)
        {
            int count = 0;
            first_index = n + 1; //더큼, new
            while (count < 9)
            {
                n--;
                if (n < 0 || n > stringIndex)
                {
                    SetEmptyPage(count);
                    return;
                }

                row = AllString[n];
                value = row.Split('\t');

                if (value[1] == Savetxt)
                {
                    NoticeNumber[count] = n;

                    urls[count] = value[5];
                    buts[count].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";

                    if (count == 0)
                        first_index = n;
                    count++;
                }
            }
            last_index = n;
            ScrapNotice.ScrapInitialize_Other(NoticeNumber);
        }

        //공지 종류(메뉴) 설정
        public void Menu1Com()
        {
            TextboxVM.TXT = "";
            if (IsScrapClicked)
                ScrapClick();

            if (PageClicked)
                BackPage();

            if (IsDivided)
                Division.SelectedIndex = 0;

            notice_menu = true;
            lastnoticeNumber = stringIndex - 1;
            SetText(lastnoticeNumber);
            butsetting();
        }

        public void Menu2Com()
        {
            url_notice = "https://www.jbnu.ac.kr/kor/?menuID=452&pno=";
            MenuSetting(1, 2);
        }

        public void Menu3Com()
        {
            url_notice = "https://www.jbnu.ac.kr/kor/?menuID=140&pno=";
            MenuSetting(1, 3);
        }

        public void Menu4Com()
        {
            url_notice = "https://www.jbnu.ac.kr/kor/?menuID=141&pno=";
            MenuSetting(1, 4);
        }

        public void SetPage(int i)
        {
            SaveIndex = i;

            switch (SaveType)
            {
                case 2:
                    coivd19notice(url_notice, SaveIndex);
                    break;

                case 3:
                    career(url_notice, SaveIndex);
                    break;

                case 4:
                    lecture(url_notice, SaveIndex);
                    break;
            }
        }

        public void MovePage(int i)
        {
            SaveIndex += i;
            if (SaveIndex <= 0)
            {
                SaveIndex = 1;
                return;
            }

            switch (SaveType)
            {
                case 2:
                    coivd19notice(url_notice, SaveIndex);
                    break;

                case 3:
                    career(url_notice, SaveIndex);
                    break;

                case 4:
                    lecture(url_notice, SaveIndex);
                    break;
            }
        }

        private void MenuSetting(int index, int type)
        {
            TextboxVM.TXT = "";
            if (IsScrapClicked)
                ScrapClick();

            if (PageClicked)
                BackPage();

            if (IsDivided)
                Division.SelectedIndex = 0;

            notice_menu = false;
            SaveIndex = index;
            SaveType = type;
            switch (type)
            {
                case 2:
                    coivd19notice(url_notice, index);
                    break;

                case 3:
                    career(url_notice, index);
                    break;

                case 4:
                    lecture(url_notice, index);
                    break;
            }

            butsetting();
        }

        public void butsetting()
        {
            if (!notice_menu)
            {
                Division.Visibility = Visibility.Hidden;
                ScrapButtons.Visibility = Visibility.Hidden;
            }
            else
            {
                Division.Visibility = Visibility.Visible;
                ScrapButtons.Visibility = Visibility.Visible;
            }
        }

        private void coivd19notice(string url, int index)
        {
            _opt.AddArgument("headless");
            _driver = new ChromeDriver(_driverService, _opt);
            _driver.Navigate().GoToUrl($"{url}{index}");
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            var element = _driver.FindElement(By.XPath("//*[@id='print_area']/div[1]/table/tbody/tr[1]/th"));

            for (int i = 1; i <= 9; i++)
            {
                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[1]/table/tbody/tr[{i}]/th"));
                number = element.Text;

                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[1]/table/tbody/tr[{i}]/td[2]/span/a"));
                title = element.Text;

                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[1]/table/tbody/tr[{i}]/td[5]"));
                date = element.Text;

                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[1]/table/tbody/tr[{i}]/td[6]"));
                seeing = element.Text;

                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[1]/table/tbody/tr[{i}]/td[2]/span/a"));
                webAddress = element.GetAttribute("href");

                buts[i - 1].Content = $"{title} {date}";
                urls[i - 1] = webAddress;
            }
        }

        private void career(string url, int index) 
        {
            _opt.AddArgument("headless");
            _driver = new ChromeDriver(_driverService, _opt);
            _driver.Navigate().GoToUrl($"{url}{index}");
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            var element = _driver.FindElement(By.XPath("//*[@id='print_area']/div[2]/table/tbody/tr[1]/th[1]"));

            for (int i = 1; i <= 9; i++)
            {
                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[2]/table/tbody/tr[{i}]/th[1]"));
                number = element.Text;

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

                buts[i - 1].Content = $"{category} {title} {date}";
                urls[i - 1] = webAddress;
            }
        }

        private void lecture(string url, int index)
        {
            _opt.AddArgument("headless");
            _driver = new ChromeDriver(_driverService, _opt);
            _driver.Navigate().GoToUrl($"{url}{index}");
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            var element = _driver.FindElement(By.XPath("//*[@id='print_area']/div[1]/table/tbody/tr[1]/th[1]"));

            for (int i = 1; i <= 9; i++)
            {
                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[1]/table/tbody/tr[{i}]/th[1]"));
                number = element.Text;

                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[1]/table/tbody/tr[{i}]/td[2]/span/a"));
                title = element.Text;

                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[1]/table/tbody/tr[{i}]/td[5]"));
                date = element.Text;

                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[1]/table/tbody/tr[{i}]/td[6]"));
                seeing = element.Text;

                element = _driver.FindElement(By.XPath($"//*[@id='print_area']/div[1]/table/tbody/tr[{i}]/td[2]/span/a"));
                webAddress = element.GetAttribute("href");

                buts[i - 1].Content = $"{title} {date}";
                urls[i - 1] = webAddress;
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

        public void WPF_Closing(object sender, ExitEventArgs e) //즐겨찾기 여부를 확인하기 위해 필요
        {
            StreamWriter sw = File.CreateText("test.txt");
            string[] temp;
            for (int i = 0; i < stringIndex; i++)
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
