using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoBangServer
{
    class GoBangBoard
    {
        public GoBangBoard()
        {
            InitializeBoard();
        }

        public void InitializeBoard()
        {
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    grid[i, j] = None;
                }
            }
            nextIndex = Black; // black first;
        }

        public bool isExist(int i,int j)  //judge the cell to see if it is empty
        {
            if (grid[i, j] != None)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Victory Judgement
        /// </summary>
        public bool IsWin(int i, int j)
        {
            int[] number = new int[4];     //
            number[0] = GetRowJudge(i, j);
            number[1] = GetColumnJudge(i, j);
            number[2] = GetSlashJudge(i, j);
            number[3] = GetBackSlashJudge(i, j);

            for (int k = 0; k < number.Length; k++)
            {
                if (number[k] >= 5)
                    return true;
            }
            return false;
        } //judge from four directions to see if any side wins

        private int GetRowJudge(int i, int j)
        {
            int num = 1; //adjacent number

            int x = i + 1;
            while (x < 15)
            {
                if (grid[i, j] == grid[x, j])
                {
                    num++;
                    x++;
                }
                else
                    break;
            }
            x = i - 1;
            while (x >= 0)
            {
                if (grid[i, j] == grid[x, j])
                {
                    num++;
                    x--;
                }
                else
                    break;
            }
            return num;
        }  //judge in horizontal direction

        private int GetColumnJudge(int i, int j)
        {
            int num = 1;
            int y = j+1;
            while (y < 15)
            {
                if (grid[i, j] == grid[i, y])
                {
                    num++;
                    y++;
                }
                else
                    break;
            }
            y = j - 1;
            while (y >= 0)
            {
                if (grid[i, j] == grid[i, y])
                {
                    num++;
                    y--;
                }
                else
                    break;
            }

            return num;
        } //judge in vertical direction

        private int GetSlashJudge(int i, int j)
        {
            int num = 1;
            int x = i + 1;
            int y = j + 1;
            while (x < 15 && y < 15)
            {
                if (grid[i, j] == grid[x, y])
                {
                    num++;
                    x++;
                    y++;
                }
                else
                    break;
            }
            x = i - 1;
            y = j - 1;
            while (x >= 0 && y >= 0)
            {
                if (grid[i, j] == grid[x, y])
                {
                    num++;
                    x--;
                    y--;
                }
                else
                    break;
            }
            return num;
        } // judge in slash direction

        private int GetBackSlashJudge(int i, int j)
        {
            int num = 1;
            int x = i + 1;
            int y = j - 1;
            while (x < 15 && y >= 0)
            {
                if (grid[i, j] == grid[x, y])
                {
                    num++;
                    x++;
                    y--;
                }
                else
                    break;
            }
            x = i - 1;
            y = j + 1;
            while (x >= 0 && y <15)
            {
                if (grid[i, j] == grid[x, y])
                {
                    num++;
                    x--;
                    y++;
                }
                else
                    break;
            }
            return num;
        } // judge in backslash direction

        public int[,] Grid
        {
            get { return grid; } 
        }

        public int NextIndex
        {
            get { return nextIndex; }
            set { nextIndex = value; }
        }

        public const int None = -1;
        public const int Black = 0;
        public const int white = 1;
        public const int BlackBoom = 2; //side add 2 equals boom color
        public const int WhiteBoom = 3;
        public const int BlackTrap = 4; //side add 4 equals trap color
        public const int WhiteTrap = 5;
        public const int BlackIllusion = 6; //side add 6 equals illusion color
        public const int WhiteIllusion = 7;
        public const int FrozendBlackChess = 8; //side add 8 equals illusion color
        public const int FrozendWhiteChess = 9;
        private int nextIndex;     //who is next turn
        private int[,] grid = new int[15, 15];
    }
}
