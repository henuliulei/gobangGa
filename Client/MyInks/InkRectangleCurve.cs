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

namespace Client.MyInks
{
    public class InkRectangleCurve : InkObject
    {
        public int aa;
        public InkRectangleCurve(MyInkCanvas myInkCanvas, bool isInkColorAtEnd)
            : base(myInkCanvas, isInkColorAtEnd)
        {
            this.inkType = InkType.矩形序列;
        }

        public override void CreateNewStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            InkStroke = new InkRectangleCurveStroke(this, e.Stroke.StylusPoints);
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
                    if (tool.inkDrawOption == InkDrawOption.仅填充)
                    {
                        dc.DrawRectangle(tool.inkBrush, null, rect);
                    }
                    else if (tool.inkDrawOption == InkDrawOption.仅轮廓)
                    {
                        dc.DrawRectangle(null, tool.inkPen, rect);
                    }
                    else
                    {
                        dc.DrawRectangle(tool.inkBrush, tool.inkPen, rect);
                    }
                    first = pt;
                }
            }
            return first;
        }

        protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
        {
            base.OnDraw(drawingContext, stylusPoints, geometry, brush);
            previousPoint = Draw(previousPoint, inkTool, drawingContext, stylusPoints);
        }
    }

    public class InkRectangleCurveStroke : InkObjectStroke
    {
        public InkRectangleCurveStroke(InkObject ink, StylusPointCollection stylusPoints)
            : base(ink, stylusPoints)
        { }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);
            Point prevPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
            ink.Draw(prevPoint, inkTool, drawingContext, StylusPoints);
        }
    }
}
