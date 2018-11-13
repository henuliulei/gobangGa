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
using System.Windows.Shapes;
using Client.GobangServiceReference;

namespace Client
{
    /// <summary>
    /// ClientWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClientWindow : Window, IGobangServiceCallback
    {
        public string UserName
        {
            get { return textBoxUserName.Text; }
            set { textBoxUserName.Text = value; }
        }

        private int nextColor = -1;        //该哪一方放置棋子（-1:不允许，0：蓝方，1：红方）(正:兰蓝方,负：红方)
        private bool isGameStart = false;  //是否已开始游戏
        private const int max = 4;       //棋盘行列最大值（0～16）
        private int maxTables;            //服务端开设的最大房间号
        private int tableIndex = -1;      //房间号（所坐的游戏桌号）
        private int tableSide = -1;       //座位号
        private Border[,] gameTables;        //开设的房间（每个房间一桌）
        private Image[,] images = new Image[max, max];  //保存棋盘上每个棋子
        //private bool isFromServer;          //是否为服务端发送过来的操作
        private GobangServiceClient client;  //客户端实例

        //当前选中的棋子坐标
        private int selectX=-1;
        private int selectY=-1;

        /// <summary>
        /// 棋盘 
        /// 蓝方棋(1,2,3,4,5,6,7,8)
        ///红方棋子(-1,-2,-3,-4,-5,-6,-7,-8)
        ///</summary>
        private int[,] grid = new int[max, max];

        /// <summary>
        /// 棋盘
        /// 1：翻牌； 
        /// 0 ：无牌；
        /// -1 ：未翻牌
        ///</summary>
        private int[,] grid_flag = new int[max, max];


        public ClientWindow()
        {
            InitializeComponent();

            //确保关闭窗口时关闭客户端
            this.Closing += ClientWindow_Closing;

            ChangeRoomsInfoVisible(false);
            ChangeRoomVisible(false);
            SetNextColor(-1);
        }

        void ClientWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChangeState(btnLogin, true, btnLogout, false);
            
            if (client != null)
            {
                if (tableIndex != -1) //如果在房间内，要求先返回大厅
                {
                    MessageBox.Show("请先返回大厅，然后再退出");
                    e.Cancel = true;
                }
                else
                {
                    client.Logout(UserName); //从大厅退出
                    client.Close();
                }
            }
        }
        
        private void ChangeRoomsInfoVisible(bool visible)
        {
            if (visible == false)
            {
                gridRooms.Visibility = System.Windows.Visibility.Collapsed;
                gridMessage.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                gridRooms.Visibility = System.Windows.Visibility.Visible;
                gridMessage.Visibility = System.Windows.Visibility.Visible;
            }
        }
        
        private void ChangeRoomVisible(bool visible)
        {
            if (visible == false)
            {
                gridRoom.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                gridRoom.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void SetNextColor(int next)
        {
            nextColor = next;
            if (nextColor == 0)
            {
                stackPanelGameTip.Visibility = System.Windows.Visibility.Visible;
                blackImage.Visibility = System.Windows.Visibility.Visible;
                whiteImage.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (nextColor == 1)
            {
                stackPanelGameTip.Visibility = System.Windows.Visibility.Visible;
                blackImage.Visibility = System.Windows.Visibility.Collapsed;
                whiteImage.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                stackPanelGameTip.Visibility = System.Windows.Visibility.Collapsed;
                blackImage.Visibility = System.Windows.Visibility.Collapsed;
                whiteImage.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void AddMessage(string str)
        {
            TextBlock t = new TextBlock();
            t.Text = "  " + str;
            t.Foreground = Brushes.White;
            listBoxMessage.Items.Add(t);
        }

        private void AddColorMessage(string str, SolidColorBrush color)
        {
            TextBlock t = new TextBlock();
            t.Text = "  " + str;
            t.Foreground = color;
            listBoxMessage.Items.Add(t);
        }

        private static void ChangeState(Button btnStart, bool isStart, Button btnStop, bool isStop)
        {
            btnStart.IsEnabled = isStart;
            btnStop.IsEnabled = isStop;
        }

        #region 鼠标和键盘事件
        private void Button_Clickmini(object sender, RoutedEventArgs e)
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


        //单击登录按钮引发的事件
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            UserName = textBoxUserName.Text;
            this.Cursor = Cursors.Wait;
            client = new GobangServiceClient(new InstanceContext(this));
            try
            {
                client.Login(textBoxUserName.Text);
                serviceTextBlock.Text = "登录成功，欢迎您，亲爱的玩家！";
                ChangeState(btnLogin, false, btnLogout, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("与服务端连接失败：" + ex.Message);
                return;
            }
            this.Cursor = Cursors.Arrow;
        }

        //单击退出按钮引发的事件
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); //在窗口的Closing事件中处理退出操作
        }

        //在某个座位坐下时引发的事件
        private void RoomSide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnLogout.IsEnabled = false;
            Border border = e.Source as Border;
            if (border != null)
            {
                string s = border.Tag.ToString();
                tableIndex = int.Parse(s[0].ToString());
                tableSide = int.Parse(s[1].ToString());
                client.SitDown(UserName, tableIndex, tableSide);
            }
        }

        //单击发送按钮引发的事件
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            client.Talk(tableIndex, UserName, textBoxTalk.Text);
        }

        //在对话文本框中按回车键时引发的事件
        private void textBoxTalk_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                client.Talk(tableIndex, UserName, textBoxTalk.Text);
            }
        }

        //单击开始按钮引发的事件
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            client.Start(UserName, tableIndex, tableSide);
            btnStart.IsEnabled = false;
            SetNextColor(-1);
        }

        //单击返回大厅按钮引发的事件
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            client.GetUp(tableIndex, tableSide);
        }


        #endregion //鼠标和键盘事件


        #region 实现服务端指定的IRndGameServiceCallback接口

        /// <summary>有用户登录</summary>
        public void ShowLogin(string loginUserName, int maxTables)
        {
            if (loginUserName == UserName)
            {
                ChangeRoomsInfoVisible(true);
                this.maxTables = maxTables;
                this.CreateTables();
            }
            AddMessage(" "+loginUserName + "进入大厅。");
        }

        /// <summary>其他用户退出</summary>
        public void ShowLogout(string userName)
        {
            AddMessage(" "+userName + "退出大厅。");
        }

        /// <summary>用户入座</summary>
        public void ShowSitDown(string userName, int side)
        {
            stackPanelGameTip.Visibility = System.Windows.Visibility.Collapsed;
            if (side == tableSide)
            {
                isGameStart = false;
                btnLogout.IsEnabled = false;
                btnStart.IsEnabled = true;
                listBoxRooms.IsEnabled = false;//返回大厅前不允许再坐到另一个位置
                textBlockRoomNumber.Text = "桌号：" + (tableIndex + 1);
                ChangeRoomVisible(true);
            }
            if (side == 0)
            {
                textBlockBlackUserName.Text = "  蓝方：" + userName;
                AddMessage(string.Format("{0}在房间{1}蓝方入座。", userName, tableIndex + 1));
            }
            else
            {
                textBlockWhiteUserName.Text = "  红方：" + userName;
                AddMessage(string.Format("{0}在房间{1}红方入座。", userName, tableIndex + 1));
            }
        }

        /// <summary>用户离座</summary>
        public void ShowGetUp(int side)
        {
            stackPanelGameTip.Visibility = System.Windows.Visibility.Collapsed;
            if (side == tableSide)
            {
                isGameStart = false;
                btnLogout.IsEnabled = true;
                listBoxRooms.IsEnabled = true;//返回大厅后允许再坐到另一个位置
                AddMessage(" "+UserName + "返回大厅。");
                this.tableIndex = -1;
                this.tableSide = -1;
                ChangeRoomVisible(false);
            }
            else
            {
                if (isGameStart)
                {
                    AddMessage(" 对方回大厅了，游戏终止。");
                    isGameStart = false;
                    btnStart.IsEnabled = true;
                }
                else
                {
                    AddMessage(" 对方返回大厅。");
                }
                if (side == 0) textBlockBlackUserName.Text = "";
                else textBlockWhiteUserName.Text = "";
            }
        }

        public void ShowStart(int side,int [] grid)
        {
            ResetGrid(grid);
            if (side == 0) { AddMessage(" 蓝方已准备。"); }
            else { AddMessage(" 红方已准备。"); }
        }

        public void ShowTalk(string userName, string message)
        {
            AddColorMessage(string.Format("{0}：{1}", userName, message), Brushes.White);
        }

        /// <summary>设置棋子状态。参数：行，列，颜色</summary>
        public void ShowSetDot(int i, int j, int k,int color)
        {
            grid[i, j] = k;
            grid_flag[i, j] = 1;

            setColor_blue(i, j, k);
            setColor_red(i, j, k);
            //((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new SolidColorBrush(Colors.White);
            //if (k > 0)
            //{
            //    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new SolidColorBrush(Colors.Blue);
            //}
            SetNextColor((color + 1) % 2);
            selectX = -1;
            selectY = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i">移动后的位置</param>
        /// <param name="j">移动后的位置</param>
        /// <param name="k">代表对应的动物</param>
        /// <param name="i1">移动前位置</param>
        /// <param name="j1">移动前位置</param>
        /// <param name="color">哪一方</param>
        /// <param name="flag">吃 消</param>
        public void ShowSetDot_1(int i, int j, int k, int i1, int j1, int color, int flag)
        {
            grid[i, j] = k;
            grid_flag[i, j] = 1;
            grid_flag[i1, j1] = 0;
            //if (k == 5)
            //{
            //    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Content = "豹";
            //}   
            // MessageBox.Show(i + "  " +j+"  " + k +" "+ i1 +"  "+ j1 +"  "+ color+"  " + flag);

            if (flag == 2)//消除棋子
            {
                ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Content = null;
                ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new SolidColorBrush(Colors.White);
                ((Button)this.FindName("btn" + i1.ToString() + j1.ToString())).Content = null;
                ((Button)this.FindName("btn" + i1.ToString() + j1.ToString())).Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                setColor_blue(i, j, k);
                setColor_red(i, j, k);
                ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new SolidColorBrush(Colors.White);
                if (k > 0)
                {
                    setColor_blue(i, j, k);
                }
                else
                {
                    setColor_red(i, j, k);
                }
                ((Button)this.FindName("btn" + i1.ToString() + j1.ToString())).Content = null;
                ((Button)this.FindName("btn" + i1.ToString() + j1.ToString())).Background = new SolidColorBrush(Colors.White);
            }
            SetNextColor((color + 1) % 2);
            selectX = -1;
            selectY = -1;
        }

        public void setColor_blue(int i,int j,int k)
        {
            switch (k)
            {
                case 8:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Blue-Elephant.jpg"))
                    };
                    break;
                case 7:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Blue-Lion.jpg"))
                    };
                    break;
                case 6:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Blue-Tiger.jpg"))
                    };
                    break;
                case 5:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Blue-Leopard.jpg"))
                    };
                    break;
                case 4:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Blue-Wolf.jpg"))
                    };
                    break;
                case 3:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Blue-Dog.jpg"))
                    };
                    break;
                case 2:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Blue-Cat.jpg"))
                    };
                    break;
                case 1:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Blue-Mouse.jpg"))
                    };
                    break;
                default: break;
            }
        }

        public void setColor_red(int i, int j, int k)
        {
            switch (k)
            {
                case -8:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Red-Elephant.jpg"))
                    };
                    break;
                case -7:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Red-Lion.jpg"))
                    };
                    break;
                case -6:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Red-Tiger.jpg"))
                    };
                    break;
                case -5:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Red-Leopard.jpg"))
                    };
                    break;
                case -4:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Red-Wolf.jpg"))
                    };
                    break;
                case -3:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Red-Dog.jpg"))
                    };
                    break;
                case -2:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Red-Cat.jpg"))
                    };
                    break;
                case -1:
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Red-Mouse.jpg"))
                    };
                    break;
                default: break;
            }
        }

        public void GameStart()
        {
            stackPanelGameTip.Visibility = System.Windows.Visibility.Visible;
            this.isGameStart = true;  //为true时才可以放棋子
            SetNextColor(0);
            blackImage.Visibility = System.Windows.Visibility.Visible;
            AddMessage(" 游戏开始");
        }

        public void GameWin(string message)
        {
            AddColorMessage("\n" + message + "\n", Brushes.Red);
            MessageBox.Show("游戏结束!"+message);
            //btnStart.IsEnabled = true;
            //stackPanelGameTip.Visibility = System.Windows.Visibility.Collapsed;
            //this.isGameStart = false;
            //SetNextColor(-1);
            //blackImage.Visibility = System.Windows.Visibility.Collapsed;
            //whiteImage.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void UpdateTablesInfo(string tablesInfo, int userCount)
        {
            textBlockMessage.Text = string.Format("在线人数：{0}", userCount);

            for (int i = 0; i < maxTables; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (tableIndex == -1)
                    {
                        if (tablesInfo[2 * i + j] == '0')
                        {
                            gameTables[i, j].Child.Visibility = System.Windows.Visibility.Hidden;
                            gameTables[i, j].Child.IsEnabled = true;
                        }
                        else
                        {
                            gameTables[i, j].Child.Visibility = System.Windows.Visibility.Visible;
                            gameTables[i, j].Child.IsEnabled = false;
                        }
                    }
                    else
                    {
                        gameTables[i, j].Child.IsEnabled = false;
                        if (tablesInfo[2 * i + j] == '0')
                        {
                            gameTables[i, j].Child.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else gameTables[i, j].Child.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }
        #endregion //实现服务端指定的IRndGameServiceCallback接口


        #region 接口调用的方法
        /// <summary>创建游戏桌</summary>
        private void CreateTables()
        {
            this.gameTables = new Border[maxTables, 2];
            //isFromServer = false;
            //创建游戏大厅中的房间（每房间一个游戏桌）
            for (int i = 0; i < maxTables; i++)
            {
                int j = i + 1;
                StackPanel sp = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5)
                };
                TextBlock text = new TextBlock()
                {
                    Text = "房间" + (i + 1),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Width = 40
                };
                gameTables[i, 0] = new Border()
                {
                    Tag = i + "0",
                    Background = Brushes.White,
                    Child = new Image()
                    {
                        Source = ((Image)this.Resources["player2"]).Source,
                        Height = 25
                    }
                };
                Image image = new Image()
                {
                    Source = ((Image)this.Resources["vs"]).Source,
                    Height = 25
                };
                gameTables[i, 1] = new Border()
                {
                    Tag = i + "1",
                    Background = Brushes.White,
                    Child = new Image()
                    {
                        Source = ((Image)this.Resources["player1"]).Source,
                        Height = 25
                    }
                };
                gameTables[i, 0].MouseDown += RoomSide_MouseDown;
                gameTables[i, 1].MouseDown += RoomSide_MouseDown;
                sp.Children.Add(text);
                sp.Children.Add(gameTables[i, 0]);
                sp.Children.Add(image);
                sp.Children.Add(gameTables[i, 1]);
                listBoxRooms.Items.Add(sp);
            }
        }

        /// <summary>重置棋盘</summary>
        private void ResetGrid(int []grid1)
        {
            // int[] card = { 1, 2, 3, 4, 5, 6, 7, 8, -1, -2, -3, -4, -5, -6, -7, -8 };
            int[] card = grid1;//client.ReSet(tableIndex);
            int tmp = 0;
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < max; j++)
                {
                    grid[i, j] = card[tmp];
                    grid_flag[i, j] = -1;
                    card[tmp] = card[tmp];
                    tmp++;
                    ((Button)this.FindName("btn" + i.ToString() + j.ToString())).Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/timg_"+"1" +".gif"))
                    };
                }
            }
        }
        #endregion //接口调用的方法


        //棋子移动与处理逻辑
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (btnStart.IsEnabled == false)
            {
                if (nextColor == tableSide)
                {
                    Button btn = e.Source as Button;
                    int i = int.Parse(btn.Name.Substring(3, 1));
                    int j = int.Parse(btn.Name.Substring(4, 1));

                    if (grid_flag[i, j] == -1)
                    {
                        grid_flag[i, j] = 1;
                        client.SetDot(tableIndex, i, j, grid[i, j], nextColor);
                    }
                    else
                        //            else if ((grid[i, j] > 0 && nextColor == 0) || (grid[i, j] < 0 && nextColor == 1))
                        //             {

                        if (grid_flag[i, j] == 0)
                    {
                        if (selectX != -1 && selectY != -1)
                        {
                            if ((selectX == i && (selectY - j == 1)) || (selectX == i && (selectY - j == -1)) || ((selectX - i == 1) && selectY == j) || ((selectX - i == -1) && selectY == j))
                            {
                                //某个grid下

                                grid[i, j] = grid[selectX, selectY];
                                grid[selectX, selectY] = 0;

                                grid_flag[selectX, selectY] = 0;//移过去变为无子

                                grid_flag[i, j] = 1;//移过来变为有子

                                client.SetDot_1(tableIndex, i, j, grid[i, j], selectX, selectY, nextColor, 0);

                                selectX = -1;
                                selectY = -1;
                            }
                            else
                            {
                                MessageBox.Show("无法移动！" + grid[selectX, selectY].ToString());
                            }
                        }
                    }
                    else if (grid_flag[i, j] == 1)
                    {
                        if (selectX != -1 && selectY != -1)
                        {
                            if ((selectX == i && (selectY - j == 1)) || (selectX == i && (selectY - j == -1)) || ((selectX - i == 1) && selectY == j) || ((selectX - i == -1) && selectY == j))
                            {
                                if (grid[i, j] * grid[selectX, selectY] > 0)
                                {
                                    MessageBox.Show("同类吃不掉");
                                }
                                else if (grid[i, j] * grid[selectX, selectY] == 0)
                                {
                                    grid[i, j] = grid[selectX, selectY];
                                    grid[selectX, selectY] = 0;

                                    grid_flag[selectX, selectY] = 0;//移过去变为无子

                                    grid_flag[i, j] = 1;//移过来变为有子

                                    client.SetDot_1(tableIndex, i, j, grid[i, j], selectX, selectY, nextColor, 0);

                                    selectX = -1;
                                    selectY = -1;
                                }
                                else
                                {
                                    if((Math.Abs(grid[selectX, selectY])==1 && Math.Abs(grid[i, j])==8))
                                    {
                                        grid[i, j] = grid[selectX, selectY];
                                        grid[selectX, selectY] = 0;
                                        grid_flag[selectX, selectY] = 0;
                                        int flag;
                                        flag = (nextColor == 0) ? -1 : 1;
                                        client.SetDot_1(tableIndex, i, j, grid[i, j], selectX, selectY, nextColor, flag);
                                        selectX = -1;
                                        selectY = -1;
                                    }
                                    else if (Math.Abs(grid[selectX, selectY]) > Math.Abs(grid[i, j]))
                                    {
                                        if ((Math.Abs(grid[selectX, selectY]) == 8 && Math.Abs(grid[i, j]) == 1))
                                        {
                                            MessageBox.Show("吃不掉");
                                        }
                                        else
                                        {
                                            grid[i, j] = grid[selectX, selectY];
                                            grid[selectX, selectY] = 0;
                                            grid_flag[selectX, selectY] = 0;
                                            int flag;
                                            flag = (nextColor == 0) ? -1 : 1;
                                            client.SetDot_1(tableIndex, i, j, grid[i, j], selectX, selectY, nextColor, flag);
                                            selectX = -1;
                                            selectY = -1;
                                        }

                                    }
                                    else if (Math.Abs(grid[selectX, selectY]) == Math.Abs(grid[i, j]))
                                    {
                                        MessageBox.Show("你确定要与它同归于尽吗？");
                                        grid[i, j] = 0;
                                        grid[selectX, selectY] = 0;
                                        grid_flag[i, j] = 0;
                                        grid_flag[selectX, selectY] = 0;

                                        client.SetDot_1(tableIndex, i, j, grid[i, j], selectX, selectY, nextColor, 2);
                                        selectX = -1;
                                        selectY = -1;

                                    }
                                    else
                                    {
                                        MessageBox.Show("吃不掉");
                                    }
                                }
                            }
                            else if (selectX == i && selectY == j)
                            {
                                selectX = -1;
                                selectY = -1;
                            }
                            else
                            {
                                MessageBox.Show("不能这样走棋！" + grid[selectX, selectY].ToString());
                            }
                        }
                        else
                        {
                            if ((grid[i, j] > 0 && nextColor == 0) || (grid[i, j] < 0 && nextColor == 1))
                            {
                                selectX = i;
                                selectY = j;
                            }
                            else
                            {
                                MessageBox.Show("不是你的棋！");
                            }
                        }

                    }
                    //   }
                }
                else
                {
                    MessageBox.Show("不是你的回合！");
                }
            }
            else
            {
                MessageBox.Show("游戏未开始！");
            }

        }
    }
}
