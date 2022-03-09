using LinearAlgebra.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra.Methods
{
    public class Polynomial
    {
        public Vector Coefficients { get; }

        public Polynomial(Vector coefficients)
        {
            this.Coefficients = coefficients;
        }

        public Polynomial(params double[] coefficients)
        {
            this.Coefficients = new Vector(coefficients);
        }

        private double FindBound()
        {
            double max = -1;
            for(int i = 0; i < Coefficients.Size - 1; i++)
            {
                if(Math.Abs(Coefficients[i]) > max)
                {
                    max = Math.Abs(Coefficients[i]);
                }
            }
            return max / Math.Abs(Coefficients[Coefficients.Size - 1]) + 1;
        }

        private static Vector GetDerivative(Vector coefficients)
        {
            var result = new Vector(coefficients.Size - 1);
            for(int i = 0; i < coefficients.Size - 1; i++)
            {
                result[i] = (i + 1) * coefficients[i + 1];
            }
            return result;
        }

        public static int GetRootMultiplicity(Vector coefficients, double root)
        {
            var multiplicity = 1;
            coefficients = GetDerivative(coefficients);
            while (Math.Abs(GetValue(coefficients, root)) <= 1e-4)
            {
                multiplicity++;
                coefficients = GetDerivative(coefficients);
            }
            return multiplicity;
        }

        public static double GetValue(Vector coefficients, double value)
        {
            double result = 0;
            for (int i = 0; i < coefficients.Size; i++)
            {
                if (coefficients[i] != 0)
                    result += coefficients[i] * Math.Pow(value, i);
            }
            return result;
        }

        private Dictionary<double, int> GetIntegerRoots(Vector coefficients)
        {
            var free = coefficients[coefficients.FindFirstNonZeroElementIndex()];
            var result = new Dictionary<double, int>(); // <корень, его кратность>
            var dividers = new List<double>();
            for (int i = 1; i < Math.Round(Math.Sqrt(free)); i++)
            {
                if (free % i == 0)
                {
                    dividers = dividers.Concat(new List<double>() { i, -i}).ToList();
                    if (i * i != free)
                        dividers = dividers.Concat(new List<double>() { free/i, -free/i }).ToList();
                }
            }
            foreach(var divider in dividers)
            {
                if(GetValue(coefficients, divider) == 0)
                    result.Add(divider, GetRootMultiplicity(coefficients, divider));
            }
            if (GetValue(coefficients, 0) == 0) // 0 - это корень
                result.Add(0, GetRootMultiplicity(coefficients, 0));
            return result;
        }

        // с учетом кратности, сортировка по убыванию кратности
        public List<double> Solve()
        {
            var bound = FindBound();
            var epsilon = 1e-4;
            var roots = GetIntegerRoots(Coefficients);
            var result = new List<double>();
            if (roots.Values.Sum() != Coefficients.Size - 1)
            {
                for (var x = -bound; x <= bound; x += epsilon)
                {
                    x = Math.Round(x, 4);
                    var left = GetValue(Coefficients, x);
                    var right = GetValue(Coefficients, x + epsilon);
                    if (left == 0 && !roots.ContainsKey(x))
                    {
                        var multiplicity = GetRootMultiplicity(Coefficients, x);
                        roots.Add(x, multiplicity);
                        x += 9 * epsilon;
                    }
                    else if (Math.Sign(left) != Math.Sign(right) && left * right != 0 && !roots.ContainsKey(x))
                    {
                        var root = Math.Round(x + 0.5 * epsilon, 4);
                        var multiplicity = GetRootMultiplicity(Coefficients, root);
                        roots.Add(root, multiplicity);
                        x += 9 * epsilon;
                    }
                }
            }
            var sorted = roots.OrderByDescending(x => x.Value);
            foreach (var root in sorted)
            {
                result.AddRange(Enumerable.Repeat(root.Key, root.Value));
            }
            if (result.Count != Coefficients.Size - 1)
                throw new ArgumentException("Equation has a complex roots");
            return result;
        }

        public override string ToString()
        {
            var output = "$ ";
            var startIndex = Coefficients.FindFirstNonZeroElementIndex();
            for(int i = 0; i < Coefficients.Size; i++)
            {
                var currentCoefficient = Coefficients[i];
                var sign = (i == startIndex) ? "" : (currentCoefficient > 0 ? "+ " : "- ");
                if (currentCoefficient != 0) // нулевой коэффициент не прописывается
                {
                    if (i == 0) // свободный член
                        output += currentCoefficient + " ";
                    else if (i == 1) // коэфициент при x 
                    {
                        if (Math.Abs(currentCoefficient) == 1) // коэфициент единица - не прописывается при x
                            output += sign + "t ";
                        else
                            output += sign + Math.Abs(currentCoefficient) + "t ";
                    }
                    else // остальные коэффициенты, учитаваются степени при x
                    {
                        if (Math.Abs(currentCoefficient) == 1)
                            output += sign + "t^{" + i + "} ";
                        else
                            output += sign + Math.Abs(currentCoefficient) + "t^{" + i + "} ";
                    }
                }
            }
            return output + "$";
        }

        public static Vector Multiply(Vector a, Vector b)
        {
            var table = new double[a.Size, b.Size];
            var result = new List<double>();
            for(int i = 0; i < a.Size;i++)
            {
                for(int j = 0; j < b.Size; j++)
                {
                    table[i, j] = a[i] * b[j];
                }
            }
            for(int j = 0; j < b.Size; j++)
            {
                int i = 0;
                int k = j;
                var sum = table[0, j];
                while (k > 0 && i + 1 < a.Size)
                {
                    k--;
                    i++;
                    sum += table[i, k];
                }
                result.Add(sum);
            }
            for(int i = 1; i < a.Size; i++)
            {
                int j = b.Size - 1;
                int k = i;
                var sum = table[i, b.Size - 1];
                while (k + 1 < a.Size && j > 0)
                {
                    k++;
                    j--;
                    sum += table[k, j];
                }
                result.Add(sum);
            }
            return new Vector(result);
        }
    }
}
