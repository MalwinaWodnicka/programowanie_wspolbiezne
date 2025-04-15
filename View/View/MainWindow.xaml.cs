using System.Windows;
using System.Windows.Controls;
using Logic;
using Logika;
using ViewModel;

namespace View
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel(new Symulator(ActualWidth, ActualHeight));
            DataContext = _viewModel;

            SizeChanged += (s, e) =>
            {
                _viewModel.UpdateGranice(ActualWidth, ActualHeight);
            };

            Loaded += (s, e) =>
            {
                _viewModel.UpdateGranice(ActualWidth, ActualHeight);
                _viewModel.Interakcja(this);
            };
        }
    }
}