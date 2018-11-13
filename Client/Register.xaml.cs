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
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            if (passWordTb.Text == qrPasswprdTb.Text)
            {
                using (MyDatabaseEntities2 db = new MyDatabaseEntities2())
                {
                    Random ran = new Random();
                    db.UserInfo.Add(new UserInfo
                    {
                        UserID = userNameTb.Text,
                        UserPassword = passWordTb.Text,
                        UserBalance = 0,
                        UserName = nameTb.Text,
                        UserCardNumber = IdTb.Text,
                        UserStartTime = DateTime.Now,
                        UserPhoneNumber = phoneTb.Text,
                    });
                   
                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("注册成功！");
                        this.Close();

                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }
                }

            }
            else
            {
                MessageBox.Show("确认密码不正确！");
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
