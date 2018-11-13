using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class InkLine : InkObject
    {
        public InkLine(MyInkCanvas myInkCanvas, bool isInkColorAtEnd)
            : base(myInkCanvas, isInkColorAtEnd)
        {
            this.inkType = InkType.直线;
        }

        public override void CreateNewStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            InkStroke = new InkLineStroke(this, e.Stroke.StylusPoints);
        }

        public override Point Draw(Point first, MyInkData tool, DrawingContext dc, StylusPointCollection points)
        {
            Point pt = (Point)points.Last();
            Vector v = Point.Subtract(pt, first);
            if (v.Length > 4)
            {
                dc.DrawLine(tool.inkPen, first, pt);
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
            Draw(previousPoint, inkTool, drawingContext, stylusPoints);
            base.OnDraw(drawingContext, stylusPoints, geometry, brush);
        }
    }

    public class InkLineStroke : InkObjectStroke
    {
        public InkLineStroke(InkObject ink, StylusPointCollection stylusPoints)
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
