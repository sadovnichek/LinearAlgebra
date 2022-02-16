using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra
{
    public class LinearOperator
    {
        public Matrix matrix;
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
            if (solution is LinearManifold)
            {
                var result = solution as LinearManifold;
                return result.GetSubspaceBasis();
            }
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
            fullMatrix = GaussJordanMethod.StepwiseForm(fullMatrix, false, matrix.ColumnSize);
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
            LinearManifold fundamentalSystem = (LinearManifold)GaussJordanMethod.Solve(A, new Vector(matrix.ColumnSize), false);
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

        public Matrix[] JordanNormalForm()
        {
            var eigenvalues = GetEigenvalues().Distinct().ToList();
            Matrix E = Matrix.GetIdentityMatrix(matrix.StringSize);
            var lastChangedMatrix = new Matrix();
            var jordanCells = new List<Matrix>();
            var transitionMatrix = new Matrix();
            Matrix layer;
            for (int i = 0; i < eigenvalues.Count(); i++)
            {
                var A = matrix - eigenvalues[i] * E;
                if (i == 0)
                    layer = E.AddMatrix(A.Transpose());
                else
                    layer = lastChangedMatrix.AddMatrix(lastChangedMatrix * A.Transpose());
                layer = GaussJordanMethod.StepwiseForm(layer, false, Size);
                lastChangedMatrix = layer.GetSubMatrix(layer.ColumnSize - Size, layer.ColumnSize);
                if(i == eigenvalues.Count() - 1)
                {
                    while (lastChangedMatrix != new Matrix(Size, Size))
                    {
                        layer = layer.AddMatrix(lastChangedMatrix * A.Transpose());
                        layer = GaussJordanMethod.StepwiseForm(layer, false, Size);
                        lastChangedMatrix = layer.GetSubMatrix(layer.ColumnSize - Size, layer.ColumnSize);
                    }
                }
                else
                {
                    while (lastChangedMatrix.Rank != (lastChangedMatrix * A.Transpose()).Rank)
                    {
                        layer = layer.AddMatrix(lastChangedMatrix * A.Transpose());
                        layer = GaussJordanMethod.StepwiseForm(layer, false, Size);
                        lastChangedMatrix = layer.GetSubMatrix(layer.ColumnSize - Size, layer.ColumnSize);
                    }
                }
                var jordanTable = new JordanTable(layer, Size);
                var nillLayers = jordanTable.GetNillLayers(false);
                foreach(var nilllayer in nillLayers)
                {
                    var jordanCellSize = nilllayer.Count();
                    var jordanCell = GetJordanCell(jordanCellSize, eigenvalues[i]);
                    jordanCells.Add(jordanCell);
                    foreach(var vector in nilllayer)
                    {
                        transitionMatrix = transitionMatrix.AddColumn(vector);
                    }
                }
            }
            var result = Matrix.BuildByDiagonalSquareBlocks(jordanCells);
            return new Matrix[] { transitionMatrix, result, MatrixEquation.GetInverseMatrix(transitionMatrix) };
        }
    }
}
