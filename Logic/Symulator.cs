using System;
using Data;
using Model;

namespace Logic
{
    public class Symulator
    {
        private readonly ZbiorKul _zbior;
        private double _szerokosc;
        private double _wysokosc;

        public Symulator(ZbiorKul zbior, double szerokosc, double wysokosc)
        {
            _zbior = zbior;
            _szerokosc = szerokosc;
            _wysokosc = wysokosc;
        }

        public void Update()
        {
            foreach (var kula in _zbior.GetKule())
            {
                kula.X += kula.PredkoscX;
                kula.Y += kula.PredkoscY;

                if (kula.X - kula.Promien <= 0 || kula.X + kula.Promien >= _szerokosc)
                    kula.PredkoscX *= -1;

                if (kula.Y - kula.Promien <= 0 || kula.Y + kula.Promien >= _wysokosc)
                    kula.PredkoscY *= -1;
            }
        }

        public void UpdateGranice(double szerokosc, double wysokosc)
        {
            _szerokosc = szerokosc;
            _wysokosc = wysokosc;
        }
    }
}