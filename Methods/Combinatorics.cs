using LinearAlgebra.Models;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra.Methods
{
    public class Combinatorics
    {
        public static long Factorial(int n)
        {
            if (n == 0 || n == 1) return 1;
            else return n * Factorial(n - 1);
        }

        public static IEnumerable<Vector> GetAllPermutations(Vector v)
        {
            var result = v;
            yield return result;
            for (int c = 0; c < Factorial(v.Size) - 1; c++)
            {
                var max_j = 0;
                for (int j = 0; j < result.Size - 1; j++)
                {
                    if (result[j] < result[j + 1])
                        max_j = j;
                }
                var max_l = 0;
                for (int l = max_j + 1; l < result.Size; l++)
                {
                    if (result[l] > result[max_j])
                        max_l = l;
                }
                var permutation = result.Swap(max_l, max_j);
                var first_part = permutation.Take(max_j + 1);
                var reversed_part = permutation.Skip(max_j + 1).Take(result.Size - max_j - 1).Reverse();
                result = new Vector(first_part.Concat(reversed_part));
                yield return result;
            }
        }
    }
}