using Client;
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

namespace Z09.Z094
{
    /// <summary>
    /// Land.xaml 的交互逻辑
    /// </summary>
    
    public partial class Land : Window
    {
        public static string userName;
        public Land()
        {
            InitializeComponent();
            
        }

        private void enterBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new MyDatabaseEntities2())
            {
                userName = userTb.Text;
                var q = from t in db.UserInfo
                        where userTb.Text == t.UserID && passwordTb.Password == t.UserPassword
                        select t;
                if (q.Count() > 0)
                {
                    if (identityLb.Text == "玩家")
                    {
                        Window w = new MainWindow();
                        this.Close();
                        w.ShowDialog();
                    }
                    else
                        MessageBox.Show("未查询到相关信息！");
                }
              
            }

        
        }
        public void passwordLook_TextChanged(object sender, TextChangedEventArgs e)
        {
            passwordTb.Password = passwordLook.Text;
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Register();
            w.ShowDialog();
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var r = e.Source as CheckBox;
            if (r.IsChecked == true)
            {
                passwordLook.Text = passwordTb.Password;
                passwordTb.Visibility = Visibility.Collapsed;
                passwordLook.Visibility = Visibility.Visible;
            }
            else
            {
                passwordTb.Visibility = Visibility.Visible;
                passwordLook.Visibility = Visibility.Collapsed;
            }
        }

    }
}
