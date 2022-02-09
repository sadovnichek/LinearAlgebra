using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebra
{
    public class HeatmapData
    {
        public HeatmapData(string title, Matrix heat, string[] xLabels, string[] yLabels)
        {
            XLabels = xLabels;
            YLabels = yLabels;
            Title = title;
            Heat = heat.Transpose().Data;
        }

        public string[] XLabels { get; }
        public string[] YLabels { get; }
        public string Title { get; }
        public double[,] Heat { get; }

        public bool Equals(HeatmapData other)
        {
            return Enumerable.Range(0, 2)
                       .All(dimension =>
                           Heat.GetLength(dimension) == other.Heat.GetLength(dimension))
                   && Heat
                       .Cast<double>()
                       .SequenceEqual(other.Heat
                           .Cast<double>());
        }
    }
}
