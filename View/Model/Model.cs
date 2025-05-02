using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ViewModel;

namespace View.Model
{
    public class Model : ObservableObject
    {
        private double _x;
        private double _y;
        private double _promien;
        private Brush _fill = Brushes.Black;

        public double X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(); }
        }

        public double Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(); }
        }

        public double Promien
        {
            get => _promien;
            set { _promien = value; OnPropertyChanged(); OnPropertyChanged(nameof(Width)); OnPropertyChanged(nameof(Height)); }
        }

        public Brush Fill
        {
            get => _fill;
            set { _fill = value; OnPropertyChanged(); }
        }

        public double Width => Promien * 2;
        public double Height => Promien * 2;
    }
}