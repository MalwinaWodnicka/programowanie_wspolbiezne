using Dane;
using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dane
{
    public interface IZbiorKul
    {
        public IEnumerable<Kula> GetKule();

        public void AddKula(Kula kula);

        public void ClearKule();

    }
}
