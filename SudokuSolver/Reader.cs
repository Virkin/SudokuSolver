using System;

namespace SudokuSolver
{
    class Reader
    {
        private string path;
        private char[,] sudoku2d = new char[9, 9];

        public Reader()
        {
            filePath("Sudoku1.txt");
        }

        public void filePath(string filename)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = System.IO.Directory.GetParent(workingDirectory).Parent.FullName;
            this.path = System.IO.Path.Combine(projectDirectory, filename);
        }

        public String getSudoku()
        {
            string file = System.IO.File.ReadAllText(this.path);
            return file;
        }

        public void fillSudoku2d()
        {
            string[] lines = System.IO.File.ReadAllLines(this.path);
            for (int i=0; i<lines.Length; i++)
            {
                string line = lines[i];
                Console.WriteLine(line);
                for (int j=0; j<lines[i].Length; j++)
                {
                    char square = line[j];
                    Console.WriteLine(square);
                    Console.WriteLine(i+":"+j);
                    this.sudoku2d[i,j] = square;
                }
            }
        }

        public void Read()
        {
            Console.WriteLine(getSudoku());
            Console.WriteLine();
            fillSudoku2d();
            Console.WriteLine();
            // this.sudoku2d[ligne,colonne];
            Console.WriteLine(this.sudoku2d[1,2]);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
