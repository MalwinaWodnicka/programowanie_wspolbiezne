using System;
using System.Collections.Generic;
using System.Text;
using Data;


namespace Logika
{
    public interface ISymulator
    {
        void UpdateGranice(double szerokosc, double wysokosc);
        void DodajLosoweKule(int liczbaKul, double minPromien, double maxPromien, double minPredkosc, double maxPredkosc);
        void Update();
        void DodajKule(double x, double y, double predkoscX, double predkoscY, double promien);
        IEnumerable<Kula> PobierzKule();
        void ClearKule();

        event EventHandler KuleUpdated;
        void StartUpdating(int intervalMs);
        void StopUpdating();
    }
}