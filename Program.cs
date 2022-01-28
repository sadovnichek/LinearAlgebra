using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra
{
    class Program
    {
        public static int f(int a, int b)
        {
            if (a == 0 && b == 0) return 1;
            if (a == 1 && b == 1) return 3;
            if (a == 2 && b == 2) return 1;
            if (a == 3 && b == 3) return 3;
            if (a == 0 && b == 1 || a == 1 && b == 0) return 2;
            if (a == 0 && b == 2 || a == 2 && b == 0) return 3;
            if (a == 0 && b == 3 || a == 3 && b == 0) return 0;
            if (a == 1 && b == 2 || a == 2 && b == 1) return 0;
            if (a == 1 && b == 3 || a == 3 && b == 1) return 1;
            if (a == 2 && b == 3 || a == 3 && b == 2) return 2;
            else
                throw new ArgumentException();
        }

        public static int g(int a, int b)
        {
            if (a == 0 && b == 0) return 1;
            if (a == 1 && b == 1) return 0;
            if (a == 2 && b == 2) return 2;
            if (a == 3 && b == 3) return 1;
            if (a == 0 && b == 1) return 3;
            if (a == 0 && b == 2) return 0;
            if (a == 0 && b == 3) return 3;
            if (a == 1 && b == 2) return 1;
            if (a == 1 && b == 3) return 2;
            if (a == 2 && b == 3) return 3;

            if (a == 1 && b == 0) return 3;
            if (a == 2 && b == 0) return 0;
            if (a == 3 && b == 0) return 2;
            if (a == 2 && b == 1) return 1;
            if (a == 3 && b == 1) return 2;
            if (a == 3 && b == 2) return 3;
            else
                throw new ArgumentException();

        }

        public static void Main()
        {
            var f_array = new Fraction[4, 4];
            var g_array = new Fraction[4, 4];
            for(int a = 0; a < 4; a++)
            {
                for(int b = 0; b < 4; b++)
                {
                    f_array[a,b] = f(a, b);
                    g_array[a, b] = g(a, b);
                }
            }
            var f_matrix = new Matrix(f_array);
            var g_matrix = new Matrix(g_array);

            var v = new Vector(0, 1, 2, 3);

            var permutations = Сombinatorics.getAllPermutations(v);
            foreach (var p in permutations)
            {
                var current_array = new Fraction[4, 4];
                for (int a = 0; a < 4; a++)
                {
                    for (int b = 0; b < 4; b++)
                    {
                        current_array[p[a].IntValue(), p[b].IntValue()] = p[f(a, b)];
                    }
                }
                var currentMatrix = new Matrix(current_array);
                Console.WriteLine(p);
                currentMatrix.Print();
            }
        }
    }
}