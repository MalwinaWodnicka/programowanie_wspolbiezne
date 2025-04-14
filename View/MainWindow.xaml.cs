using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Logic;
using Logika;
using System.Windows.Threading;

namespace View
{
    public partial class MainWindow : Window
    {
        private readonly ISymulator _symulator;
        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            _symulator = new Symulator(ActualWidth, ActualHeight);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            SizeChanged += (s, e) =>
            {
                _symulator.UpdateGranice(ActualWidth, ActualHeight);
            };

            Loaded += (s, e) =>
            {
                _symulator.UpdateGranice(ActualWidth, ActualHeight);
                _symulator.DodajLosoweKule(5, 10, 30, 1, 5);
            };
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _symulator.Update();
            RysujKule();
        }

        private void RysujKule()
        {
            MainCanvas.Children.Clear();
            foreach (var kula in _symulator.PobierzKule())
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