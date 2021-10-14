using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra
{
    public class GaussJordanMethod
    {
        public static Matrix StepwiseForm(Matrix m, bool output)
        {
            var x = (Fraction[,])m.Data.Clone();
            var matrix = new Matrix(x);
            var previousStartIndex = int.MaxValue;
            for (int i = 0; i < matrix.StringSize; i++)
            {
                var startIndex = matrix.GetString(i).FindFirstNonZeroElementIndex();
                if (previousStartIndex > startIndex && startIndex != -1)
                {
                    if(i > 1)
                        matrix.ReplaceStrings(i - 1, i);
                    else
                    {
                        var index = matrix.GetColumn(i).FindFirstNonZeroElementIndex();
                        matrix.ReplaceStrings(index, i);
                    }    
                }
                startIndex = matrix.GetString(i).FindFirstNonZeroElementIndex();
                for (int s = 1; s < matrix.StringSize - i; s++)
                {
                    if (startIndex == -1) continue;
                    var first = matrix[i, startIndex];
                    var second = matrix[i + s, startIndex];
                    if (first == 0 && second == 0 || first != 0 && second == 0) continue;
                    if (output)
                        matrix.Print();
                    var t = -1 * second / first;
                    var firstVector = t * matrix.GetString(i);
                    matrix.SetString(i + s, matrix.GetString(i + s) + firstVector);
                }
                previousStartIndex = startIndex;
            }
            return matrix;
        }

        public static Matrix IdentityForm(Matrix m, bool output)
        {
            var matrix = StepwiseForm(m, output);
            for (int i = matrix.StringSize - 1; i > 0; i--)
            {
                var startIndex = matrix.GetString(i).FindFirstNonZeroElementIndex();
                for (int s = 1; s < i + 1; s++)
                {
                    if (startIndex == -1) continue;
                    var first = matrix[i, startIndex];
                    if (first == 0) continue;
                    if (output)
                        matrix.Print();
                    var second = matrix[i - s, startIndex];
                    var t = -1 * second / first;
                    var firstVector = t * matrix.GetString(i);
                    matrix.SetString(i - s, matrix.GetString(i - s) + firstVector);
                    startIndex = matrix.GetString(i).FindFirstNonZeroElementIndex();
                    
                }
            }
            matrix.DeleteZeroStrings();
            for (int i = 0; i < matrix.StringSize; i++)
            {
                var currentVector = matrix.GetString(i);
                var firstNonZeroElement = currentVector.Where(x => x != 0).First();
                matrix.SetString(i, 1 / firstNonZeroElement * currentVector);
            }
            if (output)
                matrix.Print();
            return matrix;
        }

        private static List<int> FindFreeVariablesIndexes(Matrix m)
        {
            var freeVariablesIndexes = new List<int>();
            var notFreeVariablesIndexes = new List<int>();
            for(int i = 0; i < m.StringSize; i++)
            {
                var currentString = m.GetString(i);
                var nonZeroValues = currentString.Where(x => x != 0);
                if (nonZeroValues.Count() == 1) // значение определено и единственно
                {
                    var notFreeVariableIndex = currentString.FindFirstNonZeroElementIndex();
                    notFreeVariablesIndexes.Add(notFreeVariableIndex);
                    if (freeVariablesIndexes.Contains(notFreeVariableIndex))
                        freeVariablesIndexes.Remove(notFreeVariableIndex);
                    for(int j = 0; j < currentString.Size; j++)
                    {
                        if (currentString[j] == 0 && !notFreeVariablesIndexes.Contains(j)) 
                            freeVariablesIndexes.Add(j);
                    }
                }
                else
                {
                    var notFreeVariableIndex = currentString.FindFirstNonZeroElementIndex();
                    notFreeVariablesIndexes.Add(notFreeVariableIndex);
                    for (int j = 0; j < currentString.Size; j++)
                    {
                        if (currentString[j] != 0 && !notFreeVariablesIndexes.Contains(j))
                            freeVariablesIndexes.Add(j);
                    }
                }
            }
            freeVariablesIndexes = freeVariablesIndexes.Distinct().ToList();
            if (freeVariablesIndexes.Count + notFreeVariablesIndexes.Count != m.ColumnSize)
            {
                for(int i = 0; i < m.ColumnSize; i++)
                {
                    if (!freeVariablesIndexes.Contains(i) && !notFreeVariablesIndexes.Contains(i))
                        freeVariablesIndexes.Add(i);
                }
            }
            return freeVariablesIndexes.OrderBy(x => x).ToList();
        }

        private static Vector[] FindFundamentalSystem(Matrix matrix)
        {
            var vectors = matrix.ColumnSize - matrix.Rank;
            var result = new Matrix(new Fraction[vectors, 0]); // векторов * зависимые переменный
            var freeVariablesIndexes = FindFreeVariablesIndexes(matrix);
            if (freeVariablesIndexes.Count != vectors)
                throw new ArgumentException("Number of free variables must be equal number of vectors");
            var onePlace = 0;
            for(int i = 0; i < matrix.ColumnSize; i++)
            {
                if (freeVariablesIndexes.Contains(i))
                {
                    var f = new Vector(Enumerable.Repeat(Fraction.Zero, onePlace).Append(1).Concat(Enumerable.Repeat(Fraction.Zero, vectors - onePlace - 1)));
                    result = result.AddColumn(f);
                    onePlace++;
                }
                else
                {
                    var currentVector = matrix.GetString(i - onePlace);
                    var f = new Vector(freeVariablesIndexes.Select(x => currentVector[x]));
                    result = result.AddColumn(-1 * f);
                }
            }
            var subsetBasis = new Vector[vectors];
            for (int i = 0; i < vectors; i++)
            {
                subsetBasis[i] = result.GetString(i);
            }
            return subsetBasis;
        }

        private static Vector GetPseudoSolution(Matrix matrix, Vector b)
        {
            var aT = matrix.Transpose();
            return IdentityForm((aT * matrix).AddColumn(aT * b), true).GetColumn(matrix.ColumnSize);
        }

        public static Vector Solve(Matrix matrix, Vector b, bool output)
        {
            Matrix ultimateMatrix = matrix.AddColumn(b);
            if (ultimateMatrix.Rank != matrix.Rank)
            {
                if(output)
                    Console.WriteLine("System is not compatible. Pseudosolution: ");
                return GetPseudoSolution(matrix, b);
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