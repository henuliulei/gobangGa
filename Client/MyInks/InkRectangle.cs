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
     public class  InkRectangle : InkObject
     {
         public InkRectangle(MyInkCanvas myInkCanvas, bool isInkColorAtEnd)
             : base(myInkCanvas, isInkColorAtEnd)
         {
             this.inkType = InkType.矩形;
         }

         public override void CreateNewStroke(InkCanvasStrokeCollectedEventArgs e)
         {
             InkStroke = new InkRectangleStroke(this, e.Stroke.StylusPoints);
         }

         public override Point Draw(Point first, MyInkData tool, DrawingContext dc, StylusPointCollection points)
         {
             Point pt = (Point)points.Last();
             Vector v = Point.Subtract(pt, first);
             if (v.Length > 4)
             {
                 Rect rect = new Rect(first, v);
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

     public class  InkRectangleStroke : InkObjectStroke
     {
         public InkRectangleStroke(InkObject ink, StylusPointCollection stylusPoints)
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
