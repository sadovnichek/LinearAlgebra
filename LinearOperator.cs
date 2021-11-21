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

        public Vector[] FindKernelBasis()
        {
            var zeroVector = new Vector(matrix.StringSize);
            var solution = GaussJordanMethod.Solve(matrix, zeroVector, false);
            if (solution is LinearManifold)
            {
                var result = solution as LinearManifold;
                return result.SubspaceBasis;
            }
            throw new Exception("FindKernelBasis: result must be a linear manifold");
        }

        public Vector[] FindImageBasis()
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
            return result.ToArray();
        }

        public LinearOperator ChangeBasis(Vector[] oldBasis, Vector[] newBasis)
        {
            var currentMatrix = new Matrix(oldBasis).Transpose();
            var newMatrix = new Matrix(newBasis).Transpose();
            var transitionMatrix = MatrixEquation.Solve(currentMatrix, newMatrix);
            var inversedMatrix = MatrixEquation.GetInverseMatrix(transitionMatrix);
            return new LinearOperator(inversedMatrix * matrix * transitionMatrix);
        }
    }
}
