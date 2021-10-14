# Abstract
As you could noticed, a lot of methods in [linear algebra](https://en.wikipedia.org/wiki/Linear_algebra) involve itself basic but repetitive steps, as multiply matrices, find inverse matrix, etc. Moreover, these methods can be programmed. So I decided to develop a library on C# which can solve typical linear algebra problems:

* Solve systems of linear equations, including solving a non-compatible systems and systems with fundamental system of solutions
* Matrix equations => find inverse matrix
* Linear operator: matrix in other basis, basis of kernel, and basis of image of linear operator.
* Rank of matrix
* Gram-Schmidt process

# Disclaimer

This library works with natural, integer, and rational numbers well. With irrational numbers and real numbers works only approximately. And not works with complex numbers.

# Fractions
Just simple fractions. The first argument is numerator, the second is denominator.
```C#
public Fraction(long numerator, long denominator)
```
All computations in this library powered by fractions. It is essential when we, for example, 1 : 3 => 0.(3). But then 0.(3) * 3 = 0.(9). But it is work perfect if we would use fractions: 1 : 3 = 1/3, 1/3 * 3 = 1.
```C#
var fraction = new Fraction(1, 3);
```
You can get a approximate value of this fraction:
```C#
var fraction = new Fraction(1, 3);
Console.WriteLine(fraction.ApproximateValue); // 0.333...
```
All math operations can be applied to fractions: addition, subtract, multiply, division, and comparing: >, <=, ==, !=, etc.

```C#
var fracOne = new Fraction(3, 7);
var fracTwo = new Fraction(2, 9);
var sum = fracOne + fracTwo; // 41/63
var sub = fracOne - fracTwo; // 13/63
var mul = fracOne * fracTwo; // 2/21
var div = fracOne / fracTwo; // 27/14
```

You can parse fraction to string
```C#
var fraction = new Fraction(1, 3);
Console.WriteLine(fraction); // 1/3
```

Int to Fraction is Fraction with denominator 1. In example below Double parse to Fraction.
```C#
var num = Math.PI;
var frac = Fraction.Parse(num); // 157/50 => 3.14 - get only first two digits after point
```

# Vector
Represents a n - dimensional vector. You may enumerate all coordinates split by comma (,) or give an IEnumerable
```C#
public Vector(params Fraction[] source)
public Vector(IEnumerable<Fraction> source)
```

Also, you can create a zero vector of given size.

```C#
public Vector(int size)
```

Standart properties, what could be with vector, as Lenght, or dimention are available:

```C#
var vector = new Vector(3, 4, 5);
Console.WriteLine(vector.Lenght); // is double: 7,07106781186548
Console.WriteLine(vector.Size); // 3
```

Vector is based on IEnumerable<Fraction>, so it is correct to call it as numeric sequence, using in foreach, get and set by index. Valid operations:
* multiply vector on a scalar:
```C#
var vector = new Vector(3, 4, 5);
Console.WriteLine(2 * vector); // => [6, 8, 10] As you have noticed, Vector has it's own override of ToString(), so it is work as in example
```
* dot product
```C#
var vector1 = new Vector(3, 4, 5);
var vector2 = new Vector(1, 0, 2);
Console.WriteLine(vector1 * vector2); // => 13
```
* Add and Subtract vectors.
*Normilize - so Length of normilized vector is 1

# Linear manifold
It is a algebraic stucture, that has a vector, and a linear subspace. It is useful, when, for example, we solve a system of linear equations: and we have more than one solution, and we must to enum all basis vectors of subspace of the solutions of given system. So linear manifold - it is a single vector (also named shift vector), and a basis of linear subspace.
```C#
public LinearManifold(Vector shiftVector, Vector[] subspaceBasis)
```
  
As Vector, it can be parsed to string: "[shiftVector] + vectors in basis in angle (<) brackets"

# Matrix
I see matrix as a set of Vectors, also called string-vectors.
```C#
public Matrix(params Vector[] strings)
public Matrix(Fraction [,] matrix)
public Matrix(int stringSize, int columnSize) // initializate zero matrix
```

For each matrix next fields are available: StringSize, ColumnSize, IsSquare, Determinant, Rank.

The one of most frequent operation is transposing:
```C#
public Matrix Transpose()
```
Also, we should see how matrix changes in some process (For example, in a solving a system). For this things method Print() exists:
```C#
public void Print(int usedColumns = -1)
```
Here you can see a usedColumns argument with default value -1. If you want to print a border ( | ) between matrix and vector you can put there an index of this border.
```C#
var matrix = new Matrix(new Vector(2, 5, 7),
                        new Vector(6, 3, 4),
                        new Vector(5, -2, -3));
matrix = matrix.AddColumn(new Vector(1, 0, 1));
matrix.Print(3);

// Output:
// 2       5       7       |1
// 6       3       4       |0
// 5       -2      -3      |1
```
In some cases, it is useful to get or set string or column in Matrix. So, library provides this methods to manipulate with strings/columns
```C#
public Vector GetString(int index);
public void SetString(int index, Vector v);
public Vector GetColumn(int index);
public void SetColumn(int index, Vector v);
public void DeleteColumn(int index); // indexes start with 0
public void ReplaceStrings(int indexOne, int indexTwo);
public Matrix AddColumn(Vector b);
```

Typical operations with matrices:
  * Multiply matrix on matrix
    ```C#
    var A = new Matrix(new Vector(8, 9, 8),
                       new Vector(2, 6, 3),
                       new Vector(3, 0, 5));
    var B = new Matrix(new Vector(-2, 1, 0),
                       new Vector(0, -3, 4),
                       new Vector(-1, -2, -3));
    (A*B).Print();
    ```
  * Add matrices
  * Multiply matrix on a scalar
    ```C#
    var A = new Matrix(new Vector(1, 1, 0),
                       new Vector(-1, 2, 3),
                       new Vector(0, 0, -1));
    var B = -2 * A; // matrix
    // result is: [-2, -2, 0]
    //            [2, -4, -6]
    //            [0,  0,  2]
    ```
  * Multiply matrix on a vector

# Main algorithms
So, it was a long introduction with instruments which we will use in methods below.
  
**Class GaussJordanMethod**
  
* Stepwise and Identity form
  
  The first argument is matrix, the second one - if you need to print step by step.
  ```C#
  public static Matrix StepwiseForm(Matrix m, bool output);
  public static Matrix IdentityForm(Matrix m, bool output);
  ```
  
  Usage:
  ```C#
  var matrix = new Matrix(new Vector(3, 5, 6),
                          new Vector(-5, -8, -9),
                          new Vector(2, 3, 3));
  matrix = GaussJordanMethod.StepwiseForm(matrix, false);
  matrix.Print();
    // Output:
    //  3       5       6
    //  0       1/3     1
    //  0       0       0
  ```
* Solving a system of linear equations
  
  In terms of matrices and vectors we can write a system as
  ```C#
  Ax = b
  ```
  Where: A - is a main matrix of system, x - vector-column of variables, b - vector-column of values.
  ```C#
  public static Vector Solve(Matrix matrix, Vector b, bool output)
  ```
  Here can be 3 posible ways: system have a defined solution, system have a infinity solutions (in this case answer will be a linear manifold), system have not solutions (but algorithm will find a vector, which approximately brings a correct solution. [More in Wikipedia](https://en.wikipedia.org/wiki/Overdetermined_system) ).
  
  ```C#
  var v1 = new Vector(2, -4, 9);
  var v2 = new Vector(7, 3, -6);
  var v3 = new Vector(7, 9, -9);
  var matrix = new Matrix(v1, v2, v3);
  var b = new Vector(28, -1, 5);

  var solution = GaussJordanMethod.Solve(matrix, b, false);
  Console.WriteLine(solution); // => [2, 3, 4]
  ```
  
**Class VectorSpace**
  
  Array of vectors is a basis of vector space.
  ```C#
  public static Vector[] Vectors;

  public VectorSpace(params Vector[] vectors)
  {
      Vectors = vectors;
  }
  ```
* Projection, Component and Angle
  
  Here you can find an orhogonal component and orthogonal projection of given vector. And find an angle between vector space (as subspace) and given vector:
  ```C#
  public Vector FindProjection(Vector x);
  public Vector FindOrthogonalComponent(Vector x);
  public double GetAngle(Vector x, AngleFormat format);
  ```
  In this library there are two ways to represent angle value: by degrees and by radians, so you can choose.
* Gram-Smidt Process
  
  You have a set of vectors, and it will be useful, when this vectors are pairwise orthogonal. So, this algorithm make your vectors be pairwise orthogonal.
  ```C#
  public Vector[] GramSmidtOrthogonalization();
  ```
  
**Class MatrixEquation**
  
* Find inverse matrix
  
  Warning: inverse matrix exists if and only if this matrix is square, and determinant != 0
  ```C#
  public static Matrix GetInverseMatrix(Matrix a)
  ```
* Matrix equations
  
  When you need to solve equation with matrices, where A,B - defined matrices
  ```C#
  AX = B
  ```
  Note. If you have an equation: XA = B, you should transpose left and right part: A^T*X^T = B^T, and now you can use this function.
  ```C#
  public static Matrix Solve(Matrix a, Matrix b);
  ```
  
**Class LinearOperator**
  
  You have a linear operator, saying correctly, matrix of linear operator in some basis.
  ```C#
  public Matrix matrix;

  public LinearOperator(Matrix matrix)
  {
    this.matrix = matrix;
  }
  ```
* Find kernel and image basis
  
  You may know basis of kernel and basis of image of this operator:
  ```C#
  public Vector[] FindKernelBasis();
  public Vector[] FindImageBasis();
  ```
* Change basis  
  
  If you want to see matrix of linear operator in some another basis, use next method:
  ```C#
  public LinearOperator ChangeBasis(Vector[] oldBasis, Vector[] newBasis);
  ```
  
  Eigenvectors and eigenvalues as soon as far!
