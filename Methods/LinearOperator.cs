using LinearAlgebra.Models;
using LinearAlgebra.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LinearAlgebra.Methods
{
    public class LinearOperator
    {
        public readonly Matrix matrix;
        private int Size => matrix.ColumnSize;

        public LinearOperator(Matrix matrix)
        {
            if (!matrix.IsSquare)
                throw new ArgumentException("Matrix of linear operator must be square");
            this.matrix = matrix;
        }

        public List<Vector> FindKernelBasis()
        {
            var zeroVector = new Vector(matrix.StringSize);
            var solution = GaussJordanMethod.Solve(matrix, zeroVector, false);
            if (solution is LinearManifold linearManifold)
                return linearManifold.GetSubspaceBasis();
            throw new Exception("FindKernelBasis: result must be a linear manifold");
        }

        public List<Vector> FindImageBasis()
        {
            var result = new List<Vector>();
            var solution = matrix.Transpose();
            solution = GaussJordanMethod.StepwiseForm(solution, false);
            var zero = new Vector(matrix.ColumnSize);
            for(int i = 0; i < solution.StringSize; i++)
            {
                if (!solution.GetString(i).Equals(zero))
                    result.Add(solution.GetString(i));
            }
            return result;
        }

        public LinearOperator ChangeBasis(Vector[] oldBasis, Vector[] newBasis)
        {
            var currentMatrix = new Matrix(oldBasis).Transpose();
            var newMatrix = new Matrix(newBasis).Transpose();
            var transitionMatrix = MatrixEquation.Solve(currentMatrix, newMatrix);
            var inversedMatrix = MatrixEquation.GetInverseMatrix(transitionMatrix);
            return new LinearOperator(inversedMatrix * matrix * transitionMatrix);
        }

        public void ChurkinAlgorithm()
        {
            var identityMatrix = Matrix.GetIdentityMatrix(matrix.StringSize);
            var fullMatrix = identityMatrix.AddMatrix(matrix.Transpose());
            fullMatrix = GaussJordanMethod.StepwiseForm(fullMatrix, false, activeBlockSize: matrix.ColumnSize);
            fullMatrix.Print(matrix.ColumnSize);
            var lastMatrix = fullMatrix.GetSubMatrix(fullMatrix.ColumnSize - matrix.ColumnSize, fullMatrix.ColumnSize);
            var preLastMatrix = fullMatrix.GetSubMatrix(fullMatrix.ColumnSize - 2*matrix.ColumnSize, 
                fullMatrix.ColumnSize - matrix.ColumnSize);
            Console.WriteLine("Basis of Image (Im A): ");
            var imageDimension = 0;
            for(int i = 0; i < lastMatrix.StringSize; i++)
            {
                if (lastMatrix.GetString(i) != new Vector(lastMatrix.ColumnSize))
                {
                    Console.WriteLine(lastMatrix.GetString(i));
                    imageDimension++;
                }
            }
            Console.WriteLine("dim Im A = " + imageDimension);
            Console.WriteLine();
            Console.WriteLine("Basis of Kernel (Ker A): ");
            var kernelDimension = 0;
            for (int i = 0; i < lastMatrix.StringSize; i++)
            {
                if (lastMatrix.GetString(i) == new Vector(lastMatrix.ColumnSize))
                {
                    Console.WriteLine(preLastMatrix.GetString(i));
                    kernelDimension++;
                }
            }
            Console.WriteLine("dim Ker A = " + kernelDimension);
        }

        // Метод Леверье - Фаддеева
        public Polynomial GetCharacteristicPolynomial()
        {
            Matrix E = Matrix.GetIdentityMatrix(Size);
            var coefficients = new List<double>() { 1 };
            var Mk = E;
            for(int k = 1; k <= Size; k++)
            {
                Matrix M = matrix * Mk;
                var coefficient = -1d / k * M.Trace();
                coefficients.Add(coefficient);
                Mk = M + coefficient * E;
            }
            coefficients.Reverse();
            var result = Math.Pow(-1, Size) * new Vector(coefficients);
            return new Polynomial(result);
        }

        public List<double> GetEigenvalues()
        {
            var polynomial = GetCharacteristicPolynomial();
            return polynomial.Solve();
        }

        public List<Vector> GetEigenvectors(double eigenvalue)
        {
            var result = new List<Vector>();
            Matrix E = Matrix.GetIdentityMatrix(matrix.ColumnSize);
            Matrix A = matrix - eigenvalue * E;
            var fundamentalSystem = (LinearManifold)GaussJordanMethod.Solve(A, new Vector(matrix.ColumnSize), false);
            result.AddRange(fundamentalSystem.GetSubspaceBasis());
            return result;
        }

        public Matrix[] Diagonolize()
        {
            var eigenvalues = GetEigenvalues();
            eigenvalues.Sort();
            var diagonolizedMatrix = Matrix.GetDiagomolizedMatrix(eigenvalues, matrix.StringSize);
            var transitionMatrix = new Matrix();
            foreach (var e in eigenvalues.Distinct())
            {
                var eigenvectors = GetEigenvectors(e);
                foreach (var vector in eigenvectors)
                    transitionMatrix = transitionMatrix.AddColumn(vector);
            }
            if (transitionMatrix.Transpose().Rank != matrix.ColumnSize)
                throw new InvalidOperationException("Cannot be diagonolized. Use Jordan normal form");
            return new Matrix[] { transitionMatrix, diagonolizedMatrix, MatrixEquation.GetInverseMatrix(transitionMatrix) };
        }

        private Matrix GetJordanCell(int cellSize, double eigenvalue)
        {
            var result = new Matrix(cellSize, cellSize);
            for(int i = 0; i < cellSize - 1; i++)
            {
                result[i + 1, i] = 1;
            }
            for(int i = 0; i < cellSize; i++)
            {
                result[i, i] = eigenvalue;
            }
            return result;
        }

        private Matrix GetFirstBlock(int iteration, TexFile file, Matrix A, Matrix lastChangedMatrix)
        {
            Matrix E = Matrix.GetIdentityMatrix(matrix.StringSize);
            Matrix block;
            if (iteration == 0)
            {
                block = E.AddMatrix(A.Transpose());
                file.Write($"Составим матрицу $(E | A_{iteration + 1}^T)$:");
            }
            else
            {
                block = lastChangedMatrix.AddMatrix(lastChangedMatrix * A.Transpose());
                file.Write($@"Составим матрицу $(B | B \cdot A_{iteration + 1}^T)$:");
                file.Write("Где B = " + lastChangedMatrix.GetLatexNotation());
            }
            return block;
        }

        public Matrix[] JordanNormalForm(TexFile file)
        {
            var characteristicPolynomial = GetCharacteristicPolynomial();
            var eigenvalues = characteristicPolynomial.Solve().Distinct().ToList();
            var lastChangedMatrix = new Matrix();
            var jordanCells = new List<Matrix>();
            var transitionMatrix = new Matrix();
            for (int i = 0; i < eigenvalues.Count(); i++)
            {
                file.Write($"Текущее собственное значение $t_{i + 1} = {eigenvalues[i]}$. " + 
                $"Кратность корня: {Polynomial.GetRootMultiplicity(characteristicPolynomial.Coefficients, eigenvalues[i])}");
                var A = matrix - eigenvalues[i] * Matrix.GetIdentityMatrix(matrix.StringSize);
                file.Write($"Рассматриваемая матрица $A_{i + 1} = A - t_{i + 1}E$:");
                file.Write($"$A_{i + 1}$ = " + A.GetLatexNotation());
                var layer = GetFirstBlock(i, file, A, lastChangedMatrix);
                file.Write(layer.GetLatexNotation(Size));
                file.Write("Приводим крайний правый блок к ступенчатому виду:");
                layer = GaussJordanMethod.StepwiseForm(layer, true, file, Size);
                lastChangedMatrix = layer.GetSubMatrix(layer.ColumnSize - Size, layer.ColumnSize);
                if(i == eigenvalues.Count() - 1) // в этом случае оператор нильпотентный
                {
                    file.Write("Это " + ((i == 0) ? "единственное " : "последнее ") + "собственное значение. Значит, условие остановки: крайний блок - нулевая матрица.");
                    while (lastChangedMatrix != new Matrix(Size, Size))
                    {
                        file.Write($@"Дописываем справа матрицу $Y \cdot A_{i+1}^T$");
                        file.Write("Где Y = " + lastChangedMatrix.GetLatexNotation());
                        layer = layer.AddMatrix(lastChangedMatrix * A.Transpose());
                        file.Write(layer.GetLatexNotation(Size));
                        layer = GaussJordanMethod.StepwiseForm(layer, true, file, Size);
                        lastChangedMatrix = layer.GetSubMatrix(layer.ColumnSize - Size, layer.ColumnSize);
                    }                    
                    if (lastChangedMatrix == new Matrix(Size, Size))
                        file.Write("Крайний блок - нулевая матрица.");
                }
                else
                {
                    while (lastChangedMatrix.Rank != (lastChangedMatrix * A.Transpose()).Rank)
                    {
                        file.Write($@"Ранги двух крайних правых матриц не совпали. Дописываем справа матрицу $Y \cdot A_{i+1}^T$:");
                        file.Write("Где Y = " + lastChangedMatrix.GetLatexNotation());
                        layer = layer.AddMatrix(lastChangedMatrix * A.Transpose());
                        file.Write(layer.GetLatexNotation(Size));
                        layer = GaussJordanMethod.StepwiseForm(layer, true, file, Size);
                        lastChangedMatrix = layer.GetSubMatrix(layer.ColumnSize - Size, layer.ColumnSize);
                    }
                    if(lastChangedMatrix.Rank == (lastChangedMatrix * A.Transpose()).Rank)
                    {
                        file.Write("Ранги двух крайних матриц совпали:");
                        layer = layer.AddMatrix(lastChangedMatrix * A.Transpose());
                        file.Write(layer.GetLatexNotation(Size));
                    }
                }
                file.Write("Составим жорданову таблицу и элементарными преобразованиями найдем базис данного корневого подпространства:");
                var jordanTable = new JordanTable(layer, Size, file);
                var nillLayers = jordanTable.GetNillLayers();
                file.Write("Таким образом, базис данного подпространства образуют векторы:");
                foreach (var nilllayer in nillLayers)
                {
                    var jordanCellSize = nilllayer.Count();
                    var jordanCell = GetJordanCell(jordanCellSize, eigenvalues[i]);
                    jordanCells.Add(jordanCell);
                    foreach(var vector in nilllayer)
                    {
                        transitionMatrix = transitionMatrix.AddColumn(vector);
                        file.Write(vector.GetLatexNotation());
                    }
                }
            }
            var result = Matrix.BuildByDiagonalSquareBlocks(jordanCells);
            return new Matrix[] { transitionMatrix, result, MatrixEquation.GetInverseMatrix(transitionMatrix) };
        }
    }
}
