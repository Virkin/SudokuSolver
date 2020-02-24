using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections;


namespace SudokuSolver
{
    class Solver
    {
        private int[,] gridProblem;
        private int[,] gridSolution;

        private Dictionary<string, List<int>> domains;
        private Dictionary<string, List<string>> constraints;

        private List<int> valuesEnum;
        private int gridSize;

        private int nbStep = 0;

        public Solver(int[,] grid)
        {
            gridProblem = grid;
            gridSize = (int) Math.Sqrt(gridProblem.Length);

            valuesEnum = new List<int>();

            for(int i=1; i<=gridSize; i++){valuesEnum.Add(i);}

            gridSolution = gridProblem;

            domains = new Dictionary<string, List<int>>();
            constraints = new Dictionary<string, List<string>>();

            GenerateDomains();
            GenerateConstraints();

            AC3();
            
            Backtracking();
            
            PrintGrid();

            /*foreach(KeyValuePair<string, List<int>> item in domains)
            {
                Console.Write("({0}) : [", item.Key);
                foreach(int val in item.Value)
                {
                    Console.Write("{0} ,", val);
                }
                Console.Write("]");
                Console.WriteLine();
            }*/

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

        public string SelectUnassignedVariable()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (gridSolution[i, j] == 0)
                    {
                        return IjToCoord(i, j);
                    }
                }
            }

            return null;
        }

        public void GenerateDomains()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if(gridProblem[i,j] == 0)
                    {
                        domains.Add(IjToCoord(i, j), new List<int>(valuesEnum));
                    }
                    else
                    {
                        domains.Add(IjToCoord(i, j), new List<int> { gridProblem[i, j] });
                    }
                }
            }
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
                            constrainElem.Add(IjToCoord(k, j));
                        }
                        if(k != j)
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
                            if(k!=i || l!=j)
                            {
                                constrainElem.Add(IjToCoord(k, l));
                            }
                        }
                    }

                    constraints.Add(IjToCoord(i, j), constrainElem);
                }
            }
        }

        public bool CheckConstraints(string var, int val)
        {
            List<string> constrainElem = constraints[var];

            foreach(string elem in constrainElem)
            {
                List<int> coordIj = CoordToIj(elem);

                if (gridSolution[coordIj[0],coordIj[1]] == val)
                {
                    return false;
                }
            }

            return true;
        }

        public string MRV()
        {
            int minValues = gridSize+1;

            List<int> possibleValues;

            int vari = 0;
            int varj = 0;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (gridSolution[i, j] == 0)
                    {
                        possibleValues = new List<int>(valuesEnum);

                        string coordStr = IjToCoord(i, j);

                        List<string> constrainElem = constraints[coordStr];

                        foreach (string elem in constrainElem)
                        {
                            List<int> coordIj = CoordToIj(elem);

                            if (possibleValues.Contains(gridSolution[coordIj[0], coordIj[1]]) == true)
                            {
                                possibleValues.Remove(gridSolution[coordIj[0], coordIj[1]]);
                            }
                        }

                        if (possibleValues.Count < minValues)
                        {
                            minValues = possibleValues.Count;
                            vari = i;
                            varj = j;
                        }
                    }
                }
            }

            return IjToCoord(vari, varj);
        }

        public List<int> GetPossibleValues(string var)
        {
            List<int> possibleValues = new List<int>();

            foreach(int value in valuesEnum)
            {
                if (CheckConstraints(var, value) == true)
                {
                    possibleValues.Add(value);
                }
            }

            return possibleValues;
        }

        public List<int> LCV(string var)
        {
            List<int> varValues = new List<int>(domains[var]);

            List<string> constrainElem = constraints[var];

            Dictionary<int, int> reduceScoreRegister = new Dictionary<int, int>();

            List<int> constrainValues;

            int reduceScore;

            foreach(int value in varValues)
            {
                reduceScore = 0;

                foreach (string elem in constrainElem)
                {
                    constrainValues = GetPossibleValues(elem);

                    foreach(int cvalue in constrainValues)
                    { 
                        if(value==cvalue)
                        {
                            reduceScore++;
                            break;
                        }
                    }
                }

                reduceScoreRegister.Add(value, reduceScore);
            }

            List<int> orderedValues = new List<int>();

            foreach (KeyValuePair<int, int> item in reduceScoreRegister.OrderBy(key => key.Value))
            {
                orderedValues.Add(item.Key);
            }

            return orderedValues;
        }

        public string DegreeHeuristic()
        {
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

                        string coordStr = IjToCoord(i,j);

                        List<string> constrainElem = constraints[coordStr];

                        foreach (string elem in constrainElem)
                        {
                            List<int> coordIj = CoordToIj(elem);

                            if (gridSolution[coordIj[0], coordIj[1]] == 0)
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

            return IjToCoord(vari, varj);
        }

        public Queue<(string, string)> GenerateArcs()
        {
            Queue<(string, string)> arcs = new Queue<(string, string)>();

            foreach (KeyValuePair<string, List<string>> item in constraints)
            {
                string var = item.Key;

                foreach(string cvar in item.Value)
                {
                    arcs.Enqueue((var, cvar));
                }
            }

            return arcs;
        }

        public void AC3()
        {
            Queue<(string, string)> arcsQueue = GenerateArcs();

            while(arcsQueue.Count > 0)
            {
                (string, string) arc = arcsQueue.Dequeue();
                string xi = arc.Item1;
                string xj = arc.Item2;

                if(RemoveInconsistentValues(xi,xj))
                {
                    foreach(string xk in constraints[xi])
                    {
                        arcsQueue.Enqueue((xk, xi));
                    }
                }
            }
        }

        public bool RemoveInconsistentValues(string xi, string xj)
        {
            bool removed = false;

            foreach(int x in domains[xi].ToList())
            {
                if(CheckInconsistent(x,domains[xj]))
                {
                    domains[xi].Remove(x);
                    removed = true;
                }
            }

            return removed;
        }

        public bool CheckInconsistent(int x, List<int> ys)
        {
            foreach(int y in ys)
            {
                if(y!=x)
                {
                    return false;
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

            //Dictionary<string, List<int>> oldDomains = new Dictionary<string, List<int>>(domains);

            string var = SelectUnassignedVariable();
            //string var = MRV();
            //string var = DegreeHeuristic();

            List<int> values = new List<int>(domains[var]);
            //List<int> values = LCV(var);

            foreach (int value in values)
            {
                if (CheckConstraints(var, value) == true)
                {
                    List<int> coordIj = CoordToIj(var);

                    int i = coordIj[0];
                    int j = coordIj[1];

                    gridSolution[i, j] = value;

                    /*List<int> domain = new List<int>(domains[var]);

                    foreach(int val in domain)
                    {
                        if(val!=value)
                        {
                            domains[var].Remove(val);
                        }
                    }

                    AC3();*/

                    string printGridConf = ConfigurationManager.AppSettings.Get("PrintGrid");
                    if (printGridConf == "true")
                        PrintGrid();

                    //Console.WriteLine("var:{0} | value:{1}", var, value);

                    //Thread.Sleep(20000);

                    nbStep++;

                    int result = RecursiveBacktracking();

                    if (result == 0) { return result; }

                    gridSolution[i, j] = 0;

                    /*domains = new Dictionary<string, List<int>>(oldDomains);

                    AC3();*/
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

        public string IjToCoord(int i, int j)
        {
            char[] coord = { (char)(i + 'A'), (char)(j + '0') };

            return new string(coord);
        }

        public List<int> CoordToIj(string coord)
        {
            List<int> ij = new List<int>();
            
            ij.Add(coord[0] - 'A');
            ij.Add(coord[1] - '0');

            return ij;
        }
    }
}
