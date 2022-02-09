using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LinearAlgebra
{
    [TestFixture]
    class Tests
    {
        [Test]
        public static void DotProduct()
        {
            var v1 = new Vector(1, 2, 3);
            var v2 = new Vector(0, 1, 4);
            Assert.AreEqual(v1 * v2, 14);
        }

        [Test]
        public static void MultiplyMatrix()
        {
            var A = new Matrix(new Vector(8, 9, 8),
                               new Vector(2, 6, 3),
                               new Vector(3, 0, 5));
            var B = new Matrix(new Vector(-2, 1, 0),
                               new Vector(0, -3, 4),
                               new Vector(-1, -2, -3));
            var multiply = new Matrix(new Vector(-24, -35, 12),
                                      new Vector(-7, -22, 15),
                                      new Vector(-11, -7, -15));
            Assert.AreEqual(A * B, multiply);
        }

        [Test]
        public static void Transpose()
        {
            var matrix = new Matrix(new Vector(8, 9, 8),
                                    new Vector(2, 6, 3),
                                    new Vector(3, 0, 5));

            matrix = matrix.Transpose();

            var transposed = new Matrix(new Vector(8, 2, 3),
                                        new Vector(9, 6, 0),
                                        new Vector(8, 3, 5));
            Assert.AreEqual(transposed, matrix);
        }

        [Test]
        public static void VectorOperstions()
        {
            var v1 = new Vector(1, 2, 3);
            var v2 = new Vector(0, 1, 4);
            Assert.AreEqual(v1 + v2, new Vector(1, 3, 7));
            Assert.AreEqual(5 * v1, new Vector(5, 10, 15));
            Assert.AreEqual(v1 - v2, new Vector(1, 1, -1));
        }

        [Test]
        public static void MultiplyMatrixOnScalar()
        {
            var matrix = Matrix.GetIdentityMatrix(3);
            matrix = 5 * matrix;
            var result = new Matrix(new Vector(5, 0, 0),
                                    new Vector(0, 5, 0),
                                    new Vector(0, 0, 5));
            Assert.AreEqual(matrix, result);
        }

        [Test]
        public static void AddMatricies()
        {
            var a = new Matrix(new Vector(3, 5, 6),
                                    new Vector(-5, -8, -9),
                                    new Vector(2, 3, 3));
            var b = new Matrix(new Vector(3, 5, 6),
                                            new Vector(0, 1.0/3.0, 1),
                                            new Vector(0, 0, 0));
            var sum = new Matrix(new Vector(6, 10, 12),
                                 new Vector(-5, -23.0/3.0, -8),
                                 new Vector(2, 3, 3));
            Assert.AreEqual(a + b, sum);
        }

        [Test]
        public static void Rank()
        {
            var matrix = new Matrix(new Vector(4, 3, -5, 2, 3),
                                    new Vector(8, 6, -7, 4, 2),
                                    new Vector(4, 3, -8, 2, 7),
                                    new Vector(4, 3, 1, 2, -5),
                                    new Vector(8, 6, -1, 4, -6));
            Assert.AreEqual(matrix.Rank, 2);
        }

        [Test]
        public static void StepwiseForm1()
        {
            var matrix = new Matrix(new Vector(3, 5, 6),
                                    new Vector(-5, -8, -9),
                                    new Vector(2, 3, 3));
            var stepwiseMatrix = new Matrix(new Vector(3, 5, 6),
                                            new Vector(0, 1.0/3.0, 1),
                                            new Vector(0, 0, 0));
            matrix = GaussJordanMethod.StepwiseForm(matrix, false);
            Assert.AreEqual(matrix, stepwiseMatrix);
        }

        [Test]
        public static void StepwiseForm2()
        {
            var matrix = new Matrix(new Vector(0, 1, 0),
                                    new Vector(1, 1, 1),
                                    new Vector(0, 1, 2));
            var stepwiseMatrix = new Matrix(new Vector(1, 1, 1),
                                            new Vector(0, 1, 0),
                                            new Vector(0, 0, 2));
            matrix = GaussJordanMethod.StepwiseForm(matrix, false);
            Assert.AreEqual(matrix, stepwiseMatrix);
        }

        [Test]
        public static void SystemWithDefinedSolution()
        {
            var v1 = new Vector(2, -4, 9);
            var v2 = new Vector(7, 3, -6);
            var v3 = new Vector(7, 9, -9);
            var matrix = new Matrix(v1, v2, v3);
            var b = new Vector(28, -1, 5);

            var solution = GaussJordanMethod.Solve(matrix, b, false);
            Assert.AreEqual(solution, new Vector(2, 3, 4));
        }

        [Test]
        public static void SystemWithLinearManifold1()
        {
            var v1 = new Vector(2, -3, 5, 7);
            var v2 = new Vector(4, -6, 2, 3);
            var v3 = new Vector(2, -3, -3, -4);
            var matrix = new Matrix(v1, v2, v3);
            var b = new Vector(1, 2, 1);

            var solution = GaussJordanMethod.Solve(matrix, b, false);
            Assert.AreEqual(solution.ToString(), "[0.5, 0, 0, 0] + <[1.5, 1, 0, 0],[-0.062, 0, -1.375, 1]>");
        }

        [Test]
        public static void SystemWithLinearManifold2()
        {
            var matrix = new Matrix(new Vector(1, 0, 0, 0),
                                    new Vector(0, 0, 0, 0),
                                    new Vector(1, 0, 0, 0),
                                    new Vector(0, 0, 0, 1));
            var b = new Vector(0, 0, 0, 0);

            var solution = GaussJordanMethod.Solve(matrix, b, false);
            Assert.AreEqual(solution.ToString(), "[0, 0, 0, 0] + <[0, 1, 0, 0],[0, 0, 1, 0]>");
        }

        [Test]
        public static void PseudoSolution()
        {
            var v1 = new Vector(1, 1);
            var v2 = new Vector(2, 1);
            var v3 = new Vector(3, 1);
            var v4 = new Vector(4, 1);
            var v5 = new Vector(5, 1);
            var matrix = new Matrix(v1, v2, v3, v4, v5);
            var b = new Vector(1, 4, 9, 16, 25);

            var solution = GaussJordanMethod.Solve(matrix, b, false);
            Assert.AreEqual(solution.ToString(), "[6, -7]");
        }

        [Test]
        public static void Determinant()
        {
            var matrix = new Matrix(new Vector(8, 9, 8),
                                    new Vector(2, 6, 3),
                                    new Vector(3, 0, 5));
            Assert.AreEqual(matrix.Determinant, 87);
        }

        [Test]
        public static void InverseMatrix()
        {
            var matrix = new Matrix(new Vector(2, 5, 7),
                                    new Vector(6, 3, 4),
                                    new Vector(5, -2, -3));
            var back = new Matrix(new Vector(1, -1, 1),
                                    new Vector(-38, 41, -34),
                                    new Vector(27, -29, 24));
            Assert.AreEqual(MatrixEquation.GetInverseMatrix(matrix), back);
        }

        [Test]
        public static void MatrixEquation1()
        {
            var A = new Matrix(new Vector(1, 0),
                                    new Vector(-1, 1),
                                    new Vector(0, 1));
            var B = new Matrix(new Vector(1, 1, -1, -1),
                                    new Vector(1, 0, 1, 0),
                                    new Vector(2, 1, 0, -1));
            var x = new Matrix(new Vector(1, 1, -1, -1),
                                    new Vector(2, 1, 0, -1));
            Assert.AreEqual(MatrixEquation.Solve(A, B), x);
        }

        [Test]
        public static void CalculateEigenvalues()
        {
            var matrix = new Matrix(new Vector(4, 5, 6),
                                    new Vector(-5, -7, -9),
                                    new Vector(2, 3, 4));
            var linear_operator = new LinearOperator(matrix);
            var eval = linear_operator.GetEigenvalues();
            Assert.AreEqual(new double[] { 0, 0, 1 }, eval.ToArray());
        }

        [Test]
        public static void CalculateEigenvectors()
        {
            var matrix = new Matrix(new Vector(-1, 1, 0),
                                    new Vector(0, 0, 1),
                                    new Vector(0, 1, 1));
            var linOperator = new LinearOperator(matrix);
            var eigenvalues = linOperator.GetEigenvalues();
            var actualEigenvectors = new List<Vector>();
            var expectedEigenvectors = new List<Vector>()
            {
                new Vector(1, 0, 0),
                new Vector(-4.236, -1.618, 1),
                new Vector(0.236, 0.618, 1)
            };
            foreach (var e in eigenvalues)
            {
                var eigenvectors = linOperator.GetEigenvectors(e);
                foreach (Vector v in eigenvectors)
                {
                    Assert.IsTrue(expectedEigenvectors.Contains(v));
                    actualEigenvectors.Add(v);
                }
            }
            Assert.AreEqual(expectedEigenvectors.Count, actualEigenvectors.Count);
        }

        [Test]
        public static void Diagonalize1()
        {
            var matrix = new Matrix(new Vector(5, -1, -1),
                                    new Vector(-1, 5, -1),
                                    new Vector(-1, -1, 5));
            var linOperator = new LinearOperator(matrix);
            var d = linOperator.Diagonolize();
            var transitionMatrix = new Matrix(new Vector(1, -1, -1),
                                              new Vector(1, 1, 0),
                                              new Vector(1, 0, 1));
            var diagonilizedMatrix = new Matrix(new Vector(3, 0, 0),
                                                new Vector(0, 6, 0),
                                                new Vector(0, 0, 6));
            Assert.AreEqual(d[0], transitionMatrix);
            Assert.AreEqual(d[1], diagonilizedMatrix);
        }

        [Test]
        public static void Diagonalize2()
        {
            var matrix = new Matrix(new Vector(1, 0, 0),
                                    new Vector(0, 2, 0),
                                    new Vector(0, 0, 3));
            var linOperator = new LinearOperator(matrix);
            var d = linOperator.Diagonolize();
            var transitionMatrix = new Matrix(new Vector(1, 0, 0),
                                              new Vector(0, 1, 0),
                                              new Vector(0, 0, 1));
            var diagonilizedMatrix = new Matrix(new Vector(1, 0, 0),
                                                new Vector(0, 2, 0),
                                                new Vector(0, 0, 3));
            Assert.AreEqual(d[0], transitionMatrix);
            Assert.AreEqual(d[1], diagonilizedMatrix);
        }

        [Test]
        public static void DiagonolizedThrowException()
        {
            var matrix = new Matrix(new Vector(1, 0, 0),
                                    new Vector(1, 1, 1),
                                    new Vector(1, 0, 1));
            var linOperator = new LinearOperator(matrix);
            try
            {
                var d = linOperator.Diagonolize();
            }
            catch(InvalidOperationException e)
            {
                Assert.IsTrue(e.Message.Contains("Use Jordan normal form"));
            }
        }

        [Test]
        public static void Diagonalize3()
        {
            var matrix = new Matrix(new Vector(1, 1, 1),
                                    new Vector(1, 1, 1),
                                    new Vector(1, 1, 1));
            var linOperator = new LinearOperator(matrix);
            var d = linOperator.Diagonolize();
            var transitionMatrix = new Matrix(new Vector(-1, -1, 1),
                                              new Vector(1, 0, 1),
                                              new Vector(0, 1, 1));
            var diagonilizedMatrix = new Matrix(new Vector(0, 0, 0),
                                                new Vector(0, 0, 0),
                                                new Vector(0, 0, 3));
            Assert.AreEqual(d[0], transitionMatrix);
            Assert.AreEqual(d[1], diagonilizedMatrix);
        }

        [Test]
        public static void KernelAndImageBasis()
        {
            var matrix = new Matrix(new Vector(1, 2, 3, 4),
                                    new Vector(4, 3, 2, 1),
                                    new Vector(5, 6, 7, 8),
                                    new Vector(8, 7, 6, 5));

            var linearOperator = new LinearOperator(matrix);

            var kernel = linearOperator.FindKernelBasis();
            var image = linearOperator.FindImageBasis();

            Assert.IsTrue(kernel.Count == 2);
            Assert.IsTrue(image.Count == 2);

            Assert.AreEqual(kernel, new Vector[] { new Vector(1, -2, 1, 0), new Vector(2, -3, 0, 1) });
            Assert.AreEqual(image, new Vector[] { new Vector(1, 4, 5, 8), new Vector(0, -5, -4, -9) });
        }

        [Test]
        public static void OrthogonalProjectionAndComponent()
        {
            var vectorSpace = new VectorSpace(new Vector(1, 1, 1, 1),
                                              new Vector(1, 2, 2, -1),
                                              new Vector(1, 0, 0, 3));
            var x = new Vector(4, -1, -3, 4);

            var y = vectorSpace.FindProjection(x);
            var z = vectorSpace.FindOrthogonalComponent(x);

            Assert.AreEqual(y, new Vector(1, -1, -1, 5));
            Assert.AreEqual(z, new Vector(3, 0, -2, -1));
        }

        [Test]
        public static void FindAngle()
        {
            var vectorSpace = new VectorSpace(new Vector(3, 4, -4, -1),
                                              new Vector(0, 1, -1, 2));
            var x = new Vector(2, 2, 1, 1);
            var angle = vectorSpace.GetAngle(x, AngleFormat.Degree);
            Assert.AreEqual(angle, 60);
            angle = vectorSpace.GetAngle(x, AngleFormat.Radian);
            Assert.AreEqual(angle, 1.0472);
        }

        [Test]
        public static void Orthogonalization()
        {
            var vectorSpace = new VectorSpace(new Vector(1, 2, 2, -1),
                                              new Vector(1, 1, -5, 3),
                                              new Vector(3, 2, 8, -7));
            var ortogonalBasis = vectorSpace.GramSmidtOrthogonalization();
            Assert.AreEqual(new Vector(1, 2, 2, -1), ortogonalBasis[0]);
            Assert.AreEqual(new Vector(2, 3, -3, 2), ortogonalBasis[1]);
            Assert.AreEqual(new Vector(2, -1, -1, -2), ortogonalBasis[2]);
        }

        [Test]
        public static void OperatorInNewBasis()
        {
            var e = new Vector[] { new Vector(8, -6, 7),
                                  new Vector(-16, 7, -13),
                                  new Vector(9, -3, 7)};
            var f = new Vector[] { new Vector(1, -2, 1),
                                  new Vector(3, -1, 2),
                                  new Vector(2, 1, 2)};

            var A = new LinearOperator(new Matrix(new Vector(1, -18, 15),
                                                  new Vector(-1, -22, 20),
                                                  new Vector(1, -25, 22)));

            var B = A.ChangeBasis(e, f);
            var expected = new Matrix(new Vector(1, 2, 2),
                                      new Vector(3, -1, -2),
                                      new Vector(2, -3, 1));
            Assert.AreEqual(expected, B.matrix);
        }

        [Test]
        public static void SwapCoordinatesInVector()
        {
            Vector v = new Vector(1, 2, 3, 4, 5);
            v = v.Swap(1, 3);
            Assert.AreEqual(v, new Vector(1, 4, 3, 2, 5));
        }

        [Test]
        public static void GetPermutations()
        {
            Vector v = new Vector(1, 2, 3);
            var actual = Combinatorics.GetAllPermutations(v);
            foreach(var elem in actual)
                Console.WriteLine(elem.GetHashCode()); 
            var expected = new List<Vector>
            {
                new Vector(1, 2, 3),
                new Vector(3, 2, 1),
                new Vector(2, 3, 1),
                new Vector(1, 3, 2),
                new Vector(3, 1, 2),
                new Vector(2, 1, 3)
            };
            Assert.True(new HashSet<Vector>(actual).SetEquals(expected));
        }

        [Test]
        public static void SolveEquation()
        {
            var equation = new Vector(6, -7, 0, 1);
            var roots = Polynomial.Solve(equation);
            Assert.IsTrue(roots.Count == 3);
            Assert.IsTrue(roots.Contains(1) && roots.Contains(2) && roots.Contains(-3));

            equation = new Vector(-3, 0, 1);
            roots = Polynomial.Solve(equation);
            Assert.IsTrue(roots.Count == 2);
            Assert.IsTrue(roots.Contains(1.732) && roots.Contains(-1.732));
        }

        [Test]
        public static void FindCharacteristicPolynomial()
        {
            var matrix = new Matrix(new Vector(1, -1, 0, 2),
                                    new Vector(-1, 0, 0, 1),
                                    new Vector(1, 2, -1, 0),
                                    new Vector(-2, 0, -1, -1));
            var linOperator = new LinearOperator(matrix);
            var characteristicPolynomial = linOperator.GetCharacteristicPolynomial();
            Console.WriteLine(characteristicPolynomial);
            Assert.AreEqual(new Vector(-10, 3, 2, 1, 1), characteristicPolynomial);
        }
    }
}