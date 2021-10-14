using System;

namespace LinearAlgebra
{
    class Program
    {
        public static void Main()
        {
            var matrix = new Matrix(new Vector(3, 5, 6),
                                    new Vector(-5, -8, -9),
                                    new Vector(2, 3, 3));
            matrix = GaussJordanMethod.StepwiseForm(matrix, true);
            matrix.Print();
        }
    }
}