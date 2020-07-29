using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Mega_Board
{

    public partial class BOARDwin : Window
    {
        public BOARDwin()
        {
            InitializeComponent();
        }

        public int pl=0,plall=0;
        public ListBox traks = new ListBox();

      public void autoTsize(RichTextBox rt,int minTsize, int maxTsize)
        {
            double size = maxTsize/2;
            rt.SelectAll();
            while (rt.ViewportHeight == rt.ExtentHeight & size < maxTsize)
            {
                EditingCommands.IncreaseFontSize.Execute(null, rt);
                rt.UpdateLayout();
                size++;
            }
            
            while (rt.ViewportHeight < rt.ExtentHeight & size > minTsize)
            {
                EditingCommands.DecreaseFontSize.Execute(null, rt);
                size--;
                rt.UpdateLayout();             
            }
            rt.Selection.Select(rt.Document.ContentEnd, rt.Document.ContentEnd);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
            autoTsize(boardtext, 2, 600);
            autoTsize(boardtalon, 2, 600);
        }

        private void BoardPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (pl < plall-1)
                pl++;
            else
                pl = 0;
            Uri uri = new Uri(Convert.ToString(traks.Items[pl]));
            BoardPlayer.Source = uri;
            BoardPlayer.Play();
        }
    }
}
