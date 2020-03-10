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
            Solver solver;

            string question;
            List<string> menu;
            int selected;
            int selectedSudoku;
            int[,] sudoku;

            while (true)
            {
                Reader reader = new Reader();
                reader.Read();

                question = "What would you like to do ?";
                menu = new List<string>(new string[] { "Solve", "Generate", "Quit"});

                selected = GenerateMenu(question, menu);

                switch (selected)
                {
                    case 0:

                        question = "Which sudoku do you want to solve ?";
                        menu = reader.getListSudokuName();

                        selectedSudoku = GenerateMenu(question, menu);

                        sudoku = reader.getListSudoku()[selectedSudoku];

                        question = "Do you want to display the resolution animation ?";
                        menu = new List<string>(new string[] { "Yes", "No" });

                        selected = GenerateMenu(question, menu);

                        if (selected == 0)
                        {
                            ConfigurationManager.AppSettings.Set("PrintGrid", "true");
                        }
                        else
                        {
                            ConfigurationManager.AppSettings.Set("PrintGrid", "false");
                        }

                        question = "Do you want to use AC-3 ?";
                        menu = new List<string>(new string[] { "Yes", "No" });

                        selected = GenerateMenu(question, menu);

                        if (selected == 0)
                        {
                            ConfigurationManager.AppSettings.Set("AC3", "true");
                        }
                        else
                        {
                            ConfigurationManager.AppSettings.Set("AC3", "false");
                        }

                        question = "Which algorithm do you want to use to decide which box is the best to fill ?";
                        menu = new List<string>(new string[] { "MRV", "Degree Heuristics", "Random" });

                        selected = GenerateMenu(question, menu);

                        if (selected == 0)
                        {
                            ConfigurationManager.AppSettings.Set("VarChoose", "MRV");
                        }
                        else if (selected == 1)
                        {
                            ConfigurationManager.AppSettings.Set("VarChoose", "DH");
                        }
                        else
                        {
                            ConfigurationManager.AppSettings.Set("VarChoose", "null");
                        }

                        question = "Which algorithm do you want to use to decide which number is the best to write ? (when multiple number are possible for a box)";
                        menu = new List<string>(new string[] { "LCV", "Random" });

                        selected = GenerateMenu(question, menu);

                        if (selected == 0)
                        {
                            ConfigurationManager.AppSettings.Set("LCV", "true");
                        }
                        else
                        {
                            ConfigurationManager.AppSettings.Set("LCV", "false");
                        }

                        string name = reader.getListSudokuName()[selectedSudoku];
                        
                        Console.Clear();

                        Console.WriteLine("\t\tSolving sudoku : {0}", name);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        solver = new Solver(sudoku);
                        int res = solver.Run();

                        Console.WriteLine();

                        if (res == 0)
                        {
                            Console.WriteLine("Solve !");
                        }
                        else
                        {
                            Console.WriteLine("Can't be solve ! Please check if your grid is correct.");
                        }

                        stopwatch.Stop();
                        TimeSpan elapsed_time = stopwatch.Elapsed;
                        Console.WriteLine("Time to solve (hh:mm:ss) : {0}", elapsed_time);
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();

                        break;
                    
                    case 1:
                        question = "Do you want to display the resolution animation ?";
                        menu = new List<string>(new string[] { "Yes", "No" });

                        selected = GenerateMenu(question, menu);

                        if (selected == 0)
                        {
                            ConfigurationManager.AppSettings.Set("PrintGrid", "true");
                        }
                        else
                        {
                            ConfigurationManager.AppSettings.Set("PrintGrid", "false");
                        }

                        question = "Choose a difficulty for the generate sudoku :";
                        menu = new List<string>(new string[] { "Easy", "Medium", "Hard" });

                        selected = GenerateMenu(question, menu);

                        if (selected == 0)
                        {
                            ConfigurationManager.AppSettings.Set("Generate", "easy");
                        }
                        else if(selected == 1)
                        {
                            ConfigurationManager.AppSettings.Set("Generate", "medium");
                        }
                        else
                        {
                            ConfigurationManager.AppSettings.Set("Generate", "hard");
                        }

                        Console.WriteLine();
                        Console.Write("Choose a name for the sudoku : ");
                        string sudokuName = Console.ReadLine();

                        Console.Clear();

                        Generator generate;
                        generate = new Generator(sudokuName);

                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();

                        break;

                    case 2:
                        System.Environment.Exit(1);
                        break;
                }

                Console.Clear();
            }
        }

        static int GenerateMenu(string question, List<string> menu)
        {
            int optionsCount = menu.Count;

            int selected = 0;

            bool done = false;

            Console.WriteLine();
            Console.WriteLine(question);

            while (!done)
            {
                for (int i = 0; i < optionsCount; i++)
                {
                    if (selected == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("> ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }

                    Console.WriteLine(menu[i]);

                    Console.ResetColor();
                }

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        selected = Math.Max(0, selected - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selected = Math.Min(optionsCount - 1, selected + 1);
                        break;
                    case ConsoleKey.Enter:
                        done = true;
                        break;
                }

                if (!done)
                    Console.CursorTop = Console.CursorTop - optionsCount;
            }

            return selected;
        }
    }
}
