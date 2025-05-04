using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Xunit;

namespace Dane.Test
{
    public class KulaTest
    {
        [Theory]
        [InlineData(10, 20, 2, -3, 5)]
        [InlineData(0, 0, 0, 0, 1)]
        [InlineData(-5, -10, -1.5, 2.5, 3)]
        public void Konstruktor_PoprawnieInicjalizujeWartosci(double x, double y, double predkoscX, double predkoscY, double promien)
        {
            // Act
            var kula = new Kula(x, y, predkoscX, predkoscY, promien);

            // Assert
            Assert.Equal(x, kula.X);
            Assert.Equal(y, kula.Y);
            Assert.Equal(predkoscX, kula.PredkoscX);
            Assert.Equal(predkoscY, kula.PredkoscY);
            Assert.Equal(promien, kula.Promien);
            Assert.Equal(promien * promien, kula.Masa);
        }

        [Fact]
        public void WielowatkowaZmianaWartosci()
        {
            // Arrange
            var kula = new Kula(0, 0, 0, 0, 1);
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() => kula.X = 100));
                tasks.Add(Task.Run(() => kula.Y = 200));
                tasks.Add(Task.Run(() => kula.PredkoscX = -1.5));
                tasks.Add(Task.Run(() => kula.PredkoscY = 2.5));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.Equal(100, kula.X);
            Assert.Equal(200, kula.Y);
            Assert.Equal(-1.5, kula.PredkoscX);
            Assert.Equal(2.5, kula.PredkoscY);
        }
    }

    public class ZbiorKulTest
    {
        private IZbiorKul UtworzZbiorKul()
        {
            return new ZbiorKul();
        }

        [Fact]
        public void AddKula_DodajeKuleDoZbioru()
        {
            // Arrange
            IZbiorKul zbior = UtworzZbiorKul();
            var kula = new Kula(10, 10, 1, 1, 5);

            // Act
            zbior.AddKula(kula);

            // Assert
            var kule = zbior.GetKule().ToList();
            Assert.Single(kule);
            Assert.Equal(kula, kule.First());
        }

        [Fact]
        public void AddKula_NullNieJestDodawany()
        {
            // Arrange
            IZbiorKul zbior = UtworzZbiorKul();

            // Act
            zbior.AddKula(null);

            // Assert
            Assert.Empty(zbior.GetKule());
        }

        [Fact]
        public void ClearKule_CzysciZbior()
        {
            // Arrange
            IZbiorKul zbior = UtworzZbiorKul();
            zbior.AddKula(new Kula(10, 10, 1, 1, 5));
            zbior.AddKula(new Kula(20, 20, 2, 2, 10));

            // Act
            zbior.ClearKule();

            // Assert
            Assert.Empty(zbior.GetKule());
        }

        [Fact]
        public void GetKule_ZwracaKopieKolekcji()
        {
            // Arrange
            IZbiorKul zbior = UtworzZbiorKul();
            var kula = new Kula(10, 10, 1, 1, 5);
            zbior.AddKula(kula);

            // Act
            var kule1 = zbior.GetKule().ToList();
            var kule2 = zbior.GetKule().ToList();

            // Assert
            Assert.NotSame(kule1, kule2);
            Assert.Equal(kule1, kule2);
        }

        [Fact]
        public void OperacjeNaZbiorze_SaBezpieczneWielowatkowo()
        {
            // Arrange
            IZbiorKul zbior = UtworzZbiorKul();
            var tasks = new List<Task>();
            var kule = Enumerable.Range(1, 1000)
                .Select(i => new Kula(i, i, i, i, i % 10 + 1))
                .ToList();

            // Act
            foreach (var kula in kule)
            {
                tasks.Add(Task.Run(() => zbior.AddKula(kula)));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert
            var wynikoweKule = zbior.GetKule().ToList();
            Assert.Equal(kule.Count, wynikoweKule.Count);
            Assert.True(kule.All(k => wynikoweKule.Contains(k)));

            tasks.Clear();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => zbior.ClearKule()));
            }

            Task.WaitAll(tasks.ToArray());
            Assert.True(zbior.GetKule().Count() <= 1);
        }
    }
}