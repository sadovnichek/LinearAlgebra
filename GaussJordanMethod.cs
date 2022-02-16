using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra
{
    public class GaussJordanMethod
    {
        private static Matrix GetActiveMatrix(Matrix matrix, int activeBlockSize)
        {
            Matrix activePart = new Matrix();
            for (int i = matrix.ColumnSize - activeBlockSize; i < matrix.ColumnSize; i++)
            {
                activePart = activePart.AddColumn(matrix.GetColumn(i));
            }
            return activePart;
        }

        private static Matrix OrderStrings(Matrix matrix, int activaBlockSize)
        {
            var dict = new Dictionary<int, int>(); // <номер строки, позиция элемента>
            for(int i = 0; i < matrix.StringSize; i++)
            {
                var firstNonZeroElementIndex =  new Vector(matrix.GetString(i)
                                                     .Skip(matrix.ColumnSize - activaBlockSize))
                                                     .FindFirstNonZeroElementIndex();
                if (firstNonZeroElementIndex == -1)
                    firstNonZeroElementIndex = int.MaxValue;
                dict.Add(i, firstNonZeroElementIndex);
            }
            var sorted = dict.OrderBy(x => x.Value);
            var result = new Matrix(matrix.StringSize, matrix.ColumnSize);
            var index = 0;
            foreach(var pair in sorted)
            {
                result.SetString(index, matrix.GetString(pair.Key));
                index++;
            }
            return result;
        }

        public static Matrix StepwiseForm(Matrix m, bool output, int activeBlockSize = 0)
        {
            var copy = (double[,])m.Data.Clone();
            var matrix = new Matrix(copy);
            activeBlockSize = (activeBlockSize == 0) ? matrix.ColumnSize : activeBlockSize; // 0 - значение по умолчанию
            matrix = OrderStrings(matrix, activeBlockSize);
            if (output)
                matrix.Print(activeBlockSize);
            Matrix activePart = (activeBlockSize == matrix.ColumnSize) ? new Matrix(matrix.Data) : GetActiveMatrix(matrix, activeBlockSize);
            for (int i = 0; i < activePart.StringSize - 1; i++)
            {
                var startIndex = activePart.GetString(i).FindFirstNonZeroElementIndex();
                for (int shift = 1; shift < activePart.StringSize - i; shift++)
                {
                    if (startIndex == -1) continue;
                    var first = activePart[i, startIndex];
                    var second = activePart[i + shift, startIndex];
                    if (first == 0 && second == 0 || first != 0 && second == 0) continue;
                    var t = -1 * second / first;
                    var addFullVector = t * matrix.GetString(i);
                    var addActiveVector = t * activePart.GetString(i);
                    matrix.SetString(i + shift, matrix.GetString(i + shift) + addFullVector);
                    activePart.SetString(i + shift, activePart.GetString(i + shift) + addActiveVector);
                    if (output)
                        matrix.Print(activeBlockSize);
                }
            }
            matrix = OrderStrings(matrix, activeBlockSize);
            if (output)
                matrix.Print(activeBlockSize);
            return matrix;
        }

        public static Matrix IdentityForm(Matrix m, bool output)
        {
            var copy = (double[,])m.Data.Clone();
            var matrix = new Matrix(copy);
            matrix = StepwiseForm(matrix, output);
            for (int i = matrix.StringSize - 1; i > 0; i--)
            {
                var startIndex = matrix.GetString(i).FindFirstNonZeroElementIndex();
                for (int shift = 1; shift < i + 1; shift++)
                {
                    if (startIndex == -1) continue;
                    var first = matrix[i, startIndex];
                    if (first == 0) continue;
                    var second = matrix[i - shift, startIndex];
                    var t = -1 * second / first;
                    var firstVector = t * matrix.GetString(i);
                    matrix.SetString(i - shift, matrix.GetString(i - shift) + firstVector);
                    if (output)
                        matrix.Print();
                }
            }
            matrix = matrix.DeleteZeroStrings();
            for (int i = 0; i < matrix.StringSize; i++)
            {
                var currentVector = matrix.GetString(i);
                var firstNonZeroElement = currentVector[currentVector.FindFirstNonZeroElementIndex()];
                matrix.SetString(i, 1 / firstNonZeroElement * currentVector);
            }
            if (output)
                matrix.Print();
            return matrix;
        }

        private static List<int> FindFreeVariablesIndexes(Matrix m)
        {
            var freeVariablesIndexes = Enumerable.Range(0, m.ColumnSize).ToHashSet();
            var notFreeVariablesIndexes = new HashSet<int>();
            for(int i = 0; i < m.StringSize; i++)
            {
                var currentString = m.GetString(i);
                var notFreeVariableIndex = currentString.FindFirstNonZeroElementIndex();
                notFreeVariablesIndexes.Add(notFreeVariableIndex);
            }
            freeVariablesIndexes = freeVariablesIndexes.Except(notFreeVariablesIndexes).ToHashSet();
            return freeVariablesIndexes.OrderBy(x => x).ToList();
        }

        private static Vector[] FindFundamentalSystem(Matrix matrix)
        {
            var vectors = matrix.ColumnSize - matrix.Rank;
            var result = new Matrix(new double[vectors, 0]); // векторов * все переменные
            var freeVariablesIndexes = FindFreeVariablesIndexes(matrix);
            if (freeVariablesIndexes.Count != vectors)
                throw new Exception("Number of free variables must be equal number of vectors");
            var onePosition = 0;
            for(int i = 0; i < matrix.ColumnSize; i++)
            {
                if (freeVariablesIndexes.Contains(i))
                {
                    var f = new Vector(Enumerable.Repeat(0d, onePosition)
                                                 .Append(1)
                                                 .Concat(Enumerable.Repeat(0d, vectors - onePosition - 1)));
                    result = result.AddColumn(f);
                    onePosition++;
                }
                else
                {
                    var currentVector = matrix.GetString(matrix.GetColumn(i).FindFirstNonZeroElementIndex()); // находим строку, в которой определяется эта переменная
                    var f = new Vector(freeVariablesIndexes.Select(freeIndex => currentVector[freeIndex])); //берем значения свободных переменных в этой строке
                    result = result.AddColumn(-1 * f); // переносим за знак равно в уравнении
                }
            }
            var subsetBasis = new Vector[vectors];
            for (int i = 0; i < vectors; i++)
            {
                subsetBasis[i] = result.GetString(i);
            }
            return subsetBasis;
        }

        public static Vector Solve(Matrix matrix, Vector b, bool output)
        {
            Matrix ultimateMatrix = matrix.AddColumn(b);
            if (ultimateMatrix.Rank != matrix.Rank)
            {
                if(output)
                    Console.WriteLine("System is not compatible. Pseudosolution: ");
                var aT = matrix.Transpose();
                return Solve(aT * matrix, aT * b, output);
            }
            if(matrix.ColumnSize - matrix.Rank != 0)
            {
                var basisVectors = matrix.ColumnSize - matrix.Rank;
                var solution = IdentityForm(ultimateMatrix, output);
                if(output)
                    Console.WriteLine("Basis of general solution consists of " + basisVectors + " vectors");
                var shiftVector = new Vector(matrix.ColumnSize) + solution.GetColumn(solution.ColumnSize - 1);
                solution.DeleteColumn(solution.ColumnSize - 1);
                var fundamentalSystemBasis = FindFundamentalSystem(solution);
                return new LinearManifold(shiftVector, fundamentalSystemBasis);
            }
            return IdentityForm(ultimateMatrix, output).GetColumn(ultimateMatrix.ColumnSize - 1);
        }
    }
}