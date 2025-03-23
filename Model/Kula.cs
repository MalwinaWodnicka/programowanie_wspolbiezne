using System;

namespace Model
{
    public class Kula
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double PredkoscX { get; set; }
        public double PredkoscY { get; set; }
        public double Promien { get; set; }

        public Kula(double x, double y, double predkoscX, double predkoscY, double promien)
        {
            X = x;
            Y = y;
            PredkoscX = predkoscX;
            PredkoscY = predkoscY;
            Promien = promien;
        }
    }
}
