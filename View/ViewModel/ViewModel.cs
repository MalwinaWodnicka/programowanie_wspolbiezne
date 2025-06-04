using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Logic;
using Logika;
using Microsoft.VisualBasic;
using View.Model;

namespace ViewModel
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);
    }
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
        private ObservableCollection<Model> _kule = new ObservableCollection<Model>();
        private double _width;
        private double _height;

        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged();
                UpdateGranice();
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged();
                UpdateGranice();
            }
        }

        public ObservableCollection<Model> Kule
        {
            get => _kule;
            set
            {
                _kule = value;
                OnPropertyChanged();
            }
        }

        public ICommand InterakcjaCommand { get; }
        public ICommand LoadedCommand { get; }
        public ICommand SizeChangedCommand { get; }

        public MainViewModel()
        {
            _symulator = new Symulator(800, 450);
            _symulator.KuleUpdated += OnKuleUpdated;
            _symulator.StartUpdating(16);

            InterakcjaCommand = new RelayCommand(ExecuteInterakcja);
            LoadedCommand = new RelayCommand(OnLoaded);
            SizeChangedCommand = new RelayCommand(OnSizeChanged);
        }

        private void OnKuleUpdated(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Kule.Clear();
                foreach (var kula in _symulator.PobierzKule())
                {
                    Kule.Add(new Model
                    {
                        X = kula.X - kula.Promien,
                        Y = kula.Y - kula.Promien,
                        Promien = kula.Promien
                    });
                }
            });
        }

        private void OnLoaded(object parameter)
        {
            if (parameter is Window window)
            {
                Width = window.ActualWidth;
                Height = window.ActualHeight;
            }
        }

        public void OnSizeChanged(object parameter)
        {
            if (parameter is SizeChangedEventArgs e)
            {
                Width = e.NewSize.Width;
                Height = e.NewSize.Height;
            }
        }

        private void UpdateGranice()
        {
            _symulator.UpdateGranice(Width, Height);
        }

        private void ExecuteInterakcja(object parameter)
        {
            if (!(parameter is Window window)) return;

            string input = Interaction.InputBox(
                "Wprowadź liczbę kul (1-100):",
                "Generowanie kul",
                "5",
                (int)window.Left + (int)window.Width / 2 - 150,
                (int)window.Top + (int)window.Height / 2 - 75);

            if (int.TryParse(input, out int liczbaKul))
            {
                if (liczbaKul < 1)
                {
                    MessageBox.Show("Liczba kul nie może być mniejsza niż 1. Używam domyślnej wartości (5).",
                                  "Błąd",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    liczbaKul = 5;
                }
                else if (liczbaKul > 100)
                {
                    MessageBox.Show("Maksymalna liczba kul to 100. Ustawiam wartość na 100.",
                                  "Informacja",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                    liczbaKul = 100;
                }

                _symulator.ClearKule();
                _symulator.DodajLosoweKule(liczbaKul, 10, 30, 100, 150);
            }
            else
            {
                MessageBox.Show("Nieprawidłowa wartość. Używam domyślnej liczby kul (5).",
                              "Błąd",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                _symulator.ClearKule();
                _symulator.DodajLosoweKule(5, 10, 30, 1, 5);
            }
        }
        public void Dispose()
        {
            _symulator.StopUpdating();
            _symulator.KuleUpdated -= OnKuleUpdated;
        }
    }
}