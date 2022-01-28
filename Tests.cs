using NUnit.Framework;
using System;

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
                                            new Vector(0, new Fraction(1, 3), 1),
                                            new Vector(0, 0, 0));
            var sum = new Matrix(new Vector(6, 10, 12),
                                 new Vector(-5, new Fraction(-23, 3), -8),
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
                                            new Vector(0, new Fraction(1, 3), 1),
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
        public static void FractionOperations()
        {
            var f1 = new Fraction(3, 7);
            var f2 = new Fraction(2, 9);
            Assert.AreEqual(f1 + f2, new Fraction(41, 63));
            Assert.AreEqual(f1 - f2, new Fraction(13, 63));
            Assert.AreEqual(f1 * f2, new Fraction(2, 21));
            Assert.AreEqual(f1 / f2, new Fraction(27, 14));
        }

        [Test]
        public static void FractionsEquality()
        {
            var f1 = new Fraction(3, 7);
            var f2 = new Fraction(6, 14);
            Assert.AreEqual(f1, f2);
        }

        [Test]
        public static void FractionsParsing()
        {
            var input = "3/8";
            var fraction = Fraction.Parse(input);
            Assert.AreEqual(fraction, new Fraction(3, 8));
            var number = 44;
            Assert.AreEqual(Fraction.Parse(number), new Fraction(44, 1));
        }

        [Test]
        public static void FractionsCompare()
        {
            var f1 = new Fraction(3, 7);
            var f2 = new Fraction(6, 15);
            Assert.IsTrue(f1 > f2);
            var f3 = new Fraction(1, 4);
            Assert.IsTrue(f3 <= f1);
            Assert.AreEqual(f3, 0.25);
            Assert.AreEqual(new Fraction(314, 100), Fraction.Parse(Math.PI));
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
            Assert.AreEqual(solution.ToString(), "[0.5, 0, 0, 0] + <[1.5, 1, 0, 0],[-0.0625, 0, -1.375, 1]>");
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

        //[Test]
        //public static void EigenValuesAndVectors1()
        //{
        //    var matrix = new Matrix(new Vector(4, 5, 6),
        //                            new Vector(-5, -7, -9),
        //                            new Vector(2, 3, 4));
        //    var eval = LinearOperator.GetEigenValues(matrix);
        //    Assert.AreEqual(eval.ToArray(), new Fraction[] { 0, 1 });
        //    var evec = LinearOperator.GetEigenVectors(matrix, 0);
        //    Assert.AreEqual(evec, new Vector[] { new Vector(1, -2, 1) });
        //    evec = LinearOperator.GetEigenVectors(matrix, 1);
        //    Assert.AreEqual(evec, new Vector[] { new Vector(3, -3, 1) });
        //}

        //[Test]
        //public static void EigenValuesAndVectors2()
        //{
        //    var matrix = new Matrix(new Vector(-1, 1, 0),
        //                            new Vector(0, 0, 1),
        //                            new Vector(0, 1, 1));
        //    var eval = LinearOperator.GetEigenValues(matrix);
        //    Assert.AreEqual(eval.ToArray(), new Fraction[] { -1, new Fraction(-309, 500), new Fraction(809, 500) });
        //    var evec = LinearOperator.GetEigenVectors(matrix, -1);
        //    Assert.AreEqual(evec, new Vector[] { new Vector(1, 0, 0) });
        //    evec = LinearOperator.GetEigenVectors(matrix, new Fraction(-309, 500));
        //    Assert.True(evec.Length == 1);
        //    var eigenVector = evec.First();
        //    var mul = eigenVector.Aggregate((x, y) => x * y).PreciseValue;
        //    Assert.AreEqual(mul, 6.8541, 1e-3);
        //    evec = LinearOperator.GetEigenVectors(matrix, new Fraction(809, 500));
        //    Assert.True(evec.Length == 1);
        //    eigenVector = evec.First();
        //    mul = eigenVector.Aggregate((x, y) => x * y).PreciseValue;
        //    Assert.AreEqual(mul, 0.1459, 1e-3);
        //}

        //[Test]
        //public static void Diagonalize()
        //{
        //    var matrix = new Matrix(new Vector(5, -1, -1),
        //                            new Vector(-1, 5, -1),
        //                            new Vector(-1, -1, 5));
        //    var d = LinearOperator.Diagonalize(matrix);
        //    var transitionMatrix = new Matrix(new Vector(1, -1, -1),
        //                                      new Vector(1, 1, 0),
        //                                      new Vector(1, 0, 1));
        //    var diagonilizedMatrix = new Matrix(new Vector(3, 0, 0),
        //                                      new Vector(0, 6, 0),
        //                                      new Vector(0, 0, 6));
        //    Assert.AreEqual(d[0], transitionMatrix);
        //    Assert.AreEqual(d[1], diagonilizedMatrix);
        //}

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

            Assert.IsTrue(kernel.Length == 2);
            Assert.IsTrue(image.Length == 2);

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
    }
}