using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Solver
    {
        private int[,] gridProblem;
        private int[,] gridSolution;

        private Dictionary<string, List<string>> constraints;

        private List<int> values;
        private int gridSize;

        private int nbStep = 0;

        public Solver(int[,] grid)
        {
            gridProblem = grid;
            gridSize = (int) Math.Sqrt(gridProblem.Length);

            values = new List<int>();

            for(int i=1; i<=gridSize; i++){values.Add(i);}

            gridSolution = gridProblem;

            constraints = new Dictionary<string, List<string>>();
            GenerateConstraints();

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

        public void GenerateConstraints()
        {
            for(int i = 0; i<gridSize; i++)
            {
                for(int j=0; j<gridSize; j++)
                {
                    List<string> constrainElem = new List<string>();
                    
                    // Check line
                    for (int k = 0; k < gridSize; k++)
                    {
                        if(k != i)
                        {
                            char[] coord = { (char)(k + 'A'), (char)(j + '0') };
                            constrainElem.Add( new string(coord) );
                        }
                        if(k != j)
                        {
                            char[] coord = { (char)(i + 'A'), (char)(k + '0') };
                            constrainElem.Add (new string(coord) );
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
                            if(k!=i || l!=j)
                            {
                                char[] coord = { (char)(k + 'A'), (char)(l + '0') };
                                constrainElem.Add(new string(coord));
                            }
                        }
                    }

                    char[] coordMain = { (char)(i + 'A'), (char)(j + '0') };
                    constraints.Add(new string(coordMain), constrainElem);
                }
            }
        }

        public bool CheckConstraints(Dictionary<char, int> var, int val)
        {
            char[] coord = { (char)(var['i'] + 'A'), (char)(var['j'] + '0') };

            string coordStr = new string(coord);

            List<string> constrainElem = constraints[coordStr];

            foreach(string elem in constrainElem)
            {
                int i = elem[0] - 'A';
                int j = elem[1] - '0';

                if(gridSolution[i,j] == val)
                {
                    return false;
                }
            }

            return true;
        }

        public Dictionary<char, int> DegreeHeuristic()
        {
            Dictionary<char, int> variable = new Dictionary<char, int>();

            int nbConstraints;
            int maxNbConstraints = -1;
            int vari = -1;
            int varj = -1;
            
            for(int i=0; i<gridSize; i++)
            {
                for(int j=0; j<gridSize; j++)
                {
                    if(gridSolution[i,j] == 0)
                    {
                        nbConstraints = 0;

                        char[] coord = { (char)(i + 'A'), (char)(j + '0') };

                        string coordStr = new string(coord);

                        List<string> constrainElem = constraints[coordStr];

                        foreach (string elem in constrainElem)
                        {
                            int elemi = elem[0] - 'A';
                            int elemj = elem[1] - '0';

                            if (gridSolution[elemi, elemj] == 0)
                            {
                                nbConstraints++;
                            }
                        }

                        if (nbConstraints > maxNbConstraints)
                        {
                            maxNbConstraints = nbConstraints;
                            vari = i;
                            varj = j;
                        }
                    }  
                }               
            }

            variable.Add('i', vari);
            variable.Add('j', varj);

            return variable;
        }

        public void Backtracking()
        {
            int result = RecursiveBacktracking();
        }

        public int RecursiveBacktracking()
        {
            if(AssigmentComplete() == true) { return 0; }

            Dictionary<char, int> var = SelectUnassignedVariable();
            //Dictionary<char, int> var = DegreeHeuristic();

            foreach (int value in values)
            {
                if(CheckConstraints(var,value) == true)
                {
                    gridSolution[var['i'], var['j']] = value;

                    //PrintGrid();
                    nbStep++;

                    int result = RecursiveBacktracking();

                    if(result==0){ return result; }

                    gridSolution[var['i'], var['j']] = 0;
                }
            }

            return 1;
        }

        public void PrintGrid()
        {
            Console.SetCursorPosition(0, 0);

            for(int i=0; i<gridSize; i++)
            {
                if (i % Math.Sqrt(gridSize) == 0 && i != 0)
                {
                    for (int k = 0; k < gridSize + 2; k++)
                    {
                        Console.Write('-');
                    }

                    Console.WriteLine();
                }

                for (int j=0; j < gridSize; j++)
                {
                    if (j % Math.Sqrt(gridSize) == 0 && j!=0)
                    {
                        Console.Write('|');
                    }

                    if (gridSolution[i,j] > 0)
                    {
                        Console.Write(gridSolution[i, j]);
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }

                Console.WriteLine();

            }

            Console.WriteLine();
            Console.WriteLine("Num of steps : {0}", nbStep);
        }
    }
}
