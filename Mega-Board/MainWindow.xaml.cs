using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Data;
using System.Management;
using System.Drawing;
using System.Net;

namespace Mega_Board
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        const int NomerSborki = 6;
        BOARDwin Bwin = new BOARDwin();
        string curcat = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (boardbut.Content.ToString() == "Включить табло")
            {
                Bwin.Show();
                Button_Click_11(null, null);
                Button_Click_1(null, null);
                boardbut.Content = "Выключить табло";

            }
            else
            {
                Bwin.Hide();
                Bwin.BoardPlayer.Stop();
                playbut.Content = "Старт";
                boardbut.Content = "Включить табло";
            }
            // 
            // Разворачивание 2-й формы на втором мониторе           
            //System.Windows.Forms.Screen[] sc;
            //sc = System.Windows.Forms.Screen.AllScreens;

            //Bwin.Left = sc[1].Bounds.Width;
            //Bwin.Top = sc[1].Bounds.Height;
            //Bwin.StartPosition = FormStartPosition.Manual;
            //Bwin.Location = sc[1].Bounds.Location;
            //Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
            //Bwin.Location = p;
            //Bwin.WindowState = FormWindowState.Maximized;
            //Bwin.Show();
            //
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TextRange documentTextRange = new TextRange(
            Tred.Document.ContentStart, Tred.Document.ContentEnd);
            using (System.IO.FileStream fs = System.IO.File.Create(curcat+@"\infboard.xaml"))
            {
                documentTextRange.Save(fs, DataFormats.Xaml);
            }
            Tred.SelectAll();
            Tred.Copy();
            Bwin.boardtext.Document.Blocks.Clear();
            Bwin.boardtext.Paste();
            Bwin.autoTsize(Bwin.boardtext, 2, 600);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (playbut.Content.ToString() == "Старт")
            {
                if (visplaylist.Items.Count > 0)
                {
                    Uri uri = new Uri(Convert.ToString(visplaylist.Items[Bwin.pl]));
                    Bwin.BoardPlayer.Source = uri;
                    Bwin.BoardPlayer.Play();
                    playbut.Content = "Пауза";
                }
            }
            else
            {
                Bwin.BoardPlayer.Pause();
                playbut.Content = "Старт";
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Видео(*.mpg;*.avi;*.mp4;*.mov)|*.mpg;*.avi;*.mp4;*.mov" + " |Все файлы (*.*)|*.* ";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = true;
            if (myDialog.ShowDialog() == true)
            {
                visplaylist.Items.Clear();
                Bwin.traks.Items.Clear();
                System.IO.File.WriteAllText(curcat+@"\playlist.pl", "NewPlayList");
                foreach (string fn in myDialog.FileNames)
                {
                    visplaylist.Items.Add(fn);
                    Bwin.traks.Items.Add(fn);
                    System.IO.File.AppendAllText(curcat + @"\playlist.pl", Environment.NewLine+fn);
                }
                Bwin.plall = visplaylist.Items.Count;
                Bwin.pl = 0;             
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (visplaylist.Items.Count > 0)
            {
                Bwin.BoardPlayer.Stop();
                Bwin.pl++;
                if (Bwin.pl >= Bwin.plall)
                    Bwin.pl = 0;
                Uri uri = new Uri(Convert.ToString(visplaylist.Items[Bwin.pl]));
                Bwin.BoardPlayer.Source = uri;
                Bwin.BoardPlayer.Play();
                playbut.Content = "Пауза";
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (visplaylist.Items.Count > 0)
            {
                Bwin.BoardPlayer.Stop();
                Bwin.pl--;
                if (Bwin.pl < 0)
                    Bwin.pl = Bwin.plall - 1;
                Uri uri = new Uri(Convert.ToString(visplaylist.Items[Bwin.pl]));
                Bwin.BoardPlayer.Source = uri;
                Bwin.BoardPlayer.Play();
                playbut.Content = "Пауза";
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Bwin.BoardPlayer.Stop();
            Bwin.BoardPlayer.Source = null;
            playbut.Content = "Старт";
        }

        private void mylab_MouseMove(object sender, MouseEventArgs e)
        {
            mylab.FontSize = 14;
        }

        private void mylab_MouseLeave(object sender, MouseEventArgs e)
        {
            mylab.FontSize = 12;
        }

        private void mylab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://vk.com/erehon99");
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Bwin.BoardPlayer.Volume = voltrak.Value;
        }
        DataTable dt = new DataTable();
        private void loadtalons(string file)
        {
            dt.Clear();
            dt.Columns.Clear();
            dt.Columns.Add("№");
            dt.Columns.Add("Врач");
            dt.Columns.Add("Талонов всего");
            dt.Columns.Add("Талонов осталось");
            dt.TableName = "Vrach-talon";
            if (System.IO.File.Exists(file))
                dt.ReadXml(file);
            tk = dt.Rows.Count;
            vrachs.ItemsSource = dt.DefaultView;
        }

        private void FTPLOAD(string URL, string login, string password, string filename, string outpt, bool open)
        {
            WebClient FTPSERVER = new WebClient();
            FTPSERVER.Credentials = new NetworkCredential(login, password);
            try
            {
                byte[] newFileData = FTPSERVER.DownloadData("ftp://" + URL + filename);
                System.IO.File.WriteAllBytes(curcat + @"\"+outpt, newFileData);
                if (open)
                    System.Diagnostics.Process.Start(outpt);
            }
            catch (WebException e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FTPLOAD("192.168.1.39/", "ftadmin", "QQwerty1284", "curentversion.txt", "curentversion.txt", false);
            int curver= Convert.ToInt32(System.IO.File.ReadAllText(curcat + @"\curentversion.txt"));
             if (curver==NomerSborki)//Проверка обновлений.
            {
                if (System.IO.File.Exists(curcat + @"\lik.off"))
                {
                    string cheker = System.IO.File.ReadAllText(curcat + @"\lik.off");
                    if (cheker != Environment.MachineName)
                    {
                        this.Hide();
                        MessageBox.Show("Ошибка проверки подлиности ПО! Обратитесь к поставщику.");
                        this.Close();
                        Bwin.Close();
                    }
                    else
                    {
                        String strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                        verlab.Content = "Версия:" + strVersion;
                        loadtalons(curcat + @"\talonsof\" + DateTime.Now.DayOfWeek.ToString() + ".tal");
                        Bwin.Owner = this;
                        if (System.IO.File.Exists(curcat + @"\playlist.pl"))
                        {
                            visplaylist.Items.Clear();
                            string[] lpl = System.IO.File.ReadAllLines(curcat + @"\playlist.pl");
                            foreach (string video in lpl)
                            {
                                if (video != "NewPlayList")
                                {
                                    visplaylist.Items.Add(video);
                                    Bwin.traks.Items.Add(video);
                                }
                            }
                            Bwin.plall = visplaylist.Items.Count;
                            Bwin.pl = 0;
                            Uri uri = new Uri(Convert.ToString(visplaylist.Items[Bwin.pl]));
                            Bwin.BoardPlayer.Source = uri;
                        }
                        if (System.IO.File.Exists(curcat + @"\infboard.xaml"))
                        {
                            TextRange tr = new TextRange(Tred.Document.ContentStart, Tred.Document.ContentEnd);
                            using (FileStream fs = File.Open("infboard.xaml", FileMode.Open))
                            {
                                tr.Load(fs, DataFormats.Xaml);
                            }
                            Button_Click_1(null, null);
                        }
                    }
                }
                else
                {
                    Lisens lis = new Lisens();
                    lis.Show();
                    this.Close();
                    Bwin.Close();
                }
            }
            else //Если обновление найдено.
            {
                MessageBox.Show("найдено обновление, программа будет обновлена и перезапущенна.");
                System.IO.Directory.CreateDirectory(curcat + @"\clientup");
                FTPLOAD("192.168.1.39/", "ftadmin", "QQwerty1284", "Mega-Board.exe", @"clientup\Mega-Board.exe", false);
                System.IO.File.WriteAllText(curcat + @"\samouper.bat", "timeout /t 5 " + Environment.NewLine + @"MOVE /Y clientup\Mega-Board.exe Mega-Board.exe" + Environment.NewLine + "start Mega-Board.exe");
                System.Diagnostics.Process.Start(curcat + @"\samouper.bat");
                Bwin.Close();
                this.Close();
            }
        }
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
            save.Filter ="Файл XAML (*.xaml)|*.xaml";
            if (save.ShowDialog() == true)
            {
                TextRange documentTextRange = new TextRange(
                    Tred.Document.ContentStart, Tred.Document.ContentEnd);
                using (FileStream fs = File.Create(save.FileName))
                {
                        documentTextRange.Save(fs, DataFormats.Xaml);
                }
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
          
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Filter = "Файл XAML (*.xaml)|*.xaml";
            if (openFile.ShowDialog() == true)
            {
                TextRange tr = new TextRange(Tred.Document.ContentStart, Tred.Document.ContentEnd);
                using (FileStream fs = File.Open(openFile.FileName, FileMode.Open))
                {
                    tr.Load(fs, DataFormats.Xaml);
                }
            }
        }
        class VrachTalon
        {
            public int npp;
            public string Name;
            public int  tnow, tall;
        }
        ObservableCollection<VrachTalon> TalonBoard = new ObservableCollection<VrachTalon>();
        int tk;
        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            tk++;
            if (tk <= 10)
            {
                dt.Rows.Add(tk, edvrach.Text, edtalon.Text, edtalon.Text);
                dt.WriteXml(curcat + @"\talonsof" + DateTime.Now.DayOfWeek.ToString() + ".tal");
                Button_Click_11(null, null);
                edvrach.Text = "";
                edtalon.Text = "";
            }
            else
            { MessageBox.Show("Лимит врачей превышен!"); }
          
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            dt.WriteXml(curcat + @"\talonsof\" + DateTime.Now.DayOfWeek.ToString() + ".tal");
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {           
            Paragraph paragraph = new Paragraph();
            FlowDocument tdocument = new FlowDocument();
            paragraph.Inlines.Add("Осталось талонов:");
            for (int i=0;i<dt.Rows.Count;i++)
            {
                string sv = dt.Rows[i][1].ToString();
                string st = dt.Rows[i][3].ToString();
                paragraph.Inlines.Add(Environment.NewLine + sv + ": "+st);    
            }
            tdocument.Blocks.Add(paragraph);
            Bwin.boardtalon.Document = tdocument;
            Bwin.autoTsize(Bwin.boardtalon,2,600);
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            try
            {
                int res;
                if (dt.Rows[vrachs.SelectedIndex][3].ToString() != "Нет талонов" & Int32.TryParse(dt.Rows[vrachs.SelectedIndex][3].ToString(), out res))
                    if (Convert.ToInt32(dt.Rows[vrachs.SelectedIndex][3]) > 1)
                        dt.Rows[vrachs.SelectedIndex][3] = Convert.ToString(Convert.ToInt32(dt.Rows[vrachs.SelectedIndex][3]) - 1);
                    else
                        dt.Rows[vrachs.SelectedIndex][3] = "Нет талонов";
                Button_Click_11(null, null);
            }
            catch { MessageBox.Show("Не выбран ни один врач!"); }
        }

        private void vrachs_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Button_Click_11(null, null);
        }

        private void vrachs_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Button_Click_11(null, null);
        }

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            Bwin.WindowState = WindowState.Maximized;
            Bwin.WindowStyle = WindowStyle.None;
            Bwin.autoTsize(Bwin.boardtalon, 2, 600);
            if (visplaylist.Items.Count > 0)
            {
                Uri uri = new Uri(Convert.ToString(visplaylist.Items[Bwin.pl]));
                Bwin.BoardPlayer.Source = uri;
                Bwin.BoardPlayer.Play();
                playbut.Content = "Пауза";
            }
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
                EditingCommands.IncreaseFontSize.Execute(null, Tred);
        }

        private void Button_Click_17(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
                EditingCommands.DecreaseFontSize.Execute(null, Tred);
        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Filter = "Талоны (*.tal )|*.tal";
            if (openFile.ShowDialog() == true)
                loadtalons(openFile.FileName);
            Button_Click_11(null, null);
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {


        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            Bwin.Close();
            this.Close();
        }

            System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();

            public void iconing()
            {
                nIcon.Icon = new Icon(curcat + @"\board.ico");
                nIcon.Visible = true;
                nIcon.ShowBalloonTip(5000, "Mega-Board", "Табло продолжает свою работу, для доступа к панели управления, щелкните по этой иконке в трее.", System.Windows.Forms.ToolTipIcon.Info);
                nIcon.Click += nIcon_Click;
            }

            void nIcon_Click(object sender, EventArgs e)
            {
                //events comes here
                this.Visibility = Visibility.Visible;
                this.WindowState = WindowState.Normal;
                nIcon.Visible = false;
            }

        private void Button_Click_19(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            iconing();
        }

        private void Button_Click_20(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_21(object sender, RoutedEventArgs e)
        {

        }
    }
}
