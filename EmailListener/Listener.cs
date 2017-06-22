using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EmailListener
{
    class Listener 
    {

        public void Test()
        {

        }


        public Task Checks(MainWindow Parent, Rectangle topBar)
        {
            ListenMethods Meth = new ListenMethods() { TopBar = topBar };
            return Task.Run(() =>
            {
                do
                {

                    

                    Meth.UpdateColour(Brushes.Green);

                    Meth.Search();

                    System.Threading.Thread.Sleep(5000);

                } while (Parent.Con == true);

                Meth.UpdateColour(Brushes.Red);
                //BarColVar = Brushes.Pink;
            });
        }
    }
}
