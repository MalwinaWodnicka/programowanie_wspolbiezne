using Xunit;
using Data;
using Model;
using System.Linq;

namespace Dane.Test
{
    public class ZbiorKulTest
    {
        [Fact]
        public void AddKulaToList()
        {
            var zbior = new ZbiorKul();
            var kula = new Kula(10, 10, 1, 1, 5);

            zbior.AddKula(kula);

            Assert.Single(zbior.GetKule());
            Assert.Equal(kula, zbior.GetKule().First());
        }

        [Fact]
        public void ClearKule()
        {
            var zbior = new ZbiorKul();
            zbior.AddKula(new Kula(10, 10, 1, 1, 5));

            zbior.ClearKule();

            Assert.Empty(zbior.GetKule());
        }
    }
}