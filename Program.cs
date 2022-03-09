using LinearAlgebra.Methods;
using LinearAlgebra.Models;
using LinearAlgebra.Support;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Linq;

namespace LinearAlgebra
{
    class Program
    {
        public static void Main()
        {
            try
            {
                var matrix = new Matrix(new Vector(-1, 0, 0, 0),
                                        new Vector(0, 0, 0, 0),
                                        new Vector(0, 0, -2, -2),
                                        new Vector(0, 0, 1, 1));
                var file = new TexFile("./solution.tex");
                var linearOperator = new LinearOperator(matrix);
                var jordan = linearOperator.JordanNormalForm(file);
                Assert.AreEqual(matrix, jordan[0] * jordan[1] * jordan[2]);
                file.Write($@"{matrix.GetLatexNotation()} = {jordan[0].GetLatexNotation()} $\cdot$ " +
                    $@"{jordan[1].GetLatexNotation()} $\cdot$ {jordan[2].GetLatexNotation()}");
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}