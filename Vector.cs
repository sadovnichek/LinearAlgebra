using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LinearAlgebra
{
    public class Vector : IEnumerable<double>
    {
        private readonly List<double> coordinates = new List<double>();
        public int Size => coordinates.Count();
        public double Lenght => Math.Sqrt((this * this));

        public bool IsZero => coordinates.Sum(x => Math.Abs(x)) == 0;
        private static double Convert(double value)
        {
            var x = Math.Round(value, 6);
            if (Math.Abs(Math.Round(value) - x) <= 1e-3)
                x = (int)Math.Round(x);
            else if (Math.Abs(x) <= 1e-3)
                x = 0;
            return x;
        }

        public Vector(params double[] source)
        {
            foreach(var x in source)
            {
                coordinates.Add(Convert(x));
            }
        }

        public Vector(IEnumerable<double> source)
        {
            foreach (var x in source)
            {
                coordinates.Add(Convert(x));
            }
        }

        public Vector(int size)
        {
            coordinates = Enumerable.Repeat(0d, size).ToList();
        }

        public double this[int index]
        {
            get
            {
                if (index >= 0 && index < Size)
                    return coordinates[index];
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                if (index >= 0 && index < Size)
                    coordinates[index] = Convert(value);
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public static Vector operator +(Vector a, Vector b)
        {
            Vector sum;
            if(a.Size >= b.Size)
                sum = new Vector(a.Size);
            else
                sum = new Vector(b.Size);
            for (int i = 0; i < Math.Min(a.Size, b.Size); i++)
            {
                sum[i] = a.coordinates[i] + b.coordinates[i];
            }

            return sum;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return a + (-1) * b;
        }

        public static Vector operator *(double t, Vector a)
        {
            var sum = new List<double>();

            for (int i = 0; i < a.Size; i++)
            {
                sum.Add(Convert(a.coordinates[i] * t));
            }
            return new Vector(sum);
        }

        public static double operator *(Vector a, Vector b)
        {
            var dotProduct = 0d;
            for(int i = 0; i < a.Size; i++)
            {
                dotProduct += a[i] * b[i];
            }
            return dotProduct;
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !a.Equals(b);
        }

        // Returns index of first non-zero element in vector. Otherwise -1
        public int FindFirstNonZeroElementIndex()
        {
            for(int i = 0; i < Size; i++)
            {
                if (this[i] != 0)
                    return i;
            }
            return -1;
        }

        public Vector Normilize()
        {
            return (1 / Lenght) * this;
        }

        public IEnumerator<double> GetEnumerator()
        {
            foreach(var x in coordinates)
            {
                yield return Convert(x);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            return "[" +  string.Join(", ", coordinates.Select(x => Math.Round(x, 3))) + "]";
        }

        public string GetLatexNotation()
        {
            var result = @"$\begin{pmatrix}";
            for(int i = 0; i < Size; i++)
            {
                result += coordinates[i] + ((i != Size - 1) ? ",& " : @"\end{pmatrix}$");
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector)
            {
                var vector_obj = (Vector)obj;
                if (vector_obj.Size != this.Size)
                    return false;
                else
                {
                    for(int i = 0; i < Size; i++)
                    {
                        if (Math.Abs(vector_obj[i] - coordinates[i]) > 1e-3)
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 1;
            foreach(var elem in this)
            {
                hash = (hash << 3) + hash ^ (int)elem;
            }
            return hash;
        }

        public Vector Swap(int i, int j)
        {
            var min_index = Math.Min(i, j);
            var max_index = Math.Max(i, j);
            var newCoordinates = new List<double>();
            newCoordinates.AddRange(coordinates.Take(min_index));
            newCoordinates.Add(coordinates[max_index]);
            newCoordinates.AddRange(coordinates.Skip(1 + min_index).Take(max_index - min_index - 1));
            newCoordinates.Add(coordinates[min_index]);
            newCoordinates.AddRange(coordinates.Skip(1 + max_index).Take(coordinates.Count - max_index - 1));
            return new Vector(newCoordinates);
        }

        public static Vector GetRandomVector(int size, int maxValue)
        {
            var vector = new Vector(size);
            var x = RandomProvider.Get();
            for (int i = 0; i < size; i++)
            {
                vector.coordinates[i] = Math.Pow(-1, x.Next(2)) * x.Next(maxValue);
            }
            return vector;
        }
    }
}