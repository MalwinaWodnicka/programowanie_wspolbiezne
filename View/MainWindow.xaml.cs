using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Model;
using Data;
using Logic;
using System.Windows.Threading;

namespace View
{
    public partial class MainWindow : Window
    {
        private readonly ZbiorKul _zbior;
        private readonly Symulator _logika;
        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            _zbior = new ZbiorKul();
            _zbior.AddKula(new Kula(50, 50, 3, 2, 20));
            _zbior.AddKula(new Kula(200, 150, -2, 3, 30));

            _logika = new Symulator(_zbior, ActualWidth, ActualHeight);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            SizeChanged += (s, e) =>
            {
                _logika.UpdateGranice(ActualWidth, ActualHeight);
            };
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _logika.Update();
            RysujKule();
        }

        private void RysujKule()
        {
            MainCanvas.Children.Clear();
            foreach (var kula in _zbior.GetKule())
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = kula.Promien * 2,
                    Height = kula.Promien * 2,
                    Fill = Brushes.Black
                };

                Canvas.SetLeft(ellipse, kula.X - kula.Promien);
                Canvas.SetTop(ellipse, kula.Y - kula.Promien);
                MainCanvas.Children.Add(ellipse);
            }
        }
    }
}