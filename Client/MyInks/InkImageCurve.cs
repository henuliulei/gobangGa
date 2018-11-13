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
    public class InkImageCurve : InkObject
    {
        public BitmapImage bi;

        public InkImageCurve(MyInkCanvas myInkCanvas, bool isInkColorAtEnd)
            : base(myInkCanvas, isInkColorAtEnd)
        {
            this.inkType = InkType.图像序列;
            bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("images/tree.png", UriKind.Relative);
            bi.EndInit();
        }

        public override void CreateNewStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            InkStroke = new InkImageCurveStroke(this, e.Stroke.StylusPoints);
        }

        public override Point Draw(Point first, MyInkData tool, DrawingContext dc, StylusPointCollection points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Point pt = (Point)points[i];
                Vector v = Point.Subtract(pt, first);
                Size size;
                double distance = GetStylusInkDistance(tool, out size);
                if (v.Length >= distance)
                {
                    double x = pt.X - size.Width;
                    double y = pt.Y - size.Height;
                    Rect rect = new Rect(x, y, 2 * size.Width, 2 * size.Height);
                    dc.DrawImage(bi, rect);
                    first = pt;
                }
            }
            return first;
        }

        protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
        {
            base.OnDraw(drawingContext, stylusPoints, geometry, this.brush);
            previousPoint = Draw(previousPoint, inkTool, drawingContext, stylusPoints);
        }
    }

    public class InkImageCurveStroke : InkObjectStroke
    {
        public InkImageCurveStroke(InkObject ink, StylusPointCollection stylusPoints)
            : base(ink, stylusPoints)
        { }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);
            Point startPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
            ink.Draw(startPoint, inkTool, drawingContext, StylusPoints);
        }
    }
}
