using System.Windows;
using System.Windows.Controls;
using Logic;
using Logika;
using ViewModel;

namespace View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.OnSizeChanged(e);
            }
        }
    }
}