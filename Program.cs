using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//AUTHOR : SHENI HAMITAJ

namespace sudoku
{
    class Program
    {
        static int breakLoop = 0;
        static int[,] grid = new int [9,9];
        static int[] FirstPossible = new int[9];
        static int[] SecondPossible = new int[9];
        static int[] ThirdPossible = new int[9];
        static int[,,] solution = new int[9,9,9];
        static string logFile = "";

        static void logCell(int x, int y, int value, int mode)
        {
            logFile = logFile + "(" + x + ", " + y + ")" + " --> " + value + " ";
            if (mode == 1)
                logFile = logFile + "Choice Selection\n";
            else if (mode == 0)
                logFile = logFile + "Cell Selection\n";
        }

        public static void Open()
        {
            for(int x=0;x<9;x++)
            {
                for(int y=0;y<9;y++)
                {
                    for (int z = 0; z < 10; z++)
                    {
                        grid[x, y] = 0;
                    }
                }
            }

            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "Sudoku  (*.txt)|*.txt";
            ofd.Multiselect = false;
            ofd.Title = "Open sudoku file";

            System.Windows.Forms.DialogResult r = ofd.ShowDialog();

            try
            {
                if (r == System.Windows.Forms.DialogResult.OK)
                {
                    string txt = System.IO.File.ReadAllText(ofd.FileName);
                    int x = 0;
                    int y = 0;

                    for (int i = 0; i < txt.Length; i++)
                    {
                        string ss = txt[i].ToString();

                        if (ss.Equals('*'))
                        {
                            y++;
                            continue;
                        }

                        if (y == 9)
                        {
                            i++;
                            x++;
                            y = 0;
                            if (x == 9)
                                break;
                            continue;
                        }

                        int number = 0;
                        int.TryParse(ss, out number);

                        grid[x, y] = number;
                        y++;
                    }
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error opening sudoku file!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            Console.WriteLine("Opened file:\n");
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write("{0} ", grid[i, j]);
                }
                Console.Write("\n");
            }


        }

        static int FirstThread(int x)
        {
            //check for line possibilities
            int i = 1;
            int savecoord = 0;
            int flag = 0;
            for (int q = 0; q < 9; q++)
                FirstPossible[q] = 0;

            for (int y = 0; y < 9; y++)
            {
                for (int z = 1; z < 10; z++)
                {
                    
                    if (grid[x, y] != 0)
                        break;
                    savecoord = y;

                    for (int f = 0; f < 9; f++)
                    {
                        if (grid[x, f] == z)
                            flag = 1;
                    }

                    if (flag == 0)
                    {
                        FirstPossible[i-1] = z;
                        i++;
                    }

                    flag = 0;
                }
                i = 1;
            }
            //check for line completition
            flag = 0;
            int ind = 0;
            for(i=0;i<9;i++)
            {
                if(FirstPossible[i]!=0)
                {
                    flag++;
                    ind = i;
                }
            }
            if (flag == 1)
            {
                grid[x, savecoord] = FirstPossible[ind];
                logCell(x, savecoord, grid[x, savecoord], 0);
                FirstPossible[ind] = 0;
                return 1;
            }
            return 0;
        }

        static int SecondThread(int y)
        {
            //check for coloumn possibilities
            int savecoord = 0;
            int i = 1;
            int flag = 0;
            for (int q = 0; q < 9; q++)
                SecondPossible[q] = 0;

            for (int x = 0; x < 9; x++)
            {
                for (int z = 1; z < 10; z++)
                {

                    if (grid[x, y] != 0)
                        break;
                    savecoord = x;

                    for (int f = 1; f < 10; f++)
                    {
                        if (grid[x, y] == z)
                        {
                            flag = 1;
                        }
                    }

                    for (int f = 0; f < 9; f++)
                    {
                        if (grid[f, y] == z)
                        {
                            flag = 1;
                        }
                            

                    }

                    if (flag == 0)
                    {
                        SecondPossible[i-1] = z;
                        i++;
                    }

                    flag = 0;
                }
                i = 1;
            }
            //check for coloumn completition
            flag = 0;
            int ind = 0;
            for (i = 0; i < 9; i++)
            {
                if (SecondPossible[i] != 0)
                {
                    flag++;
                    ind = i;
                }
            }
            if (flag == 1)
            {
                grid[savecoord, y] = SecondPossible[ind];
                logCell(savecoord, y, grid[savecoord, y], 0);
                SecondPossible[ind] = 0;
                return 1;
            }
            return 0;

        }

        static int ThirdThread(int x, int y)
        {
            int flag = 0;
            int[] square = new int[9];
            int startX = (x - (x % 3));
            int startY = (y - (y % 3));

            //check for square possibilities
            for (int q = 0; q < 9; q++)
            {
                ThirdPossible[q] = 0;
                square[q] = 0;
            }

            int s = 0;
            for (int i=startX;i<(startX+3);i++)
            {
                for(int j=startY;j<(startY+3);j++,s++)
                {
                    square[s] = grid[i, j];
                }
            }
            s = 0;

            for(int i=1;i<10;i++)
            {
                flag = 0;
                for (int j = 1; j < 10; j++)
                {
                    if (i == square[j-1])
                        flag = 1;

                }
                if(flag==0)
                {
                    ThirdPossible[s] = i;
                    s++;
                }  
            }
            //check for square completition
            if (s == 1)
            {
                grid[x, y] = ThirdPossible[s];
                logCell(x, y, grid[x, y], 0);
                ThirdPossible[s] = 0;
                return 1;
            }
            return 0;
        }

        static void FourthThread(int x, int y)
        {
            if(grid[x,y]==0)
            {
                //sum up the solutions step1
                int flag = 0;
                int sCount = 0;

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (FirstPossible[i] == SecondPossible[j])
                        {
                            for (int k = 0; k < 9; k++)
                            {
                                if (FirstPossible[i] == ThirdPossible[k] && FirstPossible[i] != 0)
                                {
                                    flag = 1;
                                    solution[x, y, sCount] = FirstPossible[i];
                                    sCount++;
                                }
                            }
                        }
                    }
                }
            }
            
        }

        static void RowColoumnUpdate()
        {
            //update solutions, Method: Double Check
            for(int i=0;i<9;i++)
            {
                for(int j=0;j<9;j++)
                {
                    if(grid[i,j]!=0)
                    {
                        for(int k=0;k<9;k++)
                        {
                            for(int z=0;z<9;z++)
                            {
                                //row check
                                if (grid[i, j] == solution[i, k, z])
                                    solution[i, k, z] = 0;
                                    
                                //coloumn check
                                if (grid[i, j] == solution[k, j, z])
                                    solution[k, j, z] = 0;
                            }
                        }
                    }
                }
            }
        }

        static void squareUpdate()
        {
            //update solutions, Method: TripleCheck
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++) //loop through every cell
                {
                    int startX = (i - (i % 3));
                    int startY = (j - (j % 3));
                    if (grid[i, j] == 0)
                    {
                        for (int k = 0; k < 9; k++) // loop through every solution for that cell
                        {
                            int cc = 0;
                            for(int z=startX; z<startX+3; z++)
                            {
                                for (int v = startY; v<startY+3; v++) //loop through every solution of cells inside that square
                                {
                                    if (grid[z, v] != 0)
                                        continue;

                                    for(int x=0; x<9; x++)
                                    {
                                        if (z == i && v == j) //exclude itself
                                            continue;

                                        if (solution[i, j, k] == solution[z, v, x]) 
                                        {
                                            cc++;
                                            break;
                                        }       
                                    }
                                }
                            }
                            if(cc==0)
                            {
                                for (int delete = 0; delete < 9; delete++)
                                    if (solution[i, j, delete] != solution[i, j, k])
                                        solution[i, j, delete] = 0;
                            }
                        }
                    }
                }
            }
        }

        static int UpdateGrid()
        {
            //update sudoku values based on single solutions
            int changed = 0;
            for(int i=0;i<9;i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int sCounter = 0;
                    int ind = 0;
                    for (int k = 0; k < 9; k++)
                    {
                        if (solution[i, j, k] != 0)
                        {
                            sCounter++;
                            ind = k;
                        }
                            
                    }
                    if (sCounter == 1 && grid[i,j]==0)
                    {
                        grid[i, j] = solution[i, j, ind];
                        logCell(i, j, grid[i, j], 0);
                        solution[i, j, ind] = 0;
                        changed++;
                    }
                }
            }
            if (changed != 0)
                return 1;
            else
                return 0;
        }

        static void clearSolutions()
        {
            // 3d matrix carrying possible solutions
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        solution[i, j, k] = 0;
                    }
                }
            }
        }

        static void createSolutions()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j] != 0)
                        continue;
                    FirstThread(i);
                    SecondThread(j);
                    ThirdThread(i, j);
                    FourthThread(i, j);
                }
            }
        }

        static void printSolution()
        {
            Console.WriteLine("\nSolution:\n");
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write("{0} ", grid[i, j]);
                }
                Console.Write("\n");
            }

        }

        static void checkEmpty()
        {
            int sum = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                    sum += grid[i, j];
                    }
                }
            if(sum==0)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        grid[i, j] = ((i * 3) + (i / 3) + j) % 9 + 1;
                        logCell(i, j, grid[i, j], 1);
                    }
                }
            }
        }

        static int checkIfSolved()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j] == 0)
                        return 1;
                }
            }
            return 0;
        }

        static int chooseValue()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    for(int k=0;k<9;k++)
                    {
                        if(grid[i,j]==0)
                        {
                            if(solution[i,j,k]!=0)
                            {
                                grid[i, j] = solution[i, j, k];
                                logCell(i, j, grid[i, j], 1);
                                return 1;
                            }
                        }
                    }
                }
            }
            return 1;
        }

        [STAThread]
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Open();
            checkEmpty();
            clearSolutions();
            createSolutions();

            RowColoumnUpdate();
            squareUpdate();
            UpdateGrid();

            

            CHOICESELECTION:
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if(grid[i,j]==0)
                    {
                        clearSolutions();
                        createSolutions();
                        RowColoumnUpdate();
                        squareUpdate();
                        UpdateGrid();
                    }
            if(checkIfSolved()==1)
            {
                chooseValue();
                breakLoop++;
                if (breakLoop > 1000)
                    Console.WriteLine("Couldn't Solve, breaking loop");
                else
                    goto CHOICESELECTION;
            }

            printSolution();
            Console.WriteLine();
            Console.Write(logFile);
            Console.ReadLine();
        }
    }
}
