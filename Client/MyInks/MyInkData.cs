using System;
using System.Windows.Ink;
using System.Windows.Media;

namespace Client.MyInks
{
    public enum InkType { 球形, 矩形, 图像, 球形序列, 矩形序列, 图像序列, 直线, 曲线, 文字 }
    public enum InkStylusType { 圆笔, 竖笔, 横笔, 钢笔 }
    public enum InkClorType { 红色, 蓝色, 绿色 }
    public enum InkBrushType { 线性渐变, 仿射渐变, 纯色 }
    public enum InkDrawOption { 仅填充, 仅轮廓, 全部 }

    public class MyInkData
    {
        public Brush inkBrush;
        public Pen inkPen;
        public double inkRadius;
        public Color inkColor;
        public InkBrushType inkBrushType;
        public InkStylusType inkStylusType;
        public InkDrawOption inkDrawOption;

        public MyInkData Clone()
        {
            MyInkData data = new MyInkData()
            {
                inkRadius = this.inkRadius,
                inkColor = this.inkColor,
                inkBrushType = this.inkBrushType,
                inkStylusType = this.inkStylusType,
                inkDrawOption = this.inkDrawOption
            };
            return data;
        }
    }
}
