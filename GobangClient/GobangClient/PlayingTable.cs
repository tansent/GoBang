using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;

namespace GobangClient
{
    public partial class PlayingTable : Form
    {
        private int downIndex = 0; //Whose turn (0:black 1:white | black is always first)
        private bool isPlaying = false; //Whether the game is playing(Has it started or not)
        private bool isTactical = false;
        private bool isTactical2 = false;
        private bool isOnlineDisplayed; //Whether members of the room has been displayed


        //For black's booms
        bool UpLeft = false;
        bool DownRight = false;
        int blackBooms = 0;
        //For white's booms
        bool UpRight = false;
        bool DownLeft = false;
        int whiteBooms = 0;
        //For black's traps
        int UpRightTrap = 0;
        int DownLeftTrap = 0;
        int blackTraps = 0;
        //For white's traps
        int UpLeftTrap = 0;
        int DownRightTrap = 0;
        int whiteTraps = 0;

        bool selfdizzy = false; //player himself Triggered a trap
        bool rivaldizzy = false; //rival Triggered a trap

        int randomChessX;
        int randomChessY;

        bool hasSpellCastedThisTurn = false; //Whether a spell has been casted this turn

        bool DivisionSpell = false;

        bool IllusionSpell = false;
        int IllusionSpellTimer = 0;
        int IllusionSpellX;
        int IllusionSpellY;

        bool FrozenSpell = false;
        int frozenBlackRelease = 0;
        int frozenWhiteRelease = 0;

        bool ShackleSpell = false;
        
        private int tableIndex; //The Room(Table) number
        private int side; //Comer's classification (0:black 1:white 2:observer)
        private StreamWriter sw;

        private int[,] grid = new int[15, 15];

        private Bitmap blackBitmap;
        private Bitmap whiteBitmap;
        private Bitmap blackBoomBitmap;
        private Bitmap whiteBoomBitmap;
        private Bitmap blackTrapBitmap;
        private Bitmap whiteTrapBitmap;
        private Bitmap blackIllusionBitmap;
        private Bitmap whiteIllusionBitmap;
        private Bitmap FrozenblackChessBitmap;
        private Bitmap FrozenwhiteChessBitmap;
        private Bitmap Division;
        private Bitmap DivisionDARK;
        private Bitmap illusion;
        private Bitmap illusionDARK;
        private Bitmap FreezingBreath;
        private Bitmap FreezingBreathDARK;
        private Bitmap Shackle;
        private Bitmap ShackleDARK;

        private SoundPlayer chessSound;
        private SoundPlayer music;

        delegate void LabelDelegate(Label label, string str);
        LabelDelegate labelDelegate;

        delegate void ButtonDelegate(Button button, bool flag);
        ButtonDelegate buttonDelegate;

        delegate void SetPictureBoxDelegate(PictureBox pictureBox, Image image);  //
        SetPictureBoxDelegate setPictureBoxDelegate;

        delegate void RadioButtonDelegate(RadioButton radioButton, bool flag);
        delegate void SetDotDelegate(int i, int j, int ChessColor);               //

        public PlayingTable(int TableIndex, int Side, StreamWriter sw)
        {
            InitializeComponent();  //The indispensable code for a windows application's construction
            this.StartPosition = FormStartPosition.CenterScreen;

            this.tableIndex = TableIndex;
            this.side = Side;
            this.sw = sw;

            labelDelegate = new LabelDelegate(SetLabel);
            buttonDelegate = new ButtonDelegate(SetButton);
            setPictureBoxDelegate = new SetPictureBoxDelegate(SetPictureBoxImage);

            blackBitmap = new Bitmap(Properties.Resources.blackChess);
            whiteBitmap = new Bitmap(Properties.Resources.whiteChess);
            blackBoomBitmap = new Bitmap(Properties.Resources.blackBoom);
            whiteBoomBitmap = new Bitmap(Properties.Resources.whiteBoom);
            blackTrapBitmap = new Bitmap(Properties.Resources.blackTrap);
            whiteTrapBitmap = new Bitmap(Properties.Resources.whiteTrap);
            blackIllusionBitmap = new Bitmap(Properties.Resources.blackChessIllusion);
            whiteIllusionBitmap = new Bitmap(Properties.Resources.whiteChessIllusion);
            FrozenblackChessBitmap = new Bitmap(Properties.Resources.FrozenblackChess);
            FrozenwhiteChessBitmap = new Bitmap(Properties.Resources.FrozenwhiteChess);
            Division = new Bitmap(Properties.Resources.Division);
            DivisionDARK = new Bitmap(Properties.Resources.DivisionDARK);
            illusion = new Bitmap(Properties.Resources.illusion);
            illusionDARK = new Bitmap(Properties.Resources.illusionDARK);
            FreezingBreath = new Bitmap(Properties.Resources.FreezingBreath);
            FreezingBreathDARK = new Bitmap(Properties.Resources.FreezingBreathDARK);
            Shackle = new Bitmap(Properties.Resources.Shackle);
            ShackleDARK = new Bitmap(Properties.Resources.ShackleDARK);

            chessSound = new SoundPlayer(Properties.Resources.ChessSound);
            music = new SoundPlayer(Properties.Resources.BackgroundMusic);

            pictureBoxDivision.Image = Division;
            pictureBoxIllusion.Image = illusion;
            pictureBoxFrozen.Image = FreezingBreath;
            pictureBoxShackles.Image = Shackle;



            radioButtonMusic.Checked = true; //By default, select radioButtonMusic. Play the background music
                                             //When "checked" property is altered, it will trigger CheckedChange event
       }


        private void PlayingTable_Load(object sender, EventArgs e)  // ? Maybe Load and Construction can merge
        {
            ResetGrid();
            labelSideUp.Text = string.Empty;
            labelSideDown.Text = string.Empty;
            labelGo.Text = string.Empty;

            if (side == 2)
            {
                buttonStart.Enabled = false;
                buttonAskTie.Enabled = false;
            }
           
        }

        public void Restart(string str)
        {
            string tempStr = "";
            if (str != "")
            {
                tempStr = str.Substring(0, 5);
            }
            if (side != 2 && ShackleSpell == true)
            {
                if (tempStr == "BLACK")
                {
                    MessageBox.Show(str + Environment.NewLine + "With the effext of shack spell, BLACK wins double marks ", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(str + Environment.NewLine + "With the effext of shack spell, WHITE wins double marks ", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            else if (str != "") //？不可能不等于空
            {
                MessageBox.Show(str, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            ResetGrid();

            if (side != 2)
            {
                SetButton(buttonStart, true);
            }
            ShackleSpell = false;
        }

        public void AskTie(string str)
        {
            if (str != side.ToString())  // ?
            {
                string reply = "False";
                string sideString = str == "0" ? "Black" : "White";

                //Reply in this window whether agree to tie or not 
                if (MessageBox.Show(sideString + " side Ask to tie, agree?", "Tie", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    reply = "True";
                }
                SendToServer(string.Format("Reply,{0},{1},{2}", tableIndex, side, reply));
            }
        }

        private void ResetGrid()
        {
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    grid[i, j] = ChessColor.None;
                }
            }
            pictureBoxBoard.Invalidate();      //
        }

        private void SetLabel(Label label, string str)
        {
            if (label.InvokeRequired == true)
            {
                this.Invoke(labelDelegate, label, str);
            }
            else
            {
                label.Text = str;
            }
        }

        public void SetEnterUserTips(string userTypeString, string names, string roomOnlineNames)
        {
            string[] userNames = names.Split('/');
            if(isOnlineDisplayed == false)
            {
                string[] onlineNames = roomOnlineNames.Split('/');
                AddToRichTextBox("Original games:");
                for (int i = 0; i < onlineNames.Length; i++)
                {
                    if (onlineNames[i].Length > 0)
                    {
                        AddToRichTextBox(onlineNames[i]);
                    }
                }
                AddToRichTextBox("--------------------------");
                isOnlineDisplayed = true;
            }
            if (side == 0) //For Black Player, the label below is black
            {
                SetPictureBoxImage(pictureBoxUp, whiteBitmap);
                SetPictureBoxImage(pictureBoxDown, blackBitmap);
                SetLabel(labelSideUp, "White: " + userNames[1]);
                SetLabel(labelSideDown, "Black: " + userNames[0]);
            }
            else
            {
                SetPictureBoxImage(pictureBoxUp, blackBitmap);
                SetPictureBoxImage(pictureBoxDown, whiteBitmap);
                SetLabel(labelSideUp, "Black: " + userNames[0]);
                SetLabel(labelSideDown, "White: " + userNames[1]);
            }
            if (userTypeString == "0")
            {
                AddToRichTextBox(string.Format("{0} has entered(BLACK)", userNames[0]));
            }
            else if (userTypeString == "1")
            {
                AddToRichTextBox(string.Format("{0} has entered(WHITE)", userNames[1]));
            }
            else
            {
                AddToRichTextBox(string.Format("{0} has entered(OBSERVER)", userNames[2]));
            }

        }



        public void SetLabelGo(string str, int downIndex)
        {
            SetLabel(labelGo, str);
            this.downIndex = downIndex;

            if (downIndex == side && selfdizzy == true)
            {
                SendToServer(string.Format("RandomChess,{0},{1},{2},{3}", tableIndex, side, randomChessX, randomChessY));
                selfdizzy = false;
            }

            if (downIndex == side && FrozenSpell == true)
            {
                FrozenSpell = false;
            }

        }

        public void ReplayTie(string askSide, string str)  //Here, comparably, askSide is the side which IS asked
        {
            if (str == "True")   //这里str不可能为True
            {
                Restart("");
            }
            else
            {
                if (askSide != side.ToString()) //难道他两可能相等？
                {
                   string sideString = askSide == "0" ? "Black" : "White";
                   MessageBox.Show(sideString +" does not accede to tie");
                }
            }
        }

        private void SetButton(Button button, bool flag)
        {
            if (button.InvokeRequired == true)
            {
                this.Invoke(buttonDelegate, button, flag);
            }
            else
            {
                button.Enabled = flag; 
                if (flag == true)
                {
                    button.ForeColor = Color.Black;   //
                }
            }
        }

        private void SetPictureBoxImage(PictureBox pictureBox, Image image)
        {
            if (pictureBox.InvokeRequired == true)
            {
                this.Invoke(setPictureBoxDelegate, pictureBox, image);
            }
            else
            {
                pictureBox.Image = image;
            }
        }

        public void ShowTalk(string FromName, string content)
        {
            AddToRichTextBox(string.Format("{0} says: {1}",FromName, content));
        }

        public void ShowStart(string content)
        {
            AddToRichTextBox(string.Format(content));
        }

        public void StartTactical(bool isStartTactical)  //isStart == true
        {
            this.isTactical  = isStartTactical;
            ResetGrid();
        }

        public void Start(bool isStart)
        {
            this.isPlaying = isStart;
        }

        delegate void AddToRichTextBoxDelegate(string str);
        private void AddToRichTextBox(string str)
        {
            if (richTextBox1.InvokeRequired == true)
            {
                AddToRichTextBoxDelegate d = new AddToRichTextBoxDelegate(AddToRichTextBox);
                richTextBox1.Invoke(d, str);
            }
            else
            {
                if (richTextBox1.IsDisposed == false)  //
                {
                    richTextBox1.AppendText(str + Environment.NewLine); //
                    richTextBox1.ScrollToCaret();
                }
            }
        }

        public void SetBoom(int x, int y, int BoomColor, int side)
        {
            if (this.side == side) // he is the one who set the boom
            {
                grid[x, y] = BoomColor;
                pictureBoxBoard.Invalidate();
            }
            else
            {
                grid[x, y] = BoomColor;
                //no paint
            }

        }

        public void SetTrap(int x, int y, int TrapColor, int side)
        {
            if (this.side == side) // he is the one who set the trap
            {
                grid[x, y] = TrapColor;
                pictureBoxBoard.Invalidate();
            }
            else
            {
                grid[x, y] = TrapColor;
                //no paint
            }
        }

        public void TriggerBoom(int x, int y, int None, int side)
        {
            
            grid[x, y] = ChessColor.None;
            pictureBoxBoard.Invalidate();
            
        }

        public void TriggerTrap(int x, int y, int triggeredTrapSide, int side, int NextIndex)
        {
            grid[x, y] = triggeredTrapSide;
            pictureBoxBoard.Invalidate();
            if (this.side == side)
            {
                selfdizzy = true;  //
                randomChessX = x;
                randomChessY = y;
                //SendToServer(string.Format("dizzy,{0},{1},{2},{3},{4}", tableIndex, side, x, y, selfdizzy));
            }
            else
            {
                rivaldizzy = true;
            }
        }
        ////IllusionSpellClient illusionSpellClient;
        //List<IllusionSpellClient> list = new List<IllusionSpellClient>();
        public void ProcessIllusionSpell(int x, int y, int side)
        {
            //IllusionSpellClient illusionSpellClient = new IllusionSpellClient(side);
            
            if (side == 0)
            {
                grid[x, y] = ChessColor.WhiteIllusion;
            }
            else
            {
                grid[x, y] = ChessColor.BlackIllusion;
            }
            //illusionSpellClient.IllusionSpellTimer = illusionSpellTimer;
            //illusionSpellClient.IllusionSpellX = x;
            //illusionSpellClient.IllusionSpellY = y;
            //IllusionSpellTimer = illusionSpellTimer;
            //IllusionSpellX = x;
            //IllusionSpellY = y;
            //list.Add(illusionSpellClient);
            pictureBoxBoard.Invalidate();
        }

        //public void IllusionSpellTimeCollapse(int side)
        //{
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (list[i].IllusionSpellTimer != 0)
        //        {
        //            list[i].IllusionSpellTimer--;
        //            if (list[i].IllusionSpellTimer == 0)
        //            {
        //                SendToServer(string.Format("IllusionSpellRelease,{0},{1},{2},{3}", tableIndex, list[i].IllusionSpellX, list[i].IllusionSpellY, list[i].side));
        //            }

        //        }
        //    }
                //illusionSpellClient.IllusionSpellTimer--;
            
            //IllusionSpellTimer--;
            
        //}

        public void SetIllusionSpellRelease(int x, int y, int color)
        {
            grid[x, y] = color;
            IllusionSpell = false;
            pictureBoxBoard.Invalidate();
        }

        public void RandomSetChess(int x, int y, int side)
        {
            grid[x, y] = side;
            pictureBoxBoard.Invalidate();
        }

        public void SetFrozenedChesses(int x, int y, int FrozenedChessColor)
        {
            if (FrozenedChessColor == 0)
            {
                grid[x, y] = ChessColor.FrozenBlackChess;
            }
            else
            {
                grid[x, y] = ChessColor.FrozenWhiteChess;
            }
            pictureBoxBoard.Invalidate();
        }

        public void SetShackleSpell(int side)
        {
            MessageBox.Show("Shackle Spell Invoke!");
            ShackleSpell = true;
        }

        public void SetChess(int x,int y,int ChessColor)
        {
            AddToRichTextBox(string.Format("{0},{1},{2}",x,y,ChessColor));  //for observing use
            grid[x, y] = ChessColor;
            hasSpellCastedThisTurn = false;
            pictureBoxBoard.Invalidate();

        }

        public void ShowMessage(string str)
        {
            AddToRichTextBox(str);
        }

        public void InitializeGrid(string str)
        {
            string[] spliteString = str.Split('/');
            int k = 0;
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    grid[i, j] = int.Parse(spliteString[k]);
                    k++;
                }
            }
            pictureBoxBoard.Invalidate();  //Instantly redraw(update) the component, yet disable the user to control it(can see, but can't do)  ?
        }

        public void SetExitUserTips(string userTypeString, string names)
        {
            string[] userNames = names.Split('/');
            if (userTypeString == "2")  //observer leaves
            {
                AddToRichTextBox(string.Format("Observer [{0}] left",userNames[2])); //如果有多个旁观者怎么办
                return;
            }

            if (side == 0)  //player himself is black, (white left)
            {
                SetLabel(labelSideUp, "");
                AddToRichTextBox(string.Format("White player [{0}] has left", userNames[1]));
            }
            else if (side == 1)                                         //player himself is White or Observer
            {
                SetLabel(labelSideUp, "");
                AddToRichTextBox(string.Format("Black player [{0}] has left", userNames[0]));
                //if (userTypeString == "0")
                //{
                //    SetLabel(labelSideUp, "");
                //    AddToRichTextBox(string.Format("Black player [{0}] has left", userNames[0]));
                //}
                //else
                //{ 

                //}
            }
            else
            {
                if (userTypeString == "0")
                {
                    SetLabel(labelSideUp, "");
                    AddToRichTextBox(string.Format("Black player [{0}] has left", userNames[0]));
                }
                else
                {
                    SetLabel(labelSideDown, "");
                    AddToRichTextBox(string.Format("White player [{0}] has left", userNames[1]));
                }
            }
        }

        private void radioButtonMusic_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonMusic.Checked == true)
            {
                music.PlayLooping();  //Using a new thread to play the music over and over
            }
            else
            {
                music.Stop();
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            SendToServer(string.Format("Talk,{0},{1}",tableIndex,textBox1.Text));
            textBox1.Clear();
        }

        private void SendToServer(string str)
        {
            try
            {
                sw.WriteLine(str);
                sw.Flush();
            }
            catch
            {
                sw.WriteLine("Fail to send Message!");
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            SendToServer(string.Format("StartTactical,{0},{1}",tableIndex, side));
            buttonStart.ForeColor = SystemColors.GrayText;  //
            buttonStart.Enabled = false;
        }

        private void buttonAskTie_Click(object sender, EventArgs e)
        {
            if (ShackleSpell == true)
            {
                MessageBox.Show("With the effect of Shackle Spell, no one can leave or tie");
                return;
            }

            SendToServer(string.Format("AskTie,{0},{1}",tableIndex,side));
        }

        bool hasFrozenSpellCasted = false;
        private void pictureBoxBoard_MouseDown(object sender, MouseEventArgs e)
        {
            //Tactical Section
            if (isTactical == true)
            {
                int x = e.X / 20 - 1;
                int y = e.Y / 20 - 1;
                if (side == 0) //Black
                {
                    
                    for (int i = 3; i < 7; i++)
                    {
                        for (int j = 6; j > 6 - i + 2; j--)
                        {
                            if ((x == i && y == j) && UpLeft == false)  // The position it clicks is in the constrained area
                            {
                                if (grid[x, y] == ChessColor.None)// && downIndex == side)
                                {
                                    UpLeft = true;
                                    //SetBoom, tableIndex, side, Row, Column, Color
                                    SendToServer(string.Format("SetBoom,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                                    blackBooms++;
                                    if (radioButtonSound.Checked == true)
                                    {
                                        chessSound.Play();
                                    }
                                }

                            }
                        }
                    }

                    for (int i = 8; i < 12; i++)
                    {
                        for (int j = 8; j < 8 + 12 - i; j++)
                        {
                            if ((x == i && y == j) && DownRight == false)
                            {
                                if (grid[x, y] == ChessColor.None)// && downIndex == side)
                                {
                                    DownRight = true;
                                    //SetBoom, tableIndex, side, Row, Column, Color
                                    SendToServer(string.Format("SetBoom,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                                    blackBooms++;
                                    if (radioButtonSound.Checked == true)
                                    {
                                        chessSound.Play();
                                    }
                                }
                            }
                        }
                    }

                    
                    if (blackBooms == 2)
                    {
                        isTactical = false;
                        isTactical2 = true;
                    }
                    //if (UpLeft == false)
                    //{
                    //    MessageBox.Show("Not allowed set boom here");
                    //}
                }

                else //white
                {
                   // UpRight = false;
                    for (int i = 3; i < 7; i++)
                    {
                        for (int j = 8; j < 8 - 2 + i; j++)
                        {
                            if ((x == i && y == j) && UpRight == false)
                            {
                                if (grid[x, y] == ChessColor.None)// && downIndex == side)
                                {
                                    UpRight = true;
                                    //SetBoom, tableIndex, side, Row, Column, Color
                                    SendToServer(string.Format("SetBoom,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                                    whiteBooms++;
                                    if (radioButtonSound.Checked == true)
                                    {
                                        chessSound.Play();
                                    }
                                }
                            }
                        }
                    }

                   // DownLeft = false;
                    for (int i = 8; i < 12; i++)
                    {
                        for (int j = 6; j > i - 6; j--)
                        {
                            if ((x == i && y == j) && DownLeft == false)
                            {
                                if (grid[x, y] == ChessColor.None)// && downIndex == side)
                                {
                                    DownLeft = true;
                                    //SetBoom, tableIndex, side, Row, Column, Color
                                    SendToServer(string.Format("SetBoom,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                                    whiteBooms++;
                                    if (radioButtonSound.Checked == true)
                                    {
                                        chessSound.Play();
                                    }
                                }
                            }
                        }
                    }

                    
                    if (whiteBooms == 2)
                    {
                        isTactical = false;
                        isTactical2 = true;
                    }
                }
            
            }

            if (isTactical2 == true)  //Set Traps stage
            {
                int x = e.X / 20 - 1;
                int y = e.Y / 20 - 1;
                if (side == 0)
                {
                    for (int i = 8; i < 15; i++)
                    {
                        for (int j = 0; j < i - 7; j++)
                        {
                            if ((x == i && y == j) && UpRightTrap < 2)
                            {
                                if (grid[x, y] == ChessColor.None)
                                {
                                    UpRightTrap++;
                                    //SetBoom, tableIndex, side, Row, Column, Color
                                    SendToServer(string.Format("SetTrap,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                                    blackTraps++;
                                    if (radioButtonSound.Checked == true)
                                    {
                                        chessSound.Play();
                                    }
                                }
                            }
                        }
                    }


                    for (int i = 0; i < 7; i++)
                    {
                        for (int j = 14; j > 7 + i; j--)
                        {
                            if ((x == i && y == j) && DownLeftTrap < 2)
                            {
                                if (grid[x, y] == ChessColor.None)
                                {
                                    DownLeftTrap++;
                                    //SetBoom, tableIndex, side, Row, Column, Color
                                    SendToServer(string.Format("SetTrap,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                                    blackTraps++;
                                    if (radioButtonSound.Checked == true)
                                    {
                                        chessSound.Play();
                                    }
                                }
                            }
                        }
                    }

                    if (blackTraps == 4)
                    {
                        isTactical2 = false;
                        SendToServer(string.Format("TacticalEnd,{0},{1}", tableIndex, side));
                    }

                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        for (int j = 0; j < 7 - i; j++)
                        {
                            if ((x == i && y == j) && UpLeftTrap < 2)
                            {
                                if (grid[x, y] == ChessColor.None)
                                {
                                    UpLeftTrap++;
                                    //SetBoom, tableIndex, side, Row, Column, Color
                                    SendToServer(string.Format("SetTrap,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                                    whiteTraps++;
                                    if (radioButtonSound.Checked == true)
                                    {
                                        chessSound.Play();
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 8; i < 15; i++)
                    {
                        for (int j = 14; j > 21 - i; j--)
                        {
                            if ((x == i && y == j) && DownRightTrap < 2)
                            {
                                if (grid[x, y] == ChessColor.None)
                                {
                                    DownRightTrap++;
                                    //SetBoom, tableIndex, side, Row, Column, Color
                                    SendToServer(string.Format("SetTrap,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                                    whiteTraps++;
                                    if (radioButtonSound.Checked == true)
                                    {
                                        chessSound.Play();
                                    }
                                }
                            }
                        }
                    }

                    if (whiteTraps == 4)
                    {
                        isTactical2 = false;
                        SendToServer(string.Format("TacticalEnd,{0},{1}", tableIndex, side));
                    }
                }

            }

            //Playing Section
            if (isPlaying == true)
            {
               // this.Size.Height = 569;
                int x = e.X / 20 - 1;
                int y = e.Y / 20 - 1;
                
                if (!(x < 0 || x > 14 || y < 0 || y > 14))
                {
                    //
                    if (side == 0 && downIndex == side && FrozenSpell == true && hasFrozenSpellCasted==false)
                    {
                        SendToServer(string.Format("FrozenSpell,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                        hasFrozenSpellCasted = true;
                        if (radioButtonSound.Checked == true)  //Frozen spell Sound
                        {
                            chessSound.Play();
                        }
                    }
                    else if (side == 1 && downIndex == side && FrozenSpell == true && hasFrozenSpellCasted == false)
                    {
                        SendToServer(string.Format("FrozenSpell,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                        hasFrozenSpellCasted = true;
                        if (radioButtonSound.Checked == true)  //Frozen spell Sound
                        {
                            chessSound.Play();
                        }
                    }
                    //
                    else if (grid[x, y] == ChessColor.None && downIndex == side)
                    {
                        if (DivisionSpell == true)
                        {
                            MessageBox.Show("Please select your own chess to cast the spell!");
                            return;
                        }
                        if (IllusionSpell == true)
                        {
                            MessageBox.Show("Please select a rival's chess to cast the spell!");
                            return;
                        }
                        //SetChess, tableIndex, side, Row, Column, Color
                        SendToServer(string.Format("SetChess,{0},{1},{2},{3},{4}",tableIndex,side,x,y,side));
                        if (radioButtonSound.Checked == true)
                        {
                            chessSound.Play();
                        }
                        //for (int i = 0; i < list.Count; i++)
                        //{
                        //    if ((IllusionSpell == true) && (list[i].IllusionSpellTimer != 0))
                        //    {
                        //        SendToServer(string.Format("IllusionSpellTimeCollapse,{0}", tableIndex)); //
                        //    }
                        //}
                        

                    }
                    else if (side == 0 && grid[x, y] == ChessColor.WhiteBoom && downIndex == side) //Black triggers White's booms
                    {
                        if (DivisionSpell == true)
                        {
                            MessageBox.Show("Please select your own chess to cast the spell!");
                            return;
                        }
                        SendToServer(string.Format("TriggerBoom,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                        if (radioButtonSound.Checked == true)  //Boom Sound
                        {
                            chessSound.Play();
                        }
                    }
                    //else if(side == 0 && grid[x, y] == ChessColor.BlackBoom)
                    else if (side == 1 && grid[x, y] == ChessColor.BlackBoom && downIndex == side) //White triggers Black's booms
                    {
                        if (DivisionSpell == true)
                        {
                            MessageBox.Show("Please select your own chess to cast the spell!");
                            return;
                        }
                        SendToServer(string.Format("TriggerBoom,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                        if (radioButtonSound.Checked == true)  //Boom Sound
                        {
                            chessSound.Play();
                        }
                    }
                    else if (side == 0 && grid[x, y] == ChessColor.WhiteTrap && downIndex == side)//Black triggers White's traps
                    {
                        if (DivisionSpell == true)
                        {
                            MessageBox.Show("Please select your own chess to cast the spell!");
                            return;
                        }
                        SendToServer(string.Format("TriggerTrap,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                        if (radioButtonSound.Checked == true)  //Trap Sound
                        {
                            chessSound.Play();
                        }
                    }
                    else if (side == 1 && grid[x, y] == ChessColor.BlackTrap && downIndex == side)//White triggers Black's traps
                    {
                        if (DivisionSpell == true)
                        {
                            MessageBox.Show("Please select your own chess to cast the spell!");
                            return;
                        }
                        SendToServer(string.Format("TriggerTrap,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                        if (radioButtonSound.Checked == true)  //Trap Sound
                        {
                            chessSound.Play();
                        }
                    }
                    else if (side == 0 && grid[x, y] == ChessColor.Black && downIndex == side && DivisionSpell == true)
                    {
                        SendToServer(string.Format("DivisionSpell,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                        DivisionSpell = false;
                        hasSpellCastedThisTurn = false;
                        if (radioButtonSound.Checked == true)  //Division Sound
                        {
                            chessSound.Play();
                        }
                    }
                    else if (side == 1 && grid[x, y] == ChessColor.White && downIndex == side && DivisionSpell == true)
                    {
                        SendToServer(string.Format("DivisionSpell,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                        DivisionSpell = false;
                        hasSpellCastedThisTurn = false;
                        if (radioButtonSound.Checked == true)  //Division Sound
                        {
                            chessSound.Play();
                        }
                    }
                    else if ((side == 0 && grid[x, y] == ChessColor.White && downIndex == side && DivisionSpell == true) ||
                            (side == 0 && grid[x, y] == ChessColor.BlackBoom && downIndex == side && DivisionSpell == true) ||
                            (side == 0 && grid[x, y] == ChessColor.BlackTrap && downIndex == side && DivisionSpell == true))
                    {
                        MessageBox.Show("Please select your own chess to cast the spell!");
                        return;
                    }
                    else if ((side == 1 && grid[x, y] == ChessColor.Black && downIndex == side && DivisionSpell == true) ||
                            (side == 1 && grid[x, y] == ChessColor.WhiteBoom && downIndex == side && DivisionSpell == true) ||
                            (side == 1 && grid[x, y] == ChessColor.WhiteTrap && downIndex == side && DivisionSpell == true))
                    {
                        MessageBox.Show("Please select your own chess to cast the spell!");
                        return;
                    }
                    else if (side == 0 && grid[x, y] == ChessColor.White && downIndex == side && IllusionSpell == true)
                    {
                        SendToServer(string.Format("IllusionSpell,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                        IllusionSpell = false;
                        if (radioButtonSound.Checked == true)  //Illusion Sound
                        {
                            chessSound.Play();
                        }
                    }
                    else if (side == 1 && grid[x, y] == ChessColor.Black && downIndex == side && IllusionSpell == true)
                    {
                        SendToServer(string.Format("IllusionSpell,{0},{1},{2},{3},{4}", tableIndex, side, x, y, side));
                        IllusionSpell = false;
                        if (radioButtonSound.Checked == true)  //Illusion Sound
                        {
                            chessSound.Play();
                        }
                    }
                    else if ((side == 0 && grid[x, y] == ChessColor.Black && downIndex == side && IllusionSpell == true) ||
                             (side == 0 && grid[x, y] == ChessColor.BlackBoom && downIndex == side && IllusionSpell == true) ||
                             (side == 0 && grid[x, y] == ChessColor.BlackTrap && downIndex == side && IllusionSpell == true) ||
                             (side == 0 && grid[x, y] == ChessColor.WhiteIllusion && downIndex == side && IllusionSpell == true))
                    {       
                        MessageBox.Show("Please select a rival's chess to cast the spell!");
                        return;
                    }
                    else if ((side == 1 && grid[x, y] == ChessColor.White && downIndex == side && IllusionSpell == true) ||
                             (side == 1 && grid[x, y] == ChessColor.WhiteBoom && downIndex == side && IllusionSpell == true) ||
                             (side == 1 && grid[x, y] == ChessColor.WhiteTrap && downIndex == side && IllusionSpell == true) ||
                             (side == 1 && grid[x, y] == ChessColor.BlackIllusion && downIndex == side && IllusionSpell == true))
                    {
                        MessageBox.Show("Please select a rival's chess to cast the spell!");
                        return;
                    }
                    //

                        //
                    else if (side == 0 && grid[x,y] == ChessColor.FrozenBlackChess)
                    {
                        frozenBlackRelease++;
                        if (radioButtonSound.Checked == true)  //ice break Sound
                        {
                            chessSound.Play();
                        }
                        if (frozenBlackRelease == 5)
                        {
                            SendToServer(string.Format("FrozenSpellRelease,{0},{1},{2},{3}", tableIndex, side, x, y));
                            frozenBlackRelease = 0;
                        }
                    }
                    else if (side == 1 && grid[x, y] == ChessColor.FrozenWhiteChess)
                    {
                        frozenWhiteRelease++;
                        if (radioButtonSound.Checked == true)  //ice break Sound
                        {
                            chessSound.Play();
                        }
                        if (frozenWhiteRelease == 5)
                        {
                            SendToServer(string.Format("FrozenSpellRelease,{0},{1},{2},{3}", tableIndex, side, x, y));
                            frozenWhiteRelease = 0;
                        }
                    }

                }

            }
        }

        private void pictureBoxDivision_Click(object sender, EventArgs e)
        {
            bool hasOwnChess = false;
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    if (grid[i, j] == side)
                    {
                        hasOwnChess = true;
                        break;
                    }
                }
                if (hasOwnChess == true)
                    break;
            }

            if (isPlaying == false)
            {
                MessageBox.Show("Cannot cast the spell yet!");
                return;
            }
            else if (isPlaying == true && downIndex != side)
            {
                MessageBox.Show("Wait tell your turn!");
                return;
            }
            else if (isPlaying == true && downIndex == side && hasSpellCastedThisTurn == true)
            {
                MessageBox.Show("Spells Cool Down! ");
                return;
            }
            else if (isPlaying == true && downIndex == side && hasOwnChess == false)
            {
                MessageBox.Show("You don't have any chess on the board!");
                return;
            }
            else
            {
                DivisionSpell = true;
                pictureBoxDivision.Visible = false;
                hasSpellCastedThisTurn = true;
            }

        }

        private void pictureBoxIllusion_Click(object sender, EventArgs e)
        {
            bool hasRivalChess = false;

            int otherside;
            if (side == 0)  
                otherside = 1;
            else
                otherside = 0;

            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    if (grid[i, j] == otherside)
                    {
                        hasRivalChess = true;
                        break;
                    }
                }
                if (hasRivalChess == true)
                    break;
            }

            if (isPlaying == false)
            {
                MessageBox.Show("Cannot cast the spell yet!");
            }
            else if (isPlaying == true && downIndex != side)
            {
                MessageBox.Show("Wait tell your turn!");
            }
            else if (isPlaying == true && downIndex == side && hasSpellCastedThisTurn == true)
            {
                MessageBox.Show("Spells Cool Down! ");
                return;
            }
            else if (isPlaying == true && downIndex == side && hasRivalChess == false)
            {
                MessageBox.Show("Rival doesn't have any chess on the board!");
            }
            else
            {
                IllusionSpell = true;
                hasSpellCastedThisTurn = true;
                pictureBoxIllusion.Visible = false;
            }
        }


        private void pictureBoxFrozen_Click(object sender, EventArgs e)
        {
            if (isPlaying == false)
            {
                MessageBox.Show("Cannot cast the spell yet!");
            }
            else if (isPlaying == true && downIndex != side)
            {
                MessageBox.Show("Wait tell your turn!");
            }
            else if (isPlaying == true && downIndex == side && hasSpellCastedThisTurn == true)
            {
                MessageBox.Show("Spells Cool Down! ");
                return;
            }
            else
            {
                FrozenSpell = true;
                hasSpellCastedThisTurn = true;
                pictureBoxFrozen.Visible = false;
            }
            
        }


        private void pictureBoxShackles_Click(object sender, EventArgs e)
        {
            if (isPlaying == false)
            {
                MessageBox.Show("Cannot cast the spell yet!");
            }
            else if (isPlaying == true && downIndex != side)
            {
                MessageBox.Show("Wait tell your turn!");
            }
            else if (isPlaying == true && downIndex == side && hasSpellCastedThisTurn == true)
            {
                MessageBox.Show("Spells Cool Down! ");
                return;
            }
            else
            {
                SendToServer(string.Format("ShackleSpell,{0},{1}", tableIndex, side));
                hasSpellCastedThisTurn = true;
                pictureBoxShackles.Visible = false;
            }
        }

        private void pictureBoxBoard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    if (grid[i, j] != ChessColor.None)
                    {
                        if (grid[i, j] == ChessColor.Black)
                        {
                            g.DrawImage(blackBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                        else if (grid[i, j] == ChessColor.White)
                        {
                            g.DrawImage(whiteBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                        else if (grid[i, j] == ChessColor.BlackBoom && side == 0)
                        {
                            g.DrawImage(blackBoomBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                        else if (grid[i, j] == ChessColor.WhiteBoom && side == 1)
                        {
                            g.DrawImage(whiteBoomBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                        else if (grid[i, j] == ChessColor.BlackTrap && side == 0)
                        {
                            g.DrawImage(blackTrapBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                        else if (grid[i, j] == ChessColor.WhiteTrap && side == 1)
                        {
                            g.DrawImage(whiteTrapBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                        else if (grid[i, j] == ChessColor.BlackIllusion)
                        {
                            g.DrawImage(blackIllusionBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                        else if (grid[i, j] == ChessColor.WhiteIllusion)
                        {
                            g.DrawImage(whiteIllusionBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                        else if (grid[i, j] == ChessColor.FrozenBlackChess)
                        {
                            g.DrawImage(FrozenblackChessBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                        else if (grid[i, j] == ChessColor.FrozenWhiteChess)
                        {
                            g.DrawImage(FrozenwhiteChessBitmap, (i + 1) * 20, (j + 1) * 20);
                        }
                    }
                }
            }
        }

        private void PlayingTable_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ShackleSpell == true)
            {
                return;
            }

            music.Stop();

            isTactical = false;
            isTactical2 = false;
            isPlaying = false;

            SendToServer(string.Format("ExitRoom,{0},{1}", tableIndex, side));
        }

        private void PlayingTable_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ShackleSpell == true)
            {
                this.Show();
            }
        }

        

        



        
       
    }
}
