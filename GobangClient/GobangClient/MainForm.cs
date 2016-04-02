using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using GobangUserControl;  //

namespace GobangClient
{
    public partial class MainForm : Form
    {

        private int roomNumber = 0; // max room number receive from the server

        private GobangUserControl.GobangRoom[] tables; // Total table(each table is a cell room)

        private Label[] tableTips; //tips at the left of the tables
        

        //basic connect info
        private TcpClient client = null;
        private StreamWriter sw;
        private StreamReader sr;


        private PlayingTable playingTable; // game window

        /// <summary>
        /// Whether to receive command from the server
        /// true: Receive command from the server. Validating Event is futile
        /// false: The changes should be operated in Validating Event
        /// </summary>
        private bool isReceiveCommand = false;


        //Current status: -1:outside  0:black  1:White  2:Observer 
        private int side = -1;  // default is outside

        // the room number a player with (-1: at the game lobby )
        private int tableIndex = -1; //Whenever you click, the tableIndex change to the newly-clicked table's number

        private bool isThreadStarted = false; // if the thread has begun

        private bool isOnlineNumberDisplayed = false; // whether the number in the lobby displayed

        private bool isCreatedRoom = false; //whether the room has created

        private bool isNeedLoginAgain = false; //whether need login again


        delegate void RichTestBoxCallBack(string str);
        RichTestBoxCallBack richTextBoxCallBack; // ?


        public MainForm()
        {
            InitializeComponent();
            textBoxName.MaxLength = 10;// The max length that can entered into the textBoxName
            this.StartPosition = FormStartPosition.CenterScreen; // centralize the window 

            richTextBoxCallBack = new RichTestBoxCallBack(AddToRichTextBox);  // ?
            
            //this.WindowState = FormWindowState.Maximized;
        }

        private void AddToRichTextBox(string str)
        {
            if (richTextBox1.InvokeRequired == true)
            {
                richTextBox1.Invoke(richTextBoxCallBack, str);
            }
            else
            {
                if (richTextBox1.IsDisposed == false)
                {
                    richTextBox1.AppendText(str + Environment.NewLine);
                    richTextBox1.ScrollToCaret();   //Move the Scroll bar to the current cursor's position automatically
                }
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text.Trim().Length == 0)  // Trim is the method to remove space
            {
                MessageBox.Show("Please enter a nickname", "No Nickname", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBoxServer.Text.Length == 0) // No particular server is equivalent to using a local server by default
            {
                try
                {
                    client = new TcpClient(Dns.GetHostName(), 51888);   // build a connection
                }
                catch
                {
                    MessageBox.Show("Fail to connect to the server", "Fail connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                try
                {
                    client = new TcpClient(textBoxServer.Text, 51888);   // build a connection
                }
                catch
                {
                    MessageBox.Show("Fail to connect to the server", "Fail connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            EnableConnectToServerAgain(false);

            NetworkStream networkStream = client.GetStream(); // First client connect, then get netstream
            sw = new StreamWriter(networkStream, System.Text.Encoding.Default);   // send message
            sr = new StreamReader(networkStream, System.Text.Encoding.Default);   // receive message
            // Connection in the network is invisible

            SendToServer("Login," + textBoxName.Text.Trim());  // "Login" is the first order

            Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
            threadReceive.IsBackground = true;
            threadReceive.Start();
        }

        private void ReceiveData()
        {
            isThreadStarted = true;
            isNeedLoginAgain = false;
            bool isNeedExitWhile = false;
            while (isNeedExitWhile == false)// Once the Connection has been built, a constant receiving loop should be built immediately to get commands
                                            //Once the loop is break, it means the connection has lost   // (Self-defined)
            {
                string receiveString = null;
                try
                {
                    receiveString = sr.ReadLine();
                }
                catch 
                {
                    AddToRichTextBox("Fail to receive commands from the server!");                    
                }

                if (receiveString == null)
                {
                    MessageBox.Show("Losing Connection.", "Lost", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isNeedExitWhile = true;
                    continue;
                }

              //  AddToRichTextBox("receive: " + receiveString); // for test use, deletable

                string[] splitString = receiveString.Split(',');
                switch (splitString[0])
                {
                    // RoomNumbers, maxRoomNumbers(total rooms in the lobby), userList.Count(number of all the user)
                    case "RoomNumbers":
                        this.roomNumber = int.Parse(splitString[1]);

                        AddTablesToPanel();  //generate multiple tables(cell rooms) in the lobby 

                        if (isOnlineNumberDisplayed == false)
                        {
                            AddToRichTextBox(string.Format("Original Gamer: {0}", int.Parse(splitString[2]) - 1));
                            AddToRichTextBox("---------------------------");
                            isOnlineNumberDisplayed = true;
                        }
                        break;

                    // Tables, Tables' situation
                    case "Tables":
                        string s = receiveString.Substring(7);

                        isReceiveCommand = true;

                        string[] s1 = s.Split(','); // here in order to get the total count of the tables

                        for (int i = 0; i < s1.Length; i++)
                        {
                            string[] s2 = s1[i].Split('/');
                            if (s2[0][0] == '0')  // s2[0] (Black player) == "0" (Empty)
                            {
                                SetBoolProperty(tables[i], 0, false);
                                SetTextProperty(tables[i], 0, "");
                            }
                            else                  // s2[0] (Black player) == "1" (Sitted)
                            {
                                SetBoolProperty(tables[i], 0, true);
                                SetTextProperty(tables[i], 0, s2[0].Substring(1));
                            }
                            if (s2[1][0] == '0')  // s2[1] (White player) == "0" (Empty)
                            {
                                SetBoolProperty(tables[i], 1, false);
                                SetTextProperty(tables[i], 1, "");
                            }
                            else                  // s2[0] (White player) == "1" (Sitted)
                            {
                                SetBoolProperty(tables[i], 1, true);
                                SetTextProperty(tables[i], 1, s2[1].Substring(1));
                            }
                           // isReceiveCommand = false; //
                        }
                         isReceiveCommand = false;
                        break;

                    //Login, User's name
                    case "Login":
                        AddToRichTextBox(string.Format("{0} has entered the Game Lobby", splitString[1]));
                        break;

                    //Sorry
                    case "Sorry":  //Client only need to quit receiving commands.(quit loop) The real remove operation is done in server side
                        MessageBox.Show("The Game Lobby is full!", "Room Full", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        isNeedExitWhile = true;
                        isNeedLoginAgain = true; 
                        break;

                    //NameUsed
                    case "NameUsed":   //Client only need to quit receiving commands.(quit loop) The real remove operation is done in server side
                        MessageBox.Show("The name has been used!", "Name Used", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        isNeedExitWhile = true;
                        isNeedLoginAgain = true; 
                        break;

                    //EnterRoom ,side, Current Room Situation(/), Previous Room Situation(/)
                    case "EnterRoom":
                        if (isCreatedRoom == false)
                        {
                            CreateRoom(tableIndex); //参数可以不用传
                            isCreatedRoom = true;
                        }
                        playingTable.SetEnterUserTips(splitString[1], splitString[2], splitString[3]);
                        break;

                    //Grid, grid situation on the table
                    case "Grid":
                        if (isCreatedRoom == false) //还需要这么做吗？
                        {
                            CreateRoom(tableIndex); //参数可以不用传
                            isCreatedRoom = true;
                        }
                        playingTable.InitializeGrid(splitString[1]);
                        break;

                    //Talk, FromWhom, content
                    case "Talk":
                        if (playingTable != null)  // The player is still in the room
                        {
                            playingTable.ShowTalk(splitString[1], receiveString.Substring(splitString[0].Length + splitString[1].Length + 2));
                        }
                        break;

                    //Message, "'Who' is ready!"
                    case "Message":
                        if (playingTable != null)
                        {
                            playingTable.ShowStart(receiveString.Substring(9));
                        }
                        break;

                    //case "BoomMessage":
                    //    MessageBox.Show("You have already set boom in this area!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //break;

                    //Start, NextTurn   //NextTurn 好像没用
                    case "StartTactical":
                        playingTable.StartTactical(true);
                        goto case "NextChess";  //

                    case "StartPlaying":
                        playingTable.Start(true);
                        goto case "NextChess";//

                    //AskTie, side
                    case "AskTie":
                        playingTable.AskTie(splitString[1]);
                        break;

                    //Restart, NextTurn
                    case "Restart": //Agree to tie
                        playingTable.Restart("");
                        goto case "NextChess";

                    case "NextChess":
                        if (splitString[1] == "0") //
                        {
                            playingTable.SetLabelGo("It is BLACK's Turn!",0);
                        }
                        else
                        {
                            playingTable.SetLabelGo("It is WHITE's Turn!",1);
                        }
                        break;

                    //Reply, side, Result(False)
                    case "Reply":
                        playingTable.ReplayTie(splitString[1], splitString[2]);
                        break;

                    //SetBoom,row,column,boomColor,side
                    case "SetBoom":
                        playingTable.SetBoom(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]), int.Parse(splitString[4]));
                        break;

                    //SetTrap,row,column,trapColor,side
                    case "SetTrap":
                        playingTable.SetTrap(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]), int.Parse(splitString[4]));
                        break;

                    //TriggerBoom,row,column,None,side
                    case "TriggerBoom":
                        playingTable.TriggerBoom(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]), int.Parse(splitString[4]));
                        break;

                    //TriggerTrap,row,column,triggeredTrapSide,side,gameBoard.NextIndex
                    case "TriggerTrap":
                        playingTable.TriggerTrap(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]), int.Parse(splitString[4]), int.Parse(splitString[5]));
                        break;

                    //dizzyStatus, dizzyStatus
                    //case "dizzyStatus":
                    //    playingTable.DizzyStatus(bool.Parse(splitString[1]));
                    //    break;

                    //RandomSetChess,row,column,side
                    //case "RandomSetChess":
                    //    playingTable.RandomSetChess(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]));
                    //    break;

                    //IllusionSpell,row,column,side,illusionSpellTimer
                    case "IllusionSpell":
                        playingTable.ProcessIllusionSpell(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]));
                        break;

                    //IllusionSpellTimeCollapse,side
                    //case "IllusionSpellTimeCollapse":
                    //    playingTable.IllusionSpellTimeCollapse(int.Parse(splitString[1]));
                    //    break;

                    //SetIllusionSpellRelease,IllusionSpellReleaseX, IllusionSpellReleaseY, preIllusionState
                    case "SetIllusionSpellRelease":
                        playingTable.SetIllusionSpellRelease(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]));
                        break;

                    //FrozenChess,
                    case "FrozenChess":
                        playingTable.SetFrozenedChesses(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]));
                        break;

                    //ShackSpell, side
                    case "ShackSpell":
                        playingTable.SetShackleSpell(int.Parse(splitString[1]));
                        break;

                    //SetChess,row,column,color
                    case "SetChess":
                        playingTable.SetChess(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]));
                        break;

                    //Win, chessColor
                    case "Win":
                        string winner = "";
                        if (int.Parse(splitString[1]) == ChessColor.Black)
                        {
                            winner = "BLACK is the winner!";
                        }
                        else
                        {
                            winner = "White is the winner!";
                        }
                        playingTable.ShowMessage(winner);
                        playingTable.Restart(winner);
                        break;

                    //ExitRoom,side,names
                    case "ExitRoom":
                        if (side == int.Parse(splitString[1]))  //The one who leave is the player himself (the playing window must have been closed)
                        {
                            side = -1;
                            isCreatedRoom = false;
                            ShowNormalWindow();
                        }
                        else  //The rival or observers leave (the playing window must have not been closed)
                        {
                            playingTable.SetExitUserTips(splitString[1],splitString[2]);
                            if (splitString[1] != "2")
                            {
                                playingTable.Restart("");
                            }
                        }

                        break;
                    default:
                        break;
                }

            }
            if (isNeedLoginAgain == true) //Able to login again. (Sorry / NameUsed)
            {
                EnableConnectToServerAgain(true);
                isThreadStarted = false; //
            }
            else       //The game room has been created, yet the connection has broken (Server has collapsed)
            {
                if (isCreatedRoom == true) 
                {
                    ExitPlayingTable();
                }
                CloseForm();
            }




        }

        delegate void EnableConnectToServerAgainDelegate(bool connectEnable);
        private void EnableConnectToServerAgain(bool connectEnable)  // this method may be invoked in a auxiliary threading, so a delegate is needed
        {
            if (this.InvokeRequired == true)
            {
                EnableConnectToServerAgainDelegate d = new EnableConnectToServerAgainDelegate(EnableConnectToServerAgain);
                this.Invoke(d, connectEnable);  // "this" represents MainForm, which contains components like textBoxName, textBoxServer and buttonConnect
            }
            else
            {
                this.textBoxServer.Enabled = connectEnable;
                this.textBoxName.Enabled = connectEnable;
                this.buttonConnect.Enabled = connectEnable;
            }
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
                AddToRichTextBox("Fail to transmit message");
            }
        }

        delegate void AddTablesToPanelDelegate();
        private void AddTablesToPanel()
        {
            if (panel1.InvokeRequired == true)
            {
                AddTablesToPanelDelegate d = new AddTablesToPanelDelegate(AddTablesToPanel);
                panel1.Invoke(d);
            }
            else
            {
                tables = new GobangUserControl.GobangRoom[roomNumber];
                tableTips = new Label[roomNumber];
                for (int i = 0; i < roomNumber; i++)
                {
                    int xPosition = 15;
                    int yPosition = 15;
                    int offsetX = 60;
                    int offsetY = 60;
                    int currentStartXIndex = i % 3;
                    int currentStartYIndex = i / 3;
                    tables[i] = new GobangRoom();
                    tableTips[i] = new Label();
                    tables[i].Left = xPosition + currentStartXIndex * (tables[i].Width + offsetX);
                    tables[i].Top = yPosition + currentStartYIndex * (tables[i].Height + offsetY);

                    tables[i].HasBlackPlayer = false;
                    tables[i].BlackPlayerName = "";
                    tables[i].HasWhitePlayer = false;
                    tables[i].WhitePlayerName = "";
                    tables[i].Name = "Room" + i;
                    //tables[i].Visible = false;
                    tables[i].Validating += new CancelEventHandler(Table_Validating);  //When click the component, or alter the properties of this component
                                                                                       //Validating event will be triggered
                    tableTips[i].Left = tables[currentStartXIndex].Left + (tables[currentStartXIndex].Width - tableTips[currentStartXIndex].Width) / 2 + 25;
                    tableTips[i].Top = yPosition + currentStartYIndex * (tables[i].Height + offsetY) + 90; 
                    tableTips[i].Text = "Table " + i;
                    tableTips[i].Image = Properties.Resources._10_140312153051A1;
                }
                panel1.Controls.AddRange(tables);
                panel1.Controls.AddRange(tableTips);
            }
        }

        private void Table_Validating(object sender, CancelEventArgs e)   //
        {
            // return to here (begining of the method)
            if (isReceiveCommand == true)   
            {
                return;   // "isReceiveCommand == true" ==> When the GobangRoom components are changed, won't execute the following code
            }
            GobangRoom gobangControl = (GobangRoom)sender;
            this.tableIndex = int.Parse(gobangControl.Name.Substring(4));

            this.side = gobangControl.SelectedSide; //(0:black  1:white  2:Observer)

            //EnterRoom , tableIndex, side
            SendToServer(string.Format("EnterRoom,{0},{1}", tableIndex, side));

        }

        delegate void ExitPlayingTableDelegate();
        private void ExitPlayingTable()
        {
            if (playingTable.InvokeRequired == true)
            {
                ExitPlayingTableDelegate d = new ExitPlayingTableDelegate(ExitPlayingTable);
                playingTable.Invoke(d);  //
            }
            else
            {
                playingTable.Close();
                isCreatedRoom = false;
            }
        }

        delegate void CloseFormDelegate();
        private void CloseForm()
        {
            if (this.InvokeRequired == true)
            {
                CloseFormDelegate d = new CloseFormDelegate(CloseForm);
                try
                {
                    this.Invoke(d);
                }
                catch
                {

                    // do no_op
                }
            }
            else
            {
                isThreadStarted = false;
                this.Close();
            }
        }

        delegate void SetBoolPropertyDelegate(GobangRoom table, int index, bool boolValue);
        private void SetBoolProperty(GobangRoom table, int index, bool boolValue) //table, black or white, sitted or empty
        {
            if (table.InvokeRequired == true)
            {
                SetBoolPropertyDelegate d = new SetBoolPropertyDelegate(SetBoolProperty);
                table.Invoke(d, table, index, boolValue);
            }
            else
            {
                if (index == 0)  //Black Player
                {
                    table.HasBlackPlayer = boolValue;
                }
                else             //White Player
                {
                    table.HasWhitePlayer = boolValue;
                }
            }
        }

        delegate void SetTextPropertyDelegate(GobangRoom table, int index, string name);
        private void SetTextProperty(GobangRoom table, int index, string name)
        {
            if (table.InvokeRequired == true)
            {
                SetTextPropertyDelegate d = new SetTextPropertyDelegate(SetTextProperty);
                table.Invoke(d, table, index, name);
            }
            else
            {
                if (index == 0) //Black Player
                {
                    table.BlackPlayerName = name;
                }
                else           //White Player
                {
                    table.WhitePlayerName = name;
                }
            }
        }

        delegate void CreateRoomDelegate(int tableIndex); //The GobangRoom(tables) components in panel will be altered(disabled). So a delegate shall be created 
        private void CreateRoom(int tableIndex)
        {
            if (this.InvokeRequired == true)
            {
                CreateRoomDelegate d = new CreateRoomDelegate(CreateRoom);
                this.Invoke(d, tableIndex);
            }
            else
            {
                playingTable = new PlayingTable(tableIndex,side,sw);
                playingTable.Show();  //Show the PlayingTable Form          !!!Now the window has transfer to the PlayingTable

                for (int i = 0; i < tables.Length; i++) //Invalidate all the table(GobangRoom) component
                {
                    tables[i].Enabled = false;
                }

                this.WindowState = FormWindowState.Minimized;
            }
        }

        delegate void ShowNormalWindowDelegate();
        private void ShowNormalWindow()
        { 
            if(this.InvokeRequired == true)
            {
                ShowNormalWindowDelegate d = new ShowNormalWindowDelegate(ShowNormalWindow);
                this.Invoke(d);
            }
            else
            {
                for(int i=0;i<tables.Length;i++)
                {
                    tables[i].Enabled = true;
                }
                this.WindowState = FormWindowState.Normal;
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isThreadStarted == true) //If the thread is still running, then not allowed to close the form
            {
                e.Cancel = true;  //Just like "return" in a method

                if (isCreatedRoom == true)
                {
                    MessageBox.Show("Please exit game room first!", "Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    SendToServer("Logout");
                }
            }


        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox1.Image = Properties.Resources.BTNInnerFireOff;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.BTNInnerFire;
        }




    }
}
