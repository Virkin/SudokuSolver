using System;
using System.Collections.Generic;
using System.IO;

namespace SudokuSolver
{
    class Reader
    {
        private List<string> listSudokuPath = new List<string>();
        private List<string> listSudokuName = new List<string>();
        // List of sudoku grid
        private List<int[,]> listSudoku2d = new List<int[,]>();

        public Reader()
        {
            ListSudoku();
        }

        // Get path and name of all the sudoku .txt files in the sudoku folder
        public void ListSudoku()
        {
            string sudokuDirectory = Environment.CurrentDirectory + "/Sudoku";
            string[] fileEntries = Directory.GetFiles(sudokuDirectory, "*.txt");
            
            foreach (string filePath in fileEntries)
            {
                listSudokuPath.Add(filePath);
                listSudokuName.Add(Path.GetFileName(filePath));
            }
        }

        public List<string> getListSudokuName()
        {
            return listSudokuName;
        }

        // Get the size of a sudoku grid
        public int getSize(string path)
        {
            string[] lines = File.ReadAllLines(path);
            return lines.Length;
        }

        public List<int[,]> getListSudoku()
        {
            return listSudoku2d;
        }

        // Read the all the sudoku grids from files and put them in the list of matrix listSudoku2d
        public void fillSudoku2d()
        {
            foreach (string path in listSudokuPath)
            {
                int size = getSize(path);
                // create a sudoku matrix to store the sudoku
                int[,] sudoku2d = new int[size, size];
                // read each line of the sudoku in the file
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    // get each line
                    string line = lines[i];
                    for (int j = 0; j < lines[i].Length; j++)
                    {
                        // get each characters of the line
                        char square = line[j];
                        // convert char to int and store in the matrix
                        sudoku2d[i, j] = square - '0';
                    }
                }
                // add the matrix to the list of sudoku
                listSudoku2d.Add(sudoku2d);
            }
        }

        public void Read()
        {
            fillSudoku2d();
        }
    }
}
