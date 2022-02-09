using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace LinearAlgebra
{
    public class Matrix
    {
        public double[,] Data;

        private int precision = 6;

        public int StringSize => Data.GetLength(0);
        public int ColumnSize => Data.GetLength(1);
        public bool IsSquare => StringSize == ColumnSize;

        public bool IsDiagonal => IsMatrixDiagonal();
        public int Rank => GetRank();
        public double Determinant => GetDeterminant();

        private double Convert(double value)
        {
            var x = Math.Round(value, precision);
            if (Math.Abs(Math.Round(value) - x) <= 1e-3)
                x = (int)Math.Round(x);
            return x;
        }

        public Matrix(params Vector[] strings)
        {
            Data = new double[strings.Length, strings[0].Size];

            for(int i = 0; i < strings.Length; i++)
            {
                for(int j = 0; j < strings[0].Size; j++)
                {
                    Data[i, j] = Convert(strings[i][j]);
                }
            }
        }

        public Matrix(List<Vector> strings)
        {
            Data = new double[strings.Count, strings[0].Size];

            for (int i = 0; i < strings.Count; i++)
            {
                for (int j = 0; j < strings[0].Size; j++)
                {
                    Data[i, j] = Convert(strings[i][j]);
                }
            }
        }

        public Matrix(double [,] matrix)
        {
            Data = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for(int i = 0;i < matrix.GetLength(0);i++)
            {
                for(int j = 0;j < matrix.GetLength(1); j++)
                {
                    Data[i, j] = Convert(matrix[i, j]);
                }
            }
        }

        public Matrix(int stringSize, int columnSize)
        {
            Data = new double[stringSize, columnSize];
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    Data[i, j] = 0;
                }
            }
        }

        public Matrix()
        {
            Data = new double[0, 0];
        }

        public double this[int i, int j]
        {
            get { return Data[i, j]; }
            set
            {
                if (i < 0 || j < 0 || i > StringSize || j > ColumnSize)
                    throw new IndexOutOfRangeException();
                Data[i, j] = Convert(value);
            }
        }

        public static Matrix GetIdentityMatrix(int n)
        {
            var result = new double[n,n];
            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if (i == j)
                        result[i, j] = 1;
                    else
                        result[i, j] = 0;
                }
            }
            return new Matrix(result);
        }

        public Vector GetString(int index)
        {
            if (index >= 0 && index < StringSize)
            {
                var result = new List<double>();
                for (int j = 0; j < ColumnSize; j++)
                    result.Add(Data[index, j]);
                return new Vector(result);
            }
            else
                throw new IndexOutOfRangeException();
        }

        public void SetString(int index, Vector v)
        {
            if (index >= 0 && index < StringSize)
            {
                for (int j = 0; j < ColumnSize; j++)
                {
                    Data[index, j] = Convert(v[j]);
                }
            }
            else
                throw new IndexOutOfRangeException();
        }

        public void SetColumn(int index, Vector v)
        {
            if (index >= 0 && index < ColumnSize)
            {
                for (int j = 0; j < StringSize; j++)
                {
                    Data[j, index] = Convert(v[j]);
                }
            }
            else
                throw new IndexOutOfRangeException();
        }

        public Vector GetColumn(int index)
        {
            if (index >= 0 && index < ColumnSize)
            {
                var result = new List<double>();
                for (int i = 0; i < StringSize; i++)
                    result.Add(Data[i, index]);
                return new Vector(result);
            }
            else
                throw new IndexOutOfRangeException();
        }

        public void DeleteColumn(int index)
        {
            var newData = new double[StringSize, ColumnSize - 1];
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < index; j++)
                {
                    newData[i, j] = Data[i, j];
                }
            }
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = index + 1; j < Data.GetLength(1); j++)
                {
                    newData[i, j - 1] = Data[i, j];
                }
            }

            Data = newData;
        }

        public Matrix AddColumn(Vector b)
        {
            var newData = new double[b.Size, ColumnSize + 1];
            if (StringSize != 0 && StringSize != b.Size)
                throw new ArgumentException("Add vector: vector and matrix are different size");
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    newData[i, j] = Convert(Data[i, j]);
                }
            }
            for(int i = 0; i < b.Size; i++)
            {
                newData[i, ColumnSize] = Convert(b[i]);
            }
            return new Matrix(newData);
        }

        public Matrix AddMatrix(Matrix a)
        {
            if (StringSize != a.StringSize)
                throw new ArgumentException("Matrices are not equal by strings");
            var result = new Matrix(Data);
            for (int i = 0; i < a.ColumnSize; i++)
            {
                var currentVector = a.GetColumn(i);
                result = result.AddColumn(currentVector);
            }
            return result;
        }

        public Matrix Transpose()
        {
            var newData = new double[ColumnSize, StringSize];
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                for (int j = 0; j < Data.GetLength(0); j++)
                {
                    newData[i, j] = Convert(Data[j, i]);
                }
            }
            return new Matrix(newData);
        }

        public void ReplaceStrings(int indexOne, int indexTwo)
        {
            if (indexOne >= 0 && indexOne < StringSize && indexTwo >= 0 && indexTwo < StringSize)
            {
                var t = GetString(indexTwo);
                SetString(indexTwo, GetString(indexOne));
                SetString(indexOne, t);
            }
        }

        public Matrix DeleteZeroStrings()
        {
            var indexes = new HashSet<double>();
            for(int i = 0; i < StringSize; i++)
            {
                var v = GetString(i);
                if (v.All(x => x == 0))
                    indexes.Add(i);
            }
            var newData = new double[StringSize - indexes.Count, ColumnSize];
            var newData_i = 0;
            for (int i = 0; i < StringSize; i++)
            {
                if (!indexes.Contains(i))
                {
                    for (int j = 0; j < ColumnSize; j++)
                    {
                        newData[newData_i, j] = Data[i, j];
                    }
                    newData_i++;
                }
            }
            return new Matrix(newData);
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.ColumnSize != b.StringSize)
                throw new ArgumentException("Wrong size of matrixes");
            var result = new double[a.StringSize, b.ColumnSize];
            for(int i = 0; i < a.StringSize; i++)
            {
                for(int j = 0; j < b.ColumnSize; j++)
                {
                    result[i, j] = a.GetString(i) * b.GetColumn(j);
                }
            }
            return new Matrix(result);
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.StringSize != b.StringSize || a.ColumnSize != b.ColumnSize)
                throw new ArgumentException("Wrong size of matrixes");
            var result = new double[a.StringSize, a.ColumnSize];
            for (int i = 0; i < a.StringSize; i++)
            {
                for (int j = 0; j < a.ColumnSize; j++)
                {
                    result[i, j] = a.Data[i, j] + b.Data[i, j];
                }
            }
            return new Matrix(result);
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            return a + (-1) * b;
        }

        public static Matrix operator *(double t, Matrix a)
        {
            var result = new double[a.StringSize, a.ColumnSize];
            for (int i = 0; i < a.StringSize; i++)
            {
                for (int j = 0; j < a.ColumnSize; j++)
                {
                    result[i, j] = t * a.Data[i, j];
                }
            }
            return new Matrix(result);
        }

        public static Vector operator *(Matrix a, Vector b)
        {
            if (a.ColumnSize != b.Size)
                throw new ArgumentException("Wrong size of matrixes");
            var result = new double[a.StringSize];
            for (int i = 0; i < a.StringSize; i++)
            {
                result[i] = a.GetString(i) * b;
            }
            return new Vector(result);
        }

        public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.StringSize != b.StringSize || a.ColumnSize != b.ColumnSize)
                throw new ArgumentException("Matrix equal: Matrices of different sizes");
            for (int i = 0; i < a.StringSize; i++)
            {
                for (int j = 0; j < a.ColumnSize; j++)
                {
                    if (Math.Abs(a[i, j] - b[i, j]) > 1e-3)
                        return false;
                }
            }
            return true;
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var x = obj as Matrix;
            return this == x;
        }

        public void Print(int usedColumns = 0)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            if (usedColumns == 0)
                usedColumns = ColumnSize;
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    var value = Math.Round(Data[i, j], 3);
                    if (j % usedColumns == 0 && j != 0)
                    {
                        Console.Write("\t|\t" + value + "\t");
                    }
                    else if ((j + 1) % usedColumns == 0)
                    {
                        Console.Write(value);
                    }
                    else
                    {
                        Console.Write(value + "\t");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void PrintMany(params Matrix[] source)
        {
            var cascade = new Matrix(source[0].Data);
            for(int i = 1; i < source.Length; i++)
            {
                cascade = cascade.AddMatrix(source[i]);
            }
            cascade.Print(source[0].ColumnSize);
        }

        public string GetLatexNotation()
        {
            var result = @"$\begin{pmatrix}" + "\n";
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    var split = (j < Data.GetLength(1) - 1 ? " & " : @" \\" + "\n");
                    result += Math.Round(Data[i, j], 3) + split;
                }
            }
            result += @"\end{pmatrix}$";
            return result;
        }

        private int GetRank()
        {
            var stepwiseMatrix = GaussJordanMethod.IdentityForm(this, false);
            return stepwiseMatrix.StringSize;
        }

        private double GetDeterminant()
        {
            if (!IsSquare)
                throw new ArgumentException("Matrix must be squared");
            else
            {
                var stepwiseMatrix = GaussJordanMethod.StepwiseForm(this, false);
                double determinant = 1;
                for (int i = 0; i < StringSize; i++)
                {
                    determinant *= stepwiseMatrix.Data[i, i];
                }
                return determinant;
            }
        }

        public Matrix GetSubMatrix(int start, int end)
        {
            var result = new double[StringSize, end - start];
            for(int i = 0; i < StringSize; i++)
            {
                for(int j = start, s = 0; j < end; j++, s++)
                {
                    result[i, s] = Convert(Data[i, j]);
                }
            }
            return new Matrix(result);
        }

        public Matrix Normilize()
        {
            double maxValue = 0;
            for(int i = 0; i < StringSize; i++)
            {
                for(int j = 0; j < ColumnSize; j++)
                {
                    if(Math.Abs(Data[i, j]) > maxValue)
                    {
                        maxValue = Math.Abs(Data[i, j]);
                    }
                }
            }
            return (1 / maxValue) * this;
        }

        private bool IsMatrixDiagonal()
        {
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    if (i != j && Data[i, j] != 0)
                        return false;
                }
            }
            return true;
        }

        public static Matrix GetDiagomolizedMatrix(List<double> eigenvalues, int size)
        {
            Matrix result = new Matrix(size, size);
            for (int i = 0; i < eigenvalues.Count; i++)
            {
                result[i, i] = eigenvalues[i];
            }
            return result;
        }

        public Matrix Pow(int degree)
        {
            var temp = this;
            for (int i = 0; i < degree - 1; i++)
            {
                temp = temp * this;
            }
            return temp;
        }

        public static Matrix GetRandomMatrix(int stringSize, int columnSize)
        {
            var matrix = new Matrix();
            for(int i = 0; i < columnSize; i++)
            {
                var vector = Vector.GetRandomVector(stringSize, 2);
                matrix = matrix.AddColumn(vector);
            }
            return matrix;
        }
    }
}