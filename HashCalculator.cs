using System;

namespace Lab5 {
    public static class HashCalculator {
        public static int CalculatePolynomHash(string key) {
            var hash = 0;
            const int p = 31;
            var pow = 1;
            foreach(var c in key) {
                hash += (c - 'a' + 1) * pow;
                pow *= p;
            }
            return hash & int.MaxValue;
        }
    }
}
