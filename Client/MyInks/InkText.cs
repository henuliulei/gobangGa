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
    public class InkText : InkObject
    {
        private string s = "锄禾日当午，汗滴禾下土。";
        public int TextIndex = 0;

        private static Typeface typeface = new Typeface("楷体");
        private static FormattedText[] ftArray;

        public InkText(MyInkCanvas myInkCanvas, bool isInkColorAtEnd)
            : base(myInkCanvas, isInkColorAtEnd)
        {
            this.inkType = InkType.文字;
            ftArray = new FormattedText[s.Length];
        }

        public override void CreateNewStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            InkStroke = new InkTextStroke(this, e.Stroke.StylusPoints);
        }

        public override Point Draw(Point first, MyInkData tool, DrawingContext dc, StylusPointCollection points)
        {
            int len = s.Length;
            FormattedText[] ftArray = new FormattedText[len];
            for (int i = 0; i < len; i++)
            {
                ftArray[i] = new FormattedText(s[i].ToString(),
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, typeface, tool.inkRadius * 3, tool.inkBrush);
            }
            for (int i = 0; i < points.Count; i++)
            {
                Point pt = (Point)points[i];
                Vector v = Point.Subtract(pt, first);
                if (v.Length >= tool.inkRadius * 3)
                {
                    Point pt1 = new Point(pt.X - tool.inkRadius * 1.5, pt.Y - tool.inkRadius * 1.5);
                    dc.DrawText(ftArray[TextIndex % len], pt1);
                    TextIndex++;
                    first = pt;
                }
            }
            return first;
        }

        protected override void OnStylusDown(RawStylusInput rawStylusInput)
        {
            TextIndex = 0;
            base.OnStylusDown(rawStylusInput);
        }

        protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
        {
            base.OnDraw(drawingContext, stylusPoints, geometry, brush);
            previousPoint = Draw(previousPoint, inkTool, drawingContext, stylusPoints);
        }
    }

    public class InkTextStroke : InkObjectStroke
    {
        public InkTextStroke(InkObject ink, StylusPointCollection stylusPoints)
            : base(ink, stylusPoints)
        {
            InkText ink1 = ink as InkText;
            ink1.TextIndex = 0;
        }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);
            Point start = new Point(double.NegativeInfinity, double.NegativeInfinity);
            ink.Draw(start, inkTool, drawingContext, StylusPoints);
        }
    }
}
