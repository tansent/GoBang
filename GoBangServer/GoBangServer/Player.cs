using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoBangServer
{
    class Player
    {
        public Player()
        {
            Start = false;
            TacticalEnd = false;
            GameUser = null;
        }

        public User GameUser{ get; set;}
        public bool Start { get; set; }
        public bool TacticalEnd { get; set; }
    }
}
