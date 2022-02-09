using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra
{
    public class GaussJordanMethod
    {
        private static Matrix DropDownZeroStrings(Matrix m, int activeBlockSize = 0)
        {
            if(activeBlockSize == 0)
                activeBlockSize = m.ColumnSize;
            for(int i = 0; i < m.StringSize - 1; i++)
            {
                if(new Vector(m.GetString(i).Reverse().Take(activeBlockSize)) == new Vector(activeBlockSize))
                {
                    for(int j = i + 1; j < m.StringSize; j++)
                    {
                        if (new Vector(m.GetString(j).Reverse().Take(activeBlockSize)) != new Vector(activeBlockSize))
                            m.ReplaceStrings(i, j);
                    }
                }
            }
            return m;
        }

        public static Matrix StepwiseForm(Matrix m, bool output, int activeBlockSize = 0)
        {
            m = DropDownZeroStrings(m, activeBlockSize);
            var copy = (double[,])m.Data.Clone();
            var matrix = new Matrix(copy);
            var previousStartIndex = int.MaxValue;
            var activePart = new Matrix(m.StringSize, m.ColumnSize);
            var passivePart = new Matrix(m.StringSize, m.ColumnSize);
            if (activeBlockSize == 0)
                activeBlockSize = m.ColumnSize;
            else
            {
                passivePart = new Matrix();
                activePart = new Matrix();
                for (int i = 0; i < m.ColumnSize - activeBlockSize; i++)
                {
                    passivePart = passivePart.AddColumn(m.GetColumn(i));
                }
                for (int i = m.ColumnSize - activeBlockSize; i < m.ColumnSize; i++)
                {
                    activePart = activePart.AddColumn(m.GetColumn(i));
                }
                matrix = activePart;
            }
            for (int i = 0; i < matrix.StringSize; i++)
            {
                var startIndex = matrix.GetString(i).FindFirstNonZeroElementIndex();
                if (previousStartIndex > startIndex && startIndex != -1)
                {
                    if (i > 1)
                    {
                        matrix.ReplaceStrings(i - 1, i);
                        passivePart.ReplaceStrings(i - 1, i);
                    }
                    else
                    {
                        var index = matrix.GetColumn(i).FindFirstNonZeroElementIndex();
                        matrix.ReplaceStrings(index, i);
                        passivePart.ReplaceStrings(index, i);
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
                    var passiveVector = t * passivePart.GetString(i);
                    matrix.SetString(i + s, matrix.GetString(i + s) + firstVector);
                    passivePart.SetString(i + s, passivePart.GetString(i + s) + passiveVector);
                }
                previousStartIndex = startIndex;
            }
            if (activeBlockSize != m.ColumnSize)
                return DropDownZeroStrings(passivePart.AddMatrix(matrix), activeBlockSize);
            return DropDownZeroStrings(matrix);
        }

        public static Matrix IdentityForm(Matrix m, bool output)
        {
            var matrix = StepwiseForm(m, output);
            if (output)
                matrix.Print();
            for (int i = matrix.StringSize - 1; i > 0; i--)
            {
                var startIndex = matrix.GetString(i).FindFirstNonZeroElementIndex();
                for (int s = 1; s < i + 1; s++)
                {
                    if (startIndex == -1) continue;
                    var first = matrix[i, startIndex];
                    if (first == 0) continue;
                    
                    var second = matrix[i - s, startIndex];
                    var t = -1 * second / first;
                    var firstVector = t * matrix.GetString(i);
                    matrix.SetString(i - s, matrix.GetString(i - s) + firstVector);
                    if (output)
                        matrix.Print();
                    startIndex = matrix.GetString(i).FindFirstNonZeroElementIndex();
                }
            }
            matrix = matrix.DeleteZeroStrings();
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
            var result = new Matrix(new double[vectors, 0]); // векторов * зависимые переменный
            var freeVariablesIndexes = FindFreeVariablesIndexes(matrix);
            if (freeVariablesIndexes.Count != vectors)
                throw new ArgumentException("Number of free variables must be equal number of vectors");
            var onePlace = 0;
            for(int i = 0; i < matrix.ColumnSize; i++)
            {
                if (freeVariablesIndexes.Contains(i))
                {
                    var f = new Vector(Enumerable.Repeat(0d, onePlace).Append(1)
                        .Concat(Enumerable.Repeat(0d, vectors - onePlace - 1)));
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