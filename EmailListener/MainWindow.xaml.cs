using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmailListener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Left = (Convert.ToInt16(System.Windows.SystemParameters.PrimaryScreenWidth / 2) - (this.Width / 2));
            this.Top = 0;
            this.Topmost = true;
            
            //BarColVar =Brushes.Pink;
        }
        Listener Lis = new Listener();


        //public Brush BarColVar { set { this.TopBar.Fill = value; } get { return this.TopBar.Fill; } }

        public bool Con = false;

        //private async void MultiThread()
        //{
            
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Con)
            {
                Con = true;
                Brush item = TopBar.Fill;
                Lis.Checks(this,TopBar);
            }
            else
            {
                Con = false;
            }
            //MultiThread();
        }

        //private static Task Checks(Rectangle Rec)
        //{
        //    Listener Lis = new Listener();
        //    return Task.Run(() =>
        //    {
        //        Rec.Dispatcher.BeginInvoke(new Action(delegate ()
        //        {
        //            //Lis.Listen(Rec);

        //        }));
        //        //BarColVar = Brushes.Pink;
        //    });

        //}

    }
}







//private void Poss(bool Top)
//{
//    if (Top)
//    {
//        this.Left = (Convert.ToInt16(System.Windows.SystemParameters.PrimaryScreenWidth / 2) - (this.Width / 2));
//        this.Top = 0;
//        this.Topmost = true;
//    }
//    else
//    {


//        this.Left = (Convert.ToInt16(System.Windows.SystemParameters.PrimaryScreenWidth / 2) - (this.Width / 2));
//        this.Top = (Convert.ToInt16(Screen.PrimaryScreen.WorkingArea.Height) - (this.Height) /*73*/);
//        this.Topmost = true;
//    }

//}