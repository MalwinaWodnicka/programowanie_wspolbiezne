using System;
using System.Collections.Generic;
using Model;

namespace Data
{
    public class ZbiorKul
    {
        private readonly List<Kula> _kule = new List<Kula>();

        public IEnumerable<Kula> GetKule()
        {
            return _kule;
        }

        public void AddKula(Kula kula)
        {
            if (kula != null)
            {
                _kule.Add(kula);
            }
        }

        public void ClearKule()
        {
            _kule.Clear();
        }
    }
}