using Xunit;
using Logic;
using Data;
using Model;
using System.Linq;

namespace Logika.Test
{
    public class SymulatorTest
    {
        [Fact]
        public void Kula_OdbijaSieOdPrawejSciany()
        {
            var zbior = new ZbiorKul();
            var kula = new Kula(95, 50, 10, 0, 5);
            zbior.AddKula(kula);
            var symulator = new Symulator(zbior, 100, 100); 

            symulator.Update();

            Assert.Equal(-10, kula.PredkoscX);
        }

        [Fact]
        public void Kula_OdbijaSieOdLewejSciany()
        {
            var zbior = new ZbiorKul();
            var kula = new Kula(5, 50, -10, 0, 5);
            zbior.AddKula(kula);
            var symulator = new Symulator(zbior, 100, 100);

            symulator.Update();

            Assert.Equal(10, kula.PredkoscX);
        }

        [Fact]
        public void Kula_OdbijaSieOdDolnejSciany()
        {
            var zbior = new ZbiorKul();
            var kula = new Kula(50, 95, 0, 10, 5);
            zbior.AddKula(kula);
            var symulator = new Symulator(zbior, 100, 100);

            symulator.Update();

            Assert.Equal(-10, kula.PredkoscY);
        }

        [Fact]
        public void Kula_OdbijaSieOdGornejSciany()
        {
            var zbior = new ZbiorKul();
            var kula = new Kula(50, 5, 0, -10, 5);
            zbior.AddKula(kula);
            var symulator = new Symulator(zbior, 100, 100);

            symulator.Update();

            Assert.Equal(10, kula.PredkoscY);
        }

        [Fact]
        public void Kula_PoruszaSieBezOdbicia()
        {
            var zbior = new ZbiorKul();
            var kula = new Kula(50, 50, 5, 5, 5);
            zbior.AddKula(kula);
            var symulator = new Symulator(zbior, 200, 200);

            symulator.Update();

            Assert.Equal(55, kula.X);
            Assert.Equal(55, kula.Y);
            Assert.Equal(5, kula.PredkoscX);
            Assert.Equal(5, kula.PredkoscY);
        }
    }
}