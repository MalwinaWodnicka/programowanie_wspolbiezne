using System;
using System.Collections.Generic;

namespace Data
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
        public class ZbiorKul
        {
            private readonly List<Kula> _kule = new List<Kula>();

            public IEnumerable<Kula> GetKule()
            {
                return _kule;
            }

            public void AddKula(Kula kula)
            {
                if (kula != null)
                {
                    _kule.Add(kula);
                }
            }

            public void ClearKule()
            {
                _kule.Clear();
            }
        }
    }
