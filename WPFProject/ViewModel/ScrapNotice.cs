using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFProject.ViewModel
{
    class ScrapNotice
    {
        public NoticeVM VM { get; set; }
        public ScrapNotice(NoticeVM vm)
        {
            VM = vm;
            SetAllStrings();
        }

        BitmapImage CheckedImage, UncheckedImage;
        Image[] Scrapimages = new Image[9];
        Button[] scrapbuts = new Button[9];
        string row;
        string[] value = new string[7];
        string[] newValue = new string[8];
        string[] ScrapAllString;
        int scrapindex, lastScrapNumber, tempPage;

        public void setScrapButton(Button[] scrapbuts, Image[] Scrapimages)
        {
            this.scrapbuts = scrapbuts;
            this.Scrapimages = Scrapimages;

            for (int i = 0; i < 9; i++)
                this.scrapbuts[i].Click += Scrapbtn_Click;
            SetImage();
        }

        private void SetAllStrings()
        {
            StreamReader sr = new StreamReader(new FileStream("scrap.txt", FileMode.Open));
            string temp = sr.ReadToEnd();
            scrapindex = temp.Split('\n').Length - 1;

            ScrapAllString = null;
            ScrapAllString = new string[scrapindex];
            ScrapAllString = temp.Split('\n');

            lastScrapNumber = scrapindex - 1;
            sr.Close();
        }

        private void Scrapbtn_Click(object sender, RoutedEventArgs e)
        {
            if (!VM.notice_menu)
                return;

            Button but = (Button)sender;
            string num = but.Tag.ToString();
            int tempnum = 1;
            if (int.TryParse(num, out tempnum)) { }

            Scrapimages[tempnum].Stretch = Stretch.None;

            if (Scrapimages[tempnum].Tag.Equals("0"))
            {
                if (VM.IsScrapClicked)
                    return;
                Scrapimages[tempnum].Source = CheckedImage;
                Scrapimages[tempnum].Tag = "1";
                SaveScrap(tempnum);
            }
            else
            {
                Scrapimages[tempnum].Source = UncheckedImage;
                Scrapimages[tempnum].Tag = "0";
                DeleteScrap(tempnum);
            }
        }

        private void SetImage()
        {
            UncheckedImage = new BitmapImage();
            UncheckedImage.BeginInit();
            UncheckedImage.UriSource = new Uri("https://img.icons8.com/material-outlined/24/000000/file.png", UriKind.RelativeOrAbsolute);
            UncheckedImage.EndInit();

            CheckedImage = new BitmapImage();
            CheckedImage.BeginInit();
            CheckedImage.UriSource = new Uri("https://img.icons8.com/tiny-glyph/16/000000/experimental-file-tiny-glyph.png", UriKind.RelativeOrAbsolute);
            CheckedImage.EndInit();
        }

        public void ScrapInitialize(int n)
        {
            int tempnum = 1;
            string num;

            for (int i = 0; i < 9; i++)
            {
                Scrapimages[i].Stretch = Stretch.None;
                if (n < 0 || n > VM.stringIndex - 1)
                {
                    return;
                }

                row = VM.AllString[n];
                value = row.Split('\t');

                num = value[6];

                if (int.TryParse(num, out tempnum)) { }

                if (tempnum == 0)
                {
                    Scrapimages[i].Source = UncheckedImage;
                    Scrapimages[i].Tag = "0";
                }
                else
                {
                    Scrapimages[i].Source = CheckedImage;
                    Scrapimages[i].Tag = "1";
                }
                n--;
            }

        }

        public void ScrapInitialize_Other(int[] NoticeNumbers)
        {
            int tempnum = 1;
            string num;

            for (int i = 0; i < 9; i++)
            {
                Scrapimages[i].Stretch = Stretch.None;

                tempnum = NoticeNumbers[i];
                row = VM.AllString[tempnum];
                value = row.Split('\t');

                num = value[6];

                if (int.TryParse(num, out tempnum)) { }

                if (tempnum == 0)
                {
                    Scrapimages[i].Source = UncheckedImage;
                    Scrapimages[i].Tag = "0";
                }
                else
                {
                    Scrapimages[i].Source = CheckedImage;
                    Scrapimages[i].Tag = "1";
                }
            }
        }

        private void ScrapInitialize_Scrap(int n)
        {
            for (int i = 0; i < 9; i++)
            {
                if (n < 0 || n > scrapindex - 1)
                {
                    Scrapimages[i].Source = UncheckedImage;
                    Scrapimages[i].Tag = "0";
                }
                else
                {
                    Scrapimages[i].Source = CheckedImage;
                    Scrapimages[i].Tag = "1";
                }
                n--;
            }
        }

        private void SaveScrap(int index) //스크랩한거 저장
        {
            if (!VM.notice_menu || VM.IsScrapClicked)
                return;

            int Noticeindex, tempindex = 0;
            Noticeindex = VM.lastnoticeNumber - index;

            row = VM.AllString[Noticeindex];

            if (VM.IsDivided || VM.IsSearched)
            {
                tempindex = VM.NoticeNumber[index];
                row = VM.AllString[tempindex];
            }

            value = row.Split('\t');
            string[] Tempstr;

            for (int i = 0; i < scrapindex; i++)
            {
                Tempstr = ScrapAllString[i].Split('\t');
                if (value[2].Equals(Tempstr[2]))
                    return;
            }

            StreamWriter sw = File.AppendText("scrap.txt");

            row = $"{value[0]}\t{value[1]}\t{value[2]}\t{value[3]}\t{value[4]}\t{value[5]}\t1\t{Noticeindex}";
            row = row.Replace('\r', ' ');

            if (VM.IsDivided || VM.IsSearched)
                VM.SettingAllstring(tempindex, true);
            else
                VM.MainSettingAllstring(Noticeindex, true);

            sw.WriteLine(row);
            sw.Close();
            SetAllStrings();
        }

        private void DeleteScrap(int index)
        {
            int tempindex = 0, Noticeindex;
            if (!VM.notice_menu)
                return;

            Noticeindex = VM.lastnoticeNumber - index;

            if (VM.IsScrapClicked)
            {
                row = ScrapAllString[lastScrapNumber - index];
                newValue = row.Split('\t');

                int tempnum = 1;
                if (int.TryParse(newValue[7], out tempnum)) { }

                Noticeindex = tempnum;
            }

            StreamWriter sw = File.CreateText("scrap.txt");
            row = VM.AllString[Noticeindex];

            if (VM.IsDivided || VM.IsSearched)
            {
                tempindex = VM.NoticeNumber[index];
                row = VM.AllString[tempindex];
            }

            value = row.Split('\t');
            string[] Tempstr;
            string ReplaceString;

            for (int i = 0; i < scrapindex; i++)
            {
                Tempstr = ScrapAllString[i].Split('\t');
                if (value[2].Equals(Tempstr[2]))
                {
                    if (VM.IsDivided || VM.IsSearched)
                       VM.SettingAllstring(tempindex, false);
                    else
                        VM.MainSettingAllstring(Noticeindex, false);
                    continue;
                }
                ReplaceString = ScrapAllString[i].Replace('\r', ' ');
                sw.WriteLine(ReplaceString);
            }
            sw.Close();
            SetAllStrings();

            if (VM.IsScrapClicked)
                SetButtonToScrap(lastScrapNumber);
        }

        public void DoScrapButton()
        {
            for (int i = 0; i < 9; i++)
                VM.buts[i].Click += Scrap_notice_Click;

            SetButtonToScrap(lastScrapNumber);
        }

        public void UndoScrapButton()
        {
            for (int i = 0; i < 9; i++)
                VM.buts[i].Click -= Scrap_notice_Click;
        }

        private void SetButtonToScrap(int n)
        {
            ScrapInitialize_Scrap(n);
            int count = 0;
            for (int i = 0; i < 9; i++)
            {
                if (n < 0 || n > scrapindex - 1)
                {
                    VM.SetEmptyPage(count);
                    return;
                }

                row = ScrapAllString[n];
                value = row.Split('\t');
                VM.buts[i].Content = $"{value[0]} {value[1]} {value[2]} {value[3]}";
                n--;
                count++;
            }
            tempPage = n;
        }
        private void Scrap_notice_Click(object sender, RoutedEventArgs e)
        {
            Button but = (Button)sender;
            string num = but.Tag.ToString();
            int tempnum = 1;
            if (int.TryParse(num, out tempnum)) { }

            row = ScrapAllString[lastScrapNumber - tempnum];
            value = row.Split('\t');

            VM.ScrapNoticeClick(value[5], value[2]);
        }


        public void ScrapLeftClick()
        {
            if (lastScrapNumber >= ScrapAllString.Length - 1)
                return;

            lastScrapNumber += 9;
            SetButtonToScrap(lastScrapNumber);
        }

        public void ScrapRightClick()
        {
            if (lastScrapNumber <= 0)
                return;
            lastScrapNumber = tempPage;
            SetButtonToScrap(lastScrapNumber);
        }
    }
}
