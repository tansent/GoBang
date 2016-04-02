using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GoBangServer
{
    public partial class FormServer : Form
    {
        private int maxUsers;  // max users in a game room

        List<User> userList = new List<User>(); //all the users that connected

        private int maxRoomNumbers; //max rooms allowed

        private GameRoom[] gameRoom; //actual game rooms

        IPAddress localAddress;

        private int port = 51888;

        private TcpListener myListener;

        private Service service;

        public FormServer()
        {
            InitializeComponent();
            listBoxStatus.HorizontalScrollbar = true;
            service = new Service(listBoxStatus);
           // this.textBoxMaxTables.Text = "50";  //For test use
           // this.textBoxMaxUsers.Text = "150";  //
        }

        private void FormServer_Load(object sender, EventArgs e)
        {
            listBoxStatus.HorizontalScrollbar = true;
            IPAddress[] addrIP = Dns.GetHostAddresses(Dns.GetHostName());
            localAddress = addrIP[1];
            buttonStop.Enabled = false;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxMaxTables.Text, out maxRoomNumbers) == false ||  //the results of the parse process success are stored in maxRoomNumbers
                int.TryParse(textBoxMaxUsers.Text, out maxUsers) == false)
            {
                MessageBox.Show("Please enter an orthonormal number!");   //
                return;
            }
            if (maxUsers < 1 || maxUsers > 500)
            {
                MessageBox.Show("Only the range from 1 - 300 is permitted for total users!");
                return;
            }
            if (maxRoomNumbers < 1 || maxRoomNumbers > 100)
            {
                MessageBox.Show("Only the range from 1 - 100 is permitted for total tables!");
                return;
            }

            textBoxMaxUsers.Enabled = false;
            textBoxMaxTables.Enabled = false;
            
            gameRoom = new GameRoom[maxRoomNumbers];  //create array
            for (int i = 0; i < maxRoomNumbers; i++)
            {
                gameRoom[i] = new GameRoom(listBoxStatus);  //At the moment sides of players have not been assigned yet //
            }
            myListener = new TcpListener(localAddress, port);
            myListener.Start();
            service.AddListBoxItem(string.Format("{0:M , d, yyyy (dddd) h:m} start at {1} : {2} listen to the connection with clients", 
                                    DateTime.Now,localAddress,port));
            ThreadStart ts = new ThreadStart(ListenClientConnect);  //
            Thread myThread = new Thread(ts);
            myThread.Start();      //background ?
            buttonStart.Enabled = false; //
            buttonStop.Enabled = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            service.AddListBoxItem(string.Format("Current User Number: {0}",userList.Count));
            service.AddListBoxItem(string.Format("{0:M , d, yyyy (dddd) h:m} Service ceased, Users quit",DateTime.Now ));
            StopClientAndListener();
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void StopClientAndListener()
        {
            for (int i = 0; i < userList.Count; i++)
            {
                userList[i].client.Close();
            }
            myListener.Stop(); //
        }

        private void ListenClientConnect()
        {
            while (true)
            {
                TcpClient newClient = null;
                try
                {
                    newClient = myListener.AcceptTcpClient();
                }
                catch
                {
                    break;
                }
                ParameterizedThreadStart pts = new ParameterizedThreadStart(ReceiveData);   //
                Thread threadReceive = new Thread(pts);
                threadReceive.IsBackground = true;
                User user = new User(newClient);
                threadReceive.Start(user);
                userList.Add(user);
                service.AddListBoxItem(string.Format("[{0}]Connect to the server successfully!",newClient.Client.RemoteEndPoint));  //r
                service.AddListBoxItem(string.Format("Current connection number:{0}",userList.Count));
            }
        }

        private void ReceiveData(object obj)
        {
            User user = (User)obj;
            TcpClient client = user.client;
            bool normalExit = false;
            bool exitWhile = false;
            while (exitWhile == false)  //
            {
                string receiveString = null;
                try
                {
                    receiveString = user.sr.ReadLine();
                }
                catch  // fail to receive 
                {
                    service.AddListBoxItem("Receiving data failed!");
                    RemovePlayerFromRoom(user);   // remove the user no matter if he is on the seat (just remove from table, not delete its link)
                                                  // delete its link at the outside of the while loop
                }

                if (receiveString == null)   //  received, yet received nothing(receive failed still)
                {
                    if (normalExit == false)
                    {
                        if (client.Connected == true) // when quit listening, Connected return false
                        {
                            service.AddListBoxItem(string.Format("Losing Connection to {0}, quit getting data from the user",client.Client.RemoteEndPoint));
                        }
                        RemovePlayerFromRoom(user);
                    }
                    break;   // break the loop here, so the failed connection can be removed
                             // at the outside of the while loop, there are remove() & close() method to delete the connection 
                }
                //For observing use
                service.AddListBoxItem(string.Format("From[{0}]{1}:{2}", user.client.Client.RemoteEndPoint, user.userName, receiveString));
                //-----------------//

                string[] splitString = receiveString.Split(',');
                int tableIndex = -1;
                int side = -1;
                int anotherSide = -1;
                string sendString = "";
                switch (splitString[0])
                { 
                    case "Login":
                        //Login , UserName 
                        //for login use
                        if (userList.Count > maxUsers)
                        {
                            sendString = "Sorry";
                            service.SendToOne(user, sendString);
                            service.AddListBoxItem("Rooms full, censor " + splitString[1] + "Enter the room");
                            exitWhile = true;   // impede the user entering the game room
                        }
                        else
                        {
                            bool isSameName = false;
                            foreach (User OnlineUser in userList)
                            {
                                if (OnlineUser.userName == splitString[1])
                                {
                                    isSameName = true;  //
                                    break;
                                }
                            }
                            if (isSameName == true)
                            {
                                sendString = "NameUsed";
                                service.SendToOne(user, sendString);
                                service.AddListBoxItem("The name has existed, no reused  " + splitString[1] + "entering the room");
                                exitWhile = true;
                            }
                            else  // orthonormal name
                            {
                                user.userName = splitString[1];   //Only when everything goes correctly can name be assigned
                                service.AddListBoxItem(string.Format("[{0}] entering a game room with the name of [{1}]",client.Client.RemoteEndPoint, user.userName));
                                // send data of rooms and all the existed users to the new-comer
                                service.SendToOne(user, string.Format("RoomNumbers,{0},{1}",maxRoomNumbers,userList.Count));
                                sendString = "Tables," + this.GetTablesOnlineString();  // ?
                                service.SendToAll(userList, sendString);  // ?
                                service.SendToAll(userList, string.Format("Login,{0}",user.userName));  //
                            }
                        }
                        break;
                    case "Logout":
                        //Logout
                        //exit game room
                        service.SendToAll(userList, string.Format("Logout,{0}",user.userName));
                        normalExit = true;
                        exitWhile = true;
                        break;
                    case "EnterRoom":       //A room here is a table       //mutable
                        //EnterRoom, tableIndex, side
                        //User enter a room
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        string roomOnlineString = GetRoomOnlineString(tableIndex);  // ?
                        if (side == 2)
                        {
                            //observer
                            gameRoom[tableIndex].lookOnUser.Add(user);  // here a table stands for a room
                            service.AddListBoxItem(string.Format("{0} is oberving at table {1}", user.userName, tableIndex + 1));
                            service.SendToOne(user, "Grid," + GetBoardGrid(tableIndex));
                        }
                        else
                        { 
                            //play
                           anotherSide = side == 0 ? 1 : 0;   //
                           gameRoom[tableIndex].gamePlayer[side].GameUser = user;      // assign the player
                            service.AddListBoxItem(string.Format("{0} sits as {2}side at table{1} ",user.userName,tableIndex + 1,side == 0 ? "Black" : "White"));  //
                        }
                        string enterNames = "";//
                        if (gameRoom[tableIndex].gamePlayer[0].GameUser != null)
                        {
                            enterNames += gameRoom[tableIndex].gamePlayer[0].GameUser.userName;
                        }
                        enterNames += "/";
                        if (gameRoom[tableIndex].gamePlayer[1].GameUser != null)
                        {
                            enterNames += gameRoom[tableIndex].gamePlayer[1].GameUser.userName;
                        }
                        enterNames += "/";
                        if (side == 2)
                        {
                            enterNames += user.userName;
                        }
                                                                                  //Now & once     //Previous
                        sendString = string.Format("EnterRoom,{0},{1},{2}",side, enterNames, roomOnlineString);  
                        service.SendToRoom(gameRoom[tableIndex], sendString);
                        service.SendToAll(userList, "Tables," + this.GetTablesOnlineString());

                        break;
                    case "ExitRoom":
                        //ExitRoom, tableIndex, side
                        //quit to the game lobby
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        string names = "";
                        if (gameRoom[tableIndex].gamePlayer[0].GameUser != null)
                        {
                            names += gameRoom[tableIndex].gamePlayer[0].GameUser.userName;
                        }
                        names += "/";
                        if (gameRoom[tableIndex].gamePlayer[1].GameUser != null)
                        {
                            names += gameRoom[tableIndex].gamePlayer[1].GameUser.userName;
                        }
                        names += "/";
                        if (side == 2)
                        {
                            names += user.userName;
                        }
                        service.SendToRoom(gameRoom[tableIndex], string.Format("ExitRoom,{0},{1}",side,names));
                        if (side == 2)
                        {
                            gameRoom[tableIndex].lookOnUser.Remove(user);   //observes leave
                        }
                        else
                        {
                            //players leave
                            gameRoom[tableIndex].gamePlayer[side].Start = false;
                            gameRoom[tableIndex].gamePlayer[side].TacticalEnd = false;
                            gameRoom[tableIndex].gamePlayer[side].GameUser = null;
                            anotherSide = (side == 0 ? 1 : 0);
                            gameRoom[tableIndex].gamePlayer[anotherSide].Start = false;
                            gameRoom[tableIndex].gamePlayer[anotherSide].TacticalEnd = false;
                            gameRoom[tableIndex].GameBoard.InitializeBoard();
                        }
                        service.AddListBoxItem(string.Format("{0} back to the lobby",user.userName)); //
                        service.SendToAll(userList, "Tables," + this.GetTablesOnlineString());  //
                        break;
                    case "Talk":
                        //Talk, tableIndex, content
                        tableIndex = int.Parse(splitString[1]);
                        sendString = string.Format("Talk,{0},{1}", user.userName,   //FromWhom
                            receiveString.Substring(splitString[0].Length + splitString[1].Length + 2)); // Substring here represents contents, 2 commas so plus 2
                        service.SendToRoom(gameRoom[tableIndex], sendString);
                        break;
                    case "StartTactical":
                        //start, tableIndex, side
                        //When the user click the "Start" button
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        gameRoom[tableIndex].gamePlayer[side].Start = true;  //at this moment, the user has already been added to the gamePlayer list
                        if (side == 0)
                        {
                            anotherSide = 1;
                            sendString = "Message, Black start tactical stage!";
                        }
                        if (side == 1)       // mutable
                        {
                            anotherSide = 0;
                            sendString = "Message, White start tactical stage!";
                        }
                        service.SendToRoom(gameRoom[tableIndex], sendString);
                        if (gameRoom[tableIndex].gamePlayer[anotherSide].Start == true)    //only both players' start property are true can a game begins
                        {
                            gameRoom[tableIndex].GameBoard.InitializeBoard();
                            service.SendToRoom(gameRoom[tableIndex],
                                "StartTactical," + gameRoom[tableIndex].GameBoard.NextIndex);
                        }
                        break;

                    case "TacticalEnd":
                        //TacticalEnd, tableIndex, side
                        //When both users end tactical stages
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        gameRoom[tableIndex].gamePlayer[side].TacticalEnd = true;  //at this moment, the user has already been added to the gamePlayer list
                        if (side == 0)
                        {
                            anotherSide = 1;
                            sendString = "Message, Black is ready!";
                        }
                        if (side == 1)       // mutable
                        {
                            anotherSide = 0;
                            sendString = "Message, White is ready!";
                        }
                        service.SendToRoom(gameRoom[tableIndex], sendString);
                        if (gameRoom[tableIndex].gamePlayer[anotherSide].TacticalEnd == true)
                        {
                            service.SendToRoom(gameRoom[tableIndex],
                                "StartPlaying," + gameRoom[tableIndex].GameBoard.NextIndex);
                            gameRoom[tableIndex].StartRound();
                            gameRoom[tableIndex].IllusionSpellList.Clear();
                            gameRoom[tableIndex].frozenSpellList.Clear();
                        }
                        break;

                    case "SetBoom":
                        //SetBoom, tableIndex, side, Row, Column, BoomColor(side)
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        int BoomX = int.Parse(splitString[3]);
                        int BoomY = int.Parse(splitString[4]);
                        int BoomColor = int.Parse(splitString[5]);
                        gameRoom[tableIndex].SetBoom(user,side,BoomX, BoomY, BoomColor+2);
                        break;

                    case "SetTrap":
                        //SetBoom, tableIndex, side, Row, Column, TrapColor(side)
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        int TrapX = int.Parse(splitString[3]);
                        int TrapY = int.Parse(splitString[4]);
                        int TrapColor = int.Parse(splitString[5]);
                        gameRoom[tableIndex].SetTrap(user, side, TrapX, TrapY, TrapColor + 4);
                        break;

                    case "TriggerBoom":
                        //TriggerBoom, tableIndex, side, Row, Column, Color(side)
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        int TriggerBoomX = int.Parse(splitString[3]);
                        int TriggerBoomY = int.Parse(splitString[4]);
                        int TriggerBoomColor = int.Parse(splitString[5]);
                        gameRoom[tableIndex].SetTriggeredBoom(user, side, TriggerBoomX, TriggerBoomY, TriggerBoomColor);
                        break;

                    case "TriggerTrap":
                         //TriggerTrap, tableIndex, side, Row, Column, Color(side)
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        int TriggerTrapX = int.Parse(splitString[3]);
                        int TriggerTrapY = int.Parse(splitString[4]);
                        int TriggerTrapColor = int.Parse(splitString[5]);
                        gameRoom[tableIndex].SetTriggeredTrap(user, side, TriggerTrapX, TriggerTrapY, TriggerTrapColor);
                        break;

                    case "RandomChess":
                        //RandomChess,tableIndex, side, randomChessX, randomChessY
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        int randomChessCenterX = int.Parse(splitString[3]);
                        int randomChessCenterY = int.Parse(splitString[4]);
                        gameRoom[tableIndex].SetRandomChess(randomChessCenterX, randomChessCenterY, side);
                         break;

                    case "DivisionSpell":
                         //DivisionSpell, tableIndex, side, Row, Column, Color(side)
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        int DivisionSpellX = int.Parse(splitString[3]);
                        int DivisionSpellY = int.Parse(splitString[4]);
                        int DivisionSpellColor = int.Parse(splitString[5]);
                        gameRoom[tableIndex].SetDivisionSpell(DivisionSpellX, DivisionSpellY, DivisionSpellColor);
                         break;


                    case "IllusionSpell":
                    //IllusionSpell, tableIndex, side, Row, Column, Color(side)
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        int IllusionSpellX = int.Parse(splitString[3]);
                        int IllusionSpellY = int.Parse(splitString[4]);
                        int IllusionSpellColor = int.Parse(splitString[5]);
                        gameRoom[tableIndex].SetIllusionSpell(IllusionSpellX, IllusionSpellY, IllusionSpellColor);
                         break;

                    case "FrozenSpell":
                    //FrozenSpell, tableIndex, side, Row, Column, Color(side)
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        int FrozenSpellCenterX = int.Parse(splitString[3]);
                        int FrozenSpellCenterY = int.Parse(splitString[4]);
                        //int FrozenSpellCenter = int.Parse(splitString[5]);
                        gameRoom[tableIndex].SetFrozenSpell(FrozenSpellCenterX, FrozenSpellCenterY, side);
                        break;

                    case "FrozenSpellRelease":
                    //FrozenSpellRelease, tableIndex, side, Row, Column
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        int FrozenSpellReleaseX = int.Parse(splitString[3]);
                        int FrozenSpellReleaseY = int.Parse(splitString[4]);
                        gameRoom[tableIndex].FrozenSpellRelease(FrozenSpellReleaseX, FrozenSpellReleaseY, side);
                        break;

                    case "ShackleSpell":
                    //ShackleSpell,tableIndex, side
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        gameRoom[tableIndex].ShackSpell(side);
                        break;

                    //case "IllusionSpellTimeCollapse":
                    ////IllusionSpellTimeCollapse,tableIndex
                    //    tableIndex = int.Parse(splitString[1]);
                    //    gameRoom[tableIndex].SetIllusionSpellTimeCollapse();
                    //    break;

                    //case "IllusionSpellRelease":
                    ////IllusionSpellRelease,tableIndex, IllusionSpellX, IllusionSpellY,side
                    //    tableIndex = int.Parse(splitString[1]);
                    //    int IllusionSpellReleaseX = int.Parse(splitString[2]);
                    //    int IllusionSpellReleaseY = int.Parse(splitString[3]);
                    //    side = int.Parse(splitString[4]);
                    //    gameRoom[tableIndex].SetIllusionSpellRelease(IllusionSpellReleaseX, IllusionSpellReleaseY,side);
                    //    break;
                   // case "dizzy":
                        //SendToServer(string.Format("dizzy,{0},{1},{2},{3}", tableIndex, side, x, y));
                        //tableIndex = int.Parse(splitString[1]);
                        //side = int.Parse(splitString[2]);
                        //int dizzyX = int.Parse(splitString[3]);
                        //int dizzyY = int.Parse(splitString[4]);
                        //bool dizzyStatus = bool.Parse(splitString[5]);
                        //gameRoom[tableIndex].processDizzyPlayer(user, side, dizzyX, dizzyY, dizzyStatus);
                     //   break;

                    case "SetChess":
                        //SetChess, tableIndex, side, Row, Column, Color(side)
                        //Player sets chess
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        int xi = int.Parse(splitString[3]);
                        int xj = int.Parse(splitString[4]);
                        int color = int.Parse(splitString[5]);  // even initially color is not a number, convertion can still be done
                        gameRoom[tableIndex].SetChess(xi, xj, color);
                        break;
                    case "AskTie":
                        //AskTie, tableIndex, side
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        anotherSide = side == 0 ? 1 : 0;
                        if (gameRoom[tableIndex].gamePlayer[anotherSide].GameUser != null)
                        {
                            service.SendToOne(gameRoom[tableIndex].gamePlayer[anotherSide].GameUser,
                                "AskTie," + side);         // mutable
                        }
                        break;
                    case "Reply":
                        //Reply, tableIndex, side,  true/false(accede to tie or not)
                        tableIndex = int.Parse(splitString[1]);
                        side = int.Parse(splitString[2]);
                        anotherSide = side == 0 ? 1 : 0;  
                        if (splitString[3] == "True")
                        {
                            gameRoom[tableIndex].GameBoard.InitializeBoard();// Everthing turns back to their initial state 
                            gameRoom[tableIndex].GameBoard.NextIndex = 0;    // when tie up
                            service.SendToRoom(gameRoom[tableIndex], "Message, Draw Game!");
                            service.SendToRoom(gameRoom[tableIndex], "Restart," + gameRoom[tableIndex].GameBoard.NextIndex);
                        }
                        else  //？ 只有不同意(False)才调用Reply
                        {
                            if (gameRoom[tableIndex].gamePlayer[anotherSide].GameUser != null)
                            { 
                                service.SendToOne(
                                    gameRoom[tableIndex].gamePlayer[anotherSide].GameUser,
                                    string.Format("Reply,{0},{1}", side, splitString[3]));  //splitString[3] == False
                            }
                        }
                        break;
                    default:
                        service.SendToAll(userList, "What is the meaning of" + receiveString);
                        break;                
                }
            }
            //once quiting the while loop, it means the user has left or illegal(reused a existed name), and need to be removed
            userList.Remove(user);
            string str = user.userName;
            client.Close();
            service.AddListBoxItem(string.Format("{0} has quited the game room, there are {1} users leftover",str,userList.Count));
        }
        /// <summary>
        /// This is not the client who decide whether he is legal or what to do next,
        /// but the server decides. Server is the boss, who determines which client does
        /// what operations and which client needs to be removed.
        /// Clients only need to act according to the orders. 
        /// </summary>
        /// <param name="user"></param>

        private void RemovePlayerFromRoom(User user)   //
        {
            for (int i = 0; i < gameRoom.Length; i++)
            {
                if (gameRoom[i].BlackPlayer.GameUser != null)
                {
                    if (gameRoom[i].BlackPlayer.GameUser == user)
                    {
                        StopPlayer(i, 0);   //table i, black one  | Here stop the game
                        return;
                    }
                }
            
                if (gameRoom[i].WhitePlayer.GameUser != null)
                {
                    if (gameRoom[i].WhitePlayer.GameUser == user)
                    {
                        StopPlayer(i, 1);   //table i, White one
                        return;
                    }
                }

                for (int j = 0; j < gameRoom[i].lookOnUser.Count; j++)
                {
                    if (gameRoom[i].lookOnUser[j] != null)
                    {
                        if (gameRoom[i].lookOnUser[j] == user)
                        {
                            gameRoom[i].lookOnUser.RemoveAt(j);  // here just remove
                            return;
                        }
                    }
                }
            }
        }

        private void StopPlayer(int i, int j)     //
        {
            gameRoom[i].gamePlayer[j].Start = false;
            //gameRoom[i].gamePlayer[j].GameUser = null;

            //LostPlayer, RoomNumber, side, UserName
            service.SendToRoom(gameRoom[i], string.Format("LostPlayer,{0},{1},{2}", i, j, gameRoom[i].gamePlayer[j].GameUser.userName));  // !?
        }

        
        /// <summary>
        /// Get info from every room of the lobby.
        /// each table: BlackIsEmptyOrSitted(1 bit) + name  /  WhiteIsEmptyOrSitted(1 bit) + name
        /// 0 : empty  /  1 : sitted
        /// </summary>
        /// <returns></returns>
        private string GetTablesOnlineString()
        {
            StringBuilder strBuilder = new StringBuilder();   // When there is plenty of concatenation process, using StringBuilder instead of string
            for (int i = 0; i < gameRoom.Length; i++)
            {
                string blackName = "0";
                if (gameRoom[i].gamePlayer[0].GameUser != null )//!= null)
                {
                    blackName = "1" + gameRoom[i].gamePlayer[0].GameUser.userName;
                }
                string whiteName = "0";
                if (gameRoom[i].gamePlayer[1].GameUser != null)//!= null)
                {
                    whiteName = "1" + gameRoom[i].gamePlayer[1].GameUser.userName;
                }
                strBuilder.Append(string.Format("{0}/{1},",blackName,whiteName));
            }
            strBuilder = strBuilder.Remove(strBuilder.Length - 1, 1);
            return strBuilder.ToString();
        }

        /// <summary>
        /// Obtain info from an appointed room
        /// BlackName/WhiteName/OberverName/OberverName
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        private string GetRoomOnlineString(int tableIndex)
        {
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < gameRoom[tableIndex].gamePlayer.Length; i++)
            {
                if (gameRoom[tableIndex].gamePlayer[i].GameUser != null) //
                {
                    strBuilder.Append(gameRoom[tableIndex].gamePlayer[i].GameUser.userName);
                }
                strBuilder.Append("/");
            }
            for (int i = 0; i < gameRoom[tableIndex].lookOnUser.Count; i++)
            {
                strBuilder.Append(gameRoom[tableIndex].lookOnUser[i].userName);
                strBuilder.Append("/");
            }
            strBuilder = strBuilder.Remove(strBuilder.Length - 1, 1);
            return strBuilder.ToString();
        }

        private string GetBoardGrid(int tableIndex)
        {
            StringBuilder sb = new StringBuilder();
            int[,] grid = gameRoom[tableIndex].GameBoard.Grid;
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    sb.Append(grid[i, j].ToString());
                    sb.Append("/");
                }
            }
            sb = sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private void FormServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Quit without clicking START, myListener is null
            if (myListener != null)
            {
                StopClientAndListener();
            }
        }


    }
}
