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
    public class InkEllipseCurve : InkObject
    {
        public InkEllipseCurve(MyInkCanvas myInkCanvas, bool isInkColorAtEnd)
            : base(myInkCanvas, isInkColorAtEnd)
        {
            this.inkType = InkType.球形序列;
        }

        public override void CreateNewStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            InkStroke = new InkEllipseCurveStroke(this, e.Stroke.StylusPoints);
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
                    if (tool.inkDrawOption == InkDrawOption.仅填充)
                    {
                        dc.DrawEllipse(tool.inkBrush, null, pt, size.Width, size.Height);
                    }
                    else if (tool.inkDrawOption == InkDrawOption.仅轮廓)
                    {
                        dc.DrawEllipse(null, tool.inkPen, pt, size.Width, size.Height);
                    }
                    else
                    {
                        dc.DrawEllipse(tool.inkBrush, tool.inkPen, pt, size.Width, size.Height);
                    }
                    first = pt;
                }
            }
            return first;
        }

        protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
        {
            base.OnDraw(drawingContext, stylusPoints, geometry, brush);
            previousPoint = this.Draw(previousPoint, inkTool, drawingContext, stylusPoints);
        }
    }

    public class InkEllipseCurveStroke : InkObjectStroke
    {
        public InkEllipseCurveStroke(InkObject ink, StylusPointCollection stylusPoints)
            : base(ink, stylusPoints)
        { }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);
            Point start = new Point(double.NegativeInfinity, double.NegativeInfinity);
            ink.Draw(start, inkTool, drawingContext, this.StylusPoints);
        }
    }
}
