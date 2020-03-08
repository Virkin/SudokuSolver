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
    class Solver : Sudoku
    {
        private Dictionary<string, List<int>> domains;

        private int nbStep = 0;

        public Solver(int[,] newGrid) : base(newGrid){}

        public int Run()
        {
            domains = new Dictionary<string, List<int>>();

            GenerateDomains();

            string ac3Conf = ConfigurationManager.AppSettings.Get("AC3");
            if (ac3Conf == "true")
                AC3();

            int res = Backtracking();

            //PrintGridEvolution();

            return res;
        }

        public bool AssigmentComplete()
        {
            for(int i=0; i<gridSize; i++)
            {
                for(int j=0; j<gridSize; j++)
                {
                    if(grid[i,j] == 0)
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
                    if (grid[i, j] == 0)
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
                    if(grid[i,j] == 0)
                    {
                        domains.Add(IjToCoord(i, j), new List<int>(valuesEnum));
                    }
                    else
                    {
                        domains.Add(IjToCoord(i, j), new List<int> { grid[i, j] });
                    }
                }
            }
        }

        public bool CheckConstraints(string var, int val)
        {
            List<string> constrainElem = constraints[var];

            foreach(string elem in constrainElem)
            {
                List<int> coordIj = CoordToIj(elem);

                if (grid[coordIj[0],coordIj[1]] == val)
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
                    if (grid[i, j] == 0)
                    {
                        possibleValues = new List<int>(valuesEnum);

                        string coordStr = IjToCoord(i, j);

                        List<string> constrainElem = constraints[coordStr];

                        foreach (string elem in constrainElem)
                        {
                            List<int> coordIj = CoordToIj(elem);

                            if (possibleValues.Contains(grid[coordIj[0], coordIj[1]]) == true)
                            {
                                possibleValues.Remove(grid[coordIj[0], coordIj[1]]);
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
                    if(grid[i,j] == 0)
                    {
                        nbConstraints = 0;

                        string coordStr = IjToCoord(i,j);

                        List<string> constrainElem = constraints[coordStr];

                        foreach (string elem in constrainElem)
                        {
                            List<int> coordIj = CoordToIj(elem);

                            if (grid[coordIj[0], coordIj[1]] == 0)
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

        public Dictionary<string, List<int>> CopyDomains(Dictionary<string, List<int>> oldDomains)
        {
            Dictionary<string, List<int>> newDomains = new Dictionary<string, List<int>>();

            foreach (KeyValuePair<string, List<int>> item in oldDomains)
            {
                List<int> values = new List<int>(item.Value);
                newDomains.Add(item.Key,values);
            }

            return newDomains;
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

        public int Backtracking()
        {
            return RecursiveBacktracking();
        }

        public int RecursiveBacktracking()
        {
            if(AssigmentComplete() == true) { return 0; }

            Dictionary<string, List<int>> oldDomains = CopyDomains(domains);
            string var = null;
            List<int> values = null;

            string unassignedVariableConf = ConfigurationManager.AppSettings.Get("UnassignedVariable");
            if (unassignedVariableConf == "true")
            {
                var = SelectUnassignedVariable();
            }

            string MRVConf = ConfigurationManager.AppSettings.Get("MRV");
            if (MRVConf == "true")
            {
                var = MRV();
            }

            string degreeHeuristicConf = ConfigurationManager.AppSettings.Get("DH");
            if (degreeHeuristicConf == "true")
            {
                var = DegreeHeuristic();
            }

            string lcvConf = ConfigurationManager.AppSettings.Get("LCV");
            if (lcvConf == "true")
            {
                values = LCV(var);
            }
            else
            {
                values = new List<int>(domains[var]);
            }
            
            string ac3Conf = ConfigurationManager.AppSettings.Get("AC3");

            foreach (int value in values)
            {
                if (CheckConstraints(var, value) == true)
                {
                    List<int> coordIj = CoordToIj(var);

                    int i = coordIj[0];
                    int j = coordIj[1];

                    grid[i, j] = value;

                    List<int> domain = new List<int>(domains[var]);

                    foreach(int val in domain)
                    {
                        if(val!=value)
                        {
                            domains[var].Remove(val);
                        }
                    }

                    if (ac3Conf == "true")
                        AC3();

                    string printGridConf = ConfigurationManager.AppSettings.Get("PrintGrid");
                    if (printGridConf == "true")
                        PrintGridEvolution();

                    nbStep++;

                    int result = RecursiveBacktracking();

                    if (result == 0) { return result; }

                    grid[i, j] = 0;

                    domains = CopyDomains(oldDomains);

                    if (ac3Conf == "true")
                        AC3();
                }
            }

            return 1;
        }

        public void PrintGridEvolution()
        {
            Console.SetCursorPosition(0, 0);

            PrintGrid();

            Console.WriteLine();
            Console.WriteLine("Num of steps : {0}", nbStep);

            //Thread.Sleep(100);
        }

       
    }
}
