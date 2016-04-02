using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GoBangServer
{
    class Service
    {
        public Service(ListBox listbox)
        {
            this.listbox = listbox;
            addListBoxCallBack = new AddListBoxCallBack(AddListBoxItem);
        }

        public void AddListBoxItem(string str)
        {
            if (listbox.InvokeRequired == true)   //if the invoking thread and the creating thread are not identical
            {
                listbox.Invoke(addListBoxCallBack, str);  //
            }
            else
            {
                if (listbox.IsDisposed == false)  // the form which contains the listbox has not been closed yet
                {
                    listbox.Items.Add(str);
                    listbox.SelectedIndex = listbox.Items.Count - 1; // select the last one
                    listbox.ClearSelected();        //
                }
            }
        }

        public void SendToOne(User user, string str)
        {
            try
            {
                user.sw.WriteLine(str);
                user.sw.Flush();
                //for observing
                AddListBoxItem(string.Format("Send {0} to {1}", str, user.userName));
                //end. deletable
            }
            catch
            { 
               AddListBoxItem(string.Format("Fail to send Message to {0}", user.userName));
            }
        }

        public void SendToRoom(GameRoom gameRoom, string str)//
        {
            for (int i = 0; i < gameRoom.gamePlayer.Length; i++)//
            {
                if (gameRoom.gamePlayer[i].GameUser != null)
                {
                    SendToOne(gameRoom.gamePlayer[i].GameUser, str);
                }
            }
            for (int j = 0; j < gameRoom.lookOnUser.Count; j++)//mutable
            {
                SendToOne(gameRoom.lookOnUser[j], str);
            }
        }

        public void SendToAll(System.Collections.Generic.List<User> userlist, string str)  //
        {
            for (int i = 0; i < userlist.Count; i++)
            {
                SendToOne(userlist[i], str);
            }
        }
      //  public static List<User> userlist = new List<User>();
        private ListBox listbox;
        private delegate void AddListBoxCallBack(string str);
        private AddListBoxCallBack addListBoxCallBack;
    }
}
