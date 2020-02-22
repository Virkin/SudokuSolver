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
            Reader file = new Reader();
            
            file.Read();

            Solver solve = new Solver(file.GetGrid());

        }
    }
}
