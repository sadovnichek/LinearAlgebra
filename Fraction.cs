using System;
using System.Linq;

namespace LinearAlgebra
{
    public class Fraction : IComparable
    {
        private readonly long Numerator;
        private readonly long Denominator;

        public Fraction()
        {
            Numerator = 0;
            Denominator = 1;
        }

        public Fraction(long numerator, long denominator)
        {
            if (numerator == 0 && denominator == 0)
            {
                Numerator = 0;
                Denominator = 1;
            }
            else if(numerator % denominator == 0)
            {
                Numerator = numerator / denominator;
                Denominator = 1;
            }
            else if (Gcd(numerator, denominator) != 1)
            {
                var greatCommonDivisor = Gcd(Math.Abs(numerator), Math.Abs(denominator));
                Numerator = numerator / greatCommonDivisor;
                Denominator = denominator / greatCommonDivisor;
            }
            else
            {
                Numerator = numerator;
                Denominator = denominator;
            }
        }

        public static Fraction Zero => new Fraction(0, 1);

        private static long Gcd(long a, long b)
        {
            while (b != 0)
            {
                a %= b;
                (a, b) = (b, a);
            }
            return a;
        }

        private static long Lcm(long a, long b)
        {
            return a * b / Gcd(a, b);
        }

        public double ApproximateValue => (double) Numerator / Denominator;

        public int CompareTo(object obj)
        {
            if (obj is int || obj is double)
                return ApproximateValue.CompareTo(obj);
            else if (obj is Fraction)
            {
                var val = obj as Fraction;
                return ApproximateValue.CompareTo(val.ApproximateValue);
            }
            throw new ArgumentException();
        }

        public override string ToString()
        {
            if (Denominator == 1 || Numerator == 0)
                return Numerator.ToString();
            else if (Denominator < 0 && Numerator > 0)
                return "-" + Numerator + "/" + (-1 * Denominator);
            else if (Denominator > 0 && Numerator < 0)
                return "-" + -1 * Numerator + "/" + Denominator;
            else if (Denominator < 0 && Numerator < 0)
                return -1 * Numerator + "/" + -1 * Denominator;
            return Numerator + "/" + Denominator;
        }

        public override bool Equals(object obj)
        {
            if (obj is int @int)
            {
                var value = Parse(@int);
                return this == value;
            }
            if (obj is double @double)
            {
                var value = Parse(@double);
                return this == value;
            }
            var x = obj as Fraction;
            return Numerator == x.Numerator && Denominator == x.Denominator;
        }

        public override int GetHashCode()
        {
            return (int)(Numerator * 1023 + Denominator);
        }

        public static Fraction operator +(Fraction left, Fraction right)
        {
            if (right == Zero)
                return left;
            else if (left == Zero)
                return right;
            else if (left.Denominator == right.Denominator)
                return new Fraction(left.Numerator + right.Numerator, left.Denominator);
            else
            {
                var l = Lcm(left.Denominator, right.Denominator);
                return new Fraction(left.Numerator * (l / left.Denominator) + right.Numerator * (l / right.Denominator), l);
            }
        }

        public static Fraction operator *(Fraction left, Fraction right)
        {
            if (right == Zero || left == Zero)
                return Zero;
            return new Fraction(left.Numerator * right.Numerator, left.Denominator * right.Denominator);
        }

        public static Fraction operator -(Fraction left, Fraction right)
        {
            return left + (-1 * right);
        }

        public static Fraction operator /(Fraction left, Fraction right)
        {
            return left * new Fraction(right.Denominator, right.Numerator);
        }

        public static bool operator ==(Fraction left, Fraction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Fraction left, Fraction right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Fraction left, Fraction right)
        {
            return left.ApproximateValue < right.ApproximateValue;
        }

        public static bool operator >(Fraction left, Fraction right)
        {
            return left.ApproximateValue > right.ApproximateValue;
        }

        public static bool operator <=(Fraction left, Fraction right)
        {
            return left.ApproximateValue <= right.ApproximateValue;
        }

        public static bool operator >=(Fraction left, Fraction right)
        {
            return left.ApproximateValue >= right.ApproximateValue;
        }

        public static implicit operator Fraction(int v)
        {
            return new Fraction(v, 1);
        }

        public static Fraction Abs(Fraction value)
        {
            if (value < 0)
                return -1 * value;
            return value;
        }

        public static Fraction Parse(object value)
        {
            if(value is int @int)
                return new Fraction(@int, 1);
            if(value is double @double)
            {
                var v = Math.Round(@double, 2) * 100;
                return new Fraction((int)v, 100);
            }
            if(value is string)
            {
                var nums = value.ToString().Split('/').Select(x => int.Parse(x)).ToArray();
                if(nums.Length == 2)
                    return new Fraction(nums[0], nums[1]);
                else if (nums.Length == 1)
                    return new Fraction(nums[0], 1);
            }
            throw new ArgumentException();
        }
    }
}