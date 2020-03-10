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

            string pathSudokuFolder = Environment.CurrentDirectory + "/Sudoku/";

            DirectoryInfo di  = new DirectoryInfo(pathSudokuFolder);

            if (!di.Exists)
            { 
                di.Create();
            }
            
            while (true)
            {
                Reader reader = new Reader();
                reader.Read();

                question = "What would you like to do ?";
                menu = new List<string>(new string[] { "Solve", "Generate", "Import", "Remove", "Quit"});

                selected = GenerateMenu(question, menu);

                switch (selected)
                {
                    case 0:

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("--< Solver >--");
                        Console.WriteLine();
                        Console.ResetColor();

                        menu = reader.getListSudokuName();

                        if(menu.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("The sudoku folder is empty ! Please generate or import one grid.");
                            Console.ResetColor();
                            Console.WriteLine("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        question = "Which sudoku do you want to solve ?";

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

                        solver = new Solver(sudoku);

                        solver.PrintGrid();

                        question = "Do you want to start the solver for this grid and with parameters configure just before?";
                        menu = new List<string>(new string[] { "Yes", "No" });

                        selected = GenerateMenu(question, menu);

                        if (selected == 1)
                        {
                            break;
                        }

                        Console.Clear();

                        Console.WriteLine("\t\tSolving sudoku : {0}", name);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

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

                        solver.PrintGridEvolution();

                        Console.WriteLine("Time to solve (hh:mm:ss) : {0}", elapsed_time);
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();

                        break;
                    
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("--< Generator >--");
                        Console.WriteLine();
                        Console.ResetColor();

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

                        Console.Write("Choose a name for the sudoku : ");
                        string sudokuName = Console.ReadLine();

                        Console.Clear();

                        Generator generate;
                        generate = new Generator(sudokuName);

                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();

                        break;

                    case 2:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("--< Import >--");
                        Console.WriteLine();
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.DarkYellow;

                        Console.WriteLine("To import a sudoku, please indicate the path of the file");
                        Console.WriteLine("The grid has to be a matrix of 9x9 size which values correspond to the number of the sudoku and empty box are equals to 0");
                        Console.WriteLine("The file has to be in .txt format");
                        Console.WriteLine();

                        Console.ResetColor();

                        Console.WriteLine("Example :");
                        Console.WriteLine(  "040000179\n" +
                                            "002008054\n" +
                                            "006005008\n" +
                                            "080070910\n" +
                                            "050090030\n" +
                                            "019060040\n" +
                                            "300400700\n" +
                                            "570100200\n" +
                                            "928000060\n");

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("Path : ");
                        Console.ResetColor();
                        string path = Console.ReadLine();

                        if(File.Exists(path))
                        {
                            string filename = Path.GetFileName(path);
                            string ext = filename.Split('.')[1] ;
                            if (ext == "txt")
                            {
                                File.Copy(path, pathSudokuFolder + filename);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Wrong file extension ! Please provide a .txt file");
                                Console.ResetColor();
                                Console.WriteLine("Press any key to continue");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("The path is Wrong ! Couldn't find the file");
                            Console.ResetColor();
                            Console.WriteLine("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        break;

                    case 3:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("--< Remove >--");
                        Console.WriteLine();
                        Console.ResetColor();

                        menu = reader.getListSudokuName();

                        if (menu.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("The sudoku folder is empty !");
                            Console.ResetColor();
                            Console.WriteLine("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        question = "Which sudoku do you want to remove ?";

                        selectedSudoku = GenerateMenu(question, menu);

                        string gridRemoveName = reader.getListSudokuName()[selectedSudoku];

                        File.Delete(pathSudokuFolder + gridRemoveName);

                        break;

                    case 4:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("--< Quit >--");
                        Console.WriteLine();
                        Console.ResetColor();

                        question = "Do you want to clean the sudoku folder ?";
                        menu = new List<string>(new string[] { "No", "Yes" });

                        selected = GenerateMenu(question, menu);

                        if (selected == 1)
                        {
                            di.Delete(true);
                        }
                       
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

            Console.WriteLine();

            return selected;
        }
    }
}
