using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Solver
    {
        private int[,] gridProblem;
        private int[,] gridSolution;

        private List<int> values;
        private int gridSize;

        public Solver(int[,] grid)
        {
            gridProblem = grid;
            gridSize = (int) Math.Sqrt(gridProblem.Length);

            values = new List<int>();

            for(int i=1; i<=gridSize; i++){values.Add(i);}

            gridSolution = gridProblem;

            Backtracking();
            PrintGrid();

        }

        public bool AssigmentComplete()
        {
            for(int i=0; i<gridSize; i++)
            {
                for(int j=0; j<gridSize; j++)
                {
                    if(gridSolution[i,j] == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Dictionary<char,int> SelectUnassignedVariable()
        {
            Dictionary<char, int> variable = new Dictionary<char, int>();
            
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (gridSolution[i, j] == 0)
                    {
                        variable.Add('i', i);
                        variable.Add('j', j);
                        return variable;
                    }
                }
            }

            variable.Add('i', -1);
            variable.Add('j', -1);

            return variable;
        }

        public bool CheckConstraints(Dictionary<char, int> var, int val)
        {
            // Check vertical line
            for(int i=0; i<gridSize; i++)
            {
                if(gridSolution[i,var['j']] == val)
                {
                    return false;
                }
            }

            // Check horizontal line
            for (int j = 0; j < gridSize; j++)
            {
                if (gridSolution[var['i'], j] == val)
                {
                    return false;
                }
            }

            // Check box
            int boxSize = (int) Math.Sqrt(gridSize);
            int boxi = var['i'] - var['i'] % boxSize;
            int boxj = var['j'] - var['j'] % boxSize;

            for(int i=boxi;i<boxi+boxSize;i++)
            {
                for(int j=boxj; j<boxj+boxSize; j++)
                {
                    if(gridSolution[i,j] == val)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void Backtracking()
        {
            int result = RecursiveBacktracking();
        }

        public int RecursiveBacktracking()
        {
            if(AssigmentComplete() == true) { return 0; }

            Dictionary<char, int> var = SelectUnassignedVariable();

            foreach(int value in values)
            {
                if(CheckConstraints(var,value) == true)
                {
                    gridSolution[var['i'], var['j']] = value;

                    int result = RecursiveBacktracking();

                    if(result==0){ return result; }

                    gridSolution[var['i'], var['j']] = 0;
                }
            }

            return 1;
        }

        public void PrintGrid()
        {
            Console.Clear();

            for(int i=0; i<gridSize; i++)
            {
                for (int j=0; j < gridSize; j++)
                {
                    Console.Write(gridSolution[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}
