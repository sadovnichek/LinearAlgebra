using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebra.Support
{
    public class TexFile
    {
        private string filePath;

        private const string header = @"\documentclass[preview, border = 12pt]{standalone}" + "\n" +
@"\usepackage{amsmath}" + "\n" +
@"\usepackage[paperwidth=\maxdimen,paperheight=\maxdimen]{geometry}" + "\n" +
@"\usepackage{tikz}" + "\n" +
@"\usepackage[T2A]{ fontenc}" + "\n" +
@"\setcounter{MaxMatrixCols}{100}" + "\n\n" +
@"\begin{document}" + "\n";

        public TexFile(string filePath)
        {
            this.filePath = filePath;
            File.Delete(filePath);
            File.AppendAllText(filePath, header);
        }

        public void Write(string content)
        {
            File.AppendAllText(filePath, content + "\n" + @"\\" + "\n" + @"\\" + "\n");
        }

        public void Close()
        {
            File.AppendAllText(filePath, @"\end{document}");
        }

        public void Delete()
        {
            File.Delete(filePath);
        }
    }
}
