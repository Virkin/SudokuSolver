using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Sudoku
    {
        protected int[,] grid;
        
        protected List<int> valuesEnum;
        protected int gridSize = 9;

        protected Dictionary<string, List<string>> constraints;

        public Sudoku() 
        {
            grid = new int[gridSize,gridSize];
            Init();
        }

        public Sudoku(int[,] newGrid)
        {
            grid = newGrid;
            Init();
        }

        // Initialisation of a sudoku grid
        public void Init()
        {
            gridSize = (int)Math.Sqrt(grid.Length);

            valuesEnum = new List<int>();

            for (int i = 1; i <= gridSize; i++) { valuesEnum.Add(i); }

            valuesEnum = valuesEnum.OrderBy(x => Guid.NewGuid()).ToList();

            constraints = new Dictionary<string, List<string>>();

            GenerateConstraints();
        }

        public void NewGrid(int[,] newGrid)
        {
            grid = newGrid;
            Init();
        }

        // Genrate constraints for a sukodu problem
        public void GenerateConstraints()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    List<string> constrainElem = new List<string>();

                    // Check line
                    for (int k = 0; k < gridSize; k++)
                    {
                        if (k != i)
                        {
                            constrainElem.Add(IjToCoord(k, j));
                        }
                        if (k != j)
                        {
                            constrainElem.Add(IjToCoord(i, k));
                        }
                    }

                    // Check box
                    int boxSize = (int)Math.Sqrt(gridSize);
                    int boxi = i - i % boxSize;
                    int boxj = j - j % boxSize;

                    for (int k = boxi; k < boxi + boxSize; k++)
                    {
                        for (int l = boxj; l < boxj + boxSize; l++)
                        {
                            if (k != i || l != j)
                            {
                                constrainElem.Add(IjToCoord(k, l));
                            }
                        }
                    }

                    constraints.Add(IjToCoord(i, j), constrainElem);
                }
            }
        }

        // Convert i,j to alphanumeric coordinatse (ex: 0,0 -> A0) 
        public string IjToCoord(int i, int j)
        {
            char[] coord = { (char)(i + 'A'), (char)(j + '0') };

            return new string(coord);
        }

        // Convert alphanumeric to i,j coordinatse (ex: A0 -> 0,0) 
        public List<int> CoordToIj(string coord)
        {
            List<int> ij = new List<int>();

            ij.Add(coord[0] - 'A');
            ij.Add(coord[1] - '0');

            return ij;
        }

        // Print the grid
        public void PrintGrid()
        {
            for (int i = 0; i < gridSize; i++)
            {
                if (i % Math.Sqrt(gridSize) == 0 && i != 0)
                {
                    for (int k = 0; k < gridSize + 2; k++)
                    {
                        Console.Write('-');
                    }

                    Console.WriteLine();
                }

                for (int j = 0; j < gridSize; j++)
                {
                    if (j % Math.Sqrt(gridSize) == 0 && j != 0)
                    {
                        Console.Write('|');
                    }

                    if (grid[i, j] > 0)
                    {
                        Console.Write(grid[i, j]);
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }

                Console.WriteLine();

            }

            Console.WriteLine();
        }

        // Return the grid
        public int [,] GetGrid()
        {
            return grid;
        }
    }
}
