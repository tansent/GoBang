using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace GoBangServer
{
    class GameRoom
    {
        private int round; 

        private int illusionSpellTimer;
        private int preIllusionState;
        bool hasShowWinWithIllusionSpell = false;


        public GameRoom(ListBox listbox)
        {
            this.listbox = listbox;
            gamePlayer[0] = new Player();
            gamePlayer[1] = new Player();
            service = new Service(listbox);

            gameBoard.InitializeBoard();
        }

        public List<User> lookOnUser = new List<User>();  //observers
        public Player[] gamePlayer = new Player[2];  // players
        /// <summary>
        /// proprety is equivalent to a method
        /// </summary>
        public Player BlackPlayer    // initialized with number 0
        {
            get { return gamePlayer[0];  }   //here the index 0 stands for black
            set { gamePlayer[0] = value; }
        }

        public Player WhitePlayer   // initialized with number 1
        {
            get { return gamePlayer[1]; }
            set { gamePlayer[1] = value; }
        }

        private GoBangBoard gameBoard = new GoBangBoard();  //The construction here represents initialization of the gameboard

        public GoBangBoard GameBoard  //gameBoard can only appear on the left side of an equivalence(All its properties can only be assigned yet not allowed to set other values)
        {
            get { return gameBoard; }
        }

        // set chess
        private ListBox listbox;
        private Service service;


        public void StartRound()
        {
            round = 0;
        }

        public void SetBoom(User user, int side,int i, int j, int boomColor)
        {
            gameBoard.Grid[i, j] = boomColor;
            service.SendToRoom(this, string.Format("SetBoom,{0},{1},{2},{3}", i, j, boomColor,side));
        }

        public void SetTrap(User user, int side, int i, int j, int trapColor)
        {
            gameBoard.Grid[i, j] = trapColor;
            service.SendToRoom(this, string.Format("SetTrap,{0},{1},{2},{3}", i, j, trapColor, side));
        }

        public void SetTriggeredBoom(User user, int side, int i, int j, int triggeredBoomSide)
        {
            gameBoard.Grid[i, j] = GoBangBoard.None;  //-1 No chess state
            if (gameBoard.NextIndex == 0)  //
                gameBoard.NextIndex = 1;
            else
                gameBoard.NextIndex = 0;
            service.SendToRoom(this, string.Format("TriggerBoom,{0},{1},{2},{3}", i, j, GoBangBoard.None, side));
            service.SendToRoom(this, string.Format("NextChess,{0}", gameBoard.NextIndex));
            round++;
        }

        public void SetTriggeredTrap(User user, int side, int i, int j, int triggeredTrapSide)
        {
            gameBoard.Grid[i, j] = triggeredTrapSide;  
            if (gameBoard.NextIndex == 0)  //
                gameBoard.NextIndex = 1;
            else
                gameBoard.NextIndex = 0;
            service.SendToRoom(this, string.Format("TriggerTrap,{0},{1},{2},{3},{4}", i, j, triggeredTrapSide, side, gameBoard.NextIndex));
            if (gameBoard.IsWin(i, j))
            {
                ShowWin(triggeredTrapSide);
            }
            else
            {
                service.SendToRoom(this, string.Format("NextChess,{0}", gameBoard.NextIndex));
                round++;
            }
        }



        public List<IllusionSpell> IllusionSpellList = new List<IllusionSpell>();
        public void SetIllusionSpell(int x, int y, int color)
        {
            IllusionSpell illusionSpell = new IllusionSpell(color);
            illusionSpell.illusionSpellEnable = true;
            IllusionSpellList.Add(illusionSpell);
            if (color == 0)
            {
                gameBoard.Grid[x, y] = GoBangBoard.WhiteIllusion;
                illusionSpell.preIllusionState = 1;
                
            }
            else
            {
                gameBoard.Grid[x, y] = GoBangBoard.BlackIllusion;
                illusionSpell.preIllusionState = 0;
            }
            illusionSpell.illusionSpellX = x;
            illusionSpell.illusionSpellY = y;
            illusionSpell.setIllusionSpellTimer();
            //Random r = new Random();
            //illusionSpellTimer = 2 * (r.Next(3)) + 2;

            //if (color == 0)
            //{
            //    gameBoard.Grid[x, y] = GoBangBoard.WhiteIllusion;
            //    preIllusionState = 1;
            //}
            //else
            //{
            //    gameBoard.Grid[x, y] = GoBangBoard.BlackIllusion;    //
            //    preIllusionState = 0;
            //}

            service.SendToRoom(this, string.Format("IllusionSpell,{0},{1},{2}", x, y, color));
        }

        //public void SetIllusionSpellTimeCollapse()
        //{
        //    //illusionSpellTimer--;
        //    //for (int i = 0; i < IllusionSpellList.Count; i++)
        //    //{
        //    foreach (IllusionSpell item in IllusionSpellList)
        //    {
        //        if (item.illusionSpellTimer>0)
        //        {
        //            item.illusionSpellTimer--;
        //            service.SendToRoom(this, string.Format("IllusionSpellTimeCollapse,{0}",item.side));
        //        }               
                
        //    }
                
        //    //}
            
        //}

        //public void SetIllusionSpellRelease(int IllusionSpellReleaseX, int IllusionSpellReleaseY,int side)
        //{
        //    IllusionSpell illusionSpell = new IllusionSpell(side);  //
        //    for (int i = 0; i < IllusionSpellList.Count; i++)
        //    {
        //        if (IllusionSpellList[i].side == side)
        //        {
        //            illusionSpell = IllusionSpellList[i];
        //        }
        //    }
        //    gameBoard.Grid[IllusionSpellReleaseX, IllusionSpellReleaseY] = illusionSpell.preIllusionState ;
        //    service.SendToRoom(this, string.Format("SetIllusionSpellRelease,{0},{1},{2},{3}", IllusionSpellReleaseX, IllusionSpellReleaseY, illusionSpell.preIllusionState, side));
        //    Thread.Sleep(50);
        //    if ((gameBoard.IsWin(IllusionSpellReleaseX, IllusionSpellReleaseY))&&(hasShowWinWithIllusionSpell == false))
        //    {
        //        hasShowWinWithIllusionSpell = true; //This sentence must be put up front before the next one(ShowWin(preIllusionState);)
        //        ShowWin(preIllusionState);
                
        //    }
        //    //else
        //    //{
        //    //    service.SendToRoom(this, string.Format("NextChess,{0}", gameBoard.NextIndex));
        //    //}
        //}

        public void SetDivisionSpell(int x, int y, int color)
        {
            List<int> positions = new List<int>();
            for (int m = 0; m <= gameBoard.Grid.GetUpperBound(0); m++)
            {
                for (int n = 0; n <= gameBoard.Grid.GetUpperBound(1); n++)
                {
                    try
                    {
                        if (gameBoard.Grid[m, n] == GoBangBoard.None)
                        {
                            positions.Add(n * 15 + m);
                        }
                    }
                    catch
                    {
                        continue; //do no_op
                    }
                }
            }

            RandomSelect randomselect = new RandomSelect(positions);  //???

            try
            {
                int positionCode = randomselect.getRandomSelectNumber();
              //  Thread.Sleep(100);
                positions.Remove(positionCode);  //////////////////

                int setX1 = positionCode % 15;
                int setY1 = positionCode / 15;

                gameBoard.Grid[setX1, setY1] = color;


                RandomSelect randomselect2 = new RandomSelect(positions);

                try
                {

                    int positionCode2 = randomselect2.getRandomSelectNumber();
                  //  Thread.Sleep(100);
                    positions.Remove(positionCode2);

                    int setX2 = positionCode2 % 15;
                    int setY2 = positionCode2 / 15;

                    gameBoard.Grid[setX2, setY2] = color;

                    ////////////
                    try
                    {
                        int positionCode3 = randomselect2.getRandomSelectNumber();
                        positions.Remove(positionCode3);
                        int setX3 = positionCode3 % 15;
                        int setY3 = positionCode3 / 15;
                        gameBoard.Grid[setX3, setY3] = color;

                        try
                        {
                            int positionCode4 = randomselect2.getRandomSelectNumber();
                            positions.Remove(positionCode4);
                            int setX4 = positionCode4 % 15;
                            int setY4 = positionCode4 / 15;
                            gameBoard.Grid[setX4, setY4] = color;

                            gameBoard.Grid[x, y] = GoBangBoard.None;

                            if (gameBoard.NextIndex == 0)  //
                                gameBoard.NextIndex = 1;
                            else
                                gameBoard.NextIndex = 0;

                            service.SendToRoom(this, string.Format("TriggerBoom,{0},{1},{2},{3}", x, y, GoBangBoard.None, color)); // is equavalent to trigger a boom
                            service.SendToRoom(this, string.Format("SetChess,{0},{1},{2}", setX1, setY1, color));
                            service.SendToRoom(this, string.Format("SetChess,{0},{1},{2}", setX2, setY2, color));
                            service.SendToRoom(this, string.Format("SetChess,{0},{1},{2}", setX3, setY3, color));
                            service.SendToRoom(this, string.Format("SetChess,{0},{1},{2}", setX4, setY4, color));
                            if (gameBoard.IsWin(setX1, setY1))
                            {
                                ShowWin(color);
                            }
                            else if (gameBoard.IsWin(setX2, setY2))
                            {
                                ShowWin(color);
                            }
                            else
                            {
                                service.SendToRoom(this, string.Format("NextChess,{0}", gameBoard.NextIndex));
                            }
                        }
                        catch
                        {
                            service.SendToRoom(this, string.Format("NextChess,{0}", gameBoard.NextIndex)); //No empty cell
                            round++;
                        }
                    }
                    catch
                    {
                        service.SendToRoom(this, string.Format("NextChess,{0}", gameBoard.NextIndex)); //No empty cell
                        round++;
                    }
                }
                catch
                {
                    service.SendToRoom(this, string.Format("NextChess,{0}", gameBoard.NextIndex)); //No empty cell
                    round++;
                }
            }
            catch
            {
                service.SendToRoom(this, string.Format("NextChess,{0}", gameBoard.NextIndex)); //No empty cell
                round++;
            }

        }

        public List<FrozenSpell> frozenSpellList = new List<FrozenSpell>();
        public void SetFrozenSpell(int x, int y, int spellCastSide)
        {
            int i = x - 1;
            int j = y - 1;
            int rivalColor;
            if (spellCastSide == 0)
                rivalColor = 1;
            else
                rivalColor = 0;

            bool hasFrozenedChess = false;
            for (int m = i; m < i + 3; m++)
            {
                for (int n = j; n < j + 3; n++)
                {
                    try
                    {
                        if (gameBoard.Grid[m, n] == rivalColor)    //Then all of the rival chess in this area should be frozen
                        {
                            if (rivalColor == 0)
                            {
                                gameBoard.Grid[m, n] = GoBangBoard.FrozendBlackChess;
                                service.SendToRoom(this, string.Format("FrozenChess,{0},{1},{2}", m, n, rivalColor));
                            }
                            else
                            {
                                gameBoard.Grid[m, n] = GoBangBoard.FrozendWhiteChess;
                                service.SendToRoom(this, string.Format("FrozenChess,{0},{1},{2}", m, n, rivalColor));
                            }
                            hasFrozenedChess = true;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            if (hasFrozenedChess == true)
            {
                FrozenSpell frozenSpell = new FrozenSpell(rivalColor);
                frozenSpell.forzenSpellEnable = true;
                frozenSpell.FrozenCenterX = x;
                frozenSpell.FrozenCenterY = y;
                frozenSpell.setFrozenTime();
                frozenSpellList.Add(frozenSpell);
                //frozenSpell.timerForFrozen.Enabled
            }
           
        }

        public void SetRandomChess(int x, int y, int color)
        { 
            //set an adjacent random chess
            //the minimum index for both x and y is 0, and the maximum is 14
            int i = x - 1;
            int j = y - 1;
            List<int> positions = new List<int>();
            for (int m = i; m < i + 3; m++)
            {
                for (int n = j; n < j + 3; n++)
                {
                    try
                    {
                        if (gameBoard.Grid[m, n] == GoBangBoard.None)
                        {
                            positions.Add(n * 15 + m); 
                        }
                    }
                    catch
                    {
                        continue; //do no_op
                    }
                }
            }

            RandomSelect randomselect = new RandomSelect(positions);
            try
            {
                int positionCode = randomselect.getRandomSelectNumber();

                int setX = positionCode % 15;
                int setY = positionCode / 15;

                gameBoard.Grid[setX, setY] = color;

                if (gameBoard.NextIndex == 0)  //
                    gameBoard.NextIndex = 1;
                else
                    gameBoard.NextIndex = 0;

                service.SendToRoom(this, string.Format("SetChess,{0},{1},{2}", setX, setY, color));
                if (gameBoard.IsWin(setX, setY))
                {
                    ShowWin(color);
                }
                else
                {
                    service.SendToRoom(this, string.Format("NextChess,{0}", gameBoard.NextIndex));
                    round++;
                }
            }
            catch
            {
                service.SendToRoom(this, string.Format("NextChess,{0}", gameBoard.NextIndex));
                round++;
            }
        }

        public void ShackSpell(int side)
        {
            service.SendToRoom(this, string.Format("ShackSpell,{0}", side));
        }
       

        /// <summary>
        /// delete following method
        /// </summary>
        /// <param name="user"></param>
        /// <param name="side"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="dizzyStatus"></param>
        //public void processDizzyPlayer(User user, int side, int i, int j,bool dizzyStatus)
        //{
        //    service.SendToRoom(this, string.Format("dizzyStatus,{0}", dizzyStatus));

        //    int x = i - 1 +1;
        //    int y = j - 1 +1;
        //    int[] tempGrid = new int[8];
        //    int[,] usedAblePosition = new int[3, 3];
        //    int p;
        //    int q;
        //    for (p = 0; p <= usedAblePosition.GetUpperBound(0); p++)
        //    {
        //        for (q = 0; q <= usedAblePosition.GetUpperBound(1); q++)
        //        {
        //            if (p== 1 && q == 1) { break; }
        //            usedAblePosition[p, q] = 9; ////9 stands for nil
        //        }
        //    }
        //    p = 0;
        //    q = 0;
        //    for (int t = 0; t <= tempGrid.GetUpperBound(0); t++)
        //    {
        //        tempGrid[t] = 9; //9 stands for nil
        //    }
        //    int usedAbleRandomNumber = 0;
        //    for (int m = x; m < x + 3; m++)
        //    {
        //        for (int n = y; n < y + 3; n++)
        //        {
        //            p++;
        //            q++;
        //            if (m == x + 1 && n == y + 1)
        //            { break; }  //The traped chess
        //            if (gameBoard.Grid[m, n] == GoBangBoard.None)
        //            {
        //                tempGrid[usedAbleRandomNumber] = gameBoard.Grid[m, n]; //-1 stand for none(not nil)
        //                usedAbleRandomNumber++;
        //                usedAblePosition[p, q] = gameBoard.Grid[m, n];
        //            }
        //        }
        //    }

        //    Random r = new Random();
        //    int nextPosition = r.Next(usedAbleRandomNumber);

        //    int negativeOne = 0;

        //    for (int m = x; m < x + 3; m++)
        //    {
        //        for (int n = y; n < y + 3; n++)
        //        {
        //            if (gameBoard.Grid[m, n] == GoBangBoard.None && nextPosition == negativeOne) // this is the grid that should be set next chess
        //            {
        //                service.SendToRoom(this, string.Format("RandomSetChess,{0},{1},{2}", m, n, side));
        //            }
        //            else
        //            {
        //                negativeOne++;
        //            }
        //        }
        //    }

        //    //for (int f = 0; f <= usedAblePosition.GetUpperBound(0); f++)
        //    //{
        //    //    for (int h = 0; h <= usedAblePosition.GetUpperBound(1); h++)
        //    //    {
        //    //        if (usedAblePosition[f, h] == GoBangBoard.None && nextPosition == negativeOne)
        //    //        { 
                        
        //    //        }
        //    //        else
        //    //        { negativeOne++; }
        //    //    }
        //    //}

        //    //for (int m = x; m < x + 3; m++)
        //    //{
        //    //    for (int n = y; n < y + 3; n++)
        //    //    {
        //    //        if (m == x + 1 && n == y + 1)
        //    //        { break; }  //The traped chess
        //    //        if (gameBoard.Grid[m, n] == GoBangBoard.None)
        //    //        {
                        
        //    //        }
        //    //    }
        //    //}
        //}

        public void SetChess(int i, int j, int chessColor)
        {//
            bool hasShowWinWithTheIllusionOne = false;
            bool hasShowWinWithTheFrozenOne = false;

            gameBoard.Grid[i, j] = chessColor;
            if (gameBoard.NextIndex == 0)  //
                gameBoard.NextIndex = 1;
            else
                gameBoard.NextIndex = 0;

            //IllusionSpell calculateSpell;
            foreach (IllusionSpell item in IllusionSpellList)
            {
                if (item.illusionSpellTimer != 0 && item.illusionSpellEnable == true)
                {
                    item.IllusionSpellTimeCollapse();
                }
                else
                {
                    gameBoard.Grid[item.illusionSpellX, item.illusionSpellY] = item.preIllusionState;
                    service.SendToRoom(this, string.Format("SetIllusionSpellRelease,{0},{1},{2}", item.illusionSpellX, item.illusionSpellY, item.preIllusionState));
                    item.illusionSpellEnable = false;
                    Thread.Sleep(50);
                    if ((gameBoard.IsWin(item.illusionSpellX, item.illusionSpellY)) )//&& (hasShowWinWithIllusionSpell == false))
                    {
                       // hasShowWinWithIllusionSpell = true; //This sentence must be put up front before the next one(ShowWin(preIllusionState);)
                        ShowWin(item.preIllusionState);
                        hasShowWinWithTheIllusionOne = true;
                        //gameBoard.InitializeBoard(); //
                        break;
                    }
                }
            }

            //FrozenSpell calculateSpell
            foreach (FrozenSpell item in frozenSpellList)
            {
                if (item.frozenTime != 0 && item.forzenSpellEnable == true)
                {
                    item.FrozenTimeCollapse();
                }
                else if(item.hasRecover == true)
                {
                    //do no_op
                }
                else //recover
                {
                    item.forzenSpellEnable = false;
                    for (int m = item.FrozenCenterX - 1; m < (item.FrozenCenterX - 1) + 3; m++)
                    {
                        for (int n = item.FrozenCenterY - 1; n < (item.FrozenCenterY - 1) + 3; n++)
                        {
                            try
                            {
                                if (gameBoard.Grid[m, n] == GoBangBoard.FrozendBlackChess && 0 != chessColor)
                                {
                                    gameBoard.Grid[m, n] = 0;
                                    service.SendToRoom(this, string.Format("SetChess,{0},{1},{2}", m, n, 0));
                                    if ((gameBoard.IsWin(m, n)))
                                    {
                                        ShowWin(0);
                                        //item.hasRecover = false;
                                        hasShowWinWithTheIllusionOne = true;
                                        goto a;  //jump out of loops 
                                    }
                                }
                                else if (gameBoard.Grid[m, n] == GoBangBoard.FrozendWhiteChess && 1 != chessColor)
                                {
                                    gameBoard.Grid[m, n] = 1;
                                    service.SendToRoom(this, string.Format("SetChess,{0},{1},{2}", m, n, 1));
                                    if ((gameBoard.IsWin(m, n)))
                                    {
                                        ShowWin(1);
                                        //item.hasRecover = false;
                                        hasShowWinWithTheIllusionOne = true;
                                        goto a;
                                    }
                                }
                                //gameBoard.Grid[m, n] = item.side;
                                //service.SendToRoom(this, string.Format("SetChess,{0},{1},{2}", m, n, item.side));
                                //if ((gameBoard.IsWin(m,n))) 
                                //{
                                //    ShowWin(item.side);
                                //    hasShowWinWithTheIllusionOne = true;
                                //    break;
                                //}
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    item.hasRecover = true;
                }
            }

            a:
          //  hasShowWinWithIllusionSpell = false;  // initialization
            if (hasShowWinWithTheIllusionOne == false && hasShowWinWithTheFrozenOne == false)
            {
                service.SendToRoom(this, string.Format("SetChess,{0},{1},{2}", i, j, chessColor));
            }

            if (gameBoard.IsWin(i, j) && hasShowWinWithTheIllusionOne == false && hasShowWinWithTheFrozenOne == false)
            {
                ShowWin(chessColor);
            }
            else
            {
                service.SendToRoom(this, string.Format("NextChess,{0}",gameBoard.NextIndex));
                round++;
            }
        }

        public void FrozenSpellRelease(int x, int y, int side)
        {

            foreach (FrozenSpell item in frozenSpellList)
            {
                if (item.side == side)
                {
                    if (item.timerForFrozen == true)
                    {
                        service.SendToRoom(this, string.Format("SetChess,{0},{1},{2}", x, y, side));
                    }
                }
            }
        }

        private void ShowWin(int chessColor)
        {
            gamePlayer[0].Start = false;
            gamePlayer[1].Start = false;
            gameBoard.InitializeBoard();
            service.SendToRoom(this, string.Format("Win,{0}",chessColor)); //
        }
    }
}
