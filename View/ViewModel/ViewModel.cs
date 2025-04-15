using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Logika;
using Microsoft.VisualBasic;
using View.Model;

namespace ViewModel
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MainViewModel : ObservableObject
    {
        private readonly ISymulator _symulator;
        private readonly DispatcherTimer _timer;
        private ObservableCollection<Model> _kule = new ObservableCollection<Model>();

        public ObservableCollection<Model> Kule
        {
            get => _kule;
            set
            {
                _kule = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(ISymulator symulator)
        {
            _symulator = symulator;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public void UpdateGranice(double width, double height)
        {
            _symulator.UpdateGranice(width, height);
        }

        public void Interakcja(Window window)
        {
            string input = Interaction.InputBox(
                "Wprowadź liczbę kul:",
                "Generowanie kul",
                "5",
                (int)window.Left + (int)window.Width / 2 - 150,
                (int)window.Top + (int)window.Height / 2 - 75);

            if (int.TryParse(input, out int liczbaKul) && liczbaKul > 0)
            {
                _symulator.DodajLosoweKule(liczbaKul, 10, 30, 1, 5);
            }
            else
            {
                MessageBox.Show("Nieprawidłowa wartość. Używam domyślnej liczby kul (5).",
                                "Błąd",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                _symulator.DodajLosoweKule(5, 10, 30, 1, 5);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _symulator.Update();
            AktualizujKule();
        }

        private void AktualizujKule()
        {
            Kule.Clear();
            foreach (var kula in _symulator.PobierzKule())
            {
                Kule.Add(new Model
                {
                    X = kula.X,
                    Y = kula.Y,
                    Promien = kula.Promien
                });
            }
        }
    }
}