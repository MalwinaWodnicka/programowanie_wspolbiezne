using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace View.Model
{
    public class Model
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Promien { get; set; }
        public Brush Fill { get; set; } = Brushes.Black;
        public double Width => Promien * 2;
        public double Height => Promien * 2;
    }
}
