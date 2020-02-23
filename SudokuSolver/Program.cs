using System;
using System.Collections.Generic;
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
                Console.WriteLine("New Sudoku");
                solve = new Solver(sudoku);
                Console.WriteLine("Press any key to solve next sudoku");
                Console.ReadKey();
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }
    }
}
