using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebra
{
    public class Polynomial
    {
        private static double FindBound(Vector coefficients)
        {
            double max = -1;
            for(int i = 0; i < coefficients.Size - 1; i++)
            {
                if(Math.Abs(coefficients[i]) > max)
                {
                    max = Math.Abs(coefficients[i]);
                }
            }
            return max / Math.Abs(coefficients[coefficients.Size - 1]) + 1;
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

        private static int GetRootMultiplicity(Vector coefficients, double root)
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
                result += coefficients[i] * Math.Pow(value, i);
            }
            return result;
        }

        // с учетом кратности
        public static List<double> Solve(Vector coefficients)
        {
            var bound = FindBound(coefficients);
            var epsilon = 1e-4;
            var roots = new List<double>();
            for (var x = -bound; x <= bound; x += epsilon)
            {
                x = Math.Round(x, 4);
                var left = GetValue(coefficients, x);
                var right = GetValue(coefficients, x + epsilon);
                if (left == 0)
                {
                    var multiplicity = GetRootMultiplicity(coefficients, x);
                    roots.AddRange(Enumerable.Repeat(x, multiplicity));
                } 
                else if (Math.Sign(left) != Math.Sign(right) && left * right != 0)
                {
                    var root = Math.Round(x + 0.5 * epsilon, 4);
                    var multiplicity = GetRootMultiplicity(coefficients, root);
                    roots.AddRange(Enumerable.Repeat(root, multiplicity));
                } 
            }
            if (roots.Count != coefficients.Size - 1)
                throw new ArgumentException("Equation has a complex roots");
            return roots;
        }

        public static void Print(Vector coefficients)
        {
            var output = "";
            for(int i = 0; i < coefficients.Size; i++)
            {
                var currentCoefficient = coefficients[i];
                if (currentCoefficient != 0)
                {
                    if (i == 0)
                        output += currentCoefficient + " ";
                    else if (i == 1)
                    {
                        if (coefficients[i - 1] != 0)
                        {
                            if (currentCoefficient > 0)
                                output += "+ ";
                            else
                            {
                                output += "- ";
                                currentCoefficient = Math.Abs(currentCoefficient);
                            }
                            if (currentCoefficient == 1)
                                output += "x ";
                            else
                                output += currentCoefficient + "x ";
                        }
                        else
                            output += currentCoefficient + "x ";
                    }
                    else
                    {
                        if (coefficients[i - 1] != 0)
                        {
                            if (currentCoefficient > 0)
                                output += "+ ";
                            else
                            {
                                output += "- ";
                                currentCoefficient = Math.Abs(currentCoefficient);
                            }
                            if (currentCoefficient == 1)
                                output += "x^" + i + " ";
                            else
                                output += currentCoefficient + "x^" + i + " ";
                        }
                        else
                        {
                            if (currentCoefficient == 1)
                                output += "x^" + i + " ";
                            else
                                output += currentCoefficient + "x^" + i + " ";
                        }
                    }
                }
            }
            Console.WriteLine(output);
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
