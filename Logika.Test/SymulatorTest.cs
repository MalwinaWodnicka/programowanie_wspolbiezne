using Xunit;
using Logic;
using Data;
using System.Linq;
using Dane;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logika.Test
{
    public class SymulatorTest
    {
        private class TestZbiorKul : IZbiorKul
        {
            public List<Kula> Kule { get; } = new List<Kula>();

            public IEnumerable<Kula> GetKule() => Kule.ToList();
            public void AddKula(Kula kula) => Kule.Add(kula);
            public void ClearKule() => Kule.Clear();
        }

        private ISymulator StworzSymulatorZKula(double x, double y, double predkoscX, double predkoscY,
                                              double promien, double szerokosc, double wysokosc, out Kula kula)
        {
            var zbiorKul = new TestZbiorKul();
            var symulator = new Symulator(zbiorKul, szerokosc, wysokosc);
            symulator.DodajKule(x, y, predkoscX, predkoscY, promien);
            kula = zbiorKul.Kule.First();
            return symulator;
        }

        [Fact]
        public void Kula_OdbijaSieOdPrawejSciany()
        {
            // Arrange
            double szerokosc = 100, wysokosc = 100;
            var symulator = StworzSymulatorZKula(95, 50, 10, 0, 5, szerokosc, wysokosc, out var kula);

            // Act
            symulator.Update();

            // Assert
            Assert.Equal(-10, kula.PredkoscX);
            Assert.Equal(szerokosc - 15 - kula.Promien, kula.X);
        }

        [Fact]
        public void Kula_OdbijaSieOdLewejSciany()
        {
            // Arrange
            double szerokosc = 100, wysokosc = 100;
            var symulator = StworzSymulatorZKula(5, 50, -10, 0, 5, szerokosc, wysokosc, out var kula);

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
            double szerokosc = 100, wysokosc = 100;
            var symulator = StworzSymulatorZKula(50, 95, 0, 10, 5, szerokosc, wysokosc, out var kula);

            // Act
            symulator.Update();

            // Assert
            Assert.Equal(-10, kula.PredkoscY);
            Assert.Equal(wysokosc - 80 - kula.Promien, kula.Y);
        }

        [Fact]
        public void Kula_OdbijaSieOdGornejSciany()
        {
            // Arrange
            double szerokosc = 100, wysokosc = 100;
            var symulator = StworzSymulatorZKula(50, 5, 0, -10, 5, szerokosc, wysokosc, out var kula);

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
            double szerokosc = 200, wysokosc = 200;
            var symulator = StworzSymulatorZKula(50, 50, 5, 5, 5, szerokosc, wysokosc, out var kula);

            // Act
            symulator.Update();

            // Assert
            Assert.Equal(55, kula.X);
            Assert.Equal(55, kula.Y);
            Assert.Equal(5, kula.PredkoscX);
            Assert.Equal(5, kula.PredkoscY);
        }

        [Fact]
        public void UpdateGranice_PoprawnieAktualizujeGranice()
        {
            // Arrange
            var symulator = new Symulator(new TestZbiorKul(), 100, 100);
            double nowaSzerokosc = 200, nowaWysokosc = 300;

            // Act
            symulator.UpdateGranice(nowaSzerokosc, nowaWysokosc);

            // Assert
            var kula = new Kula(195, 295, 10, 10, 5);
            symulator.DodajKule(kula.X, kula.Y, kula.PredkoscX, kula.PredkoscY, kula.Promien);
            symulator.Update();

            Assert.Equal(nowaSzerokosc - kula.Promien, kula.X);
            Assert.Equal(nowaWysokosc - kula.Promien, kula.Y);
        }

        [Fact]
        public void DodajLosoweKule_TworzyOdpowiedniaLiczbeKul()
        {
            // Arrange
            var zbiorKul = new TestZbiorKul();
            var symulator = new Symulator(zbiorKul, 500, 500);
            int liczbaKul = 10;

            // Act
            symulator.DodajLosoweKule(liczbaKul, 5, 15, 1, 5);

            // Assert
            Assert.Equal(liczbaKul, zbiorKul.Kule.Count);
        }

        [Fact]
        public void ClearKule_CzysciWszystkieKule()
        {
            // Arrange
            var zbiorKul = new TestZbiorKul();
            var symulator = new Symulator(zbiorKul, 500, 500);
            symulator.DodajLosoweKule(5, 5, 15, 1, 5);

            // Act
            symulator.ClearKule();

            // Assert
            Assert.Empty(zbiorKul.Kule);
        }

        [Fact]
        public void Update_Wielowatkowosc_BezDeadlockow()
        {
            // Arrange
            var zbiorKul = new TestZbiorKul();
            var symulator = new Symulator(zbiorKul, 500, 500);
            symulator.DodajLosoweKule(20, 5, 15, 1, 5);

            // Act
            Parallel.For(0, 100, i => {
                symulator.Update();
            });

            // Assert
            Assert.True(zbiorKul.Kule.All(k => k.X >= 0 && k.X <= 500 - 15));
            Assert.True(zbiorKul.Kule.All(k => k.Y >= 0 && k.Y <= 500 - 80));
        }
    }
}