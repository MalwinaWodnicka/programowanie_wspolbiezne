using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Dane;

namespace Data
{
    public class Kula
    {
        private double _x;
        private double _y;
        private double _predkoscX;
        private double _predkoscY;
        private readonly double _promien;
        private readonly double _masa;
        private readonly object _lock = new object();
        private DateTime _lastUpdateTime;
        private readonly object _timeLock = new object();

        public double X
        {
            get { lock (_lock) { return _x; } }
            set { lock (_lock) { _x = value; } }
        }

        public double Y
        {
            get { lock (_lock) { return _y; } }
            set { lock (_lock) { _y = value; } }
        }

        public double PredkoscX
        {
            get { lock (_lock) { return _predkoscX; } }
            set { lock (_lock) { _predkoscX = value; } }
        }

        public double PredkoscY
        {
            get { lock (_lock) { return _predkoscY; } }
            set { lock (_lock) { _predkoscY = value; } }
        }

        public double Promien => _promien;
        public double Masa => _masa;

        public DateTime LastUpdateTime
        {
            get { lock (_timeLock) { return _lastUpdateTime; } }
            set { lock (_timeLock) { _lastUpdateTime = value; } }
        }

        public Kula(double x, double y, double predkoscX, double predkoscY, double promien)
        {
            _x = x;
            _y = y;
            _predkoscX = predkoscX;
            _predkoscY = predkoscY;
            _promien = promien;
            _masa = promien * promien;
            _lastUpdateTime = DateTime.Now;
        }
    }

    public class ZbiorKul : IZbiorKul
    {
        private readonly List<Kula> _kule = new List<Kula>();
        private readonly object _lock = new object();

        public IEnumerable<Kula> GetKule()
        {
            lock (_lock)
            {
                return new List<Kula>(_kule);
            }
        }

        public void AddKula(Kula kula)
        {
            if (kula != null)
            {
                lock (_lock)
                {
                    _kule.Add(kula);
                }
            }
        }

        public void ClearKule()
        {
            lock (_lock)
            {
                _kule.Clear();
            }
        }
    }
}