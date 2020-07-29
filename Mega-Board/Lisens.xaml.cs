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

namespace Mega_Board
{
    /// <summary>
    /// Логика взаимодействия для Lisens.xaml
    /// </summary>
    public partial class Lisens : Window
    {
        public Lisens()
        {
            InitializeComponent();
        }
        int i;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (code.Text=="EREHONER")
            {
                System.IO.File.WriteAllText("lik.off", Environment.MachineName);
                MessageBox.Show("Вы ввели верный код. Для использования запустите программу вновь. Если вы пользуетесь ей в первые, рекомендуем ознакомитсья с инструкцией.");
                Close();
            }
            else
            {
                i++;
                if (i < 5)
                {
                    MessageBox.Show("Введенный вами код не подходит! Осталось " + (5 - i).ToString() + " попыток.");
                }
                else
                {
                    MessageBox.Show("Лимит попыток превышен!");
                    System.IO.File.Create("hak");
                    Close();
                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists("hak"))
                Close();
        }
    }
}
