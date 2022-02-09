using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using ZedGraph;

namespace LinearAlgebra
{
    internal static class Charts
    {
        public static void ShowHeatmap(HeatmapData stats)
        {
            var form = new Form
            {
                Text = stats.Title,
                Size = new Size(800, 600)
            };
            form.Paint += (s, e) => DrawHeatmap(form.ClientRectangle, e.Graphics, stats);
            form.ShowDialog();
        }

        private static void DrawHeatmap(Rectangle clientRect, Graphics g, HeatmapData data)
        {
            var values = data.Heat.Cast<double>().ToList();
            var cellWidth = clientRect.Width / (data.XLabels.Length + 1);
            var cellHeight = clientRect.Height / (data.YLabels.Length + 1);
            for (var x = 0; x < data.XLabels.Length; x++)
                for (var y = 0; y < data.YLabels.Length; y++)
                {
                    var color = GetColor(Math.Abs(data.Heat[x, y]));
                    var cellRect = new Rectangle(
                        clientRect.Left + cellWidth * (1 + x),
                        clientRect.Top + cellHeight * y,
                        cellWidth,
                        cellHeight
                    );
                    g.FillRectangle(new SolidBrush(color), cellRect);
                }

            DrawLabels(g, data, cellWidth, cellHeight);
        }

        private static void DrawLabels(Graphics g, HeatmapData data, int cellWidth, int cellHeight)
        {
            var font = new Font(FontFamily.GenericMonospace, 10);
            for (var x = 0; x < data.XLabels.Length; x++)
            {
                var text = data.XLabels[x];
                var labelRect = new RectangleF(cellWidth * (1 + x), data.YLabels.Length * cellHeight, cellWidth,
                    cellHeight);
                var format = new StringFormat();
                format.LineAlignment = StringAlignment.Near;
                format.Alignment = StringAlignment.Center;
                g.DrawString(text, font, new SolidBrush(Color.Black), labelRect, format);
            }

            for (var y = 0; y < data.YLabels.Length; y++)
            {
                var text = data.YLabels[y];
                var labelRect = new RectangleF(0, y * cellHeight, cellWidth, cellHeight);
                var format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Far;
                g.DrawString(text, font, new SolidBrush(Color.Black), labelRect, format);
            }
        }


        private static Color GetColor(double value)
        {
            if (value > 1)
                value = 1;
            var color = Color.FromArgb((int)(value*255), Color.Black);
            return color;
        }
    }
}
