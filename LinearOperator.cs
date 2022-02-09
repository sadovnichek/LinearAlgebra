using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra
{
    public class LinearOperator
    {
        public Matrix matrix;

        public LinearOperator(Matrix matrix)
        {
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

        // Метод Крылова
        public Vector GetCharacteristicPolynomial()
        {
            var coefficients = new Vector(1d);
            var effort = 1;
            var y = new Vector();
            while (coefficients.Size != matrix.StringSize + 1)
            {
                if(effort <= matrix.StringSize)
                    y = new Vector(Enumerable.Repeat(0d, matrix.StringSize - effort)
                                                    .Append(1)
                                                    .Concat(Enumerable.Repeat(0d, effort - 1))
                                                    .Reverse());
                else
                    y = Vector.GetRandomVector(matrix.StringSize, 3);
                var A = new Matrix();
                A = A.AddColumn(y);
                for (int i = 1; i < matrix.ColumnSize; i++)
                {
                    var test_y = matrix * y;
                    var test = A.AddColumn(test_y).Transpose();
                    if (test.Rank != test.StringSize)
                        break;
                    y = matrix * y;
                    A = A.AddColumn(y);
                }
                var b = matrix * y;
                var solution = new Vector(((-1) * GaussJordanMethod.Solve(A, b, false))
                                                .Append(1));
                if (solution.Size > coefficients.Size)
                    coefficients = solution;
                else if(coefficients != solution)
                    coefficients = Polynomial.Multiply(coefficients, solution);
                effort++;
            }
            return Math.Pow(-1, matrix.ColumnSize) * coefficients;
        }

        public List<double> GetEigenvalues()
        {
            var mininalPolynomial = GetCharacteristicPolynomial();
            return Polynomial.Solve(mininalPolynomial);
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
    }
}
