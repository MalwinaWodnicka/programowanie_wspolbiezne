﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dane;
using Data;
using Logika;

namespace Logic
{
    public class Symulator : ISymulator
    {
        private readonly IZbiorKul _zbior;
        private double _szerokosc;
        private double _wysokosc;
        private Random _random = new Random();
        private readonly object _updateLock = new object();
        private readonly System.Timers.Timer _updateTimer;
        public event EventHandler KuleUpdated;
        private readonly Logger _logger;
        private readonly System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private long _lastUpdateTicks;
        public Symulator(double szerokosc, double wysokosc) : this(new ZbiorKul(), szerokosc, wysokosc)
        {
        }

        public Symulator(IZbiorKul zbiorKul, double szerokosc, double wysokosc)
        {
            _zbior = zbiorKul ?? throw new ArgumentNullException(nameof(zbiorKul));
            UpdateGranice(szerokosc, wysokosc);

            _logger = new Logger();

            _updateTimer = new System.Timers.Timer();
            _updateTimer.Elapsed += (sender, e) =>
            {
                _stopwatch.Start();
                long currentTicks = _stopwatch.ElapsedTicks;
                long elapsedTicks = currentTicks - _lastUpdateTicks;
                _lastUpdateTicks = currentTicks;

                double elapsedSeconds = (double)elapsedTicks / System.Diagnostics.Stopwatch.Frequency;

                UpdateWithTime(elapsedSeconds);
                KuleUpdated?.Invoke(this, EventArgs.Empty);
            };
        }

        public void StartUpdating(int intervalMs)
        {
            _updateTimer.Interval = intervalMs;
            _updateTimer.Start();
        }

        public void StopUpdating()
        {
            _updateTimer.Stop();
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

        public void UpdateWithTime(double elapsedSeconds)
        {
            lock (_updateLock)
            {
                var kule = new List<Kula>(_zbior.GetKule());
                var now = DateTime.Now;

                Parallel.ForEach(kule, kula =>
                {
                    UpdateKulaPosition(kula, elapsedSeconds);
                    HandleWallCollision(kula);
                    kula.LastUpdateTime = now;
                });

                for (int i = 0; i < kule.Count; i++)
                {
                    for (int j = i + 1; j < kule.Count; j++)
                    {
                        if (IsColliding(kule[i], kule[j]))
                        {
                            HandleBallCollision(kule[i], kule[j], elapsedSeconds);
                        }
                    }
                }
            }
        }

        private void UpdateKulaPosition(Kula kula, double elapsedSeconds)
        {
            //double speedMultiplier = 50;
            kula.X += kula.PredkoscX * elapsedSeconds; //* speedMultiplier;
            kula.Y += kula.PredkoscY * elapsedSeconds; //* speedMultiplier;
        }

        private void HandleWallCollision(Kula kula)
        {
            if (kula.X - kula.Promien < 0)
            {
                kula.X = kula.Promien;
                kula.PredkoscX = -kula.PredkoscX;
                _logger.LogWallCollision(kula, "lewa");
            }
            else if (kula.X + kula.Promien > _szerokosc)
            {
                kula.X = _szerokosc - kula.Promien;
                kula.PredkoscX = -kula.PredkoscX;
                _logger.LogWallCollision(kula, "prawa");
            }

            if (kula.Y - kula.Promien < 0)
            {
                kula.Y = kula.Promien;
                kula.PredkoscY = -kula.PredkoscY;
                _logger.LogWallCollision(kula, "gorna");
            }
            else if (kula.Y + kula.Promien > _wysokosc)
            {
                kula.Y = _wysokosc - kula.Promien;
                kula.PredkoscY = -kula.PredkoscY;
                _logger.LogWallCollision(kula, "dolna");
            }
        }


        private bool IsColliding(Kula a, Kula b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance < (a.Promien + b.Promien);
        }

        private void HandleBallCollision(Kula a, Kula b, double elapsedSeconds)
        {
            double aX = a.X + a.PredkoscX * elapsedSeconds;
            double aY = a.Y + a.PredkoscY * elapsedSeconds;
            double bX = b.X + b.PredkoscX * elapsedSeconds;
            double bY = b.Y + b.PredkoscY * elapsedSeconds;

            double nx = bX - aX;
            double ny = bY - aY;
            double distance = Math.Sqrt(nx * nx + ny * ny);
            nx /= distance;
            ny /= distance;

            double dvx = b.PredkoscX - a.PredkoscX;
            double dvy = b.PredkoscY - a.PredkoscY;

            double velocityAlongNormal = dvx * nx + dvy * ny;

            if (velocityAlongNormal > 0) return;

            double restitution = 1.0;
            double j = -(1 + restitution) * velocityAlongNormal;
            j /= (1 / a.Masa) + (1 / b.Masa);

            double impulseX = j * nx;
            double impulseY = j * ny;

            a.PredkoscX -= impulseX / a.Masa;
            a.PredkoscY -= impulseY / a.Masa;
            b.PredkoscX += impulseX / b.Masa;
            b.PredkoscY += impulseY / b.Masa;

            double overlap = (a.Promien + b.Promien) - distance;
            double moveX = overlap * nx * 0.5;
            double moveY = overlap * ny * 0.5;

            a.X -= moveX;
            a.Y -= moveY;
            b.X += moveX;
            b.Y += moveY;

            _logger.LogCollision(a, b);
        }

        public void UpdateGranice(double szerokosc, double wysokosc)
        {
            _szerokosc = szerokosc - 15;
            _wysokosc = wysokosc - 80;
        }

        public void ClearKule()
        {
            _zbior.ClearKule();
        }
        public void Dispose()
        {
            _updateTimer?.Dispose();
            _logger?.Dispose();
        }
    }
}