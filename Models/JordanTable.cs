using LinearAlgebra.Methods;
using LinearAlgebra.Support;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra.Models
{
    internal class JordanTable
    {
        private Matrix jordanMatrix;

        private int size;

        private TexFile file;

        public JordanTable(Matrix layer, int size, TexFile file)
        {
            this.size = size;
            this.file = file;
            var firstBlocks = layer.GetSubMatrix(0, layer.ColumnSize - size);
            var lastBlock = layer.GetSubMatrix(layer.ColumnSize - size, layer.ColumnSize);
            var result = new List<Vector>();
            for (int i = 0; i < lastBlock.StringSize; i++) // берем те строки, у которых в последнем блоке - нулевой вектор
            {
                if (lastBlock.GetString(i).IsZero)
                    result.Add(firstBlocks.GetString(i));
            }
            jordanMatrix = new Matrix(result); // создаём из таких строк матрицу
            Shift();
            file.Write(jordanMatrix.GetLatexNotation(size));
        }

        private Vector ShiftVectors(Vector vectorString)
        {
            var queue = new Queue<Vector>();
            var result = new List<double>();
            for(int pointer = 0; pointer < vectorString.Size; pointer += size)
            {
                var currentVector = new Vector(vectorString.Skip(pointer).Take(size));
                if(!currentVector.IsZero)
                    queue.Enqueue(currentVector);
            }
            var zeroVectors = vectorString.Size / size - queue.Count;
            for(int i = 0; i < zeroVectors; i++)
            {
                var zeroVector = Enumerable.Repeat(double.NaN, size);
                result.AddRange(zeroVector);
            }
            while(queue.Count > 0)
            {
                var vector = queue.Dequeue();
                result.AddRange(vector);
            }
            return new Vector(result);
        }

        private void Shift()
        {
            jordanMatrix = jordanMatrix.DeleteZeroStrings();
            for (int i = 0; i < jordanMatrix.StringSize; i++)
            {
                var currentString = jordanMatrix.GetString(i);
                jordanMatrix.SetString(i, ShiftVectors(currentString));
            }
        }

        private void Iterate()
        {
            while (true)
            {
                var iterated = GaussJordanMethod.StepwiseForm(jordanMatrix, true, file, size, true);
                if (iterated != jordanMatrix)
                {
                    jordanMatrix = iterated;
                    Shift();
                }
                else
                    break;
            }
        }

        public List<List<Vector>> GetNillLayers()
        {
            var result = new List<List<Vector>>();
            Iterate();
            for (int i = 0; i < jordanMatrix.StringSize; i++)
            {
                var vectorString = jordanMatrix.GetString(i);
                var nillLayer = new List<Vector>();
                for (int pointer = 0; pointer < vectorString.Size; pointer += size)
                {
                    var currentVector = new Vector(vectorString.Skip(pointer).Take(size));
                    if (!currentVector.Contains(double.NaN))
                        nillLayer.Add(currentVector);
                }
                if(nillLayer.Count > 0)
                    result.Add(nillLayer);
            }
            return result;
        }
    }
}
