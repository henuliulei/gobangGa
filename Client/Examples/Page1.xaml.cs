using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Examples
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class Page1 : Page
    {
        private string name;
        private DrawingAttributes inkDA;
        private DrawingAttributes highlighterDA;
        private Color currentColor;
        public string SelectedName
        {
            set
            {
                name = value;
                ribbonRadioButtonChecked(name);
            }
        }
        public Page1()
        {
            InitializeComponent();
            currentColor = Colors.Red;
            inkDA = new DrawingAttributes()
            {
                Color = currentColor,
                Height = 6,
                Width = 6,
                FitToCurve = false,
            };
            highlighterDA = new DrawingAttributes()
            {
                Color = Colors.Orchid,
                IsHighlighter = true,
                IgnorePressure = false,
                StylusTip = StylusTip.Rectangle,
                Height = 30,
                Width = 10,
            };
            ink1.DefaultDrawingAttributes = inkDA;
            ink1.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void InitColor()
        {
            inkDA.Color = currentColor;
            ink1.DefaultDrawingAttributes = inkDA;
        }

        private void ribbonRadioButtonChecked(string name)
        {
            switch (name)
            {
                case "钢笔": 
                    ink1.DefaultDrawingAttributes = inkDA;
                    ink1.EditingMode = InkCanvasEditingMode.Ink;
                    break;
                case "荧光笔":
                    ink1.DefaultDrawingAttributes = highlighterDA;
                    ink1.EditingMode = InkCanvasEditingMode.Ink;
                    break;
                case "红色":
                    currentColor = Colors.Red;
                    InitColor();
                    break;
                case "绿色":
                    currentColor = Colors.Green;
                    InitColor();
                    break;
                case "蓝色":
                    currentColor = Colors.Blue;
                    InitColor();
                    break;
                case "墨迹": ink1.EditingMode = InkCanvasEditingMode.Ink; break;
                case "手势": ink1.EditingMode = InkCanvasEditingMode.GestureOnly; break;
                case "套索选择": ink1.EditingMode = InkCanvasEditingMode.Select; break;
                case "点擦除": ink1.EditingMode = InkCanvasEditingMode.EraseByPoint; break;
                case "笔画擦除": ink1.EditingMode = InkCanvasEditingMode.EraseByStroke; break;
            }
        }
    }
}
