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
            // Arrange
            var symulator = StworzSymulatorZKula(95, 50, 10, 0, 5, 100, 100, out var kula);

            // Act
            symulator.Update();

            // Assert
            Assert.Equal(-10, kula.PredkoscX);
            Assert.Equal(100 - kula.Promien, kula.X); // Zapewniamy, ¿e kula nie wysz³a poza obszar
        }

        [Fact]
        public void Kula_OdbijaSieOdLewejSciany()
        {
            // Arrange
            var symulator = StworzSymulatorZKula(5, 50, -10, 0, 5, 100, 100, out var kula);

            // Act
            symulator.Update();

            // Assert
            Assert.Equal(10, kula.PredkoscX);
            Assert.Equal(kula.Promien, kula.X);
        }

        [Fact]
        public void Kula_OdbijaSieOdDolnejSciany()
        {
            // Arrange
            var symulator = StworzSymulatorZKula(50, 95, 0, 10, 5, 100, 100, out var kula);

            // Act
            symulator.Update();

            // Assert
            Assert.Equal(-10, kula.PredkoscY);
            Assert.Equal(100 - kula.Promien, kula.Y);
        }

        [Fact]
        public void Kula_OdbijaSieOdGornejSciany()
        {
            // Arrange
            var symulator = StworzSymulatorZKula(50, 5, 0, -10, 5, 100, 100, out var kula);

            // Act
            symulator.Update();

            // Assert
            Assert.Equal(10, kula.PredkoscY);
            Assert.Equal(kula.Promien, kula.Y);
        }

        [Fact]
        public void Kula_PoruszaSieBezOdbicia()
        {
            // Arrange
            var symulator = StworzSymulatorZKula(50, 50, 5, 5, 5, 200, 200, out var kula);

            // Act
            symulator.Update();

            // Assert
            Assert.Equal(55, kula.X);
            Assert.Equal(55, kula.Y);
            Assert.Equal(5, kula.PredkoscX);
            Assert.Equal(5, kula.PredkoscY);
        }
    }
}
