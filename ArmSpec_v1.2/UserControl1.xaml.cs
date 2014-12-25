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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace boxashu
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
               
        public UserControl1()
        {
            InitializeComponent();

            // Add this
            //Dispatcher.ShutdownStarted += OnDispatcherShutDownStarted;

        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
           
            ////// ... Create a List of objects.
            ////var items = new List<Dog>();
            ////items.Add(new Dog("Fido", 10));
            ////items.Add(new Dog("Spark", 20));
            ////items.Add(new Dog("Fluffy", 4));

            //// ... Assign ItemsSource of DataGrid.
            var grid = sender as DataGrid;
            ////grid.ItemsSource = items;
            grid.ItemsSource = Commands.tablRowList();
        }


        private void Window_Closing(object sender, RoutedEventArgs e)
        {
            Commands._ps = null;
        }

    }


}
