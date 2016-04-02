using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GoBangServer
{
    class FrozenSpell
    {
        public FrozenSpell(int frozenedSide)
        {
            side = frozenedSide;
            frozenTime = 0;
            hasRecover = false;
            Thread t = new Thread(timeout);
            t.IsBackground=true;
            timerForFrozen = true;
            t.Start();
            //timerForFrozen = new System.Windows.Forms.Timer();
            //timerForFrozen.Tick += this.timeout;
            //timerForFrozen.Interval = 5000;
            //timerForFrozen.Start();
        }

        public void timeout()
        {
            Thread.Sleep(3000);
            timerForFrozen = false;
        }

        public void setFrozenTime()
        {
            frozenTime = 2; 
        }

        public void FrozenTimeCollapse()
        {
            frozenTime--;
        }

        public int FrozenCenterX { set; get; }
        public int FrozenCenterY { set; get; }
        public int frozenTime { set; get; }
        public int side { set; get; }
        public bool forzenSpellEnable { set; get; }
        public bool hasRecover { set; get; }
        public bool timerForFrozen; 
    }
}
