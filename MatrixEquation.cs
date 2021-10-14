using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebra
{
    public class MatrixEquation
    {
        public static Matrix GetInverseMatrix(Matrix a)
        {
            if (a.ColumnSize != a.StringSize)
                throw new ArgumentException("GetBackMatrix: Matrix must be square");
            var identityMatrix = Matrix.GetIdentityMatrix(a.StringSize);
            return Solve(a, identityMatrix);
        }

        public static Matrix Solve(Matrix a, Matrix b)
        {
            var fullMatrix = a.AddMatrix(b);
            if (a.IsSquare && a.Determinant != 0)
            {
                var x = GaussJordanMethod.IdentityForm(fullMatrix, false)
                    .GetSubMatrix(a.ColumnSize, a.ColumnSize + b.ColumnSize);
                if (a * x == b)
                    return x;
                else
                    throw new InvalidOperationException("No solutions");
            }
            fullMatrix = GaussJordanMethod.StepwiseForm(fullMatrix, false);
            var A_c = fullMatrix.GetSubMatrix(0, a.ColumnSize);
            var B_c = fullMatrix.GetSubMatrix(a.ColumnSize, fullMatrix.ColumnSize);
            var result = new Matrix(0, 0);
            for (int i = 0; i < b.ColumnSize; i++)
            {
                var b_i = B_c.GetColumn(i);
                var vector_i = GaussJordanMethod.Solve(A_c, b_i, false);
                result = result.AddColumn(vector_i);
            }
            return result;
        }
    }
}
