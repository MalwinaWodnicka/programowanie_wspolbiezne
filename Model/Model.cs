using System.Windows.Media;

namespace Model
{
    internal class Model
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Promien { get; set; }
        public double Width => Promien * 2;
        public double Height => Promien * 2;
        public Brush Fill { get; set; } = Brushes.Black;
    }
}
