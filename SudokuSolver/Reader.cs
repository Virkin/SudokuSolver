using System;
using System.Collections.Generic;
using System.IO;

namespace SudokuSolver
{
    class Reader
    {
        private List<string> listSudokuPath = new List<string>();
        private List<string> listSudokuName = new List<string>();
        private List<int[,]> listSudoku2d = new List<int[,]>();

        public Reader()
        {
            ListSudoku();
        }

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

        public List<string> getListSudokuPath()
        {
            return listSudokuPath;
        }

        public List<string> getListSudokuName()
        {
            return listSudokuName;
        }

        public string getSudokuPath(string path)
        {
            return listSudokuPath.Find(element => element.Contains(path));
        }

        public String getSudoku(string path)
        {
            string file = File.ReadAllText(path);
            return file;
        }

        public int getSize(string path)
        {
            string[] lines = File.ReadAllLines(path);
            return lines.Length;
        }

        public List<int[,]> getListSudoku()
        {
            return listSudoku2d;
        }

        public void fillSudoku2d()
        {
            foreach (string path in listSudokuPath)
            {
                int size = getSize(path);
                int[,] sudoku2d = new int[size, size];
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    for (int j = 0; j < lines[i].Length; j++)
                    {
                        char square = line[j];
                        sudoku2d[i, j] = square - '0';
                    }
                }
                listSudoku2d.Add(sudoku2d);
            }
        }

        public void Read()
        {
            fillSudoku2d();
        }
    }
}
