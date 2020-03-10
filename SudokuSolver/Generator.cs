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

        // To generate a random grid, we solve an empty grid with the solver to get a random solution
        // Then, we remove a certain number of value (depending of the difficulty)
        // Each time we remove a variable, we test if the new grid still got one unique solution 

        public Generator(string name) : base()
        {
            sudokuName = name;

            Run();
        }

        public void Run()
        {
            solver = new Solver(grid);
            solver.Run();

            solveGrid = grid.Clone() as int[,];

            string generateConf = ConfigurationManager.AppSettings.Get("Generate");
            if (generateConf == "easy")
                nbRemoveValues = 81 - 45;
            else if (generateConf == "medium")
                nbRemoveValues = 81 - 35;
            else if (generateConf == "hard")
                nbRemoveValues = 81 - 25;

            Console.WriteLine("Generating sudoku ({0}) ...", generateConf);
            while (nbRemove < nbRemoveValues)
            {
                removeRandomValue();
            }

            PrintGeneratedGrid();
            WriteSudokuToFile();
        }

        public void WriteSudokuToFile()
        {
            string time = DateTime.Now.ToString("yyyyMMdd_HHmmss"); 
            string generateConf = ConfigurationManager.AppSettings.Get("Generate");
            string pathSudokuFolder = Environment.CurrentDirectory + "/Sudoku";
            string path = pathSudokuFolder + "\\" + sudokuName + ".txt";

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

            if(grid[i,j] > 0)
            {
                int oldValue = grid[i, j];

                grid[i, j] = 0;

                solver.NewGrid(grid.Clone() as int[,]);

                int res = solver.Run();

                if (res==0 && CompareSolveGrid(solver.GetGrid()))
                {
                    nbRemove++;
                    return;
                }

                grid[i, j] = oldValue;
            }
        }

        public bool CompareSolveGrid(int [,] compGrid)
        {
           for(int i=0; i<gridSize; i++)
           {
                for(int j=0; j<gridSize; j++)
                {
                    if (solveGrid[i,j] != compGrid[i,j])
                    {
                        return false;
                    }
                }
           }
           return true;
        }

        public void PrintGeneratedGrid()
        {
            Console.Clear();
            PrintGrid();
        }
    }
}
