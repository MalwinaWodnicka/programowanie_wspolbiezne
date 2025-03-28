using Xunit;
using Model;

namespace Model.Test
{
    public class KulaTest
    {
        [Fact]
        public void Konstruktor_PoprawnieInicjalizujeWartosci()
        {
            // Arrange
            double x = 10, y = 20, predkoscX = 2, predkoscY = -3, promien = 5;

            // Act
            var kula = new Kula(x, y, predkoscX, predkoscY, promien);

            // Assert
            Assert.Equal(x, kula.X);
            Assert.Equal(y, kula.Y);
            Assert.Equal(predkoscX, kula.PredkoscX);
            Assert.Equal(predkoscY, kula.PredkoscY);
            Assert.Equal(promien, kula.Promien);
        }

        [Fact]
        public void Wlasciwosci_MoznaZmieniac()
        {
            // Arrange
            var kula = new Kula(0, 0, 0, 0, 1);

            // Act
            kula.X = 100;
            kula.Y = 200;
            kula.PredkoscX = -1.5;
            kula.PredkoscY = 2.5;
            kula.Promien = 10;

            // Assert
            Assert.Equal(100, kula.X);
            Assert.Equal(200, kula.Y);
            Assert.Equal(-1.5, kula.PredkoscX);
            Assert.Equal(2.5, kula.PredkoscY);
            Assert.Equal(10, kula.Promien);
        }
    }
}