using System.Windows.Controls;

namespace Client.Examples
{
    /// <summary>
    /// Page2.xaml 的交互逻辑
    /// </summary>
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
        }

        public void Init(string tip, string fileName)
        {
            textBlock1.Text += tip;
            ink1.LoadInkFromFile(fileName);
            ChangeSelect("全部");
            ChangeSelect("线性渐变");
            ChangeSelect("12");
            ChangeSelect("红色");
            ChangeSelect("圆笔");
            ChangeSelect("球形");
        }

        public void ChangeSelect(string selectedName)
        {
           ink1.SetInkAttributes(selectedName);
        }

        public void OnAppMenuItemClick(string name)
        {
            if (name == "打开")
            {
                ink1.LoadInkFromFile();
            }
            else if (name == "另存为")
            {
                ink1.SaveInkToFile();
            }
        }
    }
}
