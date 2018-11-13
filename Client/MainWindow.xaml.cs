using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using Client.GobangServiceReference;
namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            StartWindow("大乔", 700, 300);
            StartWindow("周瑜", 0, 0);
        }

        private void StartWindow(string userName, int left, int top)
        {
            ClientWindow w = new ClientWindow();
            w.Left = left;
            w.Top = top;
            w.UserName = userName;
            w.Owner = this;
            w.Closed += (sender, e) => this.Activate();
            w.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)

                DragMove();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MyInk w = new MyInk();
            w.Left = 500;
            w.Top = 300;
            w.Owner = this;
            w.Show();
        }
    }
}
