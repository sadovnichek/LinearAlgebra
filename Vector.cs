using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LinearAlgebra
{
    public class Vector : IEnumerable<Fraction>
    {
        private readonly List<Fraction> coordinates;
        public int Size => coordinates.Count();
        public double Lenght => Math.Sqrt((this * this).ApproximateValue);

        public Vector(params Fraction[] source)
        {
            coordinates = source.ToList();
        }

        public Vector(IEnumerable<Fraction> source)
        {
            coordinates = source.ToList();
        }

        public Vector(int size)
        {
            coordinates = Enumerable.Repeat(Fraction.Zero, size).ToList();
        }

        public Fraction this[int index]
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
                    coordinates[index] = value;
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

        public static Vector operator *(Fraction t, Vector a)
        {
            List<Fraction> sum = new List<Fraction>();

            for (int i = 0; i < a.Size; i++)
            {
                sum.Add(a.coordinates[i] * t);
            }
            return new Vector(sum);
        }

        public static Fraction operator *(Vector a, Vector b)
        {
            Fraction dotProduct = 0;
            for(int i = 0; i < a.Size; i++)
            {
                dotProduct += a[i] * b[i];
            }
            return dotProduct;
        }

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
            return Fraction.Parse(1 / Lenght) * this;
        }

        public IEnumerator<Fraction> GetEnumerator()
        {
            foreach(var x in coordinates)
            {
                yield return x;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            return "[" +  string.Join(", ", coordinates.Select(x => Math.Round(x.ApproximateValue, 4))) + "]";
        }
        
        using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LinearAlgebra
{
    public class Vector : IEnumerable<Fraction>
    {
        private readonly List<Fraction> coordinates;
        public int Size => coordinates.Count();
        public double Lenght => Math.Sqrt((this * this).ApproximateValue);

        public Vector(params Fraction[] source)
        {
            coordinates = source.ToList();
        }

        public Vector(IEnumerable<Fraction> source)
        {
            coordinates = source.ToList();
        }

        public Vector(int size)
        {
            coordinates = Enumerable.Repeat(Fraction.Zero, size).ToList();
        }

        public Fraction this[int index]
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
                    coordinates[index] = value;
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

        public static Vector operator *(Fraction t, Vector a)
        {
            List<Fraction> sum = new List<Fraction>();

            for (int i = 0; i < a.Size; i++)
            {
                sum.Add(a.coordinates[i] * t);
            }
            return new Vector(sum);
        }

        public static Fraction operator *(Vector a, Vector b)
        {
            Fraction dotProduct = 0;
            for(int i = 0; i < a.Size; i++)
            {
                dotProduct += a[i] * b[i];
            }
            return dotProduct;
        }

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
            return Fraction.Parse(1 / Lenght) * this;
        }

        public IEnumerator<Fraction> GetEnumerator()
        {
            foreach(var x in coordinates)
            {
                yield return x;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            return "[" +  string.Join(", ", coordinates.Select(x => Math.Round(x.ApproximateValue, 4))) + "]";
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
                        if (vector_obj[i] != coordinates[i])
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
