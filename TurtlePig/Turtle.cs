using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TurtlePig
{
    public class Turtle
    {
        private readonly Canvas canvas;
        private double angle;
        private double x;
        private double y;

        public Turtle(Canvas canvas)
        {
            this.canvas = canvas;
            x = canvas.ActualWidth/2;
            y = canvas.ActualHeight/2;
        }

        public bool PenDown { get; set; }

        public void Move(double distance)
        {
            var oldX = x;
            var oldY = y;

            double rads = angle/360.0*Math.PI*2.0;
            x += Math.Sin(rads)*distance;
            y += Math.Cos(rads)*distance;

            if (PenDown)
            {
                canvas.Children.Add(new Line { X1 = oldX, Y1 = oldY, X2 = x, Y2 = y, Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0))});
            }
        }

        public void Rotate(float a)
        {
            angle += a;
        }
    }
}