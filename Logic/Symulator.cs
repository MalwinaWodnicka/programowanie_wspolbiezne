using System;
using System.Collections.Generic;
using Data;

namespace Logic
{
    public class Symulator
    {
        private readonly ZbiorKul _zbior;
        private double _szerokosc;
        private double _wysokosc;
        private Random _random = new Random();

        public Symulator(double szerokosc, double wysokosc)
        {
            _zbior = new ZbiorKul();
            UpdateGranice(szerokosc, wysokosc);
        }

        public void DodajLosoweKule(int liczbaKul, double minPromien, double maxPromien, double minPredkosc, double maxPredkosc)
        {
            for (int i = 0; i < liczbaKul; i++)
            {
                double promien = _random.NextDouble() * (maxPromien - minPromien) + minPromien;
                double x = _random.NextDouble() * (_szerokosc - 2 * promien) + promien;
                double y = _random.NextDouble() * (_wysokosc - 2 * promien) + promien;
                double predkoscX = (_random.NextDouble() * 2 - 1) * (maxPredkosc - minPredkosc) + minPredkosc;
                double predkoscY = (_random.NextDouble() * 2 - 1) * (maxPredkosc - minPredkosc) + minPredkosc;

                DodajKule(x, y, predkoscX, predkoscY, promien);
            }
        }

        public void DodajKule(double x, double y, double predkoscX, double predkoscY, double promien)
        {
            _zbior.AddKula(new Kula(x, y, predkoscX, predkoscY, promien));
        }

        public IEnumerable<Kula> PobierzKule()
        {
            return _zbior.GetKule();
        }

        public void Update()
        {
            foreach (var kula in _zbior.GetKule())
            {
                double nowyX = kula.X + kula.PredkoscX;
                double nowyY = kula.Y + kula.PredkoscY;

                if (nowyX - kula.Promien < 0)
                {
                    kula.PredkoscX *= -1;
                    nowyX = kula.Promien; 
                }
                else if (nowyX + kula.Promien > _szerokosc)
                {
                    kula.PredkoscX *= -1;
                    nowyX = _szerokosc - kula.Promien;
                }

                if (nowyY - kula.Promien < 0)
                {
                    kula.PredkoscY *= -1;
                    nowyY = kula.Promien;
                }
                else if (nowyY + kula.Promien > _wysokosc)
                {
                    kula.PredkoscY *= -1;
                    nowyY = _wysokosc - kula.Promien;
                }

                kula.X = nowyX;
                kula.Y = nowyY;
            }
        }

        public void UpdateGranice(double szerokosc, double wysokosc)
        {
            _szerokosc = szerokosc - 15;
            _wysokosc = wysokosc - 38;
        }
    }
}
