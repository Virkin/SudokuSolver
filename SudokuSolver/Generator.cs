using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;


namespace SudokuSolver
{
    class Generator : Sudoku
    {
       
        private Solver solver;

        private int nbRemoveValues;
        private int nbRemove = 0;

        private int[,] solveGrid;
        public Generator() : base()
        {
            solver = new Solver(grid);
            solver.Run();

            solveGrid = grid.Clone() as int[,];
            //PrintGrid();

            nbRemoveValues = 81;

            while (nbRemove < nbRemoveValues)
            {
                removeRandomValue();
            }

            PrintGeneratedGrid();
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

                Console.WriteLine("res : {0}", res);
                
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

           Console.WriteLine();
            
           return true;
        }

        public void PrintGeneratedGrid()
        {
            PrintGrid();
        }
    }
}
