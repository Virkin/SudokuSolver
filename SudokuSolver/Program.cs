using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            int nbSudoku = 0;
            Reader reader = new Reader();
            
            reader.Read();

            Solver solver;

            string generateConf = ConfigurationManager.AppSettings.Get("Generate");
            if (generateConf != "false")
            {
                Generator generate;
                generate = new Generator();
            }
            else
            {
                foreach (int[,] sudoku in reader.getListSudoku())
                {
                    string name = reader.getListSudokuName()[nbSudoku];
                    Console.Clear();
                
                    Console.WriteLine("\t\tSolving sudoku : {0}", name);
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    solver = new Solver(sudoku);
                    int res = solver.Run();

                    Console.WriteLine();

                    if (res==0)
                    {
                        Console.WriteLine("Solve !");
                    }
                    else
                    {
                        Console.WriteLine("Can't be solve ! Please check if your grid is correct.");
                    }

                    stopwatch.Stop();
                    nbSudoku++;
                    TimeSpan elapsed_time = stopwatch.Elapsed;
                    Console.WriteLine("Time to solve (hh:mm:ss) : {0}", elapsed_time);
                    Console.WriteLine("Press any key to solve next sudoku");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }
    }
}
