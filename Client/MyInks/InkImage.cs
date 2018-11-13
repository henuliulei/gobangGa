using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client.MyInks
{
    public class InkImage : InkObject
    {
        public BitmapImage bi;

        public InkImage(MyInkCanvas myInkCanvas, bool isInkColorAtEnd)
            : base(myInkCanvas, isInkColorAtEnd)
        {
            this.inkType = InkType.图像;
            bi = new BitmapImage();
            bi.BeginInit();
            //tree.png的【复制到输出目录】属性是“总是复制”,【生成操作】属性是“内容”
            bi.UriSource = new Uri("images/tree.png", UriKind.Relative);
            bi.EndInit();
        }
        /// <summary>
        /// 添加一个重载的构造函数
        /// </summary>
        /// <param name="myInkCanvas"></param>
        /// <param name="isInkColorAtEnd"></param>
        /// <param name="Imagepath"></param>
        public InkImage(MyInkCanvas myInkCanvas, bool isInkColorAtEnd,string Imagepath)
            : base(myInkCanvas, isInkColorAtEnd)
        {
            this.inkType = InkType.图像;
            bi = new BitmapImage();
            bi.BeginInit();
            //图片的【复制到输出目录】属性是“总是复制”,【生成操作】属性是“内容”
            bi.UriSource = new Uri(Imagepath, UriKind.Relative);
            bi.EndInit();
        }

        public override void CreateNewStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            InkStroke = new InkImageStroke(this, e.Stroke.StylusPoints);
        }

        public override Point Draw(Point first, MyInkData tool, DrawingContext dc, StylusPointCollection points)
        {
            Point pt = (Point)points.Last();
            Vector v = Point.Subtract(pt, first);
            if (v.Length > 4)
            {
                Rect rect = new Rect(first, v);
                dc.DrawImage(bi, rect);
            }
            return first;
        }

        protected override void OnStylusDown(RawStylusInput rawStylusInput)
        {
            base.OnStylusDown(rawStylusInput);
            previousPoint = (Point)rawStylusInput.GetStylusPoints().First();
        }

        protected override void OnStylusMove(RawStylusInput rawStylusInput)
        {
            StylusPointCollection stylusPoints = rawStylusInput.GetStylusPoints();
            this.Reset(Stylus.CurrentStylusDevice, stylusPoints);
            base.OnStylusMove(rawStylusInput);
        }

        protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
        {
            previousPoint = Draw(previousPoint, inkTool, drawingContext, stylusPoints);
            base.OnDraw(drawingContext, stylusPoints, geometry, brush);
        }
    }

    public class InkImageStroke : InkObjectStroke
    {
        public InkImageStroke(InkObject ink, StylusPointCollection stylusPoints)
            : base(ink, stylusPoints)
        {
            if (ink.myInkCanvas.isFromFileInk == false)
            {
                this.RemoveDirtyStylusPoints();
            }
        }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);
            Point pt1 = (Point)StylusPoints.First();
            ink.Draw(pt1, inkTool, drawingContext, StylusPoints);
        }
    }
}
