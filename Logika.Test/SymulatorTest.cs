using Xunit;
using Logic;
using Data;
using System.Linq;

namespace Logika.Test
{
    public class SymulatorTest
    {
        private Symulator StworzSymulatorZKula(double x, double y, double predkoscX, double predkoscY, double promien, double szerokosc, double wysokosc, out Kula kula)
        {
            var symulator = new Symulator(szerokosc, wysokosc);
            symulator.DodajKule(x, y, predkoscX, predkoscY, promien);
            kula = symulator.PobierzKule().First();
            return symulator;
        }

        [Fact]
        public void Kula_OdbijaSieOdPrawejSciany()
        {
            var symulator = StworzSymulatorZKula(95, 50, 10, 0, 5, 100, 100, out var kula);

            symulator.Update();

            Assert.Equal(-10, kula.PredkoscX);
            Assert.Equal(100 - kula.Promien - 15, kula.X); 
        }

        [Fact]
        public void Kula_OdbijaSieOdLewejSciany()
        {
            var symulator = StworzSymulatorZKula(5, 50, -10, 0, 5, 100, 100, out var kula);

            symulator.Update();

            Assert.Equal(10, kula.PredkoscX);
            Assert.Equal(kula.Promien, kula.X);
        }

        [Fact]
        public void Kula_OdbijaSieOdDolnejSciany()
        {
            var symulator = StworzSymulatorZKula(50, 95, 0, 10, 5, 100, 100, out var kula);

            symulator.Update();

            Assert.Equal(-10, kula.PredkoscY);
            Assert.Equal(100 - kula.Promien - 38, kula.Y);
        }

        [Fact]
        public void Kula_OdbijaSieOdGornejSciany()
        {
            var symulator = StworzSymulatorZKula(50, 5, 0, -10, 5, 100, 100, out var kula);

            symulator.Update();

            Assert.Equal(10, kula.PredkoscY);
            Assert.Equal(kula.Promien, kula.Y);
        }

        [Fact]
        public void Kula_PoruszaSieBezOdbicia()
        {
            var symulator = StworzSymulatorZKula(50, 50, 5, 5, 5, 200, 200, out var kula);

            symulator.Update();

            Assert.Equal(55, kula.X);
            Assert.Equal(55, kula.Y);
            Assert.Equal(5, kula.PredkoscX);
            Assert.Equal(5, kula.PredkoscY);
        }
    }
}
