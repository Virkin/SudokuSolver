using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Reader reader = new Reader();
            
            reader.Read();

            Solver solve;

            foreach (int[,] sudoku in reader.getListSudoku())
            {
                Console.Clear();
                
                Console.WriteLine("New Sudoku");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                solve = new Solver(sudoku);

                stopwatch.Stop();
                TimeSpan elapsed_time = stopwatch.Elapsed;
                Console.WriteLine("Time to solve (hh:mm:ss) : {0}", elapsed_time);
                Console.WriteLine("Press any key to solve next sudoku");
                Console.ReadKey();
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }
    }
}
