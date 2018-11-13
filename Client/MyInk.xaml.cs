using Client.Examples;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
namespace Client
{
    /// <summary>
    /// InkCanvas.xaml 的交互逻辑
    /// </summary>
    public partial class MyInk : RibbonWindow
    {
        private Page1 p1;
        private Page2 p2;
        private string selectedTabHeader = "例1";
        public MyInk()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
            ribbon.SelectionChanged += ribbon_SelectionChanged;
            this.rt1.IsSelected = true;
            appMenu1.Visibility = System.Windows.Visibility.Collapsed;
        }

        
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Shutdown();
          
                      
        }

        void ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RibbonTab rt = ribbon.SelectedItem as RibbonTab;
            this.selectedTabHeader = rt.Header.ToString();
            if (selectedTabHeader == "例1")
            {
                appMenu1.Visibility = System.Windows.Visibility.Collapsed;
                p1 = new Page1();
                frame1.Content = p1.Content;
            }
            else
            {
                appMenu1.Visibility = System.Windows.Visibility.Visible;
                p2 = new Page2();
                frame1.Content = p2.Content;
                switch (selectedTabHeader)
                {
                    case "例2": p2.Init("球形和球形序列1", "ink2.isf"); break;
                    case "例3": p2.Init("球形和球形序列2", "ink3.isf"); break;
                    case "例4": p2.Init("矩形和矩形序列", "ink4.isf"); break;
                    case "例5": p2.Init("图像和图像序列", "ink5.isf"); break;
                    case "例6": p2.Init("渐变直线", "ink6.isf"); break;
                    case "例7": p2.Init("曲线和文字", "ink7.isf"); break;
                }
            }
        }

        private void RibbonRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RibbonRadioButton rrb = e.Source as RibbonRadioButton;
            string parent = (rrb.Parent as RibbonGroup).Header.ToString();
            if (selectedTabHeader == "例1")
            {
                p1.SelectedName = rrb.Label;
                if (parent == "钢笔颜色")
                {
                    rrbPen.IsChecked = true;
                    p1.SelectedName = rrbPen.Label;
                }
                if (parent == "笔画类型")
                {
                    rrbInk.IsChecked = true;
                }
            }
            else
            {
                p2.ChangeSelect(rrb.Label);
                if (parent == "墨迹类型")
                {
                    switch (selectedTabHeader)
                    {
                        case "例2": rt2.rrbEllipseStylus.IsChecked = true; break;
                        case "例3": rt3.rrbEllipseStylus.IsChecked = true; break;
                        case "例4": rt4.rrbEllipseStylus.IsChecked = true; break;
                        case "例5": rt5.rrbEllipseStylus.IsChecked = true; break;
                        case "例6": rt6.rrbEllipseStylus.IsChecked = true; break;
                        case "例7": rt7.rrbEllipseStylus.IsChecked = true; break;
                    }
                }
            }
        }

        private void RibbonApplicationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RibbonApplicationMenuItem item = e.Source as RibbonApplicationMenuItem;
            string name = item.Header.ToString();
            if (name == "退出")
            {
                Application.Current.Shutdown();
            }
            else
            {
                p2.OnAppMenuItemClick(name);
            }
        }
    }
}
