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
using System.Windows.Shapes;

namespace Client.MyInks
{
    public class InkEllipse : InkObject
    {
        public InkEllipse(MyInkCanvas myInkCanvas, bool isInkColorAtEnd)
            : base(myInkCanvas, isInkColorAtEnd)
        {
            this.inkType = InkType.球形;
        }

        public override void CreateNewStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            InkStroke = new InkEllipseStroke(this, e.Stroke.StylusPoints);
        }

        public override Point Draw(Point first, MyInkData tool, DrawingContext dc, StylusPointCollection points)
        {
            Point pt = (Point)points.Last();
            Vector v = Point.Subtract(pt, first);
            double radiusX = (pt.X - first.X) / 2.0;
            double radiusY = (pt.Y - first.Y) / 2.0;
            Point center = new Point((pt.X + first.X) / 2.0, (pt.Y + first.Y) / 2.0);
            if (tool.inkDrawOption == InkDrawOption.仅填充)
            {
                dc.DrawEllipse(tool.inkBrush, null, center, radiusX, radiusY);
            }
            else if (tool.inkDrawOption == InkDrawOption.仅轮廓)
            {
                dc.DrawEllipse(null, tool.inkPen, center, radiusX, radiusY);
            }
            else
            {
                dc.DrawEllipse(tool.inkBrush, tool.inkPen, center, radiusX, radiusY);
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

    public class InkEllipseStroke : InkObjectStroke
    {
        public InkEllipseStroke(InkObject ink, StylusPointCollection stylusPoints)
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
            Point prevPoint = (Point)this.StylusPoints.First();
            ink.Draw(prevPoint, inkTool, drawingContext, StylusPoints);
        }
    }
}
