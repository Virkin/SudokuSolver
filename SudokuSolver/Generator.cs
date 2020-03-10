using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IO;

namespace SudokuSolver
{
    class Generator : Sudoku
    {
       
        private Solver solver;

        private int nbRemoveValues = 0;
        private int nbRemove = 0;

        private int[,] solveGrid;

        private string sudokuName;

        public Generator(string name) : base()
        {
            sudokuName = name;
            
            solver = new Solver(grid);
            solver.Run();

            solveGrid = grid.Clone() as int[,];
            //PrintGrid();

            string generateConf = ConfigurationManager.AppSettings.Get("Generate");
            if (generateConf == "easy")
                nbRemoveValues = 81-45;
            else if (generateConf == "medium")
                nbRemoveValues = 81-35;
            else if (generateConf == "hard")
                nbRemoveValues = 81-25;

            Console.WriteLine("Generating sudoku ({0}) ...", generateConf);
            while (nbRemove < nbRemoveValues)
            {
                removeRandomValue();
            }

            PrintGeneratedGrid();
            writeSudokuToFile();
        }

        public void writeSudokuToFile()
        {
            string time = DateTime.Now.ToString("yyyyMMdd_HHmmss"); 
            string generateConf = ConfigurationManager.AppSettings.Get("Generate");
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            //string path = projectDirectory+"\\Sudoku"+time+generateConf+".txt";
            string path = projectDirectory + "\\" + sudokuName + ".txt";

            List<string> lines = new List<string>();
            string line = "";
            int nbLine = 0;

            foreach (int number in grid)
            {
                if (nbLine == gridSize)
                {
                    lines.Add(line);
                    line = "";
                    nbLine = 0;
                }
                line+=number;
                nbLine++;
            }
            lines.Add(line);

            File.WriteAllLines(path, lines.ToArray());
        }

        public void removeRandomValue()
        {
            Random rnd = new Random();
            int i = rnd.Next(gridSize);
            int j = rnd.Next(gridSize);

            //Console.WriteLine("i:{0} ,j:{1} ,val:{2}", i, j, grid[i, j]);
            if(grid[i,j] > 0)
            {
                int oldValue = grid[i, j];

                grid[i, j] = 0;

                solver.NewGrid(grid.Clone() as int[,]);

                int res = solver.Run();

                //Console.WriteLine("res : {0}", res);
                
                if (res==0 && CompareSolveGrid(solver.GetGrid()))
                {
                    nbRemove++;
                    return;
                }

                grid[i, j] = oldValue;
            }
            

            //PrintGrid();
            //Console.ReadKey();
        }

        public bool CompareSolveGrid(int [,] compGrid)
        {
           for(int i=0; i<gridSize; i++)
           {
                for(int j=0; j<gridSize; j++)
                {
                    //Console.Write("{0}|{1} ", solveGrid[i,j], compGrid[i,j]);

                    if (solveGrid[i,j] != compGrid[i,j])
                    {
                        return false;
                    }
                }

                //Console.WriteLine();
           }

           //Console.WriteLine();
            
           return true;
        }

        public void PrintGeneratedGrid()
        {
            Console.Clear();
            PrintGrid();
        }
    }
}
