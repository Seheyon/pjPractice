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
    class ButControl
    {
        protected static ChromeDriverService _driverService = null;
        protected static ChromeOptions _opt = null;
        protected static ChromeDriver _driver = null;

        string number, category, title, date, seeing, webAddress;
        string[] value = new string[7];
        public int[] NoticeNumber = new int[9];

        AllItems items;

        public string saveURL;
        public int SaveIndex = 0, SaveType = 0, first_index, last_index;
        string savetxt, row;

        public ButControl(AllItems items)
        {
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _opt = new ChromeOptions();
            _opt.AddArgument("disable-gpu");
            this.items = items;
        }

        public void Division_Choice(string txt)
        {
            savetxt = txt;
            last_index = items.StringIndex;
            Setbut_Div(last_index);
        }

        public void Left_div_Click(object sender, RoutedEventArgs e) //좌우 이동 오류
        {
            int count = 8;
            int n = first_index;
            
            while (count > -1)
            {
                n++;
                if (n < 0 || n > items.StringIndex)
                {
                    first_index = items.StringIndex;
                    return;
                }

                row = items.AllString[n]; 
                value = row.Split('\t');

                if (value[1] == savetxt)
                {
                    NoticeNumber[count] = n;

                    items.urls[count] = value[5];
                    items.buts[count].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";
                    
                    if (count == 8)
                        last_index = n;
                    count--;
                }
            }
            first_index = n;
            items.scrapbut.ScrapInitialize_Other(NoticeNumber);
        }

        public void Right_div_Click(object sender, RoutedEventArgs e) //
        {
            Setbut_Div(last_index - 1);
        }

        private void Setbut_Div(int n)
        {
            int count = 0;
            first_index = n + 1; //더큼, new
            while(count < 9)
            {
                n--;
                if (n < 0 || n > items.StringIndex) {
                    items.SetEmptyPage(count);
                    return;
                }

                row = items.AllString[n]; 
                value = row.Split('\t');
                
                if(value[1] == savetxt)
                {
                    NoticeNumber[count] = n;

                    items.urls[count] = value[5];
                    items.buts[count].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";
                    
                    if (count == 0)
                        first_index = n;
                    count++;
                }
            }
            last_index = n;
            items.scrapbut.ScrapInitialize_Other(NoticeNumber);
        }

        public void SetPage(int i)
        {
            SaveIndex = i;

            switch (SaveType)
            {
                case 2:
                    coivd19notice(saveURL, SaveIndex);
                    break;

                case 3:
                    career(saveURL, SaveIndex);
                    break;

                case 4:
                    lecture(saveURL, SaveIndex);
                    break;
            }
        }

        public void MovePage(int i)
        {
            SaveIndex += i;
            if(SaveIndex <= 0)
            {
                SaveIndex = 1;
                return;
            }

            switch (SaveType)
            {
                case 2:
                    coivd19notice(saveURL, SaveIndex);
                    break;

                case 3:
                    career(saveURL, SaveIndex);
                    break;

                case 4:
                    lecture(saveURL, SaveIndex);
                    break;
            }
        }

        public void url_load(string url, int index, int type)
        {
            saveURL = url;
            SaveIndex = index;
            SaveType = type;
            switch (type)
            {
                case 2:
                    coivd19notice(url, index);
                    break;

                case 3:
                    career(url, index);
                    break;

                case 4:
                    lecture(url, index);
                    break;
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

                items.buts[i - 1].Content = $"{title} {date}";
                items.urls[i - 1] = webAddress;
            }
        }

        private void career(string url, int index) //ok
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

                items.buts[i - 1].Content = $"{category} {title} {date}";
                items.urls[i - 1] = webAddress;
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

                items.buts[i - 1].Content = $"{title} {date}";
                items.urls[i - 1] = webAddress;
            }
        }
    }
}
