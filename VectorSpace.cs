using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra
{
    public enum AngleFormat
    {
        Degree,
        Radian
    }

    public class VectorSpace
    {
        public static Vector[] Vectors;

        public VectorSpace(params Vector[] vectors)
        {
            Vectors = vectors;
        }

        public Vector FindProjection(Vector x)
        {
            var matrix = new Matrix(Vectors.Length, Vectors.Length);
            var b = new Vector(Vectors.Select(v => v * x));
            for(int i = 0; i < Vectors.Length; i++)
            {
                for(int j = 0; j < Vectors.Length; j++)
                {
                    matrix[i, j] = Vectors[i] * Vectors[j];
                }
            }
            var scalars = GaussJordanMethod.Solve(matrix, b, false);
            if(scalars is LinearManifold)
                scalars = (scalars as LinearManifold).GetShiftVector();
            var result = new Vector(Vectors[0].Size);
            for(int i = 0; i < Vectors.Length; i++)
            {
                result += scalars[i] * Vectors[i];
            }
            return result;
        }

        public Vector FindOrthogonalComponent(Vector x)
        {
            return x - FindProjection(x);
        }

        public double GetAngle(Vector x, AngleFormat format)
        {
            var z = FindOrthogonalComponent(x);
            var angle = Math.Asin(z.Lenght / x.Lenght) * 180 / Math.PI;
            if (format == AngleFormat.Degree)
                return Math.Round(angle, 4);
            angle = Math.Asin(z.Lenght / x.Lenght);
            return Math.Round(angle, 4);
        }

        public Vector[] GramSmidtOrthogonalization()
        {
            var result = new List<Vector> { Vectors[0] };
            for (int i = 1; i < Vectors.Length; i++)
            {
                var currentMatrix = new Matrix(result.Count, result.Count);
                for(int j = 0; j < result.Count; j++)
                {
                    currentMatrix[j, j] = result[j] * result[j];
                }
                var b = new Vector(result.Count);
                for(int j = 0; j < result.Count; j++)
                {
                    b[j] = -1 * Vectors[i] * result[j];
                }
                var scalars = GaussJordanMethod.Solve(currentMatrix, b, false);
                var newVector = new Vector(Vectors[0].Size);
                for(int j = 0; j < result.Count; j++)
                {
                    newVector += scalars[j] * result[j];
                }
                newVector += Vectors[i];
                result.Add(newVector);
            }
            return result.ToArray();
        }

        public void NormilizeVectors()
        {
            for(int i = 0; i < Vectors.Length; i++)
            {
                Vectors[i] = Vectors[i].Normilize();
            }
        }
    }
}
