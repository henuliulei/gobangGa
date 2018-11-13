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
    public class InkCurve : InkObject
    {
        public InkCurve(MyInkCanvas myInkCanvas, bool isInkColorAtEnd)
            : base(myInkCanvas, isInkColorAtEnd)
        {
            inkType = InkType.曲线;
        }

        public override void CreateNewStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            InkStroke = new InkCurveStroke(this, e.Stroke.StylusPoints);
        }

        public override Point Draw(Point first, MyInkData tool, DrawingContext dc, StylusPointCollection points)
        {
            return first;
        }

        protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
        {
            base.OnDraw(drawingContext, stylusPoints, geometry, inkTool.inkBrush);
        }

    }

    public class InkCurveStroke : InkObjectStroke
    {
        public InkCurveStroke(InkObject ink, StylusPointCollection stylusPoints)
            : base(ink, stylusPoints)
        {
            this.DrawingAttributes.FitToCurve = true;
        }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);
            Geometry geometry = this.GetGeometry();
            drawingContext.DrawGeometry(inkTool.inkBrush, null, geometry);
        }
    }
}
